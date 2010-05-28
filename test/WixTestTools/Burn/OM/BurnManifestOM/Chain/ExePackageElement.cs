﻿//-----------------------------------------------------------------------
// <copyright file="ExePackageElement.cs" company="Microsoft">
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
// <summary>Exe element OM</summary>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Tools.WindowsInstallerXml.Test.Burn.OM.ParameterInfoOM.ElementAttribute;

namespace Microsoft.Tools.WindowsInstallerXml.Test.Burn.OM.BurnManifestOM.Chain
{
    [BurnXmlElement("ExePackage")]
    public class ExePackageElement : Package
    {
        // Xml attributes
        private string m_PerMachine;

        [BurnXmlAttribute("PerMachine")]
        public string PerMachine
        {
            get { return m_PerMachine; }
            set { m_PerMachine = value; }
        }

        private string m_InstallArguments;

        [BurnXmlAttribute("InstallArguments")]
        public string InstallArguments
        {
            get { return m_InstallArguments; }
            set { m_InstallArguments = value; }
        }

        private string m_RepairArguments;

        [BurnXmlAttribute("RepairArguments")]
        public string RepairArguments
        {
            get { return m_RepairArguments; }
            set { m_RepairArguments = value; }
        }

        private string m_UninstallArguments;

        [BurnXmlAttribute("UninstallArguments")]
        public string UninstallArguments
        {
            get { return m_UninstallArguments; }
            set { m_UninstallArguments = value; }
        }
    }
}
