using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SBoard.Core.Data.Helpdesks;
using SBoard.Core.Services.Centron;
using SBoard.Core.Services.HelpdeskGroups;

namespace SBoard.Core.Queries.Helpdesks
{
    public class HelpdeskGroupQuery : IQuery<IList<HelpdeskPreview>>
    {
        public string HelpdeskListId { get; }

        public HelpdeskGroupQuery(string helpdeskListId)
        {
            this.HelpdeskListId = helpdeskListId;
        }
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
            var helpdeskLists = await this._helpdeskGroupsService.GetHelpdeskListsAsync();
            var helpdeskList = helpdeskLists.First(f => f.Id == query.HelpdeskListId);

            var helpdesks = await this._centronService.GetHelpdesksAsync(helpdeskList.WebServiceHelpdeskFilter.CustomerI3D, null);

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