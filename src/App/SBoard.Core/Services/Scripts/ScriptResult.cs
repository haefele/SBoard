using MoonSharp.Interpreter;

namespace SBoard.Core.Services.Scripts
{
    public class ScriptResult
    {
        private readonly DynValue _result;

        internal ScriptResult(DynValue result)
        {
            this._result = result;
        }

        public bool AsBoolean()
        {
            return this._result.CastToBool();
        }

        public int AsInteger()
        {
            return (int) (this._result.CastToNumber() ?? 0);
        }

        public decimal AsDecimal()
        {
            return (decimal) (this._result.CastToNumber() ?? 0);
        }

        public string AsString()
        {
            return this._result.CastToString();
        }
    }
}