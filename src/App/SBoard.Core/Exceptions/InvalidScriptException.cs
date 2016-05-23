using System;
using SBoard.Core.Strings;

namespace SBoard.Core.Exceptions
{
    public class InvalidScriptException : SBoardException
    {
        public InvalidScriptException(Exception innerException)
            : base(SBoardCoreResources.Get("Exception.InvalidScript"), innerException)
        {
            
        }
    }
}