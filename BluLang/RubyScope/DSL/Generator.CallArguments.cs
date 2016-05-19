using System.Linq;
using BluApi.Common;
using Irony.Compiler;

namespace BluLang.RubyScope.DSL
{
    public partial class Generator
    {

        private void ROOT_COMPSTMT_STMTLIST_STMT_COMMAND_CALLARGS(string att, string res, AstNode node)
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
                        if (att != null && token.Text != "[" && token.Text != "]")
                        {
                            _attList.Add(token.Text.Replace("'", ""));
                        }
                        else if (res != null && token.Text != "[" && token.Text != "]")
                        {
                            _resourceUniqeName += "->" + token.Text.SingleQuote();
                        }
                    }
                }
                ROOT_COMPSTMT_STMTLIST_STMT_COMMAND_CALLARGS(att, res, child);
            }
        }

        private void ROOT_COMPSTMT_STMTLIST_STMT_BLOCK_CALLARGS(AstNode node)
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
                        // _parameterValue += token.Text;
                        _parameterValue.Add(token.Text); 
                    }
                }
                ROOT_COMPSTMT_STMTLIST_STMT_BLOCK_CALLARGS(child);
            }
        }
    }
}
