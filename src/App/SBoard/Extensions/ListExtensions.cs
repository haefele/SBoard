using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
}