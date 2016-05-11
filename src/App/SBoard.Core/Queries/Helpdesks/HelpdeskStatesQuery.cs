using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using SBoard.Core.Data.Helpdesks;
using SBoard.Core.Services.Centron;

namespace SBoard.Core.Queries.Helpdesks
{
    public class HelpdeskStatesQuery : IQuery<IList<HelpdeskState>>, IEquatable<HelpdeskStatesQuery>
    {
        public bool OnlyActive { get; }

        public HelpdeskStatesQuery(bool onlyActive)
        {
            this.OnlyActive = onlyActive;
        }

        #region Equality
        public bool Equals(HelpdeskStatesQuery other)
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
            return Equals((HelpdeskStatesQuery) obj);
        }

        public override int GetHashCode()
        {
            return this.OnlyActive.GetHashCode();
        }

        public static bool operator ==(HelpdeskStatesQuery left, HelpdeskStatesQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HelpdeskStatesQuery left, HelpdeskStatesQuery right)
        {
            return !Equals(left, right);
        }
        #endregion
    }

    public class HelpdeskStatesQueryHandler : IQueryHandler<HelpdeskStatesQuery, IList<HelpdeskState>>
    {
        private readonly ICentronService _centronService;

        public HelpdeskStatesQueryHandler(ICentronService centronService)
        {
            this._centronService = centronService;
        }

        public async Task<IList<HelpdeskState>> ExecuteAsync(HelpdeskStatesQuery query)
        {
            var states = await this._centronService.GetHelpdeskStatesAsync();

            if (query.OnlyActive)
            {
                states = states
                    .Where(f => f.IsDeactivated == false)
                    .ToList();
            }

            return states;
        }
    }
}