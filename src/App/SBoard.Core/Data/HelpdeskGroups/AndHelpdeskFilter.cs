using System.Collections.Generic;
using System.Linq;
using SBoard.Core.Data.Helpdesks;

namespace SBoard.Core.Data.HelpdeskGroups
{
    public class AndHelpdeskFilter  : ClientHelpdeskFilter
    {
        public AndHelpdeskFilter()
        {
            this.Children = new List<ClientHelpdeskFilter>();
        }

        public List<ClientHelpdeskFilter> Children { get; set; }

        public override bool Apply(HelpdeskPreview helpdesk)
        {
            return this.Children.All(f => f.Apply(helpdesk));
        }
    }
}