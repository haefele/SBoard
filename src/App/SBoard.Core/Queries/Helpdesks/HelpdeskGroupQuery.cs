﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SBoard.Core.Data.Helpdesks;
using SBoard.Core.Services.Centron;
using SBoard.Core.Services.HelpdeskGroups;

namespace SBoard.Core.Queries.Helpdesks
{
    public class HelpdeskGroupQuery : IQuery<IList<HelpdeskPreview>>, IEquatable<HelpdeskGroupQuery>
    {
        public string HelpdeskListId { get; }

        public HelpdeskGroupQuery(string helpdeskListId)
        {
            this.HelpdeskListId = helpdeskListId;
        }

        #region Equality
        public bool Equals(HelpdeskGroupQuery other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(this.HelpdeskListId, other.HelpdeskListId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HelpdeskGroupQuery) obj);
        }

        public override int GetHashCode()
        {
            return (this.HelpdeskListId != null ? this.HelpdeskListId.GetHashCode() : 0);
        }

        public static bool operator ==(HelpdeskGroupQuery left, HelpdeskGroupQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HelpdeskGroupQuery left, HelpdeskGroupQuery right)
        {
            return !Equals(left, right);
        }
        #endregion
    }

    public class HelpdeskGroupQueryHandler : IQueryHandler<HelpdeskGroupQuery, IList<HelpdeskPreview>>
    {
        private readonly ICentronService _centronService;
        private readonly IHelpdeskGroupsService _helpdeskGroupsService;

        public HelpdeskGroupQueryHandler(ICentronService centronService, IHelpdeskGroupsService helpdeskGroupsService)
        {
            this._centronService = centronService;
            this._helpdeskGroupsService = helpdeskGroupsService;
        }

        public async Task<IList<HelpdeskPreview>> ExecuteAsync(HelpdeskGroupQuery query)
        {
            var helpdeskLists = await this._helpdeskGroupsService.GetHelpdeskGroupsAsync();
            var helpdeskList = helpdeskLists.First(f => f.Id == query.HelpdeskListId);

            var helpdesks = await this._centronService.GetHelpdesksAsync(
                helpdeskList.WebServiceHelpdeskFilter.CustomerI3D, 
                helpdeskList.WebServiceHelpdeskFilter.OnlyOwn);

            if (helpdeskList.ClientHelpdeskFilter != null)
            {
                helpdesks = helpdesks
                    .Where(helpdeskList.ClientHelpdeskFilter.Apply)
                    .ToList();
            }

            return helpdesks;
        }
    }
}