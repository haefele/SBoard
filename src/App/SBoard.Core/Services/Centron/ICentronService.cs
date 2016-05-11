using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SBoard.Core.Data.Customers;
using SBoard.Core.Data.Helpdesks;

namespace SBoard.Core.Services.Centron
{
    public interface ICentronService
    {
        Task TestLoginAsync(string webServiceAddress, string username, string password);

        Task LogoutAsync();

        Task<IList<HelpdeskPreview>> GetHelpdesksAsync(int? customerI3D, bool onlyOwn, int? helpdeskStateI3D);

        Task<IList<HelpdeskTimer>> GetHelpdeskTimersAsync(int helpdeskI3D);

        Task<IList<HelpdeskPriority>> GetHelpdeskPrioritiesAsync();

        Task<IList<HelpdeskState>> GetHelpdeskStatesAsync();

        Task<IList<HelpdeskType>> GetHelpdeskTypesAsync();

        Task<IList<HelpdeskCategory>> GetHelpdeskCategoriesAsync();

        Task<IList<CustomerPreview>> GetCustomersAsync(string searchText);
    }
}