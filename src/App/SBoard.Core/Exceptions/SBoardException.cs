using System;

namespace SBoard.Core.Common
{
    public class SBoardException : Exception
    {
        public SBoardException(string message) : base(message)
        {
        }

        public SBoardException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}