using System;
namespace BluStation.BluSprint
{
    public partial class Sprint
    {
        public void ImportTranspiledPowerShell()
        {
            SprintPs1Content += @"

# ══════════════════════════
#         RUBYTOPS
# ══════════════════════════
";
            SprintPs1Content += TranspiledPowerShell;
            SprintPs1Content += Environment.NewLine;
        }
    }
}
