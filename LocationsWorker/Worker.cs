using CarParkLocationsWorkerService.Models;
using PolylineEncoder.Net.Utility;
using System.Text;
using System.Text.Json;
using GeoCoordinatePortable;
using System.Net;
using System.Net.Http.Headers;
using LocationsWorker.Models;

namespace CarParkLocationsWorkerService;

public class Worker : BackgroundService
{
    const string API_URL = "https://api.openrouteservice.org/v2/directions/driving-car/json";
    const string API_KEY = "5b3ce3597851110001cf624805ffbce60b8e4f0aa6fca9a0766f931b";

    const string APP_ANTIFORGERY_HEADER_NAME = "X-CSRF-TOKEN";
    const string APP_LOGIN_URL = "http://localhost:5250/account/login";
    const string APP_LOCATIONS_URL = "http://localhost:5250/api/locations";

    readonly TimeSpan _signalInterval = TimeSpan.FromSeconds(10);
    readonly Random _random = new Random();
    readonly Options _options;

    public Worker(Options options)
    {
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var apiClient = new HttpClient();
        apiClient.DefaultRequestHeaders.Add("Authorization", API_KEY);

        var requestBody = new
        {
            coordinates = new double[][] { new double[] { _options.Longitude, _options.Latitude } },
            options = new {
                round_trip = new {
                    length = _options.Distance,
                    seed = _random.Next(0, 90),
                },
            },
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await apiClient.PostAsync(API_URL, content);
        var responseAsString = await response.Content.ReadAsStringAsync();
        var responseDeserialized = JsonSerializer.Deserialize<ApiResponse>(responseAsString);

        var antiforgeryData = await GetAntiforgeryData(cancellationToken);
        var authCookies = await Login(antiforgeryData, cancellationToken);

        foreach (var route in responseDeserialized.Routes)
        {
            var points = new PolylineUtility().Decode(route.Geometry).ToList();
            var steps = route.Segments.SelectMany(segment => segment.Steps);

            var lastSendingTime = DateTime.UtcNow;
            var accumulatedTime = TimeSpan.FromSeconds(0);

            var accumulatedLocations = new List<LocationDTO> { new LocationDTO {
                VehicleId = _options.VehicleId,
                DateTime = lastSendingTime,
                Point = new Point { X = points[0].Longitude, Y = points[0].Latitude }
            }};

            foreach (var step in steps)
            {
                var averageSpeed = step.Distance / step.Duration;

                var (firstPointIndex, lastPointIndex) = (step.WayPoints[0], step.WayPoints[1]);

                for (var currentPointIndex = firstPointIndex; currentPointIndex < lastPointIndex; currentPointIndex++)
                {
                    var currentPoint = points[currentPointIndex];
                    var currentGeoCoordinate = new GeoCoordinate(currentPoint.Latitude, currentPoint.Longitude);

                    var nextPoint = points[currentPointIndex + 1];
                    var nextGeoCoordinate = new GeoCoordinate(nextPoint.Latitude, nextPoint.Longitude);

                    var distance = currentGeoCoordinate.GetDistanceTo(nextGeoCoordinate);
                    accumulatedTime += TimeSpan.FromSeconds(distance / averageSpeed);

                    if (accumulatedTime > _signalInterval)
                    {
                        var postRequest = Task.Run(() => PostLocations(accumulatedLocations, authCookies, cancellationToken));
                        var delayTask = Task.Delay(_signalInterval, cancellationToken);
                        await Task.WhenAll(postRequest, delayTask);
                        accumulatedLocations.Clear();
                        lastSendingTime = DateTime.UtcNow;
                        accumulatedTime = -_signalInterval;
                    }

                    accumulatedLocations.Add(new LocationDTO
                    {
                        VehicleId = _options.VehicleId,
                        DateTime = lastSendingTime + accumulatedTime,
                        Point = new Point { X = nextPoint.Longitude, Y = nextPoint.Latitude }
                    });
                }
            }
        }
    }

    async Task<AntiforgeryData> GetAntiforgeryData(CancellationToken cancellationToken)
    {
        var handler = new HttpClientHandler();
        using var client = new HttpClient(handler);
        var response = await client.GetAsync(APP_LOGIN_URL, cancellationToken);

        return new AntiforgeryData
        {
            Cookies = handler.CookieContainer.GetAllCookies(),
            Token = response.Headers.GetValues(APP_ANTIFORGERY_HEADER_NAME).First()
        };
    }

    async Task<CookieCollection> Login(AntiforgeryData antiforgeryData, CancellationToken cancellationToken)
    {
        const string APP_IDENTITY_COOKIE_NAME = ".AspNetCore.Identity.Application";

        var cookieContainer = new CookieContainer();
        cookieContainer.Add(antiforgeryData.Cookies);

        var handler = new HttpClientHandler
        {
            CookieContainer = cookieContainer
        };

        using var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Add(APP_ANTIFORGERY_HEADER_NAME, antiforgeryData.Token);

        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("UserName", _options.Login),
            new KeyValuePair<string, string>("Password", _options.Password),
            new KeyValuePair<string, string>("RememberMe", false.ToString())
        });

        await client.PostAsync(APP_LOGIN_URL, formContent, cancellationToken);
        var cookies = handler.CookieContainer.GetAllCookies();
        var identityCookie = cookies.FirstOrDefault(x => x.Name == APP_IDENTITY_COOKIE_NAME);

        if (identityCookie is null)
            throw new Exception("Failed to login");

        return cookies;
    }

    async void PostLocations(IEnumerable<LocationDTO> locations, CookieCollection cookies, CancellationToken cancellationToken)
    {
        var cookieContainer = new CookieContainer();
        cookieContainer.Add(cookies);

        var handler = new HttpClientHandler {
            CookieContainer = cookieContainer
        };

        using var client = new HttpClient(handler);
        var json = JsonSerializer.Serialize(locations);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(APP_LOCATIONS_URL, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed attempt to send locations");
    }
}