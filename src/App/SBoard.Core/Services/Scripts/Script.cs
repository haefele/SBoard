using System;
using MoonSharp.Interpreter;
using SBoard.Core.Exceptions;
using UwCore.Common;
using LuaScript = MoonSharp.Interpreter.Script;

namespace SBoard.Core.Services.Scripts
{
    public class Script
    {
        private readonly LuaScript _script;
        private readonly string _code;

        public Script(LuaScript script, string code)
        {
            Guard.NotNull(script, nameof(script));
            Guard.NotNullOrWhiteSpace(code, nameof(code));

            this._script = script;
            this._code = code;
        }

        public void AddData(string name, object data)
        {
            Guard.NotNullOrWhiteSpace(name, nameof(name));

            this._script.Globals[name] = data;
        }

        public void AddMethod(string name, Delegate method)
        {
            Guard.NotNullOrWhiteSpace(name, nameof(name));
            Guard.NotNull(method, nameof(method));

            this._script.Globals[name] = method;
        }

        public ScriptResult Execute()
        {
            try
            {
                DynValue result = this._script.DoString(this._code);
                return new ScriptResult(result);
            }
            catch (InterpreterException exception)
            {
                throw new InvalidScriptException(exception);
            }
        }
    }
}