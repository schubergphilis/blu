using System;
using System.Collections.Generic;
using System.Linq;
using BluApi.Chef.ChefAPI;
using BluApi.Common;

namespace BluLang.RubyScope.DSL
{
    public partial class Generator
    {
        private string AttributeTransformer(List<string> attList, int equalSignindex)
        {
            string listJoin;
            List<string> sublist = attList.GetRange(equalSignindex + 1, attList.Count - equalSignindex - 1);
            if (ChefConfig.KnownAttributeNames.Contains(sublist[0]))
            {
                listJoin = "$" + sublist[0];
                for (int i = 1; i < sublist.Count; i++)
                {
                    listJoin += "['" + sublist[i] + "']";
                }
            }
            else
            {
                listJoin = string.Join(" ", sublist.ToArray());
            }
            return listJoin;
        }

        private string ResourceTransformer(string ResourceUniqueName, string ParameterKey, List<string> parameterValue)
        {
            if (ParameterKey == "notifies")
            {
                AddNotifier(ResourceUniqueName, parameterValue);
                return string.Join("", parameterValue.ToArray());
            }
            if (ChefConfig.KnownAttributeNames.Contains(parameterValue[0]))
            {
                return parameterValue.Aggregate("$", (current, item) => current + item);
            }
            return parameterValue.Aggregate(String.Empty, (current, item) => current + item).SingleQuote();
        }

        public static string TransformNotifierUniqueName(string notified)
        {
            return notified.Replace("'", String.Empty).Replace("\"", String.Empty).Replace("[", "->\'").Replace("]", "\'");
        }
    }
}
