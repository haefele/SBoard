using System.Collections.Generic;
using System.Threading.Tasks;
using SBoard.Core.Data.HelpdeskGroups;

namespace SBoard.Core.Services.HelpdeskGroups
{
    public interface IHelpdeskGroupsService
    {
        Task<IList<HelpdeskGroup>> GetHelpdeskGroupsAsync();

        Task<HelpdeskGroup> AddHelpdeskGroupAsync(string name, WebServiceHelpdeskFilter webServiceHelpdeskFilter, ClientHelpdeskFilter clientHelpdeskFilter);

        Task DeleteHelpdeskGroupAsync(string id);
    }
}