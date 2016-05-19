using System;
using System.Collections.Generic;
using System.Linq;
using Irony.Compiler;

namespace BluLang.RubyScope.DSL
{
    public partial class Generator
    {

        private string _parameterKey;
        private List<string> _parameterValue = new List<string>(); 
        
        private void ROOT_COMPSTMT_STMTLIST_STMT_BLOCK(AstNode node)
        {
            GenericNode genNode = node as GenericNode;
            if (genNode == null) return;
            foreach (AstNode child in genNode.ChildNodes.Where(child => child != null))
            {
                if (node.ToString() == "STMT" && child.ToString() == "COMMAND")
                {
                    _parameterKey = String.Empty;
                    _parameterValue.Clear();

                    ROOT_COMPSTMT_STMTLIST_STMT_BLOCK_COMMAND(child);
                    _resourceParameters.Add(_parameterKey, ResourceTransformer(_resourceUniqeName, _parameterKey, _parameterValue));
                }
                ROOT_COMPSTMT_STMTLIST_STMT_BLOCK(child);
            }
        }
    }
}
