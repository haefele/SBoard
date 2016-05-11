using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBoard.Core.Data.Helpdesks;
using SBoard.Core.Services.Centron;

namespace SBoard.Core.Queries.Helpdesks
{
    public class HelpdeskTypesQuery : IQuery<IList<HelpdeskType>>, IEquatable<HelpdeskTypesQuery>
    {
        public bool OnlyActive { get; }

        public HelpdeskTypesQuery(bool onlyActive)
        {
            this.OnlyActive = onlyActive;
        }

        #region Equality
        public bool Equals(HelpdeskTypesQuery other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.OnlyActive == other.OnlyActive;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HelpdeskTypesQuery) obj);
        }

        public override int GetHashCode()
        {
            return this.OnlyActive.GetHashCode();
        }

        public static bool operator ==(HelpdeskTypesQuery left, HelpdeskTypesQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HelpdeskTypesQuery left, HelpdeskTypesQuery right)
        {
            return !Equals(left, right);
        }
        #endregion
    }

    public class HelpdeskTypesQueryHandler : IQueryHandler<HelpdeskTypesQuery, IList<HelpdeskType>>
    {
        private readonly ICentronService _centronService;

        public HelpdeskTypesQueryHandler(ICentronService centronService)
        {
            this._centronService = centronService;
        }

        public async Task<IList<HelpdeskType>> ExecuteAsync(HelpdeskTypesQuery query)
        {
            var types = await this._centronService.GetHelpdeskTypesAsync();

            if (query.OnlyActive)
            {
                types = types
                    .Where(f => f.IsDeactivated == false)
                    .ToList();
            }

            return types;
        }
    }
}
