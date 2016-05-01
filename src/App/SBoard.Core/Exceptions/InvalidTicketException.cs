using System;
using SBoard.Core.Strings;

namespace SBoard.Core.Common
{
    public class InvalidTicketException : SBoardException
    {
        public InvalidTicketException() 
            : base(SBoardCoreResources.Get("Exception.InvalidTicket"))
        {
        }
    }
}