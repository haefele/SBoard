using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UwCore.Common;

namespace SBoard.Extensions
{
    public static class ListExtensions
    {
        public static void RemoveWhere<T>(this ICollection<T> self, Func<T, bool> filter)
        {
            var itemsToRemove = self.Where(filter).ToList();
            foreach (var itemToRemove in itemsToRemove)
            {
                self.Remove(itemToRemove);
            }
        }   
    }

    public static class StringExtensions
    {
        public static string MakeOneLiner(this string self)
        {
            Guard.NotNull(self, nameof(self));
            
            return self
                .Replace(Environment.NewLine, " ")
                .Replace("\\n", " ")
                .Replace("\\r", " ");
        }
    }
}