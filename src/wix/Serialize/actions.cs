﻿//------------------------------------------------------------------------------
// <copyright file="actions.cs" company="Microsoft">
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
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.573
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=1.1.4322.573.
// 
namespace Microsoft.Tools.WindowsInstallerXml.Serialize {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/wix/2003/04/actions")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://schemas.microsoft.com/wix/2003/04/actions", IsNullable=false)]
    public class actions {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("action")]
        public action[] action;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/wix/2003/04/actions")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://schemas.microsoft.com/wix/2003/04/actions", IsNullable=false)]
    public class action {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string condition;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
        public string sequence;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ActionsYesNoType AdminExecuteSequence;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AdminExecuteSequenceSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ActionsYesNoType AdminUISequence;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AdminUISequenceSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ActionsYesNoType AdvtExecuteSequence;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AdvtExecuteSequenceSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ActionsYesNoType InstallExecuteSequence;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool InstallExecuteSequenceSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ActionsYesNoType InstallUISequence;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool InstallUISequenceSpecified;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/wix/2003/04/actions")]
    public enum ActionsYesNoType {
        
        /// <remarks/>
        no,
        
        /// <remarks/>
        yes,
    }
}
