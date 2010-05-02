//-------------------------------------------------------------------------------------------------
// <copyright file="WixInvalidXmlException.cs" company="Microsoft">
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
// WiX xml parsing exception.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml
{
    using System;
    using System.Xml;

    /// <summary>
    /// WiX schema validation exception.
    /// </summary>
    public class WixInvalidXmlException : WixException
    {
        /// <summary>
        /// Instantiate a new WixSchemaValidationException.
        /// </summary>
        /// <param name="sourceLineNumbers">Source line information of the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public WixInvalidXmlException(SourceLineNumberCollection sourceLineNumbers, XmlException innerException) :
            base(sourceLineNumbers, WixExceptionType.InvalidXml, innerException)
        {
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <value>The error message that explains the reason for the exception, or an empty string("").</value>
        public override string Message
        {
            get
            {
                string message = this.InnerException.Message;

                // find the index of the erroneous line information and chop it off
                int length = message.IndexOf(" Line ");
                if (-1 == length)
                {
                    // couldn't find the erroreous info, so just show the whole message
                    length = message.Length;
                }

                return message.Substring(0, length);
            }
        }
    }
}
