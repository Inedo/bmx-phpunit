using System;
using Inedo.BuildMaster;
using Inedo.BuildMaster.Extensibility.Configurers.Extension;
using Inedo.BuildMaster.Web;

[assembly: ExtensionConfigurer(typeof(Inedo.BuildMasterExtensions.PHPUnit.PhpUnitConfigurer))]

namespace Inedo.BuildMasterExtensions.PHPUnit
{
    /// <summary>
    /// Provides functionality to configure the paths to the PHP executable and the PHPUnit script.
    /// </summary>
    [CustomEditor(typeof(PhpUnitConfigurerEditor))]
    public sealed class PhpUnitConfigurer : ExtensionConfigurerBase
    {
        /// <summary>
        /// Gets or sets the PHP executable path.
        /// </summary>
        [Persistent]
        public string PhpExecutablePath { get; set; }

        /// <summary>
        /// Gets or sets the PHPUnit script path.
        /// </summary>
        [Persistent]
        public string PhpUnitScriptPath { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhpUnitConfigurer"/> class.
        /// </summary>
        public PhpUnitConfigurer()
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Empty;
        }
    }
}
