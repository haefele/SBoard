using System.Collections.Generic;
using System.Threading.Tasks;
using SBoard.Core.Data.Customers;
using SBoard.Core.Services.Centron;

namespace SBoard.Core.Queries.Customers
{
    public class AllCustomersQuery : IQuery<IList<CustomerPreview>>
    {
        
    }

    public class AllCustomersQueryHandler : IQueryHandler<AllCustomersQuery, IList<CustomerPreview>>
    {
        private readonly ICentronService _centronService;

        public AllCustomersQueryHandler(ICentronService centronService)
        {
            this._centronService = centronService;
        }

        public Task<IList<CustomerPreview>> ExecuteAsync(AllCustomersQuery query)
        {
            return this._centronService.GetCustomersAsync();
        }
    }
}