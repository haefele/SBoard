﻿using System;
using Newtonsoft.Json.Linq;

namespace SBoard.Core.Data.Customers
{
    public class CustomerPreview : IEquatable<CustomerPreview>
    {
        public int I3D { get; set; }
        public string Name { get; set; }

        public JObject Original { get; set; }

        #region Equality
        public bool Equals(CustomerPreview other)
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
            return Equals((CustomerPreview) obj);
        }

        public override int GetHashCode()
        {
            return this.I3D;
        }

        public static bool operator ==(CustomerPreview left, CustomerPreview right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CustomerPreview left, CustomerPreview right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}