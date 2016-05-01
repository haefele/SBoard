using SBoard.Core.Data.Helpdesks;

namespace SBoard.Core.Data.HelpdeskLists
{
    public class NotHelpdeskFilter : ClientHelpdeskFilter
    {
        public ClientHelpdeskFilter Filter { get; set; }

        public override bool Apply(HelpdeskPreview helpdesk)
        {
            return !this.Filter.Apply(helpdesk);
        }
    }
}