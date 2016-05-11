using System.Collections.Generic;
using System.Threading.Tasks;
using SBoard.Core.Data.HelpdeskGroups;

namespace SBoard.Core.Services.HelpdeskGroups
{
    public interface IHelpdeskGroupsService
    {
        Task<IList<HelpdeskGroup>> GetHelpdeskGroupsAsync();

        Task<HelpdeskGroup> AddHelpdeskGroupAsync(string name, int? customerI3D, bool onlyOwn, int? helpdeskTypeI3D, int? helpdeskStateI3D);

        Task DeleteHelpdeskGroupAsync(string id);
    }
}