﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
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
    internal class LightStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal LightStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.Tools.WindowsInstallerXml.Tools.LightStrings", typeof(LightStrings).Assembly);
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
        ///   Looks up a localized string similar to The -bf (bind files) option is only applicable with the -xo option..
        /// </summary>
        internal static string EXP_BindFileOptionNotApplicable {
            get {
                return ResourceManager.GetString("EXP_BindFileOptionNotApplicable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot link object files (.wixobj) files with an output file (.wixout).
        /// </summary>
        internal static string EXP_CannotLinkObjFilesWithOutpuFile {
            get {
                return ResourceManager.GetString("EXP_CannotLinkObjFilesWithOutpuFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to cannot load binder file manager: {0}.  light can only load one binder file manager and has already loaded binder file manager: {1}..
        /// </summary>
        internal static string EXP_CannotLoadBinderFileManager {
            get {
                return ResourceManager.GetString("EXP_CannotLoadBinderFileManager", resourceCulture);
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
        ///   Looks up a localized string similar to  usage:  light.exe [-?] [-b basePath] [-nologo] [-out outputFile] objectFile [objectFile ...]
        ///
        ///   -ai        allow identical rows, identical rows will be treated as a warning
        ///   -au        (experimental) allow unresolved references, will not create a valid output
        ///   -b         base path to locate all files (default: current directory)
        ///   -bcgg      use backwards compatible guid generation algorithm (almost never needed)
        ///   -bf        bind files into a wixout (only valid with -xo option)
        ///   -cc        [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string HelpMessage {
            get {
                return ResourceManager.GetString("HelpMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Binder temporary directory located at &apos;{0}&apos;..
        /// </summary>
        internal static string INF_BinderTempDirLocatedAt {
            get {
                return ResourceManager.GetString("INF_BinderTempDirLocatedAt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Validator temporary directory located at &apos;{0}&apos;..
        /// </summary>
        internal static string INF_ValidatorTempDirLocatedAt {
            get {
                return ResourceManager.GetString("INF_ValidatorTempDirLocatedAt", resourceCulture);
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
