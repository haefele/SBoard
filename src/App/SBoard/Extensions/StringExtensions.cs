using System;
using UwCore.Common;

namespace SBoard.Extensions
{
    public static class StringExtensions
    {
        public static string MakeOneLiner(this string self)
        {
            Guard.NotNull(self, nameof(self));
            
            return self
                .Replace(Environment.NewLine, " ")
                .Replace("\n", " ")
                .Replace("\r", " ");
        }
    }
}