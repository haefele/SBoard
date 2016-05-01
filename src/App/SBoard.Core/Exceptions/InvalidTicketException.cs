using SBoard.Core.Strings;

namespace SBoard.Core.Exceptions
{
    public class InvalidTicketException : SBoardException
    {
        public InvalidTicketException() 
            : base(SBoardCoreResources.Get("Exception.InvalidTicket"))
        {
        }
    }
}