﻿//-----------------------------------------------------------------------
// <copyright file="LayoutManagerUnitTests.cs" company="Microsoft">
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
//     - Tests for the LayoutManager (part of the Burn test infrastructure)
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Tests.Burn.BurnTestToolsUnitTests.LayoutManager
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// These tests just build layouts containing different elements supported by Burn,
    /// launch setup and cancel.  They do NOT install, repair, uninstall anything.
    /// This is done only to verify a layout generated by the LayoutManager is usable.  
    /// These tests are not testing Burn functionality.  Burn functional tests live in WixTests.
    /// </summary>
    [TestClass]
    public class LayoutManagerUnitTests
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() 
        {
            lm = new Microsoft.Tools.WindowsInstallerXml.Test.Burn.LayoutManager.LayoutManager(
                new Microsoft.Tools.WindowsInstallerXml.Test.Burn.LayoutManager.UX.TestUX());
        }
        
        #endregion

        private string testMsiFile = System.Environment.ExpandEnvironmentVariables(@"%WIX_ROOT%\test\data\BurnTestPayloads\MSIsandMSPs\RtmProduct\product.msi");
        private string testMspFile = System.Environment.ExpandEnvironmentVariables(@"%WIX_ROOT%\test\data\BurnTestPayloads\MSIsandMSPs\GDR1\gdr1.msp"); // MSP that will target testMsiFile
        private string testExeFile = System.Environment.ExpandEnvironmentVariables(@"%WIX_ROOT%\test\data\BurnTestPayloads\Products\TestExe\TestExe.exe");
        private string testFileFile = System.Environment.ExpandEnvironmentVariables(@"%WIX_ROOT%\test\data\BurnTestPayloads\TxtFiles\10000000b.txt");
        private string testExeUrl = System.Environment.ExpandEnvironmentVariables(@"%BURN_TEST_WEBSERVER_ROOT%/BurnTestPayloads/Products/TestExe/TestExe.exe");
        private Microsoft.Tools.WindowsInstallerXml.Test.Burn.LayoutManager.LayoutManager lm;


        [TestMethod]
        [Description("verify the burnstub.exe will launch without blowing up because stuff is missing in the layout or the parameterinfo.xml isn't formed correctly")]
        public void IT_LayoutManager_Exe()
        {
            lm.AddExe(testExeFile, true);
            lm.BuildBundle();
            Assert.AreEqual(0, LaunchAndCloseSetup(lm));
        }


        [TestMethod]
        [Description("verify the burnstub.exe will launch without blowing up because stuff is missing in the layout or the parameterinfo.xml isn't formed correctly")]
        public void IT_LayoutManager_Msi()
        {
            lm.AddMsi(testMsiFile, null, null, true, string.Empty, string.Empty, string.Empty, string.Empty);
            lm.BuildBundle();
            Assert.AreEqual(0, LaunchAndCloseSetup(lm));
        }

        [TestMethod]
        [Description("verify the burnstub.exe will launch without blowing up because stuff is missing in the layout or the parameterinfo.xml isn't formed correctly")]
        public void IT_LayoutManager_Msp()
        {
            lm.AddMsp(testMspFile, true);
            lm.BuildBundle();
            Assert.AreEqual(0, LaunchAndCloseSetup(lm));
        }

        [TestMethod]
        [Description("verify the burnstub.exe will launch without blowing up because stuff is missing in the layout or the parameterinfo.xml isn't formed correctly")]
        public void IT_LayoutManager_DlExe()
        {
            lm.AddExe(testExeFile, null, testExeUrl, false);
            lm.BuildBundle();
            Assert.AreEqual(0, LaunchAndCloseSetup(lm));
        }


        /// <summary>
        /// Starts setup.exe and then closes the window as soon as it is visible
        /// </summary>
        /// <param name="layout">layout to start</param>
        /// <returns>0 if Burn launched without any errors, non-zero if there were errors (i.e. parameterinfo.xml is malformed)</returns>
        private int LaunchAndCloseSetup(Microsoft.Tools.WindowsInstallerXml.Test.Burn.LayoutManager.LayoutManager layout)
        {
            string setupExe = System.IO.Path.Combine(layout.LayoutFolder, layout.SetupBundleFilename);

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.Arguments = ""; // make it run in UI mode, not silently
            proc.StartInfo.FileName = setupExe;
            proc.Start();

            // wait for Burn UI to initialize before trying to close the window
            do
            {
                System.Threading.Thread.Sleep(100); 
                proc.Refresh();
            }
            while (proc.MainWindowHandle.ToInt64() == (Int64)0);

            proc.CloseMainWindow();
            proc.WaitForExit();
            
            return proc.ExitCode;
        }

    }
}
