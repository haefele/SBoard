using System.Collections.Generic;
using System.Threading.Tasks;
using SBoard.Core.Data.Helpdesks;
using SBoard.Core.Services.Centron;

namespace SBoard.Core.Queries.Helpdesks
{
    public class OnlyOwnHelpdesksQuery : IQuery<IList<HelpdeskPreview>>
    {

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