using Irony.Compiler;

namespace BluStation.BluSprint
{
    public partial class Sprint
    {
        public string ResourcesPs1Content { get; set; }
        public string SprintPs1Content { get; set; }
        public string TranspiledPowerShell { get; set; }
        
        public string RecipeStack { get; set; }
        public string AttributesStack { get; set; }
        public string RubyStack { get; set; }

        // RubyTops
        public SyntaxErrorList SyntaxErrors { get; set; }
        public AstNode AstRootNode { get; set; }
        public string CompiledJson { get; set; }

        public string Mode { get; set; }
    }
}
