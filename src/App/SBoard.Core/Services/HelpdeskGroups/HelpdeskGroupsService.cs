using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SBoard.Core.Data.HelpdeskGroups;
using UwCore.Services.ApplicationState;

namespace SBoard.Core.Services.HelpdeskGroups
{
    public class HelpdeskGroupsService : IHelpdeskGroupsService
    {
        private const string Key = "HelpdeskGroups";

        private readonly IApplicationStateService _applicationStateService;

        public HelpdeskGroupsService(IApplicationStateService applicationStateService)
        {
            this._applicationStateService = applicationStateService;
        }

        public Task<IList<HelpdeskGroup>> GetHelpdeskListsAsync()
        {
            var helpdeskLists = this._applicationStateService.Get<List<HelpdeskGroup>>(Key, UwCore.Services.ApplicationState.ApplicationState.Roaming);
            return Task.FromResult((IList<HelpdeskGroup>)helpdeskLists ?? new List<HelpdeskGroup>());
        }

        public async Task<HelpdeskGroup> AddHelpdeskListAsync(string name, WebServiceHelpdeskFilter webServiceHelpdeskFilter, ClientHelpdeskFilter clientHelpdeskFilter)
        {
            var item = new HelpdeskGroup
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