using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Shortner.Core
{
    public static class UniqueIdGenerator
    {
        public static int MinValue { get; set; }
        public static int MaxValue { get; set; }

        private static int CurrentValue;

        public static int GetNext()
        {
            return Interlocked.Increment(ref CurrentValue);
        }

        public static bool GetNextRange()
        {
            return true;
        }
    }
}
