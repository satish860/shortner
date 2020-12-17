using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shortner.Tests
{
    public interface IFixture : IDisposable
    {
        HttpClient httpClient { get; }
    }
}
