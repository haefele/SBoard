using SBoard.Core.Data.Helpdesks;

namespace SBoard.Core.Data.HelpdeskLists
{
    public abstract class ClientHelpdeskFilter
    {
        public abstract bool Apply(HelpdeskPreview helpdesk);
    }
}