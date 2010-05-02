﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1434
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Tools {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SmokeStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SmokeStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.Tools.WindowsInstallerXml.Tools.SmokeStrings", typeof(SmokeStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to cannot load linker extension: {0}.  light can only load one link extension and has already loaded link extension: {1}..
        /// </summary>
        internal static string EXP_CannotLoadLinkerExtension {
            get {
                return ResourceManager.GetString("EXP_CannotLoadLinkerExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown input file format - expected a .msi or .msm file..
        /// </summary>
        internal static string EXP_UnkownInputFileFormat {
            get {
                return ResourceManager.GetString("EXP_UnkownInputFileFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  usage:  smoke.exe [-?]  databaseFile [databaseFile ...]
        ///
        ///   -cub         additional .cub file containing ICEs to run
        ///   -ext         extension assembly or &quot;class, assembly&quot;
        ///   -ice:&lt;ICE&gt;   run a specific internal consistency evaluator (ICE)
        ///   -nodefault   do not add the default .cub files for .msi and .msm files
        ///   -nologo      skip printing smoke logo information
        ///   -notidy      do not delete temporary files (useful for debugging)
        ///   -pdb         path to the pdb file corresponding to the database [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string HelpMessage {
            get {
                return ResourceManager.GetString("HelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Temporary directory located at &apos;{0}&apos;..
        /// </summary>
        internal static string INF_TempDirLocatedAt {
            get {
                return ResourceManager.GetString("INF_TempDirLocatedAt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Warning, failed to delete temporary directory: {0}.
        /// </summary>
        internal static string WAR_FailedToDeleteTempDir {
            get {
                return ResourceManager.GetString("WAR_FailedToDeleteTempDir", resourceCulture);
            }
        }
    }
}
