//-----------------------------------------------------------------------
// <copyright file="SuppressWarningTests.cs" company="Microsoft">
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
// <summary>Tests how Candle handles the suppress warning switch.</summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Tests.Tools.Candle.SuppressWarnings
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Tools.WindowsInstallerXml.Test;
    
    /// <summary>
    /// Test how Candle handles the suppress warning switch.
    /// </summary>
    [TestClass]
    public class SuppressWarningsTests
    {
        private static string testFile = Environment.ExpandEnvironmentVariables(@"%WIX%\test\data\Tools\Candle\SuppressWarningsTests\Product.wxs");

        [TestMethod]
        [Description("Verify that Candle honors the sw switch when specified.")]
        [Priority(2)]
        public void SuppressSpecificWarnings()
        {
            Candle candle = new Candle();
            candle.SourceFiles.Add(testFile);
            candle.SuppressWarnings.Add(1075);
            candle.ExpectedWixMessages.Add(new WixMessage(1096, WixMessage.MessageTypeEnum.Warning));
            candle.Run();
        }

        [TestMethod]
        [Description("Verify that Candle does not suppress warnings when the sw switch is not specified.")]
        [Priority(2)]
        public void NoSuppressWarningsSwitch()
        {
            Candle candle = new Candle();
            candle.SourceFiles.Add(testFile);
            candle.ExpectedWixMessages.Add(new WixMessage(1096, WixMessage.MessageTypeEnum.Warning));
            candle.ExpectedWixMessages.Add(new WixMessage(1075, WixMessage.MessageTypeEnum.Warning));
            candle.Run();
        }

        [TestMethod]
        [Description("Verify that all warnings are suppressed with the sw switch.")]
        [Priority(2)]
        public void SuppressAllWarningsSwitch()
        {
            Candle candle = new Candle();
            candle.SourceFiles.Add(testFile);
            candle.SuppressAllWarnings = true;
            candle.Run();
        }

        [TestMethod]
        [Description("Verify that Candle honors the swall switch when specified and displays that the switch is deprecated.")]
        [Priority(2)]
        public void VerifyDeprecatedSwitch()
        {
            Candle candle = new Candle();
            candle.SourceFiles.Add(testFile);
            candle.OtherArguments = "-swall";
            candle.ExpectedWixMessages.Add(new WixMessage(1108, "The command line switch 'swall' is deprecated. Please use 'sw' instead.", WixMessage.MessageTypeEnum.Warning));
            candle.Run();
        }
    }
}