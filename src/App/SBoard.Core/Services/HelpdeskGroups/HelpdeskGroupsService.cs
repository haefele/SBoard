using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using SBoard.Core.Data.HelpdeskGroups;
using SBoard.Core.Services.HelpdeskGroups.Events;
using UwCore.Services.ApplicationState;

namespace SBoard.Core.Services.HelpdeskGroups
{
    public class HelpdeskGroupsService : IHelpdeskGroupsService
    {
        private const string Key = "HelpdeskGroups";

        private readonly IApplicationStateService _applicationStateService;
        private readonly IEventAggregator _eventAggregator;

        public HelpdeskGroupsService(IApplicationStateService applicationStateService, IEventAggregator eventAggregator)
        {
            this._applicationStateService = applicationStateService;
            this._eventAggregator = eventAggregator;
        }

        public Task<IList<HelpdeskGroup>> GetHelpdeskGroupsAsync()
        {
            var helpdeskLists = this._applicationStateService.Get<List<HelpdeskGroup>>(Key, UwCore.Services.ApplicationState.ApplicationState.Roaming);
            return Task.FromResult((IList<HelpdeskGroup>)helpdeskLists ?? new List<HelpdeskGroup>());
        }

        public async Task<HelpdeskGroup> AddHelpdeskGroupAsync(string name, int? customerI3D, bool onlyOwn, int? helpdeskTypeI3D, int? helpdeskStateI3D)
        {
            var item = new HelpdeskGroup
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = name,
                CustomerI3D = customerI3D,
                OnlyOwn = onlyOwn,
                HelpdeskTypeI3D = helpdeskTypeI3D,
                HelpdeskStateI3D = helpdeskStateI3D
            };
            var helpdeskLists = await this.GetHelpdeskGroupsAsync();
            helpdeskLists.Add(item);

            this._applicationStateService.Set(Key, helpdeskLists, UwCore.Services.ApplicationState.ApplicationState.Roaming);

            this._eventAggregator.PublishOnCurrentThread(new HelpdeskGroupAdded(item));

            return item;
        }

        public async Task DeleteHelpdeskGroupAsync(string id)
        {
            var helpdeskLists = await this.GetHelpdeskGroupsAsync();
            var found = helpdeskLists.FirstOrDefault(f => f.Id == id);

            helpdeskLists.Remove(found);

            this._applicationStateService.Set(Key, helpdeskLists, UwCore.Services.ApplicationState.ApplicationState.Roaming);

            this._eventAggregator.PublishOnCurrentThread(new HelpdeskGroupDeleted(found));
        }
    }
}