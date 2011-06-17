//-----------------------------------------------------------------------
// <copyright file="UtilExtension.PermissionExTests.cs" company="Microsoft">
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
// <summary>Util Extension PermissionEx tests</summary>
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

    using System.Security.AccessControl;
   
    /// <summary>
    /// Util extension PermissionEx element tests
    /// </summary>
    [TestClass]
    public class PermissionExTests : WixTests
    {
        private static readonly string TestDataDirectory = Environment.ExpandEnvironmentVariables(@"%WIX_ROOT%\test\data\Extensions\UtilExtension\PermissionExTests");

        [TestMethod]
        [Description("Verify that the (SecureObjects and CustomAction) Tables are created in the MSI and have expected data.")]
        [Priority(1)]
        public void PermissionEx_VerifyMSITableData()
        {
            string sourceFile = Path.Combine(PermissionExTests.TestDataDirectory, @"product.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixUtilExtension");

            Verifier.VerifyCustomActionTableData(msiFile,
                new CustomActionTableData("SchedSecureObjects", 1, "WixCA", "SchedSecureObjects"),
                new CustomActionTableData("ExecSecureObjects", 3073, "WixCA", "ExecSecureObjects"),
                new CustomActionTableData("ExecSecureObjectsRollback", 3329, "WixCA", "ExecSecureObjectsRollback"));

            // Verify SecureObjects table contains the right data
            Verifier.VerifyTableData(msiFile, MSITables.SecureObjects,
                new TableRow(SecureObjectsColumns.SecureObject.ToString(), "TestCreateFolderDirectory"),
                new TableRow(SecureObjectsColumns.Table.ToString(), "CreateFolder"),
                new TableRow(SecureObjectsColumns.Domain.ToString(), string.Empty),
                new TableRow(SecureObjectsColumns.User.ToString(), "Guests"),
                new TableRow(SecureObjectsColumns.Permission.ToString(), "128", false));

            Verifier.VerifyTableData(msiFile, MSITables.SecureObjects,
               new TableRow(SecureObjectsColumns.SecureObject.ToString(), "TestCreateFolderDirectory"),
               new TableRow(SecureObjectsColumns.Table.ToString(), "CreateFolder"),
               new TableRow(SecureObjectsColumns.Domain.ToString(), string.Empty),
               new TableRow(SecureObjectsColumns.User.ToString(), "Everyone"),
               new TableRow(SecureObjectsColumns.Permission.ToString(), "1311104", false));

            Verifier.VerifyTableData(msiFile, MSITables.SecureObjects,
               new TableRow(SecureObjectsColumns.SecureObject.ToString(), "MynewService.exe"),
               new TableRow(SecureObjectsColumns.Table.ToString(), "File"),
               new TableRow(SecureObjectsColumns.Domain.ToString(), string.Empty),
               new TableRow(SecureObjectsColumns.User.ToString(), "Guests"),
               new TableRow(SecureObjectsColumns.Permission.ToString(), "268435456", false));

            Verifier.VerifyTableData(msiFile, MSITables.SecureObjects,
               new TableRow(SecureObjectsColumns.SecureObject.ToString(), "MynewService.exe"),
               new TableRow(SecureObjectsColumns.Table.ToString(), "File"),
               new TableRow(SecureObjectsColumns.Domain.ToString(), string.Empty),
               new TableRow(SecureObjectsColumns.User.ToString(), "Everyone"),
               new TableRow(SecureObjectsColumns.Permission.ToString(), "268435456", false));

            Verifier.VerifyTableData(msiFile, MSITables.SecureObjects,
              new TableRow(SecureObjectsColumns.SecureObject.ToString(), "MynewService.exe"),
              new TableRow(SecureObjectsColumns.Table.ToString(), "File"),
              new TableRow(SecureObjectsColumns.Domain.ToString(), string.Empty),
              new TableRow(SecureObjectsColumns.User.ToString(), "LocalService"),
              new TableRow(SecureObjectsColumns.Permission.ToString(), "268435456", false));

            Verifier.VerifyTableData(msiFile, MSITables.SecureObjects,
               new TableRow(SecureObjectsColumns.SecureObject.ToString(), "MynewServiceCore"),
               new TableRow(SecureObjectsColumns.Table.ToString(), "Registry"),
               new TableRow(SecureObjectsColumns.Domain.ToString(), string.Empty),
               new TableRow(SecureObjectsColumns.User.ToString(), "Guests"),
               new TableRow(SecureObjectsColumns.Permission.ToString(), "63", false));
        }

        [TestMethod]
        [Description("Verify that the right permessions are created on install.")]
        [Priority(2)]
        [TestProperty("IsRuntimeTest", "true")]
        public void PermissionEx_Install()
        {
            string sourceFile = Path.Combine(PermissionExTests.TestDataDirectory, @"product.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixUtilExtension");

            MSIExec.InstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            // Verify File Permessions
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"WixTestFolder\MynewService.exe");
            PermissionsVerifier.VerifyFilePermession(@"BUILTIN\Guests", fileName, FileSystemRights.FullControl);
            PermissionsVerifier.VerifyFilePermession(@"Everyone", fileName, FileSystemRights.FullControl);
            PermissionsVerifier.VerifyFilePermession(@"NT AUTHORITY\LOCAL SERVICE", fileName, FileSystemRights.FullControl);

            // Verify Folder Permessions
            string folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"WixTestFolder\Create Folder");
            PermissionsVerifier.VerifyFolderPermession(@"Everyone", folderName, FileSystemRights.ReadAttributes | FileSystemRights.WriteAttributes | FileSystemRights.ChangePermissions | FileSystemRights.Synchronize);
            PermissionsVerifier.VerifyFolderPermession(@"BUILTIN\Guests", folderName, FileSystemRights.ReadAttributes);

            // Verify Registry Permessions
            PermissionsVerifier.VerifyRegistryKeyPermission(@"BUILTIN\Guests", Microsoft.Win32.Registry.LocalMachine, @"SOFTWARE\Microsoft\Office\Delivery\MynewService", RegistryRights.QueryValues | RegistryRights.SetValue | RegistryRights.CreateSubKey | RegistryRights.EnumerateSubKeys | RegistryRights.Notify | RegistryRights.CreateLink);

            // Verify Service Permessions
            // TODO: Check for Service permissions

            MSIExec.UninstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);
        }

        [TestMethod]
        [Description("Verify that the right permessions are created on install to a 64-bit specific folder.")]
        [Priority(2)]
        [TestProperty("IsRuntimeTest", "true")]
        [TestProperty("Is64BitSpecificTest", "true")]
        public void PermissionEx_Install_64bit()
        {
            string sourceFile = Path.Combine(PermissionExTests.TestDataDirectory, @"product_64.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixUtilExtension");

            MSIExec.InstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            // Verify File Permessions
            string fileName = Path.Combine(Environment.ExpandEnvironmentVariables(@"%ProgramW6432%"), @"WixTestFolder\MynewService.exe");
            PermissionsVerifier.VerifyFilePermession(@"BUILTIN\Guests", fileName, FileSystemRights.FullControl);
            PermissionsVerifier.VerifyFilePermession(@"Everyone", fileName, FileSystemRights.FullControl);
            PermissionsVerifier.VerifyFilePermession(@"NT AUTHORITY\LOCAL SERVICE", fileName, FileSystemRights.FullControl);

            // Verify Folder Permessions
            string folderName = Path.Combine(Environment.ExpandEnvironmentVariables(@"%ProgramW6432%"), @"WixTestFolder\Create Folder");
            PermissionsVerifier.VerifyFolderPermession(@"Everyone", folderName, FileSystemRights.ReadAttributes | FileSystemRights.WriteAttributes | FileSystemRights.ChangePermissions | FileSystemRights.Synchronize);
            PermissionsVerifier.VerifyFolderPermession(@"BUILTIN\Guests", folderName, FileSystemRights.ReadAttributes);

            // Verify Registry Permessions
            // PermissionsVerifier.VerifyRegistryKeyPermission(@"BUILTIN\Guests", Microsoft.Win32.RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Office\Delivery\MynewService", RegistryRights.QueryValues | RegistryRights.SetValue | RegistryRights.CreateSubKey | RegistryRights.EnumerateSubKeys | RegistryRights.Notify | RegistryRights.CreateLink);

            // Verify Service Permessions
            // TODO: Check for Service permissions

            MSIExec.UninstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);
        }

        [TestMethod]
        [Description("Verify that the right permessions are created on Repair.")]
        [Priority(2)]
        [TestProperty("IsRuntimeTest", "true")]
        public void PermissionEx_Repair()
        {
            string sourceFile = Path.Combine(PermissionExTests.TestDataDirectory, @"product.wxs");
            string msiFile = Builder.BuildPackage(sourceFile, "test.msi", "WixUtilExtension");

            MSIExec.InstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            // Change File Permessions
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"WixTestFolder\MynewService.exe");
            PermissionsVerifier.RemoveFilePermession(@"BUILTIN\Guests", fileName);
            PermissionsVerifier.RemoveFilePermession(@"Everyone", fileName);
            PermissionsVerifier.RemoveFilePermession(@"NT AUTHORITY\LOCAL SERVICE", fileName);

            // Change Folder Permessions
            string folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"WixTestFolder\Create Folder");
            PermissionsVerifier.RemoveFolderPermession(@"Everyone", folderName);
            PermissionsVerifier.RemoveFolderPermession(@"BUILTIN\Guests", folderName);

            // Change Registry Permessions
            PermissionsVerifier.RemoveRegistryKeyPermission(@"BUILTIN\Guests", Microsoft.Win32.Registry.LocalMachine, @"SOFTWARE\Microsoft\Office\Delivery\MynewService");

            // Change Service Permessions
            // TODO: Change Service permissions


            MSIExec.RepairProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);

            // Verify File Permessions
            PermissionsVerifier.VerifyFilePermession(@"BUILTIN\Guests", fileName, FileSystemRights.FullControl);
            PermissionsVerifier.VerifyFilePermession(@"Everyone", fileName, FileSystemRights.FullControl);
            PermissionsVerifier.VerifyFilePermession(@"NT AUTHORITY\LOCAL SERVICE", fileName, FileSystemRights.FullControl);

            // Verify Folder Permessions
            PermissionsVerifier.VerifyFolderPermession(@"Everyone", folderName, FileSystemRights.ReadAttributes | FileSystemRights.WriteAttributes | FileSystemRights.ChangePermissions | FileSystemRights.Synchronize);
            PermissionsVerifier.VerifyFolderPermession(@"BUILTIN\Guests", folderName, FileSystemRights.ReadAttributes);

            // Verify Registry Permessions
            PermissionsVerifier.VerifyRegistryKeyPermission(@"BUILTIN\Guests", Microsoft.Win32.Registry.LocalMachine, @"SOFTWARE\Microsoft\Office\Delivery\MynewService", RegistryRights.QueryValues | RegistryRights.SetValue | RegistryRights.CreateSubKey | RegistryRights.EnumerateSubKeys | RegistryRights.Notify | RegistryRights.CreateLink);

            // Verify Service Permessions
            // TODO: Check for Service permissions

            MSIExec.UninstallProduct(msiFile, MSIExec.MSIExecReturnCode.SUCCESS);
        }

        [TestMethod]
        [Description("Verify that files are not uninstalled during rollback.")]
        [Priority(2)]
        [TestProperty("IsRuntimeTest", "true")]
        public void PermissionEx_ExistingFile()
        {
            // install the file
            string sourceFile1 = Path.Combine(PermissionExTests.TestDataDirectory, @"existingfile.wxs");
            string msiFile1 = Builder.BuildPackage(sourceFile1, "test1.msi", "WixUtilExtension");
            MSIExec.InstallProduct(msiFile1, MSIExec.MSIExecReturnCode.SUCCESS);

            // install the permessionex test
            string sourceFile2 = Path.Combine(PermissionExTests.TestDataDirectory, @"product_fail.wxs");
            string msiFile2 = Builder.BuildPackage(sourceFile2, "test2.msi", "WixUtilExtension");

            MSIExec.InstallProduct(msiFile2, MSIExec.MSIExecReturnCode.ERROR_INSTALL_FAILURE);

            // Verify File Permessions
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"WixTestFolder\MynewService.exe");
            Assert.IsTrue(File.Exists(fileName), "File '{0}' was removed during Rollback.", fileName);

            MSIExec.UninstallProduct(msiFile1, MSIExec.MSIExecReturnCode.SUCCESS);
        }

        [TestMethod]
        [Description("Verify that the correct error message is displayed when the GroupRef element does not have a parent Component element.")]
        [Priority(3)]
        public void PermissionEx_GroupRefMissingParentComponent()
        {
            Candle candle = new Candle();
            candle.SourceFiles.Add(Path.Combine(PermissionExTests.TestDataDirectory, @"GroupRefMissingParentComponent.wxs"));
            candle.Extensions.Add("WixUtilExtension");
            candle.ExpectedWixMessages.Add(new WixMessage(5051, @"The util:GroupRef element cannot be specified unless the element has a Component as an ancestor. A util:GroupRef that does not have a Component ancestor is not installed.", WixMessage.MessageTypeEnum.Error));
            candle.ExpectedExitCode = 5051;
            candle.Run();
        }

        [TestMethod]
        [Description("Verify that the correct error message is displayed when the User element does not have a parent Component element.")]
        [Priority(3)]
        public void PermissionEx_UserfMissingParentComponent()
        {
            Candle candle = new Candle();
            candle.SourceFiles.Add(Path.Combine(PermissionExTests.TestDataDirectory, @"UserMissingParentComponent.wxs"));
            candle.Extensions.Add("WixUtilExtension");
            candle.ExpectedWixMessages.Add(new WixMessage(5050, @"The util:User/@CanNotChangePassword attribute cannot be specified unless the element has a Component as an ancestor. A util:User that does not have a Component ancestor is not installed.", WixMessage.MessageTypeEnum.Error));
            candle.ExpectedExitCode = 5050;
            candle.Run();
        }
    }
}