//-----------------------------------------------------------------------
// <copyright file="Candle.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//    
//    The use and distribution terms for this software are covered by the
//    Common Public License 1.0 (http://opensource.org/licenses/cpl1.0.php)
//    which can be found in the file CPL.TXT at the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by
//    the terms of this license.
//    
//    You must not remove this notice, or any other, from this software.
// </copyright>
// <summary>A class that wraps Candle</summary>
//-----------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// A class that wraps Candle.
    /// </summary>
    public partial class Candle : WixTool
    {

        public static string Compile(string sourceFile)
        {
            if (String.IsNullOrEmpty(sourceFile))
            {
                throw new ArgumentException("sourceFile cannot be null or empty");
            }

            string[] outputFiles = Candle.Compile(new string[] { sourceFile });

            if (null != outputFiles && outputFiles.Length > 0)
            {
                return outputFiles[0];
            }
            else
            {
                return null;
            }
        }

        public static string[] Compile(
            string[] sourceFiles,
            bool fips = false,        
            WixMessage[] expectedWixMessages = null,
            string[] extensions = null,
            string[] includeSearchPaths = null,
            bool onlyValidateDocuments =false,
            string otherArguments = null,
            bool pedantic = false, 
            string preProcessFile = null,
            Dictionary<string, string> preProcessorParams = null,
            bool setOutputFileIfNotSpecified = true,
            bool suppressAllWarnings = false,
            bool suppressMarkingVitalDefault = false,
            bool suppressSchemaValidation = false,
            int[] suppressWarnings = null,
            bool trace = false,
            int[] treatWarningsAsErrors = null,
            bool treatAllWarningsAsErrors =false,         
            bool verbose = false)
        {
            Candle candle = new Candle();

            if (null == sourceFiles || sourceFiles.Length == 0)
            {
                throw new ArgumentException("sourceFiles cannot be null or empty");
            }

            // set the passed arrguments
            candle.SourceFiles.AddRange(sourceFiles);
            if (null != expectedWixMessages)
            {
                candle.ExpectedWixMessages.AddRange(expectedWixMessages);
            }
            if (null != extensions)
            {
                candle.Extensions.AddRange(extensions);
            }
            candle.FIPS = fips;
            if (null != includeSearchPaths)
            {
                candle.IncludeSearchPaths.AddRange(includeSearchPaths);
            }
            candle.OnlyValidateDocuments = onlyValidateDocuments;
            candle.OtherArguments = otherArguments;
            candle.Pedantic = pedantic;
            candle.PreProcessFile = preProcessFile;
            if (null != preProcessorParams)
            {
                candle.PreProcessorParams = preProcessorParams;
            }
            candle.SetOutputFileIfNotSpecified = setOutputFileIfNotSpecified;
            candle.SuppressAllWarnings = suppressAllWarnings;
            candle.SuppressMarkingVitalDefault = suppressMarkingVitalDefault;
            candle.SuppressSchemaValidation = suppressSchemaValidation;
            if (null != suppressWarnings)
            {
                candle.SuppressWarnings.AddRange(suppressWarnings);
            }
            candle.Trace = trace;
            if (null != treatWarningsAsErrors)
            {
                candle.TreatWarningsAsErrors.AddRange(treatWarningsAsErrors);
            }
            candle.TreatAllWarningsAsErrors = treatAllWarningsAsErrors;
            candle.Verbose= verbose;

            candle.Run();

            return candle.ExpectedOutputFiles.ToArray();
        }
    }
}