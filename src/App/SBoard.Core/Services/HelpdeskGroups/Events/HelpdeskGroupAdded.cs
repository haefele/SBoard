using SBoard.Core.Data.HelpdeskGroups;

namespace SBoard.Core.Services.HelpdeskGroups.Events
{
    public class HelpdeskGroupAdded
    {
        public HelpdeskGroup HelpdeskGroup { get; }

        public HelpdeskGroupAdded(HelpdeskGroup helpdeskGroup)
        {
            this.HelpdeskGroup = helpdeskGroup;
        }
    }
}