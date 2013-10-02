using Inedo.BuildMaster.Extensibility.Configurers.Extension;
using Inedo.BuildMaster.Web.Controls;
using Inedo.BuildMaster.Web.Controls.Extensions;
using Inedo.Web.Controls;

namespace Inedo.BuildMasterExtensions.PHPUnit
{
    /// <summary>
    /// Provides an editor for the PHPUnit extension configurer.
    /// </summary>
    internal sealed class PhpUnitConfigurerEditor : ExtensionConfigurerEditorBase
    {
        private ValidatingTextBox txtPhpExecutablePath;
        private ValidatingTextBox txtPhpUnitScriptPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhpUnitConfigurerEditor"/> class.
        /// </summary>
        public PhpUnitConfigurerEditor()
        {
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation 
        /// to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            this.txtPhpExecutablePath = new ValidatingTextBox()
            {
                Width = 300,
                Required = true
            };

            this.txtPhpUnitScriptPath = new ValidatingTextBox()
            {
                Width = 300,
                Required = true
            };

            CUtil.Add(this,
                new FormFieldGroup(
                    "PHP Executable Path",
                    "The path to the PHP executable.",
                    false,
                    new StandardFormField("", this.txtPhpExecutablePath)
                ),
                new FormFieldGroup(
                    "PHPUnit Script Path",
                    "The path to the PHPUnit script, typically found under a PEAR installation.",
                    true,
                    new StandardFormField("", this.txtPhpUnitScriptPath)
                )
            );
        }

        /// <summary>
        /// Binds to form.
        /// </summary>
        /// <param name="extension">The extension.</param>
        public override void BindToForm(ExtensionConfigurerBase extension)
        {
            var configurer = (PhpUnitConfigurer)extension;

            this.txtPhpExecutablePath.Text = configurer.PhpExecutablePath;
            this.txtPhpUnitScriptPath.Text = configurer.PhpUnitScriptPath;
        }

        /// <summary>
        /// Creates from form.
        /// </summary>
        /// <returns></returns>
        public override ExtensionConfigurerBase CreateFromForm()
        {
            return new PhpUnitConfigurer()
            {
                PhpExecutablePath = this.txtPhpExecutablePath.Text,
                PhpUnitScriptPath = this.txtPhpUnitScriptPath.Text
            };
        }

        /// <summary>
        /// Populates the fields within the control with the appropriate default values.
        /// </summary>
        /// <remarks>
        /// This is only called when creating a new extension.
        /// </remarks>
        public override void InitializeDefaultValues()
        {
            BindToForm(new PhpUnitConfigurer());
        }
    }
}
