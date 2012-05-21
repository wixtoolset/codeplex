//-------------------------------------------------------------------------------------------------
// <copyright file="WixComponentSearchInfo.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
//    
//    The use and distribution terms for this software are covered by the
//    Common Public License 1.0 (http://opensource.org/licenses/cpl1.0.php)
//    which can be found in the file CPL.TXT at the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by
//    the terms of this license.
//    
//    You must not remove this notice, or any other, from this software.
// </copyright>
// 
// <summary>
// Utility class for all WixComponentSearches.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml
{
    using System;
    using System.Xml;

    /// <summary>
    /// Utility class for all WixComponentSearches.
    /// </summary>
    internal class WixComponentSearchInfo : WixSearchInfo
    {
        public WixComponentSearchInfo(Row row)
            : this((string)row[0], (string)row[1], (string)row[2], (int)row[3])
        {
        }

        public WixComponentSearchInfo(string id, string guid, string productCode, int attributes)
            : base(id)
        {
            this.Guid = guid;
            this.ProductCode = productCode;
            this.Attributes = (WixComponentSearchAttributes)attributes;
        }

        public string Guid { get; private set; }
        public string ProductCode { get; private set; }
        public WixComponentSearchAttributes Attributes { get; private set; }

        /// <summary>
        /// Generates Burn manifest and ParameterInfo-style markup for a component search.
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("MsiComponentSearch");
            this.WriteWixSearchAttributes(writer);

            writer.WriteAttributeString("ComponentId", this.Guid);

            if (!String.IsNullOrEmpty(this.ProductCode))
            {
                writer.WriteAttributeString("ProductCode", this.ProductCode);
            }

            if (0 != (this.Attributes & WixComponentSearchAttributes.KeyPath))
            {
                writer.WriteAttributeString("Type", "keyPath");
            }
            else if (0 != (this.Attributes & WixComponentSearchAttributes.State))
            {
                writer.WriteAttributeString("Type", "state");
            }
            else if (0 != (this.Attributes & WixComponentSearchAttributes.WantDirectory))
            {
                writer.WriteAttributeString("Type", "directory");
            }

            writer.WriteEndElement();
        }
    }

}
