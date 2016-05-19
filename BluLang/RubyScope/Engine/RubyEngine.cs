using System.Collections.Generic;
using IronRuby;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting;

namespace BluLang.RubyScope.Engine
{
    public class RubyEngine
    {
        readonly ScriptEngine _engine;
        readonly ExceptionOperations _exceptionOperations;
        SortedDictionary<string, object> _inputVariables;
        string _script;
 
        public RubyEngine()
        {
            _engine = Ruby.CreateEngine();
            _exceptionOperations = _engine.GetService<ExceptionOperations>();
        }
 
        public SortedDictionary<string, object> ScriptVariables
        {
            set { _inputVariables = value; }
        }
 
        public string Script
        {
            set { _script = value; }
        }
 
        internal ExceptionOperations ExceptionOperations
        {
            get { return _exceptionOperations; }
        }
 
        public SortedDictionary<string, object> Execute()
        {
            //Create structures
            const SourceCodeKind sc = SourceCodeKind.Statements;
            ScriptSource source = _engine.CreateScriptSourceFromString(_script, sc);
            ScriptScope scope = _engine.CreateScope();
            //Fill input variables
            foreach (KeyValuePair<string, object> variable in _inputVariables)
            {
                scope.SetVariable(variable.Key, variable.Value);
            }
            SortedDictionary<string, object> outputVariables = new SortedDictionary<string, object>();
            //Execute the script
            source.Execute(scope);
            //Recover variables
            foreach (string variable in scope.GetVariableNames())
            {
                outputVariables.Add(variable, scope.GetVariable(variable));
            }
            return outputVariables;
        }
    }
}
