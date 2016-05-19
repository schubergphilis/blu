using System.Collections.Generic;
using BluApi.Common;
using Irony.Compiler;
using ReturnType = BluApi.Common.Function;

namespace BluLang.RubyScope.DSL
{
    /// <summary>
    /// Chef DSL to PowerShell dictionary (hash) generator
    /// </summary>
    public partial class Generator
    {
        // Result dictionary that is returned finally by Generator
        private Dictionary<string, dynamic> _result = new Dictionary<string, dynamic>();

        /// <summary>
        /// Generate method
        /// </summary>
        /// <param name="rootNode">root node as AST node</param>
        /// <returns>Multiple Type rt (result as rt.Dictionary)</returns>
        public Function Generate(AstNode rootNode)
        {
            ReturnType rt = new ReturnType();
            _result.Clear();
            _result.Add("attributes", new Dictionary<string, dynamic>());
            _result.Add("resources", new Dictionary<string, dynamic>());
            _result.Add("notifiers", new Dictionary<string, dynamic>());
            RootNode(rootNode);
            rt.Dictionary = _result;
            return rt;
        }
    }
}
