using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Extensions
{
    public static class ExtensionMethods
    {
        public static bool CaseInsensitiveContains(this string str, string substring) => str.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}