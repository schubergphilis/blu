using System;
using System.Collections.Generic;
using Irony.Compiler;

namespace BluLang.RubyScope.DSL
{
    public partial class Generator
    {
        /// <summary>
        /// Chef Resource unique name, e.g directly=>c:\\my_directory
        /// When adding this unique name to _result["resource"], if the name is duplicate, an exeption will raise
        /// We catch exception to clearly return that the resource+name (unique name) can not be duplicate
        /// </summary>
        private string _resourceUniqeName = String.Empty;
        private Dictionary<string, string> _resourceParameters = new Dictionary<string, string>();

        /// <summary>
        /// Adds a Chef resource to the resource list (_result["resource"])
        /// </summary>
        /// <param name="res">Resource unique name </param>
        /// <param name="args">Arguments node as AstNode</param>
        /// <param name="resBlock">Resource BLOCK as AstNode</param>
        private void Resource(string res, AstNode args, AstNode resBlock)
        {
            _resourceUniqeName = res;
            _resourceParameters = new Dictionary<string, string>();
            ROOT_COMPSTMT_STMTLIST_STMT_COMMAND_CALLARGS(null, res, args);
            ROOT_COMPSTMT_STMTLIST_STMT_BLOCK(resBlock);
            try
            {
                _result["resources"].Add(_resourceUniqeName, _resourceParameters);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("resource " + _resourceUniqeName + " is duplicate. " + ex.Message);
            }
        }
    }
}

