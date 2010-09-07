﻿//-----------------------------------------------------------------------
// <copyright file="MspFixture.cs" company="Microsoft">
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
//     - Tests for Burn MSP feature.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Tests.Burn.RebootResume
{
    using Microsoft.Tools.WindowsInstallerXml.Test.Tests.Burn.CommonTestFixture;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests that cover burnstub.exe MSP scenarios.  
    /// </summary>
    [TestClass]
    public class MspTests : BurnTests
    {
        private MspFixture fixture;

        #region Additional test attributes
        [TestInitialize()]
        public void MyTestInitialize()
        {
            WixTests.SetTraceToOutputToConsole();

            fixture = new MspFixture();
            fixture.CleanUp();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            fixture.CleanUp();
        }

        #endregion

        [TestMethod]
        [Description("verify the burnstub.exe will install MSP and Patches as expected")]
        [Timeout(1800000)] // 30 minutes 
        [TestProperty("IsRuntimeTest", "true")]
        public void BurnMspMultiTarget()
        {
            // BUGBUG convert this to a Data-Driven test

            //MspFixture.MsiInstallState msiInstallState = (MspFixture.MsiInstallState)Enum.Parse(typeof(MspFixture.MsiInstallState), (string)TestContext.DataRow[0], true);
            //MspFixture.MspPayloadType mspPayloadType = (MspFixture.MspPayloadType)Enum.Parse(typeof(MspFixture.MspPayloadType), (string)TestContext.DataRow[1], true);
            //BurnCommonTestFixture.InstallMode installMode = (BurnCommonTestFixture.InstallMode)Enum.Parse(typeof(BurnCommonTestFixture.InstallMode), (string)TestContext.DataRow[2], true);
            //BurnCommonTestFixture.UiMode uiMode = (BurnCommonTestFixture.UiMode)Enum.Parse(typeof(BurnCommonTestFixture.UiMode), (string)TestContext.DataRow[3], true);
            //BurnCommonTestFixture.UserType userType = (BurnCommonTestFixture.UserType)Enum.Parse(typeof(BurnCommonTestFixture.UserType), (string)TestContext.DataRow[4], true);

            MspFixture.MsiInstallState msiInstallState = MspFixture.MsiInstallState.MsiAMsiB;
            MspFixture.MspPayloadType mspPayloadType = MspFixture.MspPayloadType.MspAB;
            BurnCommonTestFixture.InstallMode installMode = BurnCommonTestFixture.InstallMode.install;
            BurnCommonTestFixture.UiMode uiMode = BurnCommonTestFixture.UiMode.Passive;
            BurnCommonTestFixture.UserType userType = BurnCommonTestFixture.UserType.CurrentUser;

            fixture.RunTest(msiInstallState,
                mspPayloadType,
                installMode,
                uiMode,
                userType);
            Assert.IsTrue(fixture.TestPasses(), "Failed!");
        }

    }
}
