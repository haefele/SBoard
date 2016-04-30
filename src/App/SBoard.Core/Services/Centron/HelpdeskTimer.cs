using System;
using Newtonsoft.Json.Linq;

namespace SBoard.Core.Services.Centron
{
    public class HelpdeskTimer : IEquatable<HelpdeskTimer>
    {
        public int I3D { get; set; }

        public bool IsCalculable { get; set; }
        public string Description { get; set; }
        public string InternalDescription { get; set; }
        public bool IsPlanned { get; set; }
        public TimeSpan LunchTime { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public JObject Original { get; set; }

        #region Equality
        public bool Equals(HelpdeskTimer other)
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
            return this.Equals((HelpdeskTimer) obj);
        }

        public override int GetHashCode()
        {
            return this.I3D;
        }

        public static bool operator ==(HelpdeskTimer left, HelpdeskTimer right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HelpdeskTimer left, HelpdeskTimer right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}