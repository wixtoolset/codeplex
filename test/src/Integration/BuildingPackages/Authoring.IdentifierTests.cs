﻿//-----------------------------------------------------------------------
// <copyright file="Authoring.IdentifierTests.cs" company="Microsoft">
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
//     Tests for Identifiers (Ids)
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Tests.Integration.BuildingPackages.Authoring
{
    using System;
    using System.IO;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Tools.WindowsInstallerXml.Test;

    /// <summary>
    /// Tests for Identifiers (Ids)
    /// </summary>
    [TestClass]
    public class IdentifierTests
    {
        private static readonly string TestDataDirectory = Environment.ExpandEnvironmentVariables(@"%WIX%\test\data\Integration\BuildingPackages\Authoring\IdentifierTests");

        [TestMethod]
        [Description("Verify that Identifiers with long names can be defined and referenced.")]
        [Priority(1)]
        [TestProperty("Bug Link", "https://sourceforge.net/tracker/index.php?func=detail&aid=1867881&group_id=105970&atid=642714")]
        public void LongIdentifiers()
        {
            string longDirectoryName = "Directory_01234567890123456789012345678901234567890123456789012345678901234567890123456789";
            string longComponentName = "Component_01234567890123456789012345678901234567890123456789012345678901234567890123456789";
            string longFileName = "Test_txt_01234567890123456789012345678901234567890123456789012345678901234567890123456789";

            Candle candle = new Candle();
            candle.SourceFiles.Add(Path.Combine(IdentifierTests.TestDataDirectory, @"LongIdentifiers\product.wxs"));
            candle.ExpectedWixMessages.Add(new WixMessage(1026, string.Format("The Directory/@Id attribute's value, '{0}', is too long for an identifier.  Standard identifiers are 72 characters long or less.", longDirectoryName),WixMessage.MessageTypeEnum.Warning));
            candle.ExpectedWixMessages.Add(new WixMessage(1026, string.Format("The Component/@Id attribute's value, '{0}', is too long for an identifier.  Standard identifiers are 72 characters long or less.", longComponentName), WixMessage.MessageTypeEnum.Warning));
            candle.ExpectedWixMessages.Add(new WixMessage(1026, string.Format("The File/@Id attribute's value, '{0}', is too long for an identifier.  Standard identifiers are 72 characters long or less.", longFileName), WixMessage.MessageTypeEnum.Warning));
            candle.ExpectedWixMessages.Add(new WixMessage(1026, string.Format("The ComponentRef/@Id attribute's value, '{0}', is too long for an identifier.  Standard identifiers are 72 characters long or less.", longComponentName), WixMessage.MessageTypeEnum.Warning));
            candle.Run();

            Light light = new Light(candle);
            light.SuppressedICEs.Add("ICE03");
            light.Run();

            // verify long names in the resulting msi
            string query = string.Format("SELECT `Directory` FROM `Directory` WHERE `Directory` = '{0}'", longDirectoryName);
            string queryResult = Verifier.Query(light.OutputFile, query);
            Assert.AreEqual(longDirectoryName, queryResult);

            query = string.Format("SELECT `Component` FROM `Component` WHERE `Component` = '{0}'", longComponentName);
            queryResult = Verifier.Query(light.OutputFile, query);
            Assert.AreEqual(longComponentName, queryResult);

            query = string.Format("SELECT `File` FROM `File` WHERE `File` = '{0}'", longFileName);
            queryResult = Verifier.Query(light.OutputFile, query);
            Assert.AreEqual(longFileName, queryResult);

            query = string.Format("SELECT `Component_` FROM `FeatureComponents` WHERE `Component_` = '{0}'", longComponentName);
            queryResult = Verifier.Query(light.OutputFile, query);
            Assert.AreEqual(longComponentName, queryResult);
        }
    }
}