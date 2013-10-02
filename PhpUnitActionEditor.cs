using System;
using System.Web.UI.WebControls;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Web.Controls;
using Inedo.BuildMaster.Web.Controls.Extensions;
using Inedo.Web.Controls;

namespace Inedo.BuildMasterExtensions.PHPUnit
{
    /// <summary>
    /// Provides a custom editor for the PHPUnit unit test action.
    /// </summary>
    internal sealed class PhpUnitActionEditor : ActionEditorBase
    {
        private static class TestsToRun
        {
            public const string RunAllTests = "all";
            public const string UseXmlConfiguration = "config";
            public const string SpecifyTestClass = "specify";
        }

        /// <summary>
        /// Gets a value indicating whether a textbox to edit the source directory should be displayed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if a textbox to edit the source directory should be displayed; otherwise, <c>false</c>.
        /// </value>
        public override bool DisplaySourceDirectory { get { return true; } }

        private ValidatingTextBox txtTestsToRun;
        private ValidatingTextBox txtAdditionalArguments;
        private ValidatingTextBox txtGroupName;
        private DropDownList ddlTestsToRun;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhpUnitActionEditor"/> class.
        /// </summary>
        public PhpUnitActionEditor()
        {
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation 
        /// to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            this.txtTestsToRun = new ValidatingTextBox()
            {
                Width = 300,
                Required = false
            };

            this.txtAdditionalArguments = new ValidatingTextBox()
            {
                Width = 300,
                Required = false
            };

            this.txtGroupName = new ValidatingTextBox()
            {
                Width = 300,
                Required = false
            };

            this.ddlTestsToRun = new DropDownList()
            {
                ID = "ddlTestsToRun",
                Items = 
                { 
                    new ListItem("Run All Tests In Source Directory", TestsToRun.RunAllTests),
                    new ListItem("Use phpunit.xml Configuration", TestsToRun.UseXmlConfiguration),
                    new ListItem("Specify Test Class...", TestsToRun.SpecifyTestClass)
                }
            };

            var ctlTestsToRun = new StandardFormField("Test Class:", this.txtTestsToRun) { ID = "ctlTestsToRun" };

            CUtil.Add(this,
                new FormFieldGroup(
                    "Tests to Run",
                    "Select which tests should run. When specifying a test class, note that tests in the class \"UnitTest\" are expected to be declared in a source file named \"UnitTest.php\".",
                    false,
                    new StandardFormField("Tests to Run:", this.ddlTestsToRun),
                    ctlTestsToRun
                ),
                new RenderJQueryDocReadyDelegator(w => w.Write(@"
$('#" + this.ddlTestsToRun.ClientID + @"').change(function(){
    if($(this).val() == '" + TestsToRun.SpecifyTestClass + @"') {
        $('#" + ctlTestsToRun.ClientID + @"').show();
    }
    else {
        $('#" + ctlTestsToRun.ClientID + @"').hide();
    }
}).change();")
                ),
                new FormFieldGroup(
                    "Additional Arguments",
                    "Any additional arguments that will be supplied to the PHPUnit script.",
                    false,
                    new StandardFormField("Additional Arguments:", this.txtAdditionalArguments)
                ),
                new FormFieldGroup(
                    "Group Name",
                    "The group name allows you to easily identify the unit test.",
                    true,
                    new StandardFormField("Group Name:", this.txtGroupName)
                )
            );
        }

        /// <summary>
        /// Binds to form.
        /// </summary>
        /// <param name="extension">The extension.</param>
        public override void BindToForm(ActionBase extension)
        {
            var action = (PhpUnitTestAction)extension;
            if (action.TestToRun == ".")
            {
                this.ddlTestsToRun.SelectedValue = TestsToRun.RunAllTests;
            }
            else if (String.IsNullOrEmpty(action.TestToRun))
            {
                this.ddlTestsToRun.SelectedValue = TestsToRun.UseXmlConfiguration;
            }
            else
            {
                this.ddlTestsToRun.SelectedValue = TestsToRun.SpecifyTestClass;
                this.txtTestsToRun.Text = action.TestToRun;
            }
            
            this.txtAdditionalArguments.Text = action.AdditionalArguments;
            this.txtGroupName.Text = action.GroupName;
        }

        /// <summary>
        /// Creates from form.
        /// </summary>
        public override ActionBase CreateFromForm()
        {
            string testToRun;
            {
                if (this.ddlTestsToRun.SelectedValue == TestsToRun.RunAllTests)
                    testToRun = ".";
                else if (this.ddlTestsToRun.SelectedValue == TestsToRun.UseXmlConfiguration)
                    testToRun = String.Empty;
                else
                    testToRun = this.txtTestsToRun.Text.Trim();
            }

            return new PhpUnitTestAction()
            {
                TestToRun = testToRun,
                AdditionalArguments = this.txtAdditionalArguments.Text,
                GroupName = this.txtGroupName.Text
            };
        }
    }
}
