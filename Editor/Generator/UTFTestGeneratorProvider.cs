﻿using BoDi;
using System.CodeDom;
using System.Collections.Generic;
using UnityFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Generator;
using UnityEngine;
using System;
using UnityFlow;

namespace UnityFlow.Generator
{
    public class UTFTestGeneratorProvider : IUnitTestGeneratorProvider
    {
        protected internal const string TESTFIXTURESETUP_ATTR_NUNIT3 = "NUnit.Framework.OneTimeSetUpAttribute";
        protected internal const string TESTFIXTURETEARDOWN_ATTR_NUNIT3 = "NUnit.Framework.OneTimeTearDownAttribute";
        protected internal const string NONPARALLELIZABLE_ATTR = "NUnit.Framework.NonParallelizableAttribute";
        protected internal const string TESTFIXTURE_ATTR = "NUnit.Framework.TestFixtureAttribute";
        protected internal const string TEST_ATTR = "UnityEngine.TestTools.UnityTestAttribute";
        protected internal const string ROW_ATTR = "NUnit.Framework.TestCaseAttribute";
        protected internal const string CATEGORY_ATTR = "NUnit.Framework.CategoryAttribute";
        protected internal const string TESTSETUP_ATTR = "NUnit.Framework.SetUpAttribute";
        protected internal const string TESTTEARDOWN_ATTR = "NUnit.Framework.TearDownAttribute";
        protected internal const string IGNORE_ATTR = "NUnit.Framework.IgnoreAttribute";
        protected internal const string DESCRIPTION_ATTR = "NUnit.Framework.DescriptionAttribute";
        protected internal const string TESTCONTEXT_TYPE = "NUnit.Framework.TestContext";
        protected internal const string TESTCONTEXT_INSTANCE = "NUnit.Framework.TestContext.CurrentContext";

        public UTFTestGeneratorProvider(CodeDomHelper codeDomHelper)
        {
            CodeDomHelper = codeDomHelper;
        }

        protected CodeDomHelper CodeDomHelper { get; set; }

        public bool GenerateParallelCodeForFeature { get; set; }

        public virtual UnitTestGeneratorTraits GetTraits()
        {
            return UnitTestGeneratorTraits.None;
        }

        public virtual void SetTestClassIgnore(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, IGNORE_ATTR, "Ignored feature");
        }

        public virtual void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            CodeDomHelper.AddAttribute(testMethod, IGNORE_ATTR, "Ignored scenario");
        }

        public virtual void SetTestClassInitializeMethod(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClassInitializeMethod, TESTFIXTURESETUP_ATTR_NUNIT3);
        }

        public virtual void SetTestClassCleanupMethod(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClassCleanupMethod, TESTFIXTURETEARDOWN_ATTR_NUNIT3);
        }

        public virtual void SetTestClassNonParallelizable(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, NONPARALLELIZABLE_ATTR);
        }

        public void SetTestClass(TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, TESTFIXTURE_ATTR);
            CodeDomHelper.AddAttribute(generationContext.TestClass, DESCRIPTION_ATTR, featureTitle);
        }

        public void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
        {
            CodeDomHelper.AddAttributeForEachValue(generationContext.TestClass, CATEGORY_ATTR, featureCategories);
        }

        public virtual void FinalizeTestClass(TestClassGenerationContext generationContext)
        {
            // testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
            generationContext.ScenarioInitializeMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodeFieldReferenceExpression(null, generationContext.TestRunnerField.Name),
                                nameof(ScenarioContext)),
                            nameof(ScenarioContext.ScenarioContainer)),
                        nameof(IObjectContainer.RegisterInstanceAs),
                        new CodeTypeReference(TESTCONTEXT_TYPE)),
                    new CodeVariableReferenceExpression(TESTCONTEXT_INSTANCE)));
        }

        public void SetTestInitializeMethod(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestInitializeMethod, TESTSETUP_ATTR);
        }

        public void SetTestCleanupMethod(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestCleanupMethod, TESTTEARDOWN_ATTR);
        }

        public virtual void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string friendlyTestName)
        {
            CodeDomHelper.AddAttribute(testMethod, TEST_ATTR);
            CodeDomHelper.AddAttribute(testMethod, DESCRIPTION_ATTR, friendlyTestName);
        }

        public virtual void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            CodeDomHelper.AddAttributeForEachValue(testMethod, CATEGORY_ATTR, scenarioCategories);
        }

        public virtual void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            Debug.LogError("Row tests are not supported in Unity");
            throw new NotSupportedException();
            //SetTestMethod(generationContext, testMethod, scenarioTitle);

        }

        public void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored)
        {
            Debug.LogError("Row tests are not supported in Unity");
            throw new NotSupportedException();

            //var args = arguments.Select(
            //    arg => new CodeAttributeArgument(new CodePrimitiveExpression(arg))).ToList();

            //// addressing ReSharper bug: TestCase attribute with empty string[] param causes inconclusive result - https://github.com/techtalk/SpecFlow/issues/116
            //bool hasExampleTags = tags.Any();
            //var exampleTagExpressionList = tags.Select(t => new CodePrimitiveExpression(t));
            //var exampleTagsExpression = hasExampleTags
            //    ? new CodeArrayCreateExpression(typeof(string[]), exampleTagExpressionList.ToArray())
            //    : (CodeExpression)new CodePrimitiveExpression(null);

            //args.Add(new CodeAttributeArgument(exampleTagsExpression));

            //// adds 'Category' named parameter so that NUnit also understands that this test case belongs to the given categories
            //if (hasExampleTags)
            //{
            //    CodeExpression exampleTagsStringExpr = new CodePrimitiveExpression(string.Join(",", tags.ToArray()));
            //    args.Add(new CodeAttributeArgument("Category", exampleTagsStringExpr));
            //}

            //if (isIgnored)
            //    args.Add(new CodeAttributeArgument("Ignored", new CodePrimitiveExpression(true)));

            //CodeDomHelper.AddAttribute(testMethod, ROW_ATTR, args.ToArray());
        }

        public void SetTestMethodAsRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle, string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments)
        {
            // No additional setup needed
            //string readableTestName = $"{scenarioTitle} with parameters: ";
            //foreach (var pair in arguments) {
            //    readableTestName += $"({pair.Key}: {pair.Value})";
            //}

        }
    }
}
