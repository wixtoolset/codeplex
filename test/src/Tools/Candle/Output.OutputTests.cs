//-----------------------------------------------------------------------
// <copyright file="Output.OutputTests.cs" company="Microsoft">
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
// <summary>Test how Candle handles the Out switch. </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Tests.Tools.Candle.Output
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Tools.WindowsInstallerXml.Test;
    
    /// <summary>
    /// Test how Candle handles the Out switch.
    /// </summary>
    [TestClass]
    public class OutputTests
    {
        [TestMethod]
        [Description("Verify that Candle handles the -out switch and creates the wixobj in the specified directory.")]
        [Priority(1)]
        public void OutSwitch()
        {
            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Candle candle = new Candle();
            candle.SourceFiles.Add(Path.Combine(Tests.WixTests.SharedAuthoringDirectory, "BasicProduct.wxs"));
            candle.OutputFile = Path.Combine(outputDirectory, "BasicProduct.wixobj"); ;
            candle.Run();
        }

        [TestMethod]
        [Description("Verify that the appropriate error message is generated for output filenames containing illegal characters.")]
        [Priority(2)]
        public void InvalidOutputFileName()
        {
            string[] invalidFileNames = new string[] { "testfile>wixobj", "testfile<wixobj", "testfile?wixobj", "testfile|wixobj", "testfile*wixobj" };
            Candle candle;

            foreach (string invalidFileName in invalidFileNames)
            {
                candle = new Candle();
                candle.SourceFiles.Add(WixTests.BasicProductWxs);
                candle.OutputFile = string.Empty;
                candle.OtherArguments = string.Format(" -out {0}", invalidFileName);
                string expectedOutput = string.Format("Invalid file name specified on the command line: '{0}'. Error message: 'Illegal characters in path.'", invalidFileName);
                candle.ExpectedWixMessages.Add(new WixMessage(284, expectedOutput, WixMessage.MessageTypeEnum.Error));
                candle.ExpectedExitCode = 284;
                candle.Run();
            }
        }

        [TestMethod]
        [Description("Verify that the appropriate error message is generated for output filenames containing double quotes.")]
        [Priority(2)]
        public void DoubleQuotesInOutputFileName()
        {
            Candle candle = new Candle();
            candle.SourceFiles.Add(WixTests.BasicProductWxs);
            candle.OutputFile = string.Empty;
            candle.OtherArguments = " -out testfile\\\"wixobj";
            string expectedOutput2 = string.Format("Your file or directory path '{0}' cannot contain a quote. Quotes are often accidentally introduced when trying to refer to a directory path with spaces in it, such as \"C:\\Out Directory\\\".  The correct representation for that path is: \"C:\\Out Directory\\\\\".", "testfile\"wixobj");
            candle.ExpectedWixMessages.Add(new WixMessage(117, expectedOutput2, WixMessage.MessageTypeEnum.Error));
            candle.ExpectedExitCode = 117;
            candle.Run();
        }
    }
}