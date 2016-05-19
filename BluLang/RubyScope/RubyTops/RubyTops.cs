using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluApi.Common;
using BluLang.RubyScope.DSL;
using Irony.Compiler;

namespace BluLang.RubyScope.RubyTops
{
    public partial class RubyTops
    {
        public string RubyScript { get; set; }
        public SyntaxErrorList SyntaxErrors { get; set; }
        public AstNode AstRootNode { get; set; }
        public Dictionary<string, dynamic> GeneratedDictionary { get; set; }
        public string CompiledJson { get; set; }
        public string TranspiledPowerShell { get; set; }

        public void Transpile()
        {
            Grammar rubyGrammar = new RubyGrammar();
            LanguageCompiler rubyCompiler;
            SyntaxErrors = new SyntaxErrorList();
            Generator generator = new Generator();

            try
            {
                rubyCompiler = new LanguageCompiler(rubyGrammar);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Ruby Grammar: " + ex.Message);
            }

            try
            {
                AstRootNode = rubyCompiler.Parse(RubyScript);
                SyntaxErrors = rubyCompiler.Context.Errors;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Parser error: " + rubyCompiler.Parser.CurrentState + " " + ex.Message);
            }

            if (SyntaxErrors.Count > 0)
            {
                foreach (SyntaxError err in SyntaxErrors)
                {
                    Logger.log("error", err + " >> " + RubyScript.Insert(err.Location.Position, "§"));
                }
            }
            try
            {
                GeneratedDictionary = generator.Generate(AstRootNode).Dictionary;
            }
            catch (Exception ex)
            {
                Logger.log("error", "Compile error: " + ex.Message);
            }

            try
            {
                TranspiledPowerShell = ToPowerShell();
            }
            catch (Exception ex)
            {
                Logger.log("error", "Error transpiling to PowerShell: " + ex.Message);
            }

            try
            {
                CompiledJson = JsonHelper.Serialize(GeneratedDictionary);
            }
            catch (Exception ex)
            {
                Logger.log("error", "Error generating json result: " + ex.Message);
            }
        }
    }
}
