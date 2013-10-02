using System;
using System.IO;
using System.Xml;
using Inedo.BuildMaster;
using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Extensibility.Actions.Testing;
using Inedo.BuildMaster.Web;

namespace Inedo.BuildMasterExtensions.PHPUnit
{
    /// <summary>
    /// Provides functionality for running PHPUnit unit tests.
    /// </summary>
    [ActionProperties(
        "Execute PHPUnit Tests",
        "Runs PHPUnit tests on the specified test, test file, or all tests in the specified directory.",
        "Testing")]
    [CustomEditor(typeof(PhpUnitActionEditor))]
    public sealed class PhpUnitTestAction : UnitTestActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhpUnitTestAction"/> class.
        /// </summary>
        public PhpUnitTestAction()
        {
        }

        /// <summary>
        /// Gets or sets the test to run in the source directory.
        /// </summary>
        [Persistent]
        public string TestToRun { get; set; }

        /// <summary>
        /// Gets or sets the additional arguments used by the phpunit script.
        /// </summary>
        [Persistent]
        public string AdditionalArguments { get; set; }

        /// <summary>
        /// Runs a unit test against a single DLL, Project File, or test configuration file
        /// After test is run, use RecordResult to save the test result to the database
        /// </summary>
        protected override void RunTests()
        {
            var configurer = (PhpUnitConfigurer)GetExtensionConfigurer();

            if (String.IsNullOrEmpty(configurer.PhpExecutablePath)) throw new InvalidOperationException("PHP executable path is not set in the configurer");
            if (String.IsNullOrEmpty(configurer.PhpUnitScriptPath)) throw new InvalidOperationException("PHPUnit script path is not set in the configurer");

            string tmpXmlPath = Path.Combine(this.Context.TempDirectory, Guid.NewGuid().ToString() + ".xml");

            DateTime testStart = DateTime.Now;

            ExecuteCommandLine(
                configurer.PhpExecutablePath,
                String.Format("{0} {1} --log-junit {2} {3}", QuotePath(configurer.PhpUnitScriptPath), this.AdditionalArguments, QuotePath(tmpXmlPath), QuotePath(this.TestToRun)),
                this.Context.SourceDirectory
            );

            if (!File.Exists(tmpXmlPath))
            {
                LogWarning("PHPUnit did not generate an output XML file, therefore no tests were run. This can be caused if there are no test cases in the action's source directory, or the test file names do not end with \"Test\" (case-sensitive).");
                return;
            }

            var xml = new XmlDocument();
            xml.Load(tmpXmlPath);
            foreach (XmlNode node in xml.SelectNodes("/testsuites//testcase"))
            {
                XmlNode failureNode = node.SelectSingleNode("failure | error");
                bool testPassed = (failureNode == null);
                string testResult = String.Empty;
                DateTime testEnd = testStart.Add(TimeSpan.FromSeconds(double.Parse(node.Attributes["time"].Value)));

                if (!testPassed)
                {
                    testResult = String.Format(
                        "{0} - Type: {1} - Details: {2}",
                        failureNode.LocalName,
                        failureNode.Attributes["type"].Value,
                        failureNode.InnerText
                    );
                }

                RecordResult(
                    node.Attributes["name"].Value,
                    testPassed,
                    testResult,
                    testStart,
                    testEnd
                );

                testStart = testEnd;
            }

        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <remarks>
        /// This should return a user-friendly string describing what the Action does
        /// and the state of its important persistent properties.
        /// </remarks>
        public override string ToString()
        {
            return String.Format(
                "Run{0} PHPUnit Tests{1}{2}{3} in {4}",
                this.TestToRun == "." ? " all" : "",
                Util.ConcatNE(" (", Util.NullIf(this.TestToRun, "."), ")"),
                String.IsNullOrEmpty(this.TestToRun) ? " using the phpunit.xml or phpunit.xml.dist XML configuration file" : "",
                Util.ConcatNE(" with the additional arguments \"", this.AdditionalArguments, "\""),
                String.IsNullOrEmpty(this.OverriddenSourceDirectory) ? "the default directory" : this.OverriddenSourceDirectory
            );
        }

        /// <summary>
        /// Removes whitespace and ensures leading and trailing quotation marks around the specified path.
        /// If the path is null or empty, the empty string is returned.
        /// </summary>
        /// <param name="path">The path.</param>
        private static string QuotePath(string path)
        {
            if (String.IsNullOrEmpty(path)) 
                return String.Empty;
            return "\"" + path.Trim().Trim('"') + "\"";
        }
    }
}
