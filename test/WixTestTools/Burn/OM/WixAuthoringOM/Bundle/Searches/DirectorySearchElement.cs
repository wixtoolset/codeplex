﻿//-----------------------------------------------------------------------
// <copyright file="VariableElement.cs" company="Microsoft">
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
// <summary>DirectorySearch element OM</summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Burn.OM.WixAuthoringOM.Bundle.Searches
{
    using Microsoft.Tools.WindowsInstallerXml.Test.Burn.OM.ElementAttribute;

    [BurnXmlElement("DirectorySearch", BundleElement.wixUtilExtNamespace)]
    public class DirectorySearchElement : Searches
    {
        public enum DirectorySearchResultType
        {
            Exists
        }

        # region Private member

        private string m_Path;
        private DirectorySearchResultType m_ResultType;

        # endregion

        # region Public property

        [BurnXmlAttribute("Path")]
        public string Path
        {
            get
            {
                return m_Path;
            }
            set
            {
                m_Path = value;
            }
        }

        [BurnXmlAttribute("Result")]
        public DirectorySearchResultType Result
        {
            get
            {
                return m_ResultType;
            }
            set
            {
                m_ResultType = value;
            }
        }

        # endregion

    }
}
