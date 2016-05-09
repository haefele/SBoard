using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SBoard.Core.Data.Customers;
using SBoard.Core.Services.Centron;

namespace SBoard.Core.Queries.Customers
{
    public class SearchCustomersQuery : IQuery<IList<CustomerPreview>>, IEquatable<SearchCustomersQuery>
    {
        public string SearchText { get; }

        public SearchCustomersQuery(string searchText)
        {
            this.SearchText = searchText;
        }

        #region Equality
        public bool Equals(SearchCustomersQuery other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(this.SearchText, other.SearchText);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SearchCustomersQuery) obj);
        }

        public override int GetHashCode()
        {
            return (this.SearchText != null ? this.SearchText.GetHashCode() : 0);
        }

        public static bool operator ==(SearchCustomersQuery left, SearchCustomersQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SearchCustomersQuery left, SearchCustomersQuery right)
        {
            return !Equals(left, right);
        }
        #endregion
    }

    public class SearchCustomersQueryHandler : IQueryHandler<SearchCustomersQuery, IList<CustomerPreview>>
    {
        private readonly ICentronService _centronService;

        public SearchCustomersQueryHandler(ICentronService centronService)
        {
            this._centronService = centronService;
        }

        public Task<IList<CustomerPreview>> ExecuteAsync(SearchCustomersQuery query)
        {
            return this._centronService.GetCustomersAsync(query.SearchText);
        }
    }
}