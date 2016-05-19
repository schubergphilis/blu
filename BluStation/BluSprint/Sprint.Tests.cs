using System;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluStation.BluSprint
{
    public partial class Sprint
    {
        /// <summary>
        /// Reads test.blu.rb recipe from blu_sprint cookbook and adds it to the compiled script
        /// </summary>
        /// <returns>Multiple type rt. rt.Result 0 for success and 3 for failure. rt.Message for exceptions.</returns>
        public Function ImportTest()
        {
            SprintPs1Content += @"
# ══════════════════════════
#  Instant Unit Tests
# ══════════════════════════
# Instant Unit Tests are executed at each Sprint to ensure basic functionality and integration
# More extensive NUnit tests are executed when BluStation.dll is build
";
            string mainTestScript = SprintData.SprintPath + @"\recipes\test.blu.rb";
            var rt = ExtactBluScript(mainTestScript);
            if (rt.Result != 0) return rt;
            SprintPs1Content += rt.Data;
            SprintPs1Content += Environment.NewLine + Environment.NewLine;
            return rt;
        }
    }
}
