using Shortner.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Shortner.Tests
{
    public class UniqueKeyGeneratorTests
    {
        [Fact]
        public void GenerateUniqueKeyTests()
        {
            var token = Base62Convertor.Convert(1000000);
            Assert.Equal(1000000, Base62Convertor.Decode(token));
        }
    }
}
