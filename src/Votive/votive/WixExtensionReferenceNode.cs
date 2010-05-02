//-------------------------------------------------------------------------------------------------
// <copyright file="WixExtensionReferenceNode.cs" company="Microsoft">
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
// Contains the WixExtensionReferenceNode class.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.VisualStudio
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using Microsoft.Tools.WindowsInstallerXml;
    using Microsoft.Tools.WindowsInstallerXml.Build.Tasks;
    using Microsoft.VisualStudio.Package;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// Represents a Wix extension reference node.
    /// </summary>
    public class WixExtensionReferenceNode : WixReferenceNode
    {
        // =========================================================================================
        // Member Variables
        // =========================================================================================

        private const string ExtensionDirectoryToken = "$(" + WixProjectFileConstants.WixExtDir + ")";

        // Extension directory is the default location where Wix extension dlls are located.
        private static string extensionDirectory;

        private Version version;

        // =========================================================================================
        // Constructors
        // =========================================================================================

        /// <summary>
        /// Initializes a new instance of the <see cref="WixExtensionReferenceNode"/> class.
        /// </summary>
        /// <param name="root">The root <see cref="WixProjectNode"/> that contains this node.</param>
        /// <param name="element">The element that contains MSBuild properties.</param>
        public WixExtensionReferenceNode(WixProjectNode root, ProjectElement element)
            : base(root, element)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WixExtensionReferenceNode"/> class.
        /// </summary>
        /// <param name="root">The root <see cref="WixProjectNode"/> that contains this node.</param>
        /// <param name="referencePath">The path to the wixlib reference file.</param>
        public WixExtensionReferenceNode(WixProjectNode root, string referencePath)
            : base(root, referencePath, WixProjectFileConstants.WixExtension)
        {
            referencePath = WixHelperMethods.ReplacePathWithBuildProperty(referencePath, ExtensionDirectoryToken, this.ExtensionDirectory);

            if (!referencePath.StartsWith(ExtensionDirectoryToken, StringComparison.Ordinal) && null != root)
            {
                referencePath = root.GetRelativePath(referencePath);
            }

            this.ItemNode.SetMetadata(ProjectFileConstants.HintPath, referencePath);
        }

        // =========================================================================================
        // Properties
        // =========================================================================================

        /// <summary>
        /// Gets the version of the WiX extension file.
        /// </summary>
        /// <value>The version of the WiX extension file.</value>
        public Version Version
        {
            get
            {
                if (this.version == null)
                {
                    this.ExtractPropertiesFromFile();
                }

                return this.version;
            }
        }

        /// <summary>
        /// Lazy loads evaluated Wix Extension Directory value. Calling this property in the constructor 
        /// will result in returning empty value. 
        /// </summary>
        private string ExtensionDirectory
        {
            get
            {
                if (extensionDirectory == null)
                {
                    string configName = null;
                    string platformName = null;
                    try
                    {
                        EnvDTE.Project automationObject = this.ProjectMgr.GetAutomationObject() as EnvDTE.Project;
                        configName = Utilities.GetActiveConfigurationName(automationObject);
                        platformName = Utilities.GetActivePlatformName(automationObject);
                    }
                    catch (COMException)
                    {
                        // If there's no active configuration, just return an empty string for now but try again later.
                        return String.Empty;
                    }

                    this.ProjectMgr.Build(configName, platformName, WixProjectFileConstants.MsBuildTarget.GetTargetPath);

                    extensionDirectory = (string)this.ProjectMgr.GetMsBuildProperty(WixProjectFileConstants.WixExtDir, true);
                    if (extensionDirectory == null)
                    {
                        extensionDirectory = String.Empty;
                    }
                }

                return extensionDirectory;
            }
        }

        // =========================================================================================
        // Methods
        // =========================================================================================

        /// <summary>
        /// Validates that a reference can be added.
        /// </summary>
        /// <param name="errorHandler">A CannotAddReferenceErrorMessage delegate to show the error message.</param>
        /// <returns>true if the reference can be added.</returns>
        protected override bool CanAddReference(out CannotAddReferenceErrorMessage errorHandler)
        {
            if (!base.CanAddReference(out errorHandler))
            {
                return false;
            }

            errorHandler = null;
            if (!WixReferenceValidator.IsValidWixExtension(this.Url, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), this.ExtensionDirectory))
            {
                errorHandler = new CannotAddReferenceErrorMessage(this.ShowInvalidWixReferenceMessage);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a reference is already added. The method parses all references and compares the filename.
        /// </summary>
        /// <returns>true if the extension reference has already been added.</returns>
        protected override bool IsAlreadyAdded()
        {
            ReferenceContainerNode referencesFolder = this.ProjectMgr.FindChild(ReferenceContainerNode.ReferencesNodeVirtualName) as ReferenceContainerNode;
            Debug.Assert(referencesFolder != null, "Could not find the References node");

            string thisName = Path.GetFileNameWithoutExtension(this.ItemNode.Item.Include);
            for (HierarchyNode n = referencesFolder.FirstChild; n != null; n = n.NextSibling)
            {
                WixExtensionReferenceNode otherReference = n as WixExtensionReferenceNode;
                if (otherReference != null)
                {
                    string otherName = Path.GetFileNameWithoutExtension(otherReference.Url);
                    if (String.Equals(thisName, otherName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Creates an object derived from <see cref="NodeProperties"/> that will be used to expose
        /// properties specific for this object to the property browser.
        /// </summary>
        /// <returns>A new <see cref="WixExtensionReferenceNodeProperties"/> object.</returns>
        protected override NodeProperties CreatePropertiesObject()
        {
            return new WixExtensionReferenceNodeProperties(this);
        }

        /// <summary>
        /// Replaces build properties in the path.
        /// </summary>
        /// <param name="path">Input path with build propeties.</param>
        /// <returns>Path with build properties evaluated and substituted.</returns>
        protected override string ReplacePropertiesInPath(string path)
        {
            path = WixHelperMethods.ReplaceBuildPropertyWithPath(path, ExtensionDirectoryToken, this.ExtensionDirectory);

            return base.ReplacePropertiesInPath(path);
        }

        /// <summary>
        /// Opens the wixlib file and read properties from the file.
        /// </summary>
        private void ExtractPropertiesFromFile()
        {
            if (!String.IsNullOrEmpty(this.Url) && File.Exists(this.Url))
            {
                try
                {
                    byte[] rawAssembly = File.ReadAllBytes(this.Url);
                    Assembly extensionAssembly = Assembly.ReflectionOnlyLoad(rawAssembly);
                    this.version = extensionAssembly.GetName().Version;
                    return;
                }
                catch (UnauthorizedAccessException e)
                {
                    CCITracing.Trace(e);
                }
                catch (FileLoadException e)
                {
                    CCITracing.Trace(e);
                }
                catch (BadImageFormatException e)
                {
                    CCITracing.Trace(e);
                }
                catch (IOException e)
                {
                    CCITracing.Trace(e);
                }
                catch (SecurityException e)
                {
                    CCITracing.Trace(e);
                }
            }

            this.version = new Version();
        }
    }
}