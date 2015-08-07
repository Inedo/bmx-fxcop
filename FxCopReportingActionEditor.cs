using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Web.Controls;
using Inedo.BuildMaster.Web.Controls.Extensions;
using Inedo.Web.Controls;

namespace Inedo.BuildMasterExtensions.FxCop
{
    internal sealed class FxCopReportingActionEditor : ActionEditorBase
    {
        private SourceControlFileFolderPicker txtFxCopExecutablePath;
        private SourceControlFileFolderPicker txtWorkingDirectory;
        private SourceControlFileFolderPicker txtRuleSetDirectory;
        private SourceControlFileFolderPicker txtCustomDictionary;
        private ValidatingTextBox txtOutputName;
        private ValidatingTextBox txtTestFile;
        private ValidatingTextBox txtCustomOutputFile;
        private ValidatingTextBox txtCustomOutputXslFile;
        private ValidatingTextBox txtDependencyDirectories;
        private ValidatingTextBox txtRuleIds;
        private ValidatingTextBox txtRules;
        private ValidatingTextBox txtRuleSets;
        private ValidatingTextBox txtAdditionalArguments;
        private CheckBox chkVerboseOutput;
        private CheckBox chkSearchGac;
        private CheckBox chkFailOnMissingRules;

        public override bool DisplaySourceDirectory => true;
        public override string SourceDirectoryLabel => "FxCop Working Directory:";

        protected override void CreateChildControls()
        {
            this.txtFxCopExecutablePath = new SourceControlFileFolderPicker
            {
                ServerId = this.ServerId,
                Required = true
            };

            this.txtWorkingDirectory = new SourceControlFileFolderPicker
            {
                ServerId = this.ServerId,
                DisplayMode = SourceControlBrowser.DisplayModes.Folders
            };

            this.txtCustomDictionary = new SourceControlFileFolderPicker
            {
                ServerId = this.ServerId
            };

            this.txtRuleSetDirectory = new SourceControlFileFolderPicker
            {
                ServerId = this.ServerId,
                DisplayMode = SourceControlBrowser.DisplayModes.Folders
            };

            this.txtOutputName = new ValidatingTextBox
            {
                Width = Unit.Pixel(300),
                Required = true
            };

            this.txtTestFile = new ValidatingTextBox
            {
                Width = Unit.Pixel(300),
                Required = true
            };

            this.txtAdditionalArguments = new ValidatingTextBox
            {
                Width = Unit.Pixel(300)
            };

            this.txtCustomOutputFile = new ValidatingTextBox
            {
                Width = Unit.Pixel(300)
            };

            this.txtCustomOutputXslFile = new ValidatingTextBox
            {
                Width = Unit.Pixel(300)
            };

            this.txtDependencyDirectories = new ValidatingTextBox
            {
                TextMode = TextBoxMode.MultiLine,
                Rows = 5,
                Width = 300
            };

            this.txtRules = new ValidatingTextBox
            {
                TextMode = TextBoxMode.MultiLine,
                Rows = 5,
                Width = 300
            };

            this.txtRuleIds = new ValidatingTextBox
            {
                TextMode = TextBoxMode.MultiLine,
                Rows = 5,
                Width = 300
            };

            this.txtRuleSets = new ValidatingTextBox
            {
                TextMode = TextBoxMode.MultiLine,
                Rows = 5,
                Width = 300
            };

            this.chkVerboseOutput = new CheckBox() { Text = "Verbose console output" };
            this.chkSearchGac = new CheckBox() { Text = "Include the Global Assembly Cache in the search" };
            this.chkFailOnMissingRules = new CheckBox() { Text = "Treat missing rules as an error" };

            this.Controls.Add(
                new FormFieldGroup("Report Name",
                    "The name of the report to be generated.",
                    false,
                    new StandardFormField("Report Name:", this.txtOutputName)
                ),
                new FormFieldGroup("FxCop Executable Path",
                    "The full path of the FxCop command line utility (FxCopCmd.exe) used to generate the report output.",
                    false,
                    new StandardFormField("FxCop Executable Path:", this.txtFxCopExecutablePath)
                ),
                new FormFieldGroup("Test File",
                    "The file or assembly to analyze, relative to the working directory.",
                    false,
                    new StandardFormField("Test File:", this.txtTestFile)
                ),
                new FormFieldGroup("Custom Output",
                    "If you wish to output the report to a custom file or use an optional XSL template,"+
                    " specify one here; otherwise, leave it blank.",
                    false,
                    new StandardFormField("Output File:", this.txtCustomOutputFile),
                    new StandardFormField("Output XSL Template:", this.txtCustomOutputXslFile)
                ),
                new FormFieldGroup("Assembly Dependencies",
                    "Optionally specify directories to search for assembly dependencies, separated by newlines.",
                    false,
                    new StandardFormField("Dependency Directories (one per line):", this.txtDependencyDirectories),
                    new StandardFormField("", this.chkSearchGac)
                ),
                new FormFieldGroup("Rule Sets",
                    "Optionally specify rule sets, separated by newlines. An individual rule set must start with one of" + 
                    " the following three characters:<br />"+
                    "<b>+</b> - enables all rules in rule set<br />"+
                    "<b>-</b> - disables all rules in the rule set, or <br />"+
                    "<b>=</b> - enables all rules in the rule set and disables all other rules not explicitly defined" +
                    " in the rule set.",
                    false,
                    new StandardFormField("Rule Set Directory:", this.txtRuleSetDirectory),
                    new StandardFormField("Rule Sets (one per line):", this.txtRuleSets)
                ),
                new FormFieldGroup("Rule Assemblies",
                    "Optionally specify rule assemblies or directories, separated by newlines. An individual" +
                    " rule assembly or directory must start with one of the following two characters:<br />" +
                    "<b>+</b> - enables all rules in rule assembly or directory, or<br />" +
                    "<b>-</b> - disables all rules in the rule assembly or directory",
                    false,
                    new StandardFormField("Rule Assemblies/Directories (one per line):", this.txtRules)
                ),
                new FormFieldGroup("Rule IDs",
                    "Optionally specify rule IDs (in the format Namespace.CheckId), separated by newlines. An individual" +
                    " rule ID must start with one of the following two characters:<br />" +
                    "<b>+</b> - enables the rule with the given ID, or<br />" +
                    "<b>-</b> - disables the rule",
                    false,
                    new StandardFormField("Rule IDs (one per line):", this.txtRuleIds)
                ),
                new FormFieldGroup("Custom Dictionary",
                    "Specify a custom dictionary, or leave blank to use the default.",
                    false,
                    new StandardFormField("Custom Dictionary:", this.txtCustomDictionary)
                ),
                new FormFieldGroup("Additional Options",
                    "Additional options to pass to the FxCop executable.",
                    true,
                    new StandardFormField("", this.chkVerboseOutput),
                    new StandardFormField("", this.chkFailOnMissingRules),
                    new StandardFormField("Additional Arguments:", this.txtAdditionalArguments)
                )
            );
        }

