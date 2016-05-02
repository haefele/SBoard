using SBoard.Core.Data.Helpdesks;

namespace SBoard.Core.Data.HelpdeskGroups
{
    public abstract class ClientHelpdeskFilter
    {
        public abstract bool Apply(HelpdeskPreview helpdesk);
    }
}