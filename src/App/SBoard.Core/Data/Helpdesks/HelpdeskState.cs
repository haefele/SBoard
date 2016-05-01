using System;
using Newtonsoft.Json.Linq;

namespace SBoard.Core.Data.Helpdesks
{
    public class HelpdeskState : IEquatable<HelpdeskState>
    {
        public int I3D { get; set; }
        public string Name { get; set; }
        public bool IsDeactivated { get; set; }

        public JObject Original { get; set; }

        #region Equality
        public bool Equals(HelpdeskState other)
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
            return this.Equals((HelpdeskState) obj);
        }

        public override int GetHashCode()
        {
            return this.I3D;
        }

        public static bool operator ==(HelpdeskState left, HelpdeskState right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HelpdeskState left, HelpdeskState right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}