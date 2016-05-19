using BluLang.RubyScope.RubyTops;
using ReturnType = BluApi.Common.Function;

namespace BluStation.BluSprint
{
    public partial class Sprint
    {
        public void Compile()
        {
            RubyTops rubyTops = new RubyTops { RubyScript = RubyStack };
            rubyTops.Transpile();
            AstRootNode = rubyTops.AstRootNode;
            CompiledJson = rubyTops.CompiledJson;
            SyntaxErrors = rubyTops.SyntaxErrors;
            TranspiledPowerShell = rubyTops.TranspiledPowerShell;
            ImportTranspiledPowerShell();
        }
    }
}
