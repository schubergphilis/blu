using System;
using System.Reflection;
using System.Management.Automation;
using BluApi.Chef.ChefAPI;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;


namespace BluStation.CmdLets
{
    /// <summary>
    /// A CmdLet to debug methods: it's just a place for quick and dirty tests
    /// It might be useful in the future for unit testing
    /// TODO: Decide to keep or retire this CmdLet
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Debug, "Methods")]
    public class DebugMethods : PSCmdlet
    {
        /// <summary>
        /// Main Processing function of the CmdLet
        /// </summary>
        protected override void ProcessRecord()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => Assembly.Load(AssemblyResolver.ResolveAssembly(args));
            ChefConfigurator chefConfigurator = new ChefConfigurator();
            chefConfigurator.LoadConfig();

            // 01. Testing BuildRunList method
            // BuildRunList();

            // 02. Testing Compilation
            TestCompile();

        }

        /// <summary>
        /// Testing BuildRunList method
        /// </summary>
        /// <returns>rt: see BluApi.Common.Function</returns>
        public Function BuildRunList()
        {
            BluSprint.Sprint chefRuntime = new BluSprint.Sprint();
            var rt = chefRuntime.BuildRunList();
            return rt;
        }


        public void TestCompile()
        {
            string ruby = @"
registry_key ""HKEY_LOCAL_MACHINE\\Software\\Blu\\Test"" do
    name            'PingWho'
    type            :string
    content          node['blu']['ping_who'] 
    recursive        true
    action          :create
    notifies :run,  'execute[ping_localhost]', :immediately    
end


execute ""ping_localhost"" do
    command         'ping localhost'
    action          :run
end
";

            /*
            Compiler rubyCompiler = new Compiler();
            var rt = rubyCompiler.Compile(ruby);

            Console.WriteLine(rt.Result);
            Console.WriteLine(rt.Message);
            Console.WriteLine(rt.Dictionary);
            Console.WriteLine(rt.Object);
             */
        }
    }
}
