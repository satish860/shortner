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
            var Id = Base62Convertor.Convert(1000000);
            Assert.Equal(10, Id.Length);
        }
    }
}
