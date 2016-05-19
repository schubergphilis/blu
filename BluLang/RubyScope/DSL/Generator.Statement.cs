using System;
using System.Linq;
using Irony.Compiler;

namespace BluLang.RubyScope.DSL
{
    /// <summary>
    /// Includes all AST Statement walker methods:
    /// - COMPTSTMT (Composite Statement)
    /// - STMTLIST (Statement List)
    /// - STMT (Statement)
    /// </summary>
    public partial class Generator
    {
        /// <summary>
        /// Walk COMPSTMT (Composite Statement) node directly under the ROOT node
        /// </summary>
        /// <param name="node">COMPSTMT node as AstNode</param>
        private void ROOT_COMPSTMT(AstNode node)
        {
            GenericNode genNode = node as GenericNode;
            if (genNode == null) return;
            foreach (AstNode child in genNode.ChildNodes.Where(child => child != null))
            {
                if (child is Token)
                {
                    Token token = child as Token;
                    // We are not looking for terminal tokens directly in composite statement
                    // If token is terminal, ignore and continue
                    if (token.Terminal.Category != TokenCategory.Content) continue;
                }
                switch (child.ToString())
                {
                    case "STMT_list":
                        // If child node is STMT_list (Statement List) walk STMTLIST
                        ROOT_COMPSTMT_STMTLIST(child);
                        break;

                    case "STMT":
                        // If child node is STMT (Statement) walk STMT
                        ROOT_COMPSTMT_STMTLIST_STMT(child);
                        break;
                }
            }
        }

        /// <summary>
        /// Walk STMTLIST (Statement List) node under COMPSTMT
        /// </summary>
        /// <param name="node">STMTLIST as AstNode</param>
        private void ROOT_COMPSTMT_STMTLIST(AstNode node)
        {
            GenericNode genNode = node as GenericNode;
            if (genNode == null) return;
            foreach (AstNode child in genNode.ChildNodes.Where(child => child != null))
            {
                if (child is Token)
                {
                    Token token = child as Token;
                    // We are not looking for terminal tokens directly under STMT_list
                    // If token is terminal, ignore and continue
                    if (token.Terminal.Category != TokenCategory.Content) continue;
                }
                switch (child.ToString())
                {
                    case "STMT":
                        // If child node is STMT (Statement) walk STMT
                        ROOT_COMPSTMT_STMTLIST_STMT(child);
                        break;
                }
            }
        }

        /// <summary>
        /// Walk STMT (Statement) nodes in STMTLIST
        /// </summary>
        /// <param name="node">STMT as AstNode</param>
        private void ROOT_COMPSTMT_STMTLIST_STMT(AstNode node)
        {
            GenericNode genNode = node as GenericNode;
            if (genNode == null) return;

            for (int i = 0; i < genNode.ChildNodes.Count; i++)
            {
                AstNode child = genNode.ChildNodes[i];
                if (child == null) continue;
                if (child is Token)
                {
                    Token token = child as Token;
                    // We are not looking for terminal tokens directly under STMT
                    // If token is terminal, ignore and continue
                    if (token.Terminal.Category != TokenCategory.Content) continue;
                }
                switch (child.ToString())
                {
                    case "COMMAND":
                        // Child node is "COMMAND"
                        // We also need the next child node (ChildNodes[i + 1])
                        AstNode next = null;
                        try { next = genNode.ChildNodes[i + 1]; }
                        catch (Exception)
                        {
                            // ignored
                        }
                        // Walk COMMAND node
                        ROOT_COMPSTMT_STMTLIST_STMT_COMMAND(child, next);
                        break;
                }
            }
        }
    }
}
