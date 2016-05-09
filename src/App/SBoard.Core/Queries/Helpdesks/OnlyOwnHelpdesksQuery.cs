using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SBoard.Core.Data.Helpdesks;
using SBoard.Core.Services.Centron;

namespace SBoard.Core.Queries.Helpdesks
{
    public class OnlyOwnHelpdesksQuery : IQuery<IList<HelpdeskPreview>>, IEquatable<OnlyOwnHelpdesksQuery>
    {
        #region Equality
        public bool Equals(OnlyOwnHelpdesksQuery other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OnlyOwnHelpdesksQuery) obj);
        }

        public override int GetHashCode()
        {
            return -1;
        }

        public static bool operator ==(OnlyOwnHelpdesksQuery left, OnlyOwnHelpdesksQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OnlyOwnHelpdesksQuery left, OnlyOwnHelpdesksQuery right)
        {
            return !Equals(left, right);
        }
        #endregion
    }

    public class OnlyOwnHelpdesksQueryHandler : IQueryHandler<OnlyOwnHelpdesksQuery, IList<HelpdeskPreview>>
    {
        private readonly ICentronService _centronService;

        public OnlyOwnHelpdesksQueryHandler(ICentronService centronService)
        {
            this._centronService = centronService;
        }

        public async Task<IList<HelpdeskPreview>> ExecuteAsync(OnlyOwnHelpdesksQuery query)
        {
            return await this._centronService.GetHelpdesksAsync(null, true);
        }
    }
}