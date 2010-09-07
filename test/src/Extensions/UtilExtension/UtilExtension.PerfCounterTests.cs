//-----------------------------------------------------------------------
// <copyright file="UtilExtension.PerfCounterTests.cs" company="Microsoft">
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
// <summary>Util Extension PerfCounter tests</summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Tests.Extensions.UtilExtension
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Microsoft.Tools.WindowsInstallerXml.Test;
    using Microsoft.Tools.WindowsInstallerXml.Test.Verifiers;
    using Microsoft.Tools.WindowsInstallerXml.Test.Verifiers.Extensions;
    
    using Microsoft.Win32;
   
    /// <summary>
    /// Util extension PerfCounter element tests
    /// </summary>
    [TestClass]
    public class PerfCounterTests : WixTests
    {
        private static readonly string TestDataDirectory = Environment.ExpandEnvironmentVariables(@"%WIX_ROOT%\test\data\Extensions\UtilExtension\PerfCounterTests");

        [TestMethod]
        [Description("Verify that the (Perfmon and CustomAction) Tables are created in the MSI and have expected data.")]
        [Priority(1)]
        public void PerfCounter_VerifyMSITableData()
        {
            string sourceFile = Path.Combine(PerfCounterTests.TestDataDirectory, @"product.wxs");
            string msiFile = Builder.BuildPackage(Environment.CurrentDirectory, sourceFile, "test.msi", "-ext WixUtilExtension -sw5153 ", "-ext WixUtilExtension"); // suppress the depricated warrning message

            Verifier.VerifyCustomActionTableData(msiFile,
                new CustomActionTableData("ConfigurePerfmonInstall", 1, "ScaSchedule", "ConfigurePerfmonInstall"),
                new CustomActionTableData("ConfigurePerfmonUninstall", 1, "ScaSchedule", "ConfigurePerfmonUninstall"),
                new CustomActionTableData("RegisterPerfmon", 3073, "ScaExecute", "RegisterPerfmon"),
                new CustomActionTableData("UnregisterPerfmon", 3073, "ScaExecute", "UnregisterPerfmon"),
                new CustomActionTableData("RollbackRegisterPerfmon", 3329, "ScaExecute", "UnregisterPerfmon"),
                new CustomActionTableData("RollbackUnregisterPerfmon", 3329, "ScaExecute", "RegisterPerfmon"));

            Verifier.VerifyTableData(msiFile, MSITables.Perfmon,
                new TableRow(PerfmonColumns.Component_.ToString(), "TestPerfmonProductComponent"),
                new TableRow(PerfmonColumns.File.ToString(), "[#SymFile.ini]"),
                new TableRow(PerfmonColumns.Name.ToString(), "MyApplication"));
        }

        [TestMethod]
        [Description("Verify that the Performance counter is created upon install.")]
        [Priority(2)]
        [TestProperty("IsRuntimeTest", "true")]
        public void PerfCounter_Install()
        {
            string sourceFile = Path.Combine(PerfCounterTests.TestDataDirectory, @"product.wxs");
            string msiFile = Builder.BuildPackage(Environment.CurrentDirectory, sourceFile, "test.msi", "-ext WixUtilExtension -sw5153 ", "-ext WixUtilExtension"); // suppress the depricated warrning message

            MSIExec.InstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            string keyName = @"system\currentcontrolset\services\MyApplication\Performance";
            string valueName = "PerfIniFile";
            string expectedValue = "SymFile.ini";
            RegistryVerifier.VerifyRegistryKeyValue(RegistryHive.LocalMachine, keyName, valueName, expectedValue);

            MSIExec.UninstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);
        }

        [TestMethod]
        [Description("Verify that the Performance counter was removed upon rollback.")]
        [Priority(2)]
        [TestProperty("IsRuntimeTest", "true")]
        public void PerfCounter_InstallFailure()
        {
            string sourceFile = Path.Combine(PerfCounterTests.TestDataDirectory, @"product_fail.wxs");
            string msiFile = Builder.BuildPackage(Environment.CurrentDirectory, sourceFile, "test.msi", "-ext WixUtilExtension -sw5153 ", "-ext WixUtilExtension"); // suppress the depricated warrning message

            MSIExec.InstallProduct(msiFile, MSIExec.MSIExecReturnCode.ERROR_INSTALL_FAILURE);

            string registryKey = @"system\currentcontrolset\services\InstrumentationDemo";
            
            // Verify that the key was not created
            Assert.IsFalse(RegistryVerifier.RegistryKeyExists(RegistryHive.LocalMachine, registryKey), "Registry Key '{0}' was not removed on Rollback.", registryKey);
        }
    }
}
