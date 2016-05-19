using Irony.Compiler;

namespace BluLang.RubyScope.DSL
{
    public partial class Generator
    {
        /// <summary>
        /// Walk AST root node and search for "COMPSTMT"
        /// </summary>
        /// <param name="node">root node as AstNode</param>
        private void RootNode(AstNode node)
        {
            if (node == null) return;
            if (node is Token)
            {
                Token token = (Token) node;
                if (token.Terminal.Category != TokenCategory.Content) return;
            }
            if (node.ToString() == "COMPSTMT")
            {
                ROOT_COMPSTMT(node);
            }
        }
    }
}
