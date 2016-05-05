using SBoard.Core.Data.HelpdeskGroups;

namespace SBoard.Core.Services.HelpdeskGroups.Events
{
    public class HelpdeskGroupDeleted
    {
        public HelpdeskGroup HelpdeskGroup { get; }

        public HelpdeskGroupDeleted(HelpdeskGroup helpdeskGroup)
        {
            this.HelpdeskGroup = helpdeskGroup;
        }
    }
}