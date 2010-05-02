//-----------------------------------------------------------------------
// <copyright file="ICEs.ICEsTests.cs" company="Microsoft">
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
//     Tests for running only the specified ICEs
// </summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Tests.Tools.Light.ICEs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Tools.WindowsInstallerXml.Test;

    /// <summary>
    /// Tests for running only the specified ICEs
    /// </summary>
    [TestClass]
    public class ICEsTests
    {
        private static readonly string TestDataDirectory = Environment.ExpandEnvironmentVariables(@"%WIX%\test\data\Tools\Light\ICEs\ICEsTests");

        [TestMethod]
        [Description("Verify that Light will only run the ICE specified by the -ice switch")]
        [Priority(1)]
        public void SimpleICE()
        {
            Candle candle = new Candle();
            candle.SourceFiles.Add(Path.Combine(ICEsTests.TestDataDirectory, @"SimpleICE\product.wxs"));
            candle.Run();

            Light light = new Light(candle);

            // The product violates ICE16 and ICE18, but ICE18 should not get run because we are using the -ice switch
            light.ICEs.Add("ICE16");
            
            light.ExpectedWixMessages.Add(new WixMessage(204, "ICE16: ProductName: '1234567890123456789012345678901234567890123456789012345678901234567890' is greater than 63 characters in length. Current length: 70", WixMessage.MessageTypeEnum.Error));
            light.ExpectedExitCode = 204;
            light.Run();
        }
    }
}