//------------------------------------------------------------------------------------------------------------------------
// <copyright file="Output.FileSystemTests.cs" company="Microsoft">
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
// <summary>
//     Tests for Lit interaction with the file system to producing output. 
// </summary>
//------------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Tests.Tools.Lit.Output
{
    using System;
    using System.IO;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Tools.WindowsInstallerXml.Test;
    
    /// <summary>
    /// Tests for Lit interaction with the file system to producing output.
    /// </summary>
    [TestClass]
    public class FileSystemTests : WixTests
    {
        [TestMethod]
        [Description("Verify that Lit fails gracefully in case of creating a output file on a network share with no permissions.")]
        [Priority(2)]
        [Ignore]
        public void NetworkShareNoPermissions()
        {
        }

        [TestMethod]
        [Description("Verify that Lit can output nonalphanumeric characters in the filename")]
        [Priority(2)]
        public void NonAlphaNumericCharactersInFileName()
        {
            DirectoryInfo outputDirectory = Directory.CreateDirectory(Builder.GetUniqueFileName());

            Lit lit = new Lit();
            lit.ObjectFiles.Add(Candle.Compile(WixTests.PropertyFragmentWxs));
            lit.OutputFile = Path.Combine(outputDirectory.FullName, "~!@#$%^&()_-+=,;.wix");
            lit.Run();
        }

        [TestMethod]
        [Description("Verify that Lit can output files to a read only share")]
        [Priority(2)]
        [Ignore]
        public void ReadOnlyShare()
        {
        }

        [TestMethod]
        [Description("Verify that Lit can output files to a network share")]
        [Priority(2)]
        [Ignore]
        public void FileToNetworkShare()
        {
        }

        [TestMethod]
        [Description("Verify that Lit can output file names with single quotes")]
        [Priority(3)]
        public void FileNameWithSingleQuotes()
        {
            DirectoryInfo outputDirectory = Directory.CreateDirectory(Builder.GetUniqueFileName());

            Lit lit = new Lit();
            lit.ObjectFiles.Add(Candle.Compile(WixTests.PropertyFragmentWxs));
            lit.OutputFile = Path.Combine(outputDirectory.FullName, "Simple'Fragment'.wix");
            lit.Run();
        }
               
        [TestMethod]
        [Description("Verify that Lit can output a file with space in its name.")]
        [Priority(3)]
        public void FileNameWithSpace()
        {
            DirectoryInfo outputDirectory = Directory.CreateDirectory(Builder.GetUniqueFileName());

            Lit lit = new Lit();
            lit.ObjectFiles.Add(Candle.Compile(WixTests.PropertyFragmentWxs));
            lit.OutputFile = Path.Combine(outputDirectory.FullName, " Simple  Fragment                          .wix");
            lit.Run();
        }

        [TestMethod]
        [Description("Verify that Lit output to a file path that is more than 256 characters.")]
        [Priority(3)]
        public void LongFilePath()
        {
            DirectoryInfo tempDirectory = Directory.CreateDirectory(Builder.GetUniqueFileName());
            string longFileName = FileSystemTests.GetRandomString(256);

            Lit lit = new Lit();
            lit.ObjectFiles.Add(Candle.Compile(WixTests.PropertyFragmentWxs));
            lit.OutputFile = tempDirectory.FullName +Path.DirectorySeparatorChar+ longFileName;
            string errorMessage = string.Format("Invalid file name specified on the command line: '{0}'. Error message: 'The specified path, file name, or both are too long. The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters.'", lit.OutputFile);
            lit.ExpectedWixMessages.Add(new WixMessage(284, errorMessage, WixMessage.MessageTypeEnum.Error));
            lit.ExpectedExitCode = 284;

            lit.Run();
        }
              
        [TestMethod]
        [Description("Verify that Lit can output a file to a URI path.")]
        [Priority(3)]
        [Ignore]
        public void URI()
        {
        }

        /// <summary>
        /// Generates a string of random lowercase chars with a specific length
        /// </summary>
        /// <param name="length">The length of the random string</param>
        /// <returns>Random string with the specified length</returns>
        private static string GetRandomString(int length)
        {
            StringBuilder stringBuilder = new StringBuilder(length);
            Random random = new Random(WixTests.Seed);

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(Convert.ToChar(random.Next(Convert.ToInt32('a'),Convert.ToInt32('z'))));
            }

            return stringBuilder.ToString();
        }
    }
}