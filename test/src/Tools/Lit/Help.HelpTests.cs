//-----------------------------------------------------------------------
// <copyright file="Help.HelpTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//    
//    The use and distribution terms for this software are covered by the
//    Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
//    which can be found in the file CPL.TXT at the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by
//    the terms of this license.
//    
//    You must not remove this notice, or any other, from this software.
// </copyright>
// <summary>
//     Test that the help is printed correctly
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Tests.Tools.Lit.Help
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Tools.WindowsInstallerXml.Test;

    /// <summary>
    /// Test that the help is printed correctly
    /// </summary>
    [TestClass]
    public class HelpTests : WixTests
    {
        [TestMethod]
        [Description("Verify that Lit help text is printed correctly")]
        [Priority(2)]
        public void Help()
        {
            Lit lit = new Lit();
            lit.Help = true;
            this.AddExpectedHelpText(lit);     
            lit.Run();
        }

        [TestMethod]
        [Description("Verify that Lit ignores other commandline switches when after /?")]
        [Priority(2)]
        public void IgnoreOtherSwitches()
        {
            Lit lit = new Lit();
            lit.Help = true;
            lit.OtherArguments = " -abc";
            this.AddExpectedHelpText(lit);
            lit.Run();
        }

        /// <summary>
        /// Add expected help text to lit object.
        /// </summary>
        /// <param name="candle">Lit object.</param>
        private void AddExpectedHelpText(Lit lit)
        {
            lit.ExpectedOutputStrings.Add("usage:  lit.exe [-?] [-nologo] [-out libraryFile] objectFile [objectFile ...]");
            lit.ExpectedOutputStrings.Add("-b         base path to locate all files (default: current directory)");
            lit.ExpectedOutputStrings.Add("-bf        bind files into the library file");
            lit.ExpectedOutputStrings.Add("-ext <extension>  extension assembly or \"class, assembly\"");
            lit.ExpectedOutputStrings.Add("-loc <loc.wxl>  bind localization strings from a wxl into the library");
            lit.ExpectedOutputStrings.Add("-nologo    skip printing lit logo information");
            lit.ExpectedOutputStrings.Add("-o[ut]     specify output file (default: write to current directory)");
            lit.ExpectedOutputStrings.Add("-pedantic  show pedantic messages");
            lit.ExpectedOutputStrings.Add("-ss        suppress schema validation of documents (performance boost)");
            lit.ExpectedOutputStrings.Add("-sv        suppress intermediate file version mismatch checking");
            lit.ExpectedOutputStrings.Add("-sw[N]     suppress all warnings or a specific message ID");
            lit.ExpectedOutputStrings.Add("(example: -sw1011 -sw1012)");
            lit.ExpectedOutputStrings.Add("-swall     suppress all warnings (deprecated)");
            lit.ExpectedOutputStrings.Add("-v         verbose output");
            lit.ExpectedOutputStrings.Add("-wx[N]     treat all warnings or a specific message ID as an error");
            lit.ExpectedOutputStrings.Add("(example: -wx1011 -wx1012)");
            lit.ExpectedOutputStrings.Add("-wxall     treat all warnings as errors (deprecated)");
            lit.ExpectedOutputStrings.Add("-? | -help this help information");
            lit.ExpectedOutputStrings.Add("For more information see: http://wix.sourceforge.net");
        }
    }
}