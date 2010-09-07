//-----------------------------------------------------------------------
// <copyright file="UtilExtension.ServiceConfigTests.cs" company="Microsoft">
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
// <summary>Util Extension ServiceConfig tests</summary>
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
   
    /// <summary>
    /// Util extension ServiceConfig element tests
    /// </summary>
    [TestClass]
    public class ServiceConfigTests : WixTests
    {
        private static readonly string TestDataDirectory = Environment.ExpandEnvironmentVariables(@"%WIX_ROOT%\test\data\Extensions\UtilExtension\ServiceConfigTests");

        [TestMethod]
        [Description("Verify that the (ServiceConfig and CustomAction) Tables are created in the MSI and have expected data.")]
        [Priority(1)]
        public void ServiceConfig_VerifyMSITableData()
        {
            string sourceFile = Path.Combine(ServiceConfigTests.TestDataDirectory, @"product.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixUtilExtension");

            Verifier.VerifyCustomActionTableData(msiFile,
                new CustomActionTableData("SchedServiceConfig", 1, "WixCA", "SchedServiceConfig"),
                new CustomActionTableData("ExecServiceConfig", 3073, "WixCA", "ExecServiceConfig"),
                new CustomActionTableData("RollbackServiceConfig", 3329, "WixCA", "RollbackServiceConfig"));

            // Verify ServiceConfig table contains the right data
            Verifier.VerifyTableData(msiFile, MSITables.ServiceConfig,
                new TableRow(ServiceConfigColumns.ServiceName.ToString(), "W32Time"),
                new TableRow(ServiceConfigColumns.Component_.ToString(), "Component1"),
                new TableRow(ServiceConfigColumns.NewService.ToString(), "0", false),
                new TableRow(ServiceConfigColumns.FirstFailureActionType.ToString(), "restart"),
                new TableRow(ServiceConfigColumns.SecondFailureActionType.ToString(), "reboot"),
                new TableRow(ServiceConfigColumns.ThirdFailureActionType.ToString(), "none"),
                new TableRow(ServiceConfigColumns.ResetPeriodInDays.ToString(), "1", false),
                new TableRow(ServiceConfigColumns.RestartServiceDelayInSeconds.ToString(), string.Empty, false),
                new TableRow(ServiceConfigColumns.ProgramCommandLine.ToString(), string.Empty),
                new TableRow(ServiceConfigColumns.RebootMessage.ToString(), string.Empty));

            Verifier.VerifyTableData(msiFile, MSITables.ServiceConfig,
                new TableRow(ServiceConfigColumns.ServiceName.ToString(), "MynewService"),
                new TableRow(ServiceConfigColumns.Component_.ToString(), "Component2"),
                new TableRow(ServiceConfigColumns.NewService.ToString(), "1", false),
                new TableRow(ServiceConfigColumns.FirstFailureActionType.ToString(), "reboot"),
                new TableRow(ServiceConfigColumns.SecondFailureActionType.ToString(), "restart"),
                new TableRow(ServiceConfigColumns.ThirdFailureActionType.ToString(), "none"),
                new TableRow(ServiceConfigColumns.ResetPeriodInDays.ToString(), "3", false),
                new TableRow(ServiceConfigColumns.RestartServiceDelayInSeconds.ToString(), string.Empty, false),
                new TableRow(ServiceConfigColumns.ProgramCommandLine.ToString(), string.Empty),
                new TableRow(ServiceConfigColumns.RebootMessage.ToString(), string.Empty));
        }

        [TestMethod]
        [Description("Verify that the Services are being installed and configured as expected.")]
        [Priority(2)]
        [TestProperty("IsRuntimeTest", "true")]
        public void ServiceConfig_Install()
        {
            string sourceFile = Path.Combine(ServiceConfigTests.TestDataDirectory, @"product.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixUtilExtension");

            MSIExec.InstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            // Validate Existing Service Information.
            ServiceFailureActionType[] expectedFailureActions = new ServiceFailureActionType[] { ServiceFailureActionType.RestartService, ServiceFailureActionType.RebootComputer, ServiceFailureActionType.None };
            ServiceVerifier.VerifyServiceInformation("W32Time", 1, expectedFailureActions);

            // Validate New Service Information.
            expectedFailureActions = new ServiceFailureActionType[] { ServiceFailureActionType.RebootComputer, ServiceFailureActionType.RestartService, ServiceFailureActionType.None };
            ServiceVerifier.VerifyServiceInformation("MynewService", 3, expectedFailureActions);

            MSIExec.UninstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            // Validate New Service Does NOT exist any more.
            Assert.IsFalse(ServiceVerifier.ServiceExists("MynewService"), "Service '{0}' was NOT removed on Uninstall.", "MynewService");
        }

        [TestMethod]
        [Description("Verify that the Services are is repaired as expected.")]
        [Priority(2)]
        [TestProperty("IsRuntimeTest", "true")]
        public void ServiceConfig_Repair()
        {
            string sourceFile = Path.Combine(ServiceConfigTests.TestDataDirectory, @"product.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixUtilExtension");

            MSIExec.InstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            // Change the service details
            ServiceFailureActionType[] expectedFailureActions = new ServiceFailureActionType[] { ServiceFailureActionType.RestartService, ServiceFailureActionType.RestartService, ServiceFailureActionType.RestartService };
            ServiceVerifier.SetServiceInformation("MynewService", 4, expectedFailureActions);

            MSIExec.RepairProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            // Validate Existing Service Information.
            expectedFailureActions = new ServiceFailureActionType[] { ServiceFailureActionType.RestartService, ServiceFailureActionType.RebootComputer, ServiceFailureActionType.None };
            ServiceVerifier.VerifyServiceInformation("W32Time", 1, expectedFailureActions);

            // Validate New Service Information.
            expectedFailureActions = new ServiceFailureActionType[] { ServiceFailureActionType.RebootComputer, ServiceFailureActionType.RestartService, ServiceFailureActionType.None };
            ServiceVerifier.VerifyServiceInformation("MynewService", 3, expectedFailureActions);

            MSIExec.UninstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            // Validate New Service Does NOT exist any more.
            Assert.IsFalse(ServiceVerifier.ServiceExists("MynewService"), "Service '{0}' was NOT removed on Uninstall.", "MynewService");
        }

        [TestMethod]
        [Description("Verify that the Installation fails if ServiceConfig references a non-existing service.")]
        [Priority(2)]
        [TestProperty("IsRuntimeTest", "true")]
        public void ServiceConfig_NonExistingService()
        {
            string sourceFile = Path.Combine(ServiceConfigTests.TestDataDirectory, @"NonExistingService.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixUtilExtension");

            MSIExec.InstallProduct(msiFile, MSIExec.MSIExecReturnCode.ERROR_INSTALL_FAILURE);

            Assert.IsFalse(ServiceVerifier.ServiceExists("NonExistingService"), "Service '{0}' was created on Rollback.", "NonExistingService");
        }
    }
}
