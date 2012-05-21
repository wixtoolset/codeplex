﻿//-----------------------------------------------------------------------
// <copyright file="WixUnitArguments.cs" company="Microsoft">
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
//     Fields, properties and methods for working with WixUnit arguments
// </summary>
//-----------------------------------------------------------------------

namespace WixTest
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Fields, properties and methods for working with WixUnit arguments.
    /// </summary>
    public partial class WixUnit
    {
        #region Private Members

        /// <summary>
        /// Environment variables
        /// </summary>
        private Dictionary<string, string> wixUnitEnvironmentVariables;

        /// <summary>
        /// NoTidy
        /// </summary>
        private bool noTidy;

        /// <summary>
        /// Re-run failed tests
        /// </summary>
        private bool runFailedTests;

        /// <summary>
        /// Run tests on a single thread
        /// </summary>
        private bool singleThreaded;

        /// <summary>
        /// The file that contains the tests
        /// </summary>
        private string testFile;

        /// <summary>
        /// List of tests to run
        /// </summary>
        private List<string> tests;

        /// <summary>
        /// Update out-of-date tests
        /// </summary>
        private bool update;

        /// <summary>
        /// Perform MSI/MSM validation
        /// </summary>
        private bool validate;

        /// <summary>
        /// Verbose output
        /// </summary>
        private bool verboseOutput;

        #endregion

        #region Public Properties

        /// <summary>
        /// The arguments as they would be passed on the command line.
        /// </summary>
        /// <remarks>
        /// To allow for negative testing, checking for invalid combinations
        /// of arguments is not performed.
        /// </remarks>
        public override string Arguments
        {
            get
            {
                StringBuilder arguments = new StringBuilder(base.Arguments);

                // Test file
                if (!String.IsNullOrEmpty(this.TestFile))
                {
                    arguments.AppendFormat(" {0}", this.TestFile);
                }

                // Environment variables
                foreach (string environmentVariable in this.WixUnitEnvironmentVariables.Keys)
                {
                    arguments.AppendFormat(@" -env:{0}={1}", environmentVariable, this.WixUnitEnvironmentVariables[environmentVariable]);
                }

                // NoTidy
                if (this.NoTidy)
                {
                    arguments.Append(" -notidy");
                }

                // Re-run failed tests
                if (this.RunFailedTests)
                {
                    arguments.Append(" -rf");
                }

                // Run tests on a single thread
                if (this.SingleThreaded)
                {
                    arguments.Append(" -st");
                }

                // Tests to run
                foreach (string test in this.Tests)
                {
                    arguments.AppendFormat(" -test:{0}", test);
                }

                // Update
                if (this.Update)
                {
                    arguments.Append(" -update");
                }

                // Validation
                if (this.Validate)
                {
                    arguments.Append(" -val");
                }

                // VerboseOutput
                if (this.VerboseOutput)
                {
                    arguments.Append(" -v");
                }

                return arguments.ToString();
            }
        }

        /// <summary>
        /// Environment variables
        /// </summary>
        public Dictionary<string, string> WixUnitEnvironmentVariables
        {
            get { return this.wixUnitEnvironmentVariables; }
            set { this.wixUnitEnvironmentVariables = value; }
        }

        /// <summary>
        /// NoLogo
        /// </summary>
        public override bool NoLogo
        {
            get { return false; }
        }

        /// <summary>
        /// NoTidy
        /// </summary>
        public bool NoTidy
        {
            get { return this.noTidy; }
            set { this.noTidy = value; }
        }
        
        /// <summary>
        /// Re-run failed tests
        /// </summary>
        public bool RunFailedTests
        {
            get { return this.runFailedTests; }
            set { this.runFailedTests = value; }
        }

        /// <summary>
        /// Run tests on a single thread
        /// </summary>
        public bool SingleThreaded
        {
            get { return this.singleThreaded; }
            set { this.singleThreaded = value; }
        }

        /// <summary>
        /// The file that contains the tests
        /// </summary>
        public string TestFile
        {
            get { return this.testFile; }
            set { this.testFile = value; }
        }

        /// <summary>
        /// List of tests to run
        /// </summary>
        public List<string> Tests
        {
            get { return this.tests; }
            set { this.tests = value; }
        }

        /// <summary>
        /// Update out-of-date tests
        /// </summary>
        public bool Update
        {
            get { return this.update; }
            set { this.update = value; }
        }

        /// <summary>
        /// Perform MSI/MSM validation
        /// </summary>
        public bool Validate
        {
            get { return this.validate; }
            set { this.validate = value; }
        }

        /// <summary>
        /// Verbose output
        /// </summary>
        public bool VerboseOutput
        {
            get { return this.verboseOutput; }
            set { this.verboseOutput = value; }
        }

        #endregion

        /// <summary>
        /// Clears all of the assigned arguments and resets them to the default values.
        /// </summary>
        public override void SetDefaultArguments()
        {
            this.WixUnitEnvironmentVariables = new Dictionary<string, string>();
            this.NoTidy = false;
            this.RunFailedTests = false;
            this.SingleThreaded = false;
            this.TestFile = String.Empty;
            this.Tests = new List<string>();
            this.Update = false;
            this.Validate = false;
            this.VerboseOutput = false;
        }
    }
}
