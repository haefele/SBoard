using System.Collections.Generic;
using UwCore.Services.ApplicationState;
using AState = UwCore.Services.ApplicationState.ApplicationState;

namespace SBoard.Core.Services.ApplicationState
{
    public static class ApplicationStateServiceExtensions
    {
        public static string GetWebServiceAddress(this IApplicationStateService self)
        {
            return self.Get<string>("WebServiceAddress", AState.Roaming);
        }
        public static void SetWebServiceAddress(this IApplicationStateService self, string webServiceAddress)
        {
            self.Set("WebServiceAddress", webServiceAddress, AState.Roaming);
        }

        public static string GetUsername(this IApplicationStateService self)
        {
            return self.Get<string>("Username", AState.Roaming);
        }
        public static void SetUsername(this IApplicationStateService self, string username)
        {
            self.Set("Username", username, AState.Roaming);
        }

        public static string GetPassword(this IApplicationStateService self)
        {
            return self.Get<string>("Password", AState.Vault);
        }
        public static void SetPassword(this IApplicationStateService self, string password)
        {
            self.Set("Password", password, AState.Vault);
        }

        public static IList<TicketList> GetTicketLists(this IApplicationStateService self)
        {
            return self.Get<IList<TicketList>>("TicketLists", AState.Roaming) ?? new List<TicketList>();
        }
        public static void SetTicketLists(this IApplicationStateService self, IList<TicketList> ticketLists)
        {
            self.Set("TicketLists", ticketLists, AState.Roaming);
        }
    }
}