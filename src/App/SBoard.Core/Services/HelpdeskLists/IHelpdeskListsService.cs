using System.Collections.Generic;
using System.Threading.Tasks;
using SBoard.Core.Data.HelpdeskLists;
using SBoard.Core.Services.ApplicationState;

namespace SBoard.Core.Services.HelpdeskLists
{
    public interface IHelpdeskListsService
    {
        Task<IList<HelpdeskList>> GetHelpdeskListsAsync();

        Task<HelpdeskList> AddHelpdeskListAsync(string name, WebServiceHelpdeskFilter webServiceHelpdeskFilter, ClientHelpdeskFilter clientHelpdeskFilter);

        Task DeleteHelpdeskListAsync(string id);
    }
}