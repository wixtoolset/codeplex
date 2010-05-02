//-------------------------------------------------------------------------------------------------
// <copyright file="CabinetWorkItem.cs" company="Microsoft">
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
// 
// <summary>
// A cabinet builder work item.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml
{
    using System;

    /// <summary>
    /// A cabinet builder work item.
    /// </summary>
    internal sealed class CabinetWorkItem
    {
        private string cabinetFile;
        private Cab.CompressionLevel compressionLevel;
        private FileRowCollection fileRows;
        private BinderFileManager binderFileManager;
        private int maxThreshold;

        /// <summary>
        /// Instantiate a new CabinetWorkItem.
        /// </summary>
        /// <param name="fileRows">The collection of files in this cabinet.</param>
        /// <param name="cabinetFile">The cabinet file.</param>
        /// <param name="maxThreshold">Maximum threshold for each cabinet.</param>
        /// <param name="compressionLevel">The compression level of the cabinet.</param>
        /// <param name="binderFileManager">The binder file manager.</param>
        public CabinetWorkItem(FileRowCollection fileRows, string cabinetFile, int maxThreshold, Cab.CompressionLevel compressionLevel, BinderFileManager binderFileManager)
        {
            this.cabinetFile = cabinetFile;
            this.compressionLevel = compressionLevel;
            this.fileRows = fileRows;
            this.binderFileManager = binderFileManager;
            this.maxThreshold = maxThreshold;
        }

        /// <summary>
        /// Gets the cabinet file.
        /// </summary>
        /// <value>The cabinet file.</value>
        public string CabinetFile
        {
            get { return this.cabinetFile; }
        }

        /// <summary>
        /// Gets the compression level of the cabinet.
        /// </summary>
        /// <value>The compression level of the cabinet.</value>
        public Cab.CompressionLevel CompressionLevel
        {
            get { return this.compressionLevel; }
        }

        /// <summary>
        /// Gets the collection of files in this cabinet.
        /// </summary>
        /// <value>The collection of files in this cabinet.</value>
        public FileRowCollection FileRows
        {
            get { return this.fileRows; }
        }

        /// <summary>
        /// Gets the binder file manager.
        /// </summary>
        /// <value>The binder file manager.</value>
        public BinderFileManager BinderFileManager
        {
            get { return this.binderFileManager; }
        }

        /// <summary>
        /// Gets the max threshold.
        /// </summary>
        /// <value>The maximum threshold for a folder in a cabinet.</value>
        public int MaxThreshold
        {
            get { return this.maxThreshold; }
        }
    }
}