        public override void BindToForm(ActionBase extension)
        {
            this.EnsureChildControls();

            var fxCopAction = (FxCopReportingAction)extension;

            this.txtFxCopExecutablePath.Text = fxCopAction.ExePath;
            this.txtWorkingDirectory.Text = fxCopAction.OverriddenSourceDirectory;
            this.txtRuleSetDirectory.Text = fxCopAction.RuleSetDirectory;
            this.txtCustomDictionary.Text = fxCopAction.CustomDictionary;
            this.txtOutputName.Text = fxCopAction.OutputName;
            this.txtTestFile.Text = fxCopAction.TestFile;
            this.txtCustomOutputFile.Text = fxCopAction.CustomOutputFile;
            this.txtCustomOutputXslFile.Text = fxCopAction.CustomOutputXslFile;
            this.txtDependencyDirectories.Text = JoinValues(fxCopAction.DependencyDirectories);
            this.txtRuleIds.Text = JoinValues(fxCopAction.RuleIds);
            this.txtRules.Text = JoinValues(fxCopAction.Rules);
            this.txtRuleSets.Text = JoinValues(fxCopAction.RuleSets);
            this.txtAdditionalArguments.Text = fxCopAction.Arguments;
            this.chkVerboseOutput.Checked = fxCopAction.VerboseOutput;
            this.chkSearchGac.Checked = fxCopAction.SearchGac;
            this.chkFailOnMissingRules.Checked = fxCopAction.FailOnMissingRules;
        }

        public override ActionBase CreateFromForm()
        {
            this.EnsureChildControls();

            return new FxCopReportingAction
            {
                ExePath = this.txtFxCopExecutablePath.Text,
                OverriddenSourceDirectory = this.txtWorkingDirectory.Text,
                RuleSetDirectory = this.txtRuleSetDirectory.Text,
                CustomDictionary = this.txtCustomDictionary.Text,
                OutputName = this.txtOutputName.Text,
                TestFile = this.txtTestFile.Text,
                CustomOutputFile = this.txtCustomOutputFile.Text,
                CustomOutputXslFile = this.txtCustomOutputXslFile.Text,
                DependencyDirectories = SplitText(this.txtDependencyDirectories.Text),
                RuleIds = SplitText(this.txtRuleIds.Text),
                Rules = SplitText(this.txtRules.Text),
                RuleSets = SplitText(this.txtRuleSets.Text),
                Arguments = this.txtAdditionalArguments.Text,
                VerboseOutput = this.chkVerboseOutput.Checked,
                SearchGac = this.chkSearchGac.Checked,
                FailOnMissingRules = this.chkFailOnMissingRules.Checked
            };
        }

        private static string JoinValues(string[] values)
        {
            return string.Join(Environment.NewLine, values ?? new string[0]);
        }

        private static string[] SplitText(string value)
        {
            var list = new List<string>();
            foreach (string val in value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                list.Add(val.Trim().Trim('\"'));

            return list.ToArray();
        }
    }
}
