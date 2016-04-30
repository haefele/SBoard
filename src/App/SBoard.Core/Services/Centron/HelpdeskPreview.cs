using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SBoard.Core.Services.Centron
{
    public class HelpdeskPreview : IEquatable<HelpdeskPreview>
    {
        public int I3D { get; set; }

        public int Number { get; set; }

        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string InternalNote { get; set; }

        public TimeSpan PlannedDuration { get; set; }
        public DateTime? DueDate { get; set; }

        public Employee ResponsiblePerson { get; set; }
        public IList<Employee> Editors { get; set; }

        public int? PriorityI3D { get; set; }
        public string PriorityCaption { get; set; }

        public int? StatusI3D { get; set; }
        public string StatusCaption { get; set; }

        public int? TypeI3D { get; set; }
        public string TypeCaption { get; set; }

        public int? CategoryI3D { get; set; }
        public string CategoryCaption { get; set; }

        public int? SubCategory1I3D { get; set; }
        public string SubCategory1Caption { get; set; }

        public int? SubCategory2I3D { get; set; }
        public string SubCategory2Caption { get; set; }

        public bool IsCalculable { get; set; }

        public JObject Original { get; set; }

        #region Equality
        public bool Equals(HelpdeskPreview other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.I3D == other.I3D;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HelpdeskPreview) obj);
        }

        public override int GetHashCode()
        {
            return this.I3D;
        }

        public static bool operator ==(HelpdeskPreview left, HelpdeskPreview right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HelpdeskPreview left, HelpdeskPreview right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}