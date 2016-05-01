namespace SBoard.Core.Data.HelpdeskLists
{
    public class HelpdeskList
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public WebServiceHelpdeskFilter WebServiceHelpdeskFilter { get; set; }
        public ClientHelpdeskFilter ClientHelpdeskFilter { get; set; }
    }
}