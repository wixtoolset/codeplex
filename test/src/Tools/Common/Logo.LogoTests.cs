﻿//-----------------------------------------------------------------------
// <copyright file="Logo.LogoTests.cs" company="Microsoft">
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
//     Test how different Wix tools handle the NoLogo switch.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Tests.Tools.Common.Logo
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Tools.WindowsInstallerXml.Test;

    /// <summary>
    /// Test how different Wix tools handle the NoLogo switch.
    /// </summary>
    [TestClass]
    public class LogoTests
    {
        private static readonly string LogoOutputRegexString = @"Microsoft \(R\) Windows Installer Xml {0} version 3\.0\.\d\d\d\d.0" + Environment.NewLine + @"Copyright \(C\) Microsoft Corporation\. All rights reserved\.";
        private List<WixTool> wixTools;
        
        [TestInitialize]
        public void TestInitialize()
        {
            wixTools = new List<WixTool>();
            wixTools.Add(new Candle());
            wixTools.Add(new Dark());
            wixTools.Add(new Light());
            wixTools.Add(new Lit());
            wixTools.Add(new Pyro());
            wixTools.Add(new Smoke());
            wixTools.Add(new Torch());
        }

        [TestMethod]
        [Description("Verify that different Wix tools print the Logo information.")]
        [Priority(2)]
        public void PrintLogo()
        {
            foreach (WixTool wixTool in this.wixTools)
            {
                wixTool.NoLogo = false;
                wixTool.ExpectedOutputRegexs.Add(new Regex(string.Format(LogoTests.LogoOutputRegexString, Regex.Escape(wixTool.ToolDescription))));
                wixTool.Run();
            }
        }

        [TestMethod]
        [Description("Verify that different Wix tools do not print the Logo information.")]
        [Priority(2)]
        public void PrintWithoutLogo()
        {
            bool missingLogo = false;
            string errorMessage = string.Empty;

            foreach (WixTool wixTool in this.wixTools)
            {
                wixTool.NoLogo = true;
                Result result = wixTool.Run();

                Regex LogoOutputRegex = new Regex("(.)*" + string.Format(LogoTests.LogoOutputRegexString, Regex.Escape(wixTool.ToolDescription)) + "(.)*");

                if (LogoOutputRegex.IsMatch(result.StandardOutput))
                {
                    missingLogo = true;
                    errorMessage += string.Format("Wix Tool {0} prints the Logo information with -nolog set.{1}", wixTool.ToolDescription, Environment.NewLine);
                }
            }

            Assert.IsFalse(missingLogo, errorMessage);
        }


        [TestMethod]
        [Description("Verify that logo is printed before any other warnings/messages.")]
        [Priority(2)]
        public void LogoPrintingOrder()
        {
            bool missingLogo = false;
            string errorMessage = string.Empty;

            foreach (WixTool wixTool in this.wixTools)
            {
                wixTool.NoLogo = false;
                wixTool.OtherArguments = " -InvalidCommandLineArgument";
                Result result = wixTool.Run();
                Regex LogoOutputRegex = new Regex(string.Format(LogoTests.LogoOutputRegexString, Regex.Escape(wixTool.ToolDescription)) + "(.)*");

                if (!LogoOutputRegex.IsMatch(result.StandardOutput))
                {
                    missingLogo = true;
                    errorMessage += string.Format("Wix Tool {0} Logo information does not show as the first line with -nolog set.{1}", wixTool.ToolDescription, Environment.NewLine);
                }
            }

            Assert.IsFalse(missingLogo, errorMessage);
        }
    }
}