﻿//-----------------------------------------------------------------------
// <copyright file="MSIExec.cs" company="Microsoft">
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
// <summary>A class that wraps MSIExec</summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Tools.WindowsInstallerXml.Test.Utilities;

    /// <summary>
    /// A class that wraps MSIExec.
    /// </summary>
    public partial class MSIExec : TestTool
    {
        /// <summary>
        /// The expected exit code of the tool
        /// </summary>
        public new MSIExecReturnCode ExpectedExitCode
        {
            get { return (MSIExecReturnCode)base.ExpectedExitCode; }
            set { base.ExpectedExitCode = (int?) value; }
        }

        /// <summary>
        /// Constructor that uses the default location for MSIExec.
        /// </summary>
        public MSIExec()
            : this(Environment.SystemDirectory)
        {
        }

        /// <summary>
        /// Constructor that accepts a path to the MSIExec location.
        /// </summary>
        /// <param name="toolDirectory">The directory of MSIExec.exe.</param>
        public MSIExec(string toolDirectory)
            : base(Path.Combine(toolDirectory, "MSIExec.exe"), null)
        {
            this.SetDefaultArguments();
            this.LogFile = FileUtilities.GetUniqueFileName();
        }


        /// <summary>
        /// Installs a .msi file
        /// </summary>
        /// <param name="sourceFile">Path the .msi file to install</param>
        /// <param name="expectedExitCode">Expected exit code</param>
        /// <returns>MSIExec log File</returns>
        public static string InstallProduct(string sourceFile, MSIExecReturnCode expectedExitCode)
        {
            if (String.IsNullOrEmpty(sourceFile))
            {
                throw new ArgumentException("sourceFile cannot be null or empty");
            }
            string logFile = string.Empty;
            MSIExecReturnCode exitCode = RunMSIExec(sourceFile, MSIExecMode.Install, expectedExitCode, out logFile);

            // Add the product to the list of installed products
            if ((MSIExecReturnCode.SUCCESS == exitCode || 
                MSIExecReturnCode.ERROR_SUCCESS_REBOOT_INITIATED == exitCode || 
                MSIExecReturnCode.ERROR_SUCCESS_REBOOT_REQUIRED == exitCode) &&
                (! MSIExec.InstalledMSI.Contains(sourceFile)))

            {
                MSIExec.InstalledMSI.Add(sourceFile);
            }

            return logFile;
        }

        /// <summary>
        /// Uninstalls a .msi file
        /// </summary>
        /// <param name="sourceFile">Path the .msi file to uninstall</param>
        /// <param name="expectedExitCode">Expected exit code</param>
        /// <returns>MSIExec log File</returns>
        public static string UninstallProduct(string sourceFile, MSIExecReturnCode expectedExitCode)
        {
            if (String.IsNullOrEmpty(sourceFile))
            {
                throw new ArgumentException("sourceFile cannot be null or empty");
            }
            
            string logFile = string.Empty;
            MSIExecReturnCode exitCode = RunMSIExec(sourceFile, MSIExecMode.Uninstall, expectedExitCode, out logFile);

            // Remove the product form the list of installed products
            if ((MSIExecReturnCode.SUCCESS == exitCode ||
               MSIExecReturnCode.ERROR_SUCCESS_REBOOT_INITIATED == exitCode ||
               MSIExecReturnCode.ERROR_SUCCESS_REBOOT_REQUIRED == exitCode) &&
               (MSIExec.InstalledMSI.Contains(sourceFile)))
            {
                MSIExec.InstalledMSI.Remove(sourceFile);
            }

            return logFile;
        }

        /// <summary>
        /// Repairs a .msi file
        /// </summary>
        /// <param name="sourceFile">Path the .msi file to repair</param>
        /// <param name="expectedExitCode">Expected exit code</param>
        /// <returns>MSIExec log File</returns>
        public static string RepairProduct(string sourceFile, MSIExecReturnCode expectedExitCode)
        {
            if (String.IsNullOrEmpty(sourceFile))
            {
                throw new ArgumentException("sourceFile cannot be null or empty");
            }

            string logFile = string.Empty;
            RunMSIExec(sourceFile, MSIExecMode.Repair, expectedExitCode, out logFile);

            return logFile;
        }

        /// <summary>
        /// Attempt to uninstall all the installed msi's
        /// </summary>
        /// <remarks>
        /// TODO: implement ignore_return_code option
        /// </remarks>
        public static void UninstallAllInstalledProducts()
        {
            foreach (string sourceFile in MSIExec.InstalledMSI)
            {
                // This is a best effort attempt to clean up the machine after a test run. 
                // The loop will attempt to uninstall all the msi files registered to be installed.
                try
                {
                    string logFile = string.Empty;
                    MSIExecReturnCode exitCode = MSIExec.RunMSIExec(sourceFile, MSIExecMode.Uninstall, MSIExecReturnCode.SUCCESS, out logFile);

                    if (MSIExecReturnCode.SUCCESS != exitCode)
                    {
                        Console.WriteLine(string.Format("Failed to uninstall msi '{0}'. Exit code: '{1}'", sourceFile, exitCode.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to uninstall msi '{0}'. Exception raised: '{1}'", sourceFile, ex.Message));
                }
            }

            MSIExec.InstalledMSI.Clear();
        }

        /// <summary>
        /// List of the msi files installed using this wrapper
        /// </summary>
        private static List<string> InstalledMSI = new List<string>();

        /// <summary>
        /// Executes MSIExec on a .msi file
        /// </summary>
        /// <param name="sourceFile">Path the .msi file to use</param>
        /// <param name="mode">Mode of execution for MSIExec</param>
        /// <param name="expectedExitCode">Expected exit code</param>
        /// <returns>MSIExec exit code</returns>
        private static MSIExecReturnCode RunMSIExec(string sourceFile, MSIExecMode mode, MSIExecReturnCode expectedExitCode, out string logFile)
        {
            MSIExec msiexec = new MSIExec();
            msiexec.Product = sourceFile;
            msiexec.ExecutionMode = mode;
            msiexec.ExpectedExitCode = expectedExitCode;

            Result result = msiexec.Run();
            logFile = msiexec.LogFile;

            return (MSIExecReturnCode)result.ExitCode;
        }

    }
}