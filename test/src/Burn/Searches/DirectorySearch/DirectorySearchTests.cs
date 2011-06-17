﻿//-----------------------------------------------------------------------
// <copyright file="DirectorySearchTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//    
//    The use and distribution terms for this software are covered by the
//    Common Public License 1.0 (http://opensource.org/licenses/cpl1.0.php)
//    which can be found in the file CPL.TXT at the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by
//    the terms of this license.
//    
//    You must not remove this notice, or any other, from this software.
// </copyright>
// <summary>
//     - Tests for Burn Directory Search feature.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Tests.Burn.Searches
{
    using System;
    using Microsoft.Tools.WindowsInstallerXml.Test.Tests;
    using Microsoft.Tools.WindowsInstallerXml.Test.Tests.Burn;
    using Microsoft.Tools.WindowsInstallerXml.Test.Tests.Burn.Searches;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Tools.WindowsInstallerXml.Test.Burn.OM.WixAuthoringOM.Bundle.Searches;

    [TestClass]
    public class DirectorySearchTests : BurnTests
    {
        private BurnDirectorySearchFixture fixture;

        #region Additional test attributes
        [TestInitialize()]
        public void MyTestInitialize()
        {
            WixTests.SetTraceToOutputToConsole();

            fixture = new BurnDirectorySearchFixture();
            fixture.CleanUp();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            fixture.CleanUp();
        }

        #endregion

        [TestMethod]
        [Description("Directory search")]
        [Timeout(1800000)] // 30 minutes 
        [DeploymentItem(@"%WIX_ROOT%\test\src\Burn\Searches\DirectorySearch\DirectorySearchData.xlsx")]
        [DataSource("System.Data.OleDb",
            "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=DirectorySearchData.xlsx;Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\"",
            "Install$",
            DataAccessMethod.Sequential)]
        public void BurnDirectorySearchDataDriven()
        {
            string id = (string)TestContext.DataRow[0];

            DirectorySearchElement.DirectorySearchResultType type = (DirectorySearchElement.DirectorySearchResultType)Enum.Parse
                (typeof(DirectorySearchElement.DirectorySearchResultType), (string)TestContext.DataRow[1], true);

            string path = Environment.ExpandEnvironmentVariables(TestContext.DataRow[2].ToString());

            string variable = (string)TestContext.DataRow[3];

            string ManifestAuthoringPath = TestContext.DataRow[4].ToString();

            fixture.CreateDirectorySearchElement(id, type, ManifestAuthoringPath, variable);

            bool result = fixture.SearchResultVerification(path, variable);

            Assert.IsTrue(result
                , string.Format("Directory search result does not matches with expected value for path: {0}. See trace for more detail"
                , path));

        }
    }
}