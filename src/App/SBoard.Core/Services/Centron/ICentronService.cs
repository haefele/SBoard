using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SBoard.Core.Services.Centron
{
    public interface ICentronService
    {
        Task LoginAsync();

        Task LogoutAsync();

        Task<IList<HelpdeskPreview>> GetHelpdesksAsync(int customerI3D);

        Task<IList<HelpdeskTimer>> GetHelpdeskTimersAsync(int helpdeskI3D);

        Task<IList<HelpdeskPriority>> GetHelpdeskPrioritiesAsync();

        Task<IList<HelpdeskState>> GetHelpdeskStatesAsync();

        Task<IList<HelpdeskType>> GetHelpdeskTypesAsync();

        Task<IList<HelpdeskCategory>> GetHelpdeskCategoriesAsync();
    }
}