using System.Collections.Generic;
using System.Threading.Tasks;
using SBoard.Core.Data.HelpdeskGroups;

namespace SBoard.Core.Services.HelpdeskGroups
{
    public interface IHelpdeskGroupsService
    {
        Task<IList<HelpdeskGroup>> GetHelpdeskListsAsync();

        Task<HelpdeskGroup> AddHelpdeskListAsync(string name, WebServiceHelpdeskFilter webServiceHelpdeskFilter, ClientHelpdeskFilter clientHelpdeskFilter);

        Task DeleteHelpdeskListAsync(string id);
    }
}