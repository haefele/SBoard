using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UwCore.Common;

namespace SBoard.Core.Services.Scripts
{
    public class ScriptEngine : IScriptEngine
    {
        private readonly Dictionary<string, object> _globalData;
        private readonly Dictionary<string, Delegate> _globalMethods;

        public ScriptEngine()
        {
            this._globalData = new Dictionary<string, object>();
            this._globalMethods = new Dictionary<string, Delegate>();
        }

        public void AddGlobalData(string name, object data)
        {
            Guard.NotNullOrWhiteSpace(name, nameof(name));

            this._globalData[name] = data;
        }

        public void AddGlobalMethod(string name, Delegate method)
        {
            Guard.NotNullOrWhiteSpace(name, nameof(name));
            Guard.NotNull(method, nameof(method));

            this._globalMethods[name] = method;
        }

        public Script CreateFor(string script)
        {
            Guard.NotNullOrWhiteSpace(script, nameof(script));

            var luaScript = new MoonSharp.Interpreter.Script(CoreModules.Preset_HardSandbox);
            var result = new Script(luaScript, script);

            foreach (var pair in this._globalData)
            {
                result.AddData(pair.Key, pair.Value);
            }

            foreach (var pair in this._globalMethods)
            {
                result.AddMethod(pair.Key, pair.Value);
            }

            return result;
        }
    }
}