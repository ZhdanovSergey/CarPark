using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocationsWorker.Models;

internal sealed class AntiforgeryData
{
    public required CookieCollection Cookies { get; init; }
    public required string Token { get; init; }
}
