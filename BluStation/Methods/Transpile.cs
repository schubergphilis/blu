using System;
using System.Collections.Generic;
using BluApi.Common;
using BluLang.RubyScope.RubyTops;
using ReturnType = BluApi.Common.Function;

namespace Blu
{
    public static partial class Method
    {
        public static Dictionary<string, dynamic> Transpile(string rubyScript)
        {
            RubyTops rubyTops = new RubyTops {RubyScript = rubyScript};
            rubyTops.Transpile();
            return rubyTops.GeneratedDictionary;
        }
    }
}
