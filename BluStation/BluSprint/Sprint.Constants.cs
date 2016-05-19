using System;
using BluApi.Chef.ChefAPI;
using ReturnType = BluApi.Common.Function;

namespace BluStation.BluSprint
{
    public partial class Sprint
    {
        public void ImportConstants()
        {
            SprintPs1Content += @"
# ══════════════════════════
#  Constant vairables
# ══════════════════════════
";
            SprintPs1Content += @"if (!$root) { Set-Variable root -option Constant -value '" + ChefConfig.Root + "' };" + Environment.NewLine;
            SprintPs1Content += @"if (!$node_name) { Set-Variable node_name -option Constant -value '" + ChefConfig.NodeName + "' };" + Environment.NewLine;
        }
    }
}
