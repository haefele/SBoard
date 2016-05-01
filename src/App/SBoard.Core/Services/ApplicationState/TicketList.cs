using System.Collections.Generic;
using System.Linq;
using SBoard.Core.Services.Centron;

namespace SBoard.Core.Services.ApplicationState
{
    public class TicketList
    {
        public string Name { get; set; }
        public WebServiceTicketFilter WebServiceTicketFilter { get; set; }
        public ClientTicketFilter ClientTicketFilter { get; set; }
    }

    public class WebServiceTicketFilter
    {
        public int CustomerI3D { get; set; }
    }

    public abstract class ClientTicketFilter
    {
        public abstract bool Apply(HelpdeskPreview helpdesk);
    }

    public class AndTicketFilter  : ClientTicketFilter
    {
        public AndTicketFilter()
        {
            this.Children = new List<ClientTicketFilter>();
        }

        public List<ClientTicketFilter> Children { get; set; }

        public override bool Apply(HelpdeskPreview helpdesk)
        {
            return this.Children.All(f => f.Apply(helpdesk));
        }
    }

    public class OrTicketFilter : ClientTicketFilter
    {
        public OrTicketFilter()
        {
            this.Children = new List<ClientTicketFilter>();
        }

        public List<ClientTicketFilter> Children { get; set; }

        public override bool Apply(HelpdeskPreview helpdesk)
        {
            return this.Children.Any(f => f.Apply(helpdesk));
        }
    }

    public class PriorityTicketFilter : ClientTicketFilter
    {
        public int PriorityI3D { get; set; }

        public override bool Apply(HelpdeskPreview helpdesk)
        {
            return helpdesk.PriorityI3D == this.PriorityI3D;
        }
    }
}