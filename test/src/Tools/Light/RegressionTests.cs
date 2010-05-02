//-----------------------------------------------------------------------
// <copyright file="RegressionTests.cs" company="Microsoft">
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
// <summary>Regresssion tests for Light</summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Tests.Tools.Light
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Microsoft.Tools.WindowsInstallerXml.Test;

    /// <summary>
    /// Regresssion tests for Light
    /// </summary>
    [TestClass]
    public class RegressionTests
    {
        [TestMethod]
        [Description("Verify that Light can handle directories that don't exist in the -out argument. The directory should be created.")]
        [TestProperty("Bug Link", "http://sourceforge.net/tracker/index.php?func=detail&aid=1771330&group_id=105970&atid=642714")]
        [Priority(3)]
        public void NonExistentDirectoryOutArgument()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Candle candle = new Candle();
            candle.SourceFiles.Add(Path.Combine(WixTests.SharedAuthoringDirectory, "BasicProduct.wxs"));
            candle.Run();

            // Run light with an output directory that does not exist
            string nonExistentDirectory = Path.Combine(tempDirectory, Path.GetRandomFileName());

            Light light = new Light(candle);
            light.OutputFile = Path.Combine(nonExistentDirectory, "test.msi");
            light.Run();

            Assert.IsTrue(File.Exists(light.OutputFile), "The output file was not created");
        }

        [TestMethod]
        [Description("Verify that Light does not ignore commandline arguments it does not recognize.")]
        [Priority(2)]
        public void InvalidCommandLineArguments()
        {
            Candle candle = new Candle();
            candle.SourceFiles.Add(@"%WIX%\test\data\SharedData\Authoring\BasicProduct.wxs");
            candle.Run();

            Light light = new Light();
            light.ObjectFiles = candle.ExpectedOutputFiles;
            light.OtherArguments = " -abc";
            light.ExpectedWixMessages.Add(new WixMessage(1098, "'abc' is not a valid command line argument.", WixMessage.MessageTypeEnum.Warning));
            light.ExpectedExitCode = 0;
            light.Run();
        }

        [TestMethod]
        [Description("Verify that Light displays an error when the wrong assembly type is specified for a file")]
        [TestProperty("Bug Link", "http://sourceforge.net/tracker/index.php?func=detail&aid=1575866&group_id=105970&atid=642714")]
        [Priority(2)]
        public void IncorrectAssemblyType()
        {
            Candle candle = new Candle();
            string testFile = Environment.ExpandEnvironmentVariables(@"%WIX%\test\data\SharedData\Files\TextFile1.txt");
            candle.SourceFiles.Add(@"%WIX%\test\data\Tools\Light\RegressionTests\IncorrectAssemblyType\IncorrectAssemblyType.wxs");
            candle.Run();

            Light light = new Light();
            light.ObjectFiles = candle.ExpectedOutputFiles;
            string outputString = String.Format("The assembly file '{0}' appears to be invalid.  Please ensure this is a valid assembly file and that the user has the appropriate access rights to this file.  More information: HRESULT: 0x80131018", testFile);
            light.ExpectedWixMessages.Add(new WixMessage(132, outputString, WixMessage.MessageTypeEnum.Error));
            light.ExpectedExitCode = 132;
            light.Run();
        }

        [TestMethod]
        [Description("Verify that there is an error from Light stating that a path must be specified with -cc")]
        [Priority(1)]
        public void MissingCabCachePath()
        {
            Candle candle = new Candle();
            candle.SourceFiles.Add(@"%WIX%\test\data\SharedData\Authoring\BasicProduct.wxs");
            candle.Run();

            Light light = new Light();
            light.ObjectFiles = candle.ExpectedOutputFiles;
            light.ReuseCab = true;
            light.OtherArguments = " -cc";
            light.ExpectedExitCode = 280;
            light.ExpectedWixMessages.Add(new WixMessage(280,String.Concat("The -cc option requires a directory, but the provided path is a file: " , candle.ExpectedOutputFiles[0]), WixMessage.MessageTypeEnum.Error));
            light.Run();
        }
    }
}