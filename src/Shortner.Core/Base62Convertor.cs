﻿using Base62;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shortner.Core
{
    public static class Base62Convertor 
    {
        public static readonly string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
        public static readonly long Base = Alphabet.Length;

        /// <summary>
        /// Generates the specified unique identifier.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <returns>Token for Url</returns>
        public static string Convert(long uniqueId)
        {
            if (uniqueId == 0) return Alphabet[0].ToString();

            var s = string.Empty;

            while (uniqueId > 0)
            {
                int marker = (int)(uniqueId % Base);
                s += Alphabet[marker];
                uniqueId = uniqueId / Base;
            }

            return string.Join(string.Empty, s.Reverse());
        }

        public static int Decode(string s)
        {
            var i = 0;

            //foreach (var c in s)
            //{
            //    i = (i * Base) + Alphabet.IndexOf(c);
            //}

            return i;
        }
    }
}
