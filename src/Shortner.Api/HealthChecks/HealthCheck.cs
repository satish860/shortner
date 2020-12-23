using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shortner.Api.HealthChecks
{
    public class HealthCheck
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public TimeSpan Duration { get; set; }
        public ICollection<HealthInfo> Info { get; set; }
    }
}
