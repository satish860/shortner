using Base62;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shortner.Core
{
    public static class Base62Convertor 
    {
        private static Base62Converter converter = new Base62Converter();
        /// <summary>
        /// Generates the specified unique identifier.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <returns>Token for Url</returns>
        public static string Convert(long uniqueId)
        {
            var encodedString = converter.Encode(uniqueId.ToString());
            if(encodedString.Length <=7)
            {
                var remainingCount = 7 - encodedString.Length;
                var zerosWithComma = string.Join(",",Enumerable.Repeat(0, remainingCount));
                var zeros = zerosWithComma.Replace(",", "");
                return encodedString + zeros;
            }
            return encodedString;
        }
    }
}
