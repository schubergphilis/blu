using System;
using System.Linq;
using Irony.Compiler;

namespace BluLang.RubyScope.DSL
{
    /// <summary>
    /// Includes COMMAND walker methods:
    /// - COMMAND node directly under STMT
    /// - COMMAND node in BLOCK (it represent Chef resource parameters)
    /// </summary>
    public partial class Generator
    {
        private void ROOT_COMPSTMT_STMTLIST_STMT_COMMAND(AstNode node, AstNode next)
        {
            GenericNode genNode = node as GenericNode;
            if (genNode == null) return;

            string att = String.Empty;
            string res = String.Empty;
            AstNode args = null;
            AstNode block = null;

            int type = 0; // 0 = resource 1 = attribute

            foreach (AstNode child in genNode.ChildNodes.Where(child => child != null))
            {
                if (child is Token)
                {
                    Token token = child as Token;
                    if (token.Terminal.Category != TokenCategory.Content) continue;
                    if (next != null)
                    {
                        if (next.ToString() != "BLOCK") continue;
                        // Next node is BLOCK: then COMMAND represent a Chef resource, set type to 1 (resource)
                        res = token.Text;
                        block = next;
                        type = 1;
                    }
                    else
                    {
                        // It is a Chef attribute, set type to 0 (attribute)
                        att = token.Text;
                        type = 0;
                    }
                }
                else
                {
                    if (child.ToString() == "CALL_ARGS")
                    {
                        args = child;
                    }
                }
            }
            switch (type)
            {
                case 0:
                    Attribute(att, args);
                    break;
                case 1:
                    Resource(res, args, block);
                    break;
            }
        }


        private void ROOT_COMPSTMT_STMTLIST_STMT_BLOCK_COMMAND(AstNode node)
        {
            GenericNode genNode = node as GenericNode;
            if (genNode == null) return;
            foreach (AstNode child in genNode.ChildNodes.Where(child => child != null))
            {
                if (child is Token)
                {
                    Token token = child as Token;
                    if (token.Terminal.Category == TokenCategory.Content)
                    {
                        _parameterKey = token.Text;
                    }
                }
                else if (child.ToString() == "CALL_ARGS" || child.ToString() == "COMMAND")
                {
                    _parameterValue.Clear();
                    ROOT_COMPSTMT_STMTLIST_STMT_BLOCK_CALLARGS(child);
                }
            }
        }
    }
}
