//-------------------------------------------------------------------------------------------------
// <copyright file="ObjectField.cs" company="Microsoft">
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
//    Field containing data for an object column in a row.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml
{
    using System;
    using System.Diagnostics;
    using System.Xml;

    /// <summary>
    /// Field containing data for an object column in a row.
    /// </summary>
    public sealed class ObjectField : Field
    {
        private string baseUri;
        private string cabinetFileId;
        private string previousCabinetFileId;
        private string previousBaseUri;

        /// <summary>
        /// Instantiates a new Field.
        /// </summary>
        /// <param name="columnDefinition">Column definition for this field.</param>
        internal ObjectField(ColumnDefinition columnDefinition) :
            base(columnDefinition)
        {
        }

        /// <summary>
        /// Gets or sets the identifier of the file in the cabinet.
        /// </summary>
        /// <value>The identifier of the file in the cabinet.</value>
        public string CabinetFileId
        {
            get { return this.cabinetFileId; }
            set { this.cabinetFileId = value; }
        }

        /// <summary>
        /// Gets or sets the previous identifier of the file in the cabinet.
        /// </summary>
        /// <value>The identifier of the file in the cabinet.</value>
        public string PreviousCabinetFileId
        {
            get { return this.previousCabinetFileId; }
            set { this.previousCabinetFileId = value; }
        }

        /// <summary>
        /// Gets or sets the path to the embedded cabinet of the previous file.
        /// </summary>
        /// <value>The path of the cabinet containing the previous file.</value>
        public string PreviousBaseUri
        {
            get { return this.previousBaseUri; }
            set { this.previousBaseUri = value; }
        }

        /// <summary>
        /// Gets the base URI of the object field.
        /// </summary>
        /// <value>The base URI of the object field.</value>
        internal string BaseUri
        {
            get { return this.baseUri; }
        }

        /// <summary>
        /// Parse a field from the xml.
        /// </summary>
        /// <param name="reader">XmlReader where the intermediate is persisted.</param>
        internal override void Parse(XmlReader reader)
        {
            Debug.Assert("field" == reader.LocalName);

            bool empty = reader.IsEmptyElement;

            this.baseUri = reader.BaseURI;

            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "cabinetFileId":
                        this.cabinetFileId = reader.Value;
                        break;
                    case "modified":
                        this.Modified = Common.IsYes(SourceLineNumberCollection.FromUri(reader.BaseURI), "field", reader.Name, reader.Value);
                        break;
                    case "previousData":
                        this.PreviousData = reader.Value;
                        break;
                    case "previousCabinetFileId":
                        this.previousCabinetFileId = reader.Value;
                        break;
                    default:
                        if (!reader.NamespaceURI.StartsWith("http://www.w3.org/", StringComparison.Ordinal))
                        {
                            throw new WixException(WixErrors.UnexpectedAttribute(SourceLineNumberCollection.FromUri(reader.BaseURI), "field", reader.Name));
                        }
                        break;
                }
            }

            if (!empty)
            {
                bool done = false;

                while (!done && reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            throw new WixException(WixErrors.UnexpectedElement(SourceLineNumberCollection.FromUri(reader.BaseURI), "field", reader.Name));
                        case XmlNodeType.CDATA:
                        case XmlNodeType.Text:
                            if (0 < reader.Value.Length)
                            {
                                this.Data = reader.Value;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            done = true;
                            break;
                    }
                }

                if (!done)
                {
                    throw new WixException(WixErrors.ExpectedEndElement(SourceLineNumberCollection.FromUri(reader.BaseURI), "field"));
                }
            }
        }

        /// <summary>
        /// Persists a field in an XML format.
        /// </summary>
        /// <param name="writer">XmlWriter where the Field should persist itself as XML.</param>
        internal override void Persist(XmlWriter writer)
        {
            string text;

            // convert the data to a string that will persist nicely
            if (null == this.Data)
            {
                text = String.Empty;
            }
            else
            {
                text = (string)this.Data;
            }

            writer.WriteStartElement("field", Intermediate.XmlNamespaceUri);

            if (null != this.cabinetFileId)
            {
                writer.WriteAttributeString("cabinetFileId", this.cabinetFileId);
            }

            if (this.Modified)
            {
                writer.WriteAttributeString("modified", "yes");
            }

            if (null != this.PreviousData)
            {
                writer.WriteAttributeString("previousData", this.PreviousData);
            }

            if (null != this.previousCabinetFileId)
            {
                writer.WriteAttributeString("previousCabinetFileId", this.previousCabinetFileId);
            }

            if (this.Column.UseCData)
            {
                writer.WriteCData(text);
            }
            else
            {
                writer.WriteString(text);
            }

            writer.WriteEndElement();
        }
    }
}
