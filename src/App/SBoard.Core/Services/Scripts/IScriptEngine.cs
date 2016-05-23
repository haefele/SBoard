using System;
using System.Collections.Generic;

namespace SBoard.Core.Services.Scripts
{
    public interface IScriptEngine
    {
        void AddGlobalData(string name, object data);
        void AddGlobalMethod(string name, Delegate method);

        Script CreateFor(string script);
    }
}