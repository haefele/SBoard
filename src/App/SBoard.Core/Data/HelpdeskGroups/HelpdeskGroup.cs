namespace SBoard.Core.Data.HelpdeskGroups
{
    public class HelpdeskGroup
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int? CustomerI3D { get; set; }
        public bool OnlyOwn { get; set; }
        public int? HelpdeskTypeI3D { get; set; }
        public int? HelpdeskStateI3D { get; set; }
    }
}