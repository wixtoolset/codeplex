//-------------------------------------------------------------------------------------------------
// <copyright file="Pdb.cs" company="Outercurve Foundation">
//   Copyright (c) 2004, Outercurve Foundation.
//   This software is released under Microsoft Reciprocal License (MS-RL).
//   The license and further copyright text can be found in the file
//   LICENSE.TXT at the root directory of the distribution.
// </copyright>
// 
// <summary>
// Pdb containing metadata about the wix build.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace WixToolset.Data
{
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// Pdb generated by the binder.
    /// </summary>
    public sealed class Pdb
    {
        public const string XmlNamespaceUri = "http://wixtoolset.org/schemas/v4/wixpdb";
        private static readonly Version currentVersion = new Version("4.0.0.0");

        private static readonly object lockObject = new object();

        private Output output;
        
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private SourceLineNumber sourceLineNumbers;

        /// <summary>
        /// Creates a new empty pdb object.
        /// </summary>
        /// <param name="sourceLineNumbers">The source line information for the pdb.</param>
        public Pdb(SourceLineNumber sourceLineNumbers)
        {
            this.sourceLineNumbers = sourceLineNumbers;
        }

        /// <summary>
        /// Gets or sets the output that is a part of this pdb.
        /// </summary>
        /// <value>Type of the output.</value>
        public Output Output
        {
            get { return this.output; }
            set { this.output = value; }
        }

        /// <summary>
        /// Loads a pdb from a path on disk.
        /// </summary>
        /// <param name="path">Path to pdb file saved on disk.</param>
        /// <param name="suppressVersionCheck">Suppresses wix.dll version mismatch check.</param>
        /// <returns>Pdb pdb.</returns>
        public static Pdb Load(string path, bool suppressVersionCheck)
        {
            try
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    return Load(stream, new Uri(Path.GetFullPath(path)), suppressVersionCheck);
                }
            }
            catch (FileNotFoundException e)
            {
                throw new WixException(WixDataErrors.WixFileNotFound(path), e);
            }
        }

        /// <summary>
        /// Loads a pdb from a path on disk.
        /// </summary>
        /// <param name="stream">Stream containing the pdb file.</param>
        /// <param name="uri">Uri for finding this stream.</param>
        /// <param name="suppressVersionCheck">Suppresses wix.dll version mismatch check.</param>
        /// <returns>Returns the loaded pdb.</returns>
        /// <remarks>This method will set the Path and SourcePath properties to the appropriate values on successful load.</remarks>
        internal static Pdb Load(Stream stream, Uri uri, bool suppressVersionCheck)
        {
            XmlReader reader = null;
            string cabPath = null;

            // look for the Microsoft cabinet file header and save the cabinet data if found
            if ('M' == stream.ReadByte() && 'S' == stream.ReadByte() && 'C' == stream.ReadByte() && 'F' == stream.ReadByte())
            {
                long cabFileSize = 0;
                byte[] offsetBuffer = new byte[4];
                using (TempFileCollection tempFileCollection = new TempFileCollection())
                {
                    cabPath = tempFileCollection.AddExtension("cab", true);
                }

                // skip the header checksum
                stream.Seek(4, SeekOrigin.Current);

                // get the cabinet file size
                stream.Read(offsetBuffer, 0, 4);
                cabFileSize = BitConverter.ToInt32(offsetBuffer, 0);

                stream.Seek(0, SeekOrigin.Begin);

                // Create the cab file from stream
                using (FileStream fs = File.Create(cabPath))
                {
                    for (int i = 0; i < cabFileSize; i++)
                    {
                        fs.WriteByte((byte)stream.ReadByte());
                    }
                }
            }
            else // plain xml file - start reading xml at the beginning of the stream
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            // read the xml
            try
            {
                reader = new XmlTextReader(uri.AbsoluteUri, stream);

                reader.MoveToContent();

                if ("wixPdb" != reader.LocalName)
                {
                    throw new WixNotOutputException(WixDataErrors.InvalidDocumentElement(SourceLineNumber.CreateFromUri(reader.BaseURI), reader.Name, "pdb", "wixPdb"));
                }

                Pdb pdb = Parse(reader, suppressVersionCheck);

                if (null != cabPath)
                {
                    if (pdb.Output.TempFiles == null)
                    {
                        pdb.Output.TempFiles = new TempFileCollection();
                    }

                    pdb.Output.TempFiles.AddFile(cabPath, false);
                }

                return pdb;
            }
            catch (XmlException xe)
            {
                throw new WixException(WixDataErrors.InvalidXml(SourceLineNumber.CreateFromUri(reader.BaseURI), "output", xe.Message));
            }
            finally
            {
                if (null != reader)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Processes an XmlReader and builds up the pdb object.
        /// </summary>
        /// <param name="reader">Reader to get data from.</param>
        /// <param name="suppressVersionCheck">Suppresses wix.dll version mismatch check.</param>
        /// <returns>The Pdb represented by the Xml.</returns>
        internal static Pdb Parse(XmlReader reader, bool suppressVersionCheck)
        {
            Debug.Assert("wixPdb" == reader.LocalName);

            bool empty = reader.IsEmptyElement;
            Pdb pdb = new Pdb(SourceLineNumber.CreateFromUri(reader.BaseURI));
            Version version = null;

            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "version":
                        version = new Version(reader.Value);
                        break;
                    default:
                        if (!reader.NamespaceURI.StartsWith("http://www.w3.org/", StringComparison.Ordinal))
                        {
                            throw new WixException(WixDataErrors.UnexpectedAttribute(SourceLineNumber.CreateFromUri(reader.BaseURI), "wixPdb", reader.Name));
                        }
                        break;
                }
            }

            if (null != version)
            {
                if (0 != currentVersion.CompareTo(version))
                {
                    throw new WixException(WixDataErrors.VersionMismatch(SourceLineNumber.CreateFromUri(reader.BaseURI), "wixPdb", version.ToString(), currentVersion.ToString()));
                }
            }

            // loop through the rest of the pdb building up the Output object
            if (!empty)
            {
                bool done = false;

                // loop through all the fields in a row
                while (!done && reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.LocalName)
                            {
                                case "wixOutput":
                                    pdb.output = Output.Read(reader, suppressVersionCheck);
                                    break;
                                default:
                                    throw new WixException(WixDataErrors.UnexpectedElement(SourceLineNumber.CreateFromUri(reader.BaseURI), "wixPdb", reader.Name));
                            }
                            break;
                        case XmlNodeType.EndElement:
                            done = true;
                            break;
                    }
                }

                if (!done)
                {
                    throw new WixException(WixDataErrors.ExpectedEndElement(SourceLineNumber.CreateFromUri(reader.BaseURI), "wixOutput"));
                }
            }

            return pdb;
        }

        /// <summary>
        /// Saves a pdb to a path on disk.
        /// </summary>
        /// <param name="path">Path to save pdb file to on disk.</param>
        /// <param name="binderFileManager">If provided, the binder file manager is used to bind files into the pdb.</param>
        /// <param name="wixVariableResolver">The Wix variable resolver.</param>
        /// <param name="tempFilesLocation">Location for temporary files.</param>
        public void Save(string path, string tempFilesLocation)
        {
            FileMode fileMode = FileMode.Create;

            // Assure the location to output the xml exists
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));

            // save the xml
            try
            {
                using (FileStream fs = new FileStream(path, fileMode))
                {
                    XmlWriter writer = null;

                    try
                    {
                        writer = new XmlTextWriter(fs, System.Text.Encoding.UTF8);

                        writer.WriteStartDocument();
                        this.Persist(writer);
                        writer.WriteEndDocument();
                    }
                    finally
                    {
                        if (null != writer)
                        {
                            writer.Close();
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new WixException(WixDataErrors.UnauthorizedAccess(path));
            }
        }

        /// <summary>
        /// Persists a pdb in an XML format.
        /// </summary>
        /// <param name="writer">XmlWriter where the Pdb should persist itself as XML.</param>
        internal void Persist(XmlWriter writer)
        {
            writer.WriteStartElement("wixPdb", XmlNamespaceUri);

            writer.WriteAttributeString("version", currentVersion.ToString());

            this.output.Persist(writer);

            writer.WriteEndElement();
        }
    }
}
