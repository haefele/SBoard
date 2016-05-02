namespace SBoard.Core.Data.HelpdeskGroups
{
    public class HelpdeskGroup
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public WebServiceHelpdeskFilter WebServiceHelpdeskFilter { get; set; }
        public ClientHelpdeskFilter ClientHelpdeskFilter { get; set; }
    }
}