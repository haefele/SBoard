using System;
using Newtonsoft.Json.Linq;

namespace SBoard.Core.Services.Centron
{
    public class Employee : IEquatable<Employee>
    {
        public int I3D { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShortSign { get; set; }
        public string EmailAddress { get; set; }
        
        public JObject Original { get; set; }

        #region Equality
        public bool Equals(Employee other)
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
            return this.Equals((Employee) obj);
        }

        public override int GetHashCode()
        {
            return this.I3D;
        }

        public static bool operator ==(Employee left, Employee right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Employee left, Employee right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}