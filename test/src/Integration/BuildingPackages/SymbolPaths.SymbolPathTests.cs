//-----------------------------------------------------------------------
// <copyright file="SymbolPaths.SymbolPathTests.cs" company="Microsoft">
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
//     Tests for SymbolPath elements.
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Tests.Integration.BuildingPackages.SymbolPaths
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Microsoft.Tools.WindowsInstallerXml.Test;

    /// <summary>
    /// Tests for SymbolPath elements.
    /// </summary>
    [TestClass]
    public class SymbolPathTests
    {
        private static readonly string TestDataDirectory = Environment.ExpandEnvironmentVariables(@"%WIX%\test\data\Integration\BuildingPackages\SymbolPaths\SymbolPathTests");

        [TestMethod]
        [Description("Verify that a SymbolPath element can exist under a component")]
        [Priority(2)]
        public void ComponentSymbolPath()
        {
            string sourceFile = Path.Combine(SymbolPathTests.TestDataDirectory, @"ComponentSymbolPath\product.wxs");
            string msi = Builder.BuildPackage(sourceFile, "test.msi");
            Verifier.VerifyResults(Path.Combine(SymbolPathTests.TestDataDirectory, @"ComponentSymbolPath\expected.msi"), msi);
        }
    }
}