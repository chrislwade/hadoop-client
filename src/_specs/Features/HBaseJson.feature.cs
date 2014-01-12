﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.18052
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace _specs.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("HBase JSON")]
    public partial class HBaseJSONFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "HBaseJson.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "HBase JSON", "In order to work with HBase content in the JSON format\r\nAs an application develop" +
                    "er\r\nI want to be able to create and parse HBase JSON content using basic informa" +
                    "tion", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 6
#line 7
 testRunner.Given("I have everything I need to test a content converter for JSON", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Create JSON for single cell")]
        [NUnit.Framework.TestCaseAttribute("1", "alpha", "", "", "hello world", "HBaseJson_1Alpha_HelloWorld", null)]
        [NUnit.Framework.TestCaseAttribute("1", "alpha", "x", "", "hello world", "HBaseJson_1AlphaX_HelloWorld", null)]
        [NUnit.Framework.TestCaseAttribute("1", "alpha", "", "4", "hello world", "HBaseJson_1Alpha4_HelloWorld", null)]
        [NUnit.Framework.TestCaseAttribute("1", "alpha", "x", "4", "hello world", "HBaseJson_1AlphaX4_HelloWorld", null)]
        public virtual void CreateJSONForSingleCell(string row, string column, string qualifier, string timestamp, string value, string expectedJson, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Create JSON for single cell", exampleTags);
#line 9
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 10
 testRunner.Given(string.Format("I have a cell with a {0}, {1}, {2}, {3}, and {4}", row, column, qualifier, timestamp, value), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 11
 testRunner.When("I convert my cell to raw content", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 12
 testRunner.Then(string.Format("my raw JSON content should be equivalent to the resource called \"{0}\"", expectedJson), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Parse JSON for single cell")]
        [NUnit.Framework.TestCaseAttribute("HBaseJson_1Alpha_HelloWorld", "1", "alpha", "", "", "hello world", null)]
        [NUnit.Framework.TestCaseAttribute("HBaseJson_1AlphaX_HelloWorld", "1", "alpha", "x", "", "hello world", null)]
        [NUnit.Framework.TestCaseAttribute("HBaseJson_1Alpha4_HelloWorld", "1", "alpha", "", "4", "hello world", null)]
        [NUnit.Framework.TestCaseAttribute("HBaseJson_1AlphaX4_HelloWorld", "1", "alpha", "x", "4", "hello world", null)]
        public virtual void ParseJSONForSingleCell(string initialJson, string row, string column, string qualifier, string timestamp, string value, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Parse JSON for single cell", exampleTags);
#line 20
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 21
 testRunner.Given(string.Format("I have raw content equal to the resource called \"{0}\"", initialJson), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 22
 testRunner.When("I convert my raw content to a cell", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 23
 testRunner.Then(string.Format("my cell should have a {0}, {1}, {2}, {3}, and {4}", row, column, qualifier, timestamp, value), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Create JSON for a set of cells")]
        public virtual void CreateJSONForASetOfCells()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Create JSON for a set of cells", ((string[])(null)));
#line 31
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 32
 testRunner.Given("I have created a set of cells", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "row",
                        "column",
                        "qualifier",
                        "timestamp",
                        "value"});
            table1.AddRow(new string[] {
                        "1",
                        "alpha",
                        "",
                        "",
                        "hello world"});
            table1.AddRow(new string[] {
                        "1",
                        "alpha",
                        "x",
                        "",
                        "hello world"});
            table1.AddRow(new string[] {
                        "1",
                        "alpha",
                        "",
                        "4",
                        "hello world"});
            table1.AddRow(new string[] {
                        "1",
                        "alpha",
                        "x",
                        "4",
                        "hello world"});
#line 33
 testRunner.And("I have added a cell to my set with the following properties:", ((string)(null)), table1, "And ");
#line 39
 testRunner.When("I convert my set of cells to raw content", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 40
 testRunner.Then("my raw JSON content should be equivalent to the resource called \"HBaseJson_Set_He" +
                    "lloWorld\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Parse JSON for a set of cells")]
        public virtual void ParseJSONForASetOfCells()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Parse JSON for a set of cells", ((string[])(null)));
#line 42
this.ScenarioSetup(scenarioInfo);
#line 6
this.FeatureBackground();
#line 43
 testRunner.Given("I have raw content equal to the resource called \"HBaseJson_Set_HelloWorld\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 44
 testRunner.When("I convert my raw content to a set of cells", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 45
 testRunner.Then("my set should contain 4 cells", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "row",
                        "column",
                        "qualifier",
                        "timestamp",
                        "value"});
            table2.AddRow(new string[] {
                        "1",
                        "alpha",
                        "",
                        "",
                        "hello world"});
            table2.AddRow(new string[] {
                        "1",
                        "alpha",
                        "x",
                        "",
                        "hello world"});
            table2.AddRow(new string[] {
                        "1",
                        "alpha",
                        "",
                        "4",
                        "hello world"});
            table2.AddRow(new string[] {
                        "1",
                        "alpha",
                        "x",
                        "4",
                        "hello world"});
#line 46
 testRunner.And("one of the cells in my set should have the following properties:", ((string)(null)), table2, "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
