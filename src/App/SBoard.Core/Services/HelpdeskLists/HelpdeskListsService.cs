using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SBoard.Core.Data.HelpdeskLists;
using SBoard.Core.Services.ApplicationState;
using UwCore.Services.ApplicationState;

namespace SBoard.Core.Services.HelpdeskLists
{
    public class HelpdeskListsService : IHelpdeskListsService
    {
        private const string Key = "HelpdeskLists";

        private readonly IApplicationStateService _applicationStateService;

        public HelpdeskListsService(IApplicationStateService applicationStateService)
        {
            this._applicationStateService = applicationStateService;
        }

        public Task<IList<HelpdeskList>> GetHelpdeskListsAsync()
        {
            var helpdeskLists = this._applicationStateService.Get<List<HelpdeskList>>(Key, UwCore.Services.ApplicationState.ApplicationState.Roaming);
            return Task.FromResult((IList<HelpdeskList>)helpdeskLists);
        }

        public async Task<HelpdeskList> AddHelpdeskListAsync(string name, WebServiceHelpdeskFilter webServiceHelpdeskFilter, ClientHelpdeskFilter clientHelpdeskFilter)
        {
            var item = new HelpdeskList
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = name,
                WebServiceHelpdeskFilter = webServiceHelpdeskFilter,
                ClientHelpdeskFilter = clientHelpdeskFilter
            };
            var helpdeskLists = await this.GetHelpdeskListsAsync();
            helpdeskLists.Add(item);

            this._applicationStateService.Set(Key, helpdeskLists, UwCore.Services.ApplicationState.ApplicationState.Roaming);

            return item;
        }

        public async Task DeleteHelpdeskListAsync(string id)
        {
            var helpdeskLists = await this.GetHelpdeskListsAsync();
            var found = helpdeskLists.FirstOrDefault(f => f.Id == id);

            helpdeskLists.Remove(found);

            this._applicationStateService.Set(Key, helpdeskLists, UwCore.Services.ApplicationState.ApplicationState.Roaming);
        }
    }
}