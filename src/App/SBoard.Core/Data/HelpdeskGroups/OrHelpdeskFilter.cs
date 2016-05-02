using System.Collections.Generic;
using System.Linq;
using SBoard.Core.Data.Helpdesks;

namespace SBoard.Core.Data.HelpdeskGroups
{
    public class OrHelpdeskFilter : ClientHelpdeskFilter
    {
        public OrHelpdeskFilter()
        {
            this.Children = new List<ClientHelpdeskFilter>();
        }

        public List<ClientHelpdeskFilter> Children { get; set; }

        public override bool Apply(HelpdeskPreview helpdesk)
        {
            return this.Children.Any(f => f.Apply(helpdesk));
        }
    }
}