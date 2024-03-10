﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityFlow.General.Configuration;
using UnityFlow.Generator.Roslyn;

namespace UnityFlow.Generator.Generation
{
    public class LinePragmaHandler
    {
        private readonly SpecFlowConfiguration _specFlowConfiguration;
        private readonly RoslynHelper _roslynHelper;

        public LinePragmaHandler(SpecFlowConfiguration specFlowConfiguration, RoslynHelper roslynHelper)
        {
            _specFlowConfiguration = specFlowConfiguration;
            _roslynHelper = roslynHelper;
        }


        public void AddLinePragmaInitial(ClassDeclarationSyntax testType, string sourceFile)
        {
            if (_specFlowConfiguration.AllowDebugGeneratedFiles)
                return;

            //_roslynHelper.BindTypeToSourceFile(testType, Path.GetFileName(sourceFile));
        }
    }
}