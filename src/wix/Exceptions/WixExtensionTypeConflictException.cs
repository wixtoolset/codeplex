//-------------------------------------------------------------------------------------------------
// <copyright file="WixExtensionTypeConflictException.cs" company="Microsoft">
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
// WixException thrown when an preprocessor extension is invalid.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml
{
    using System;

    /// <summary>
    /// WixException thrown when an preprocessor extension is invalid.
    /// </summary>
    public class WixExtensionTypeConflictException : WixException
    {
        private const WixExceptionType ExceptionType = WixExceptionType.ExtensionTypeConflict;
        private PreprocessorExtension extension;
        private PreprocessorExtension existingExtension;

        /// <summary>
        /// Instantiate a new WixExtensionTypeConflictException.
        /// </summary>
        /// <param name="extension">Extension with the conflict.</param>
        /// <param name="existingExtension">Existing extension with the same type</param>
        public WixExtensionTypeConflictException(PreprocessorExtension extension, PreprocessorExtension existingExtension) :
            base(null, ExceptionType)
        {
            this.extension = extension;
            this.existingExtension = existingExtension;
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <value>The error message that explains the reason for the exception, or an empty string("").</value>
        public override string Message
        {
            get
            {
                return String.Format("Extension '{0}' uses the same type '{1}' as already loaded extension '{2}'.  Either remove one of the extensions or rename the type to avoid the conflict.", this.extension.ToString(), this.extension.Type, this.existingExtension.ToString());
            }
        }
    }
}
