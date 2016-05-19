using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluStation.BluSprint
{
    public partial class Sprint
    {
        public void ImportAttributes()
        {
            SprintPs1Content += @"
# ══════════════════════════
#  Attributes
# ══════════════════════════
";
            SprintPs1Content += @"$node = [Blu.Method]::GetNodeObject();";
            SprintPs1Content += Environment.NewLine;
        }

        public Function CollectAttributes()
        {
            ReturnType rt = new ReturnType { Result = 0 };
            Logger.log("info", "Compiling attributes in runlist...");
            try
            {
                foreach (var attributeFilePath in SprintData.AttributeFileList)
                {
                    AttributesStack += Environment.NewLine;
                    AttributesStack += "#---------------------------------------------------------------" + Environment.NewLine; ;
                    AttributesStack += "# " + attributeFilePath + Environment.NewLine;
                    AttributesStack += "#---------------------------------------------------------------" + Environment.NewLine; ;
                    AttributesStack += Environment.NewLine;
                    string[] lines = File.ReadAllLines(attributeFilePath);
                    foreach (string line in lines)
                    {
                        AttributesStack += line.Trim() + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                rt.Result = 3;
                rt.Message = ex.Message;
                return rt;
            }
            return rt;
        }
    }
}

