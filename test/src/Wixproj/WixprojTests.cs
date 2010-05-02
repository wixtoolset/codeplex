﻿//-----------------------------------------------------------------------
// <copyright file="WixprojTests.cs" company="Microsoft">
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
//     Tests for Wix projects
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Tests.Wixproj
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Tools.WindowsInstallerXml.Test;

    /// <summary>
    /// Tests for Wix projects
    /// </summary>
    [TestClass]
    public class WixprojTests : WixTests
    {
        private static readonly string TestDataDirectory = Environment.ExpandEnvironmentVariables(@"%WIX%\test\data\Wixproj\WixprojTests");

        [TestMethod]
        [Description("Verify that a simple MSI gets built")]
        [Priority(2)]
        public void SimpleInstaller()
        {
            WixprojMSBuild wixproj = new WixprojMSBuild(Builder.GetUniqueFileName());
            wixproj.ProjectFile = Path.Combine(WixprojTests.TestDataDirectory, @"SimpleInstaller\WixProject.wixproj");            
            wixproj.Run();

            string expectedMSI = Path.Combine(wixproj.OutputPath, "WixProject.msi");
            Assert.IsTrue(File.Exists(expectedMSI), "Could not find the expected output file {0}", expectedMSI);
        }

        [TestMethod]
        [Description("Verify that the proper parameters get passed to the tasks when building an MSI")]
        [Priority(1)]
        public void InstallerWithParameters()
        {
            WixprojMSBuild wixproj = new WixprojMSBuild(Builder.GetUniqueFileName());
            wixproj.ProjectFile = Path.Combine(WixprojTests.TestDataDirectory, @"InstallerWithParameters\WixProject.wixproj");
            wixproj.Run();

            wixproj.AssertTaskSubstring("Candle", "-dVar1=1");
            wixproj.AssertTaskSubstring("Candle", "-dVar2");
            wixproj.AssertTaskSubstring("Candle", "-d\"Var3=<3>\"");
            wixproj.AssertTaskSubstring("Candle", "-dVar4=4");
            wixproj.AssertTaskSubstring("Candle", "-pedantic");
            wixproj.AssertTaskSubstring("Candle", "-sw1");
            wixproj.AssertTaskSubstring("Candle", "-wx");
            wixproj.AssertTaskSubstring("Candle", "-v");

            wixproj.AssertTaskSubstring("Light", "-cultures:en-US");
            wixproj.AssertTaskSubstring("Light", "-dWixVar1=1");
            wixproj.AssertTaskSubstring("Light", "-dWixVar2=2");
            wixproj.AssertTaskSubstring("Light", "-notidy");
            wixproj.AssertTaskSubstring("Light", "-pedantic");
            wixproj.AssertTaskSubstring("Light", "-spdb");
            wixproj.AssertTaskSubstring("Light", "-sw1");
            wixproj.AssertTaskSubstring("Light", "-wx");
            wixproj.AssertTaskSubstring("Light", "-v");
        }

        [TestMethod]
        [Description("Verify that the wixproj references are imported correctly")]
        [Priority(1)]
        public void WixprojWithReferences()
        {
            WixprojMSBuild wixproj = new WixprojMSBuild(Builder.GetUniqueFileName());
            wixproj.ProjectFile = Path.Combine(WixprojTests.TestDataDirectory, @"WixprojWithReferences\WixProject.wixproj");
            wixproj.Run();

            string expectedMSI = Path.Combine(wixproj.OutputPath, "WixProject.msi");
            Assert.IsTrue(File.Exists(expectedMSI), "Could not find the expected output file {0}", expectedMSI);
        }

        [TestMethod]
        [Description("Verify that a simple MSM gets built")]
        [Priority(2)]
        public void SimpleMergeModule()
        {
            WixprojMSBuild wixproj = new WixprojMSBuild(Builder.GetUniqueFileName());
            wixproj.ProjectFile = Path.Combine(WixprojTests.TestDataDirectory, @"SimpleMergeModule\WixProject.wixproj");
            wixproj.Run();

            string expectedMSM = Path.Combine(wixproj.OutputPath, "WixProject.msm");
            Assert.IsTrue(File.Exists(expectedMSM), "Could not find the expected output file {0}", expectedMSM);
        }

        [TestMethod]
        [Description("Verify that the proper parameters get passed to the tasks when building an MSI")]
        [Priority(2)]
        public void MergeModuleWithParameters()
        {
            WixprojMSBuild wixproj = new WixprojMSBuild(Builder.GetUniqueFileName());
            wixproj.ProjectFile = Path.Combine(WixprojTests.TestDataDirectory, @"MergeModuleWithParameters\WixProject.wixproj");
            wixproj.Run();

            wixproj.AssertTaskSubstring("Candle", "-dVar1=1");
            wixproj.AssertTaskSubstring("Candle", "-dVar4=4");
            wixproj.AssertTaskSubstring("Candle", "-sw");

            wixproj.AssertTaskSubstring("Light", "-cultures:ja-JP");
            wixproj.AssertTaskSubstring("Light", "-dWixVar1=1");
            wixproj.AssertTaskSubstring("Light", "-dWixVar2=2");
        }

        [TestMethod]
        [Description("Verify that a simple Wixlib gets built")]
        [Priority(2)]
        public void SimpleLibrary()
        {
            WixprojMSBuild wixproj = new WixprojMSBuild(Builder.GetUniqueFileName());
            wixproj.ProjectFile = Path.Combine(WixprojTests.TestDataDirectory, @"SimpleLibrary\WixProject.wixproj");
            wixproj.Run();

            string expectedWixlib = Path.Combine(wixproj.OutputPath, "WixProject.wixlib");
            Assert.IsTrue(File.Exists(expectedWixlib), "Could not find the expected output file {0}", expectedWixlib);
        }

        [TestMethod]
        [Description("Verify that the proper parameters get passed to the tasks when building a Wixlib")]
        [Priority(2)]
        public void LibraryWithParameters()
        {
            WixprojMSBuild wixproj = new WixprojMSBuild(Builder.GetUniqueFileName());
            wixproj.ProjectFile = Path.Combine(WixprojTests.TestDataDirectory, @"LibraryWithParameters\WixProject.wixproj");
            wixproj.Run();

            wixproj.AssertTaskSubstring("Candle", "-dVar1=1");
            wixproj.AssertTaskSubstring("Candle", "-dVar4=4");
            wixproj.AssertTaskSubstring("Candle", "-sw1");
            wixproj.AssertTaskSubstring("Candle", "-wx");

            wixproj.AssertTaskSubstring("Lit", "-bf");
            wixproj.AssertTaskSubstring("Lit", "-nologo");
            wixproj.AssertTaskSubstring("Lit", "-sw1");
            wixproj.AssertTaskSubstring("Lit", "-wx");
        }
    }
}