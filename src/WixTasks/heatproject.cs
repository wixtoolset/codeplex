//-------------------------------------------------------------------------------------------------
// <copyright file="HeatProject.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
//    
//    The use and distribution terms for this software are covered by the
//    Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
//    which can be found in the file CPL.TXT at the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by
//    the terms of this license.
//    
//    You must not remove this notice, or any other, from this software.
// </copyright>
// 
// <summary>
// Build task to execute the VS Project harvester extension of the Windows Installer Xml toolset.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Build.Tasks
{
    using System;
    using Microsoft.Build.Framework;

    public sealed class HeatProject : HeatTask
    {
        private string configuration;
        private string directoryIds;
        private string generateType;
        private string platform;
        private string project;
        private string projectName;
        private string[] projectOutputGroups;

        public string Configuration
        {
            get { return this.configuration; }
            set { this.configuration = value; }
        }

        public string DirectoryIds
        {
            get { return this.directoryIds; }
            set { this.directoryIds = value; }
        }

        public string GenerateType
        {
            get { return this.generateType; }
            set { this.generateType = value; }
        }

        public string Platform
        {
            get { return this.platform; }
            set { this.platform = value; }
        }

        [Required]
        public string Project
        {
            get { return this.project; }
            set { this.project = value; }
        }

        public string ProjectName
        {
            get { return this.projectName; }
            set { this.projectName = value; }
        }

        public string[] ProjectOutputGroups
        {
            get
            {
                return this.projectOutputGroups;
            }
            set
            {
                this.projectOutputGroups = value;

                // If it's just one string and it contains semicolons, let's
                // split it into separate items.
                if (this.projectOutputGroups.Length == 1)
                {
                    this.projectOutputGroups = this.projectOutputGroups[0].Split(new char[] { ';' });
                }
            }
        }

        protected override string OperationName
        {
            get { return "project"; }
        }

        /// <summary>
        /// Generate the command line arguments to write to the response file from the properties.
        /// </summary>
        /// <returns>Command line string.</returns>
        protected override string GenerateResponseFileCommands()
        {
            WixCommandLineBuilder commandLineBuilder = new WixCommandLineBuilder();

            commandLineBuilder.AppendSwitch(this.OperationName);

            // Ensure there are quotes around the project path, if there aren't already
            if (!this.Project.StartsWith("\"", StringComparison.Ordinal))
            {
                this.Project = "\"" + this.Project + "\"";
            }
            commandLineBuilder.AppendSwitch(this.Project);

            commandLineBuilder.AppendSwitchIfNotNull("-configuration ", this.Configuration);
            commandLineBuilder.AppendSwitchIfNotNull("-directoryid ", this.DirectoryIds);
            commandLineBuilder.AppendSwitchIfNotNull("-generate ", this.GenerateType);
            commandLineBuilder.AppendSwitchIfNotNull("-platform ", this.Platform);
            commandLineBuilder.AppendArrayIfNotNull("-pog ", this.ProjectOutputGroups);
            commandLineBuilder.AppendSwitchIfNotNull("-projectname ", this.ProjectName);

            base.BuildCommandLine(commandLineBuilder);
            return commandLineBuilder.ToString();
        }
    }
}
