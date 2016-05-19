using System;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluStation.BluSprint
{
    public partial class Sprint
    {
        public Function CollectResources()
        {
            ReturnType rt = new ReturnType {Result = 0};
            bool resourceExists = false;

            ResourcesPs1Content += @"<#
This script is automatically created by Blu
Do not edit it manually, it will be overwitten by the next run.
Copyright 2016, Schuberg Philis
Maintainer: Gitlocker <aahmadi@schubergphilis.com>
All rights reserved.
═════════════════════════════════════════════════════

.SYNOPSIS
This the compiled resources script created by Start-Sprint CmdLet of BluStation.dll

.DESCRIPTION
It includes all codes compiled from the resources in the run_list
Only cookbooks that supports blu (in metadata: supports 'blu') are compiled.
#>
";
            try
            {
                foreach (var resourcePath in SprintData.ResourceFileList)
                {
                    rt = ExtactBluScript(resourcePath);
                    if (rt.Result != 0 || string.IsNullOrWhiteSpace(rt.Data)) continue;
                    ResourcesPs1Content += Environment.NewLine;
                    ResourcesPs1Content += "# Collected from: " + resourcePath + Environment.NewLine;
                    ResourcesPs1Content += rt.Data;
                    ResourcesPs1Content += Environment.NewLine;
                    resourceExists = true;
                }
            }
            catch (Exception ex)
            {
                rt.Result = 3;
                rt.Message = ex.Message;
                return rt;
            }

            if (!resourceExists)
            {
                rt.Result = 3;
                rt.Message = "Sprint does not contain any resource.";
                return rt;
            }

        SprintPs1Content += @"
# ══════════════════════════
#  Import Resources Module
# ══════════════════════════
";
            SprintPs1Content += "Import-Module '" + SprintData.RuntimePath + "\\Resources.psm1';" + Environment.NewLine;
            return rt;
        }
    }
}
