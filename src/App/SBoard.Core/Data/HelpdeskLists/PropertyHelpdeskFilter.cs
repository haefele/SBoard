using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using SBoard.Core.Data.Helpdesks;

namespace SBoard.Core.Data.HelpdeskLists
{
    public class PropertyHelpdeskFilter : ClientHelpdeskFilter
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }
        public PropertyTicketFilterComparator Comparator { get; set; }

        public override bool Apply(HelpdeskPreview helpdesk)
        {
            var property = typeof(HelpdeskPreview).GetProperty(this.PropertyName);

            if (property == null)
                return false;

            var actualValue = property.GetValue(helpdesk);

            switch (this.Comparator)
            {
                case PropertyTicketFilterComparator.Equals:
                    var valueInCorrectType = Convert.ChangeType(this.Value, property.PropertyType);
                    return object.Equals(actualValue, valueInCorrectType);

                case PropertyTicketFilterComparator.Contains:
                    var actualValueAsString = actualValue as string;
                    if (actualValueAsString != null)
                    {
                        return actualValueAsString.Contains(this.Value);
                    }

                    var actualValueAsEnumerable = actualValue as IEnumerable;
                    if (actualValueAsEnumerable != null)
                    {
                        return actualValueAsEnumerable.Cast<object>().Any(f => f == Convert.ChangeType(this.Value, f.GetType()));
                    }

                    return false;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}