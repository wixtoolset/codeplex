//-------------------------------------------------------------------------------------------------
// <copyright file="VariableInfo.cs" company="Microsoft">
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
// Utility class for Burn variable information.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml
{
    using System;
    using System.Xml;

    /// <summary>
    /// Utility class for Burn variable information.
    /// </summary>
    internal class VariableInfo
    {
        public VariableInfo(Row row)
            : this((string)row[0], (string)row[1], (string)row[2], (int)row[3] == 1 ? true : false, (int)row[4] == 1 ? true : false)
        {
        }

        public VariableInfo(string id, string value, string type, bool hidden, bool persisted)
        {
            this.Id = id;
            this.Value = value;
            this.Type = type;
            this.Hidden = hidden;
            this.Persisted = persisted;
        }

        public bool Hidden { get; private set; }
        public string Id { get; private set; }
        public bool Persisted { get; private set; }
        public string Value { get; private set; }
        public string Type { get; private set; }

        /// <summary>
        /// Generates Burn manifest markup for a variable.
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("Variable");
            writer.WriteAttributeString("Id", this.Id);
            if (null != this.Type)
            {
                writer.WriteAttributeString("Value", this.Value);
                writer.WriteAttributeString("Type", this.Type);
            }
            writer.WriteAttributeString("Hidden", this.Hidden ? "yes" : "no");
            writer.WriteAttributeString("Persisted", this.Persisted ? "yes" : "no");
            writer.WriteEndElement();
        }
    }
}
