//-------------------------------------------------------------------------------------------------
// <copyright file="WixNotLibraryException.cs" company="Microsoft">
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
// Exception thrown when trying to create an library from a file that is not an library file.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml
{
    using System;

    /// <summary>
    /// Exception thrown when trying to create an library from a file that is not an library file.
    /// </summary>
    [Serializable]
    public sealed class WixNotLibraryException : WixException
    {
        /// <summary>
        /// Instantiate a new WixNotLibraryException.
        /// </summary>
        /// <param name="error">Localized error information.</param>
        public WixNotLibraryException(WixErrorEventArgs error)
            : base(error)
        {
        }
    }
}