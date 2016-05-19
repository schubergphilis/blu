using System;
using System.Collections.Generic;
using System.Linq;
using BluApi.Chef.ChefAPI;
using Irony.Compiler;

namespace BluLang.RubyScope.DSL
{
    public partial class Generator
    {
        readonly List<string> _attList = new List<string>();

        private void Attribute(string att, AstNode args)
        {
            _attList.Clear();
            ROOT_COMPSTMT_STMTLIST_STMT_COMMAND_CALLARGS(att, null, args);
            if (!_result["attributes"].ContainsKey(att)) _result["attributes"].Add(att, new Dictionary<string, dynamic>());

            int eq = _attList.IndexOf("=");
            if (eq > 5) 
                throw new InvalidOperationException("nested hash for more than 5 level is not supported.");
            if (eq < 1) 
                throw new InvalidOperationException("attribute equal sign index can not be smaller than 1. There is an error in attribute definitions.");

            if (eq >= 1)
            {
                if (!_result["attributes"][att]
                    .ContainsKey(_attList[0]))
                {
                    if (eq == 1) _result["attributes"][att]
                        .Add(_attList[0],
                        AttributeTransformer(_attList, eq));
                    else _result["attributes"][att]
                        .Add(_attList[0], 
                        new Dictionary<string, dynamic>());
                }
            }
            if (eq >= 2)
            {
                if (!_result["attributes"][att]
                    [_attList[0]]
                    .ContainsKey(_attList[1]))
                {
                    if (eq == 2) 
                        _result["attributes"][att]
                        [_attList[0]].Add(_attList[1],
                        AttributeTransformer(_attList, eq));
                    else 
                        _result["attributes"][att]
                        [_attList[0]]
                        .Add(_attList[1], 
                        new Dictionary<string, dynamic>());
                }
            }
            if (eq >= 3)
            {
                if (!_result["attributes"][att]
                    [_attList[0]]
                    [_attList[1]]
                    .ContainsKey(_attList[2]))
                {
                    if (eq == 3) 
                        _result["attributes"][att]
                        [_attList[0]]
                        [_attList[1]]
                        .Add(_attList[2],
                        AttributeTransformer(_attList, eq));
                    else 
                        _result["attributes"][att]
                        [_attList[0]][_attList[1]]
                        .Add(_attList[2], 
                        new Dictionary<string, dynamic>());
                }
            }
            if (eq >= 4)
            {
                if (!_result["attributes"][att]
                    [_attList[0]]
                    [_attList[1]]
                    [_attList[2]]
                    .ContainsKey(_attList[3]))
                {
                    if (eq == 4) 
                        _result["attributes"][att]
                        [_attList[0]]
                        [_attList[1]]
                        [_attList[2]]
                        .Add(_attList[3],
                        AttributeTransformer(_attList, eq));
                    else 
                        _result["attributes"][att]
                        [_attList[0]]
                        [_attList[1]]
                        [_attList[2]]
                        .Add(_attList[3], 
                        new Dictionary<string, dynamic>());
                }
            }
            if (eq == 5)
            {
                if (!_result["attributes"][att]
                    [_attList[0]]
                    [_attList[1]]
                    [_attList[2]]
                    [_attList[3]]
                    .ContainsKey(_attList[4]))
                {
                    _result["attributes"][att]
                    [_attList[0]]
                    [_attList[1]]
                    [_attList[2]]
                    [_attList[3]]
                    .Add(_attList[4], AttributeTransformer(_attList, eq));
                }
            }
        }
    }
}

