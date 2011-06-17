//-------------------------------------------------------------------------------------------------
// <copyright file="NetFXVerifier.cs" company="Microsoft">
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
// 
// <summary>
//      Contains methods for verification for NetFX Extension
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Test.Verifiers.Extensions
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.DirectoryServices;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Contains methods for NetFX Extension test verification
    /// </summary>
    public static class NetFXVerifier
    {
        public enum FrameworkVersion
        {
            NetFX20,
            NetFX40
        };

        public enum FrameworkArch
        {
            x86,
            x64
        };

        public static bool NativeImageExists(string fileName, FrameworkVersion version, FrameworkArch arch)
        {
            string assymblyFolder = Path.Combine(Environment.ExpandEnvironmentVariables("%WInDir%"), "assembly");
            string nativeImageFileName = Path.GetFileNameWithoutExtension(fileName) + ".ni" + Path.GetExtension(fileName);
            string nativeImageFolderName = "NativeImages";
            
            if (FrameworkVersion.NetFX20 == version)
            {
                nativeImageFolderName += "_v2.0.50727";
            }
            else if (FrameworkVersion.NetFX40 == version)
            {
                // version number will keep changing up untill 4.0 RTM
                nativeImageFolderName += "_v4.0.*";
            }

            if (FrameworkArch.x86 == arch)
            {
                nativeImageFolderName += "_32";
            }
            else if (FrameworkArch.x64 == arch)
            {
                nativeImageFolderName += "_64";
            }
            
            // search for all directories matching the widcard to suppor 4.0
            DirectoryInfo directory = new DirectoryInfo(assymblyFolder);
            DirectoryInfo[] nativeImageDirectoryList = directory.GetDirectories(nativeImageFolderName);

            if (null == nativeImageDirectoryList || nativeImageDirectoryList.Length < 1)
            {
                return false;
            }

            FileInfo[] nativeImageFileList = nativeImageDirectoryList[0].GetFiles(nativeImageFileName, SearchOption.AllDirectories);
            if (null == nativeImageFileList || nativeImageFileList.Length < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}