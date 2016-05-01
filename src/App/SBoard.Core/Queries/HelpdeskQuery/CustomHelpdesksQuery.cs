using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SBoard.Core.Data.Helpdesks;
using SBoard.Core.Services.Centron;
using SBoard.Core.Services.HelpdeskLists;

namespace SBoard.Core.Queries.HelpdeskQuery
{
    public class CustomHelpdesksQuery : IQuery<IList<HelpdeskPreview>>
    {
        public string HelpdeskListId { get; }

        public CustomHelpdesksQuery(string helpdeskListId)
        {
            this.HelpdeskListId = helpdeskListId;
        }
    }

    public class CustomHelpdesksQueryHandler : IQueryHandler<CustomHelpdesksQuery, IList<HelpdeskPreview>>
    {
        private readonly ICentronService _centronService;
        private readonly IHelpdeskListsService _helpdeskListsService;

        public CustomHelpdesksQueryHandler(ICentronService centronService, IHelpdeskListsService helpdeskListsService)
        {
            this._centronService = centronService;
            this._helpdeskListsService = helpdeskListsService;
        }

        public async Task<IList<HelpdeskPreview>> ExecuteAsync(CustomHelpdesksQuery query)
        {
            var helpdeskLists = await this._helpdeskListsService.GetHelpdeskListsAsync();
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