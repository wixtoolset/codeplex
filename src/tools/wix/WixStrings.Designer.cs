﻿//------------------------------------------------------------------------------
// <copyright file="WixStrings.Designer.cs" company="Outercurve Foundation">
//   Copyright (c) 2004, Outercurve Foundation.
//   This software is released under Microsoft Reciprocal License (MS-RL).
//   The license and further copyright text can be found in the file
//   LICENSE.TXT at the root directory of the distribution.
// </copyright>
// 
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml {
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
    internal class WixStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal WixStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.Tools.WindowsInstallerXml.WixStrings", typeof(WixStrings).Assembly);
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
        ///   Looks up a localized string similar to    -bcgg      use backwards compatible guid generation algorithm
        ///              (almost never needed)
        ///   -cc &lt;path&gt; path to cache built cabinets (will not be deleted after linking)
        ///   -ct &lt;N&gt;    number of threads to use when creating cabinets
        ///              (default: %NUMBER_OF_PROCESSORS%)
        ///   -cub &lt;file.cub&gt; additional .cub file containing ICEs to run
        ///   -dcl:level set default cabinet compression level
        ///              (low, medium, high, none, mszip; mszip default)
        ///   -eav       exact assembly versions [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string BinderArguments {
            get {
                return ResourceManager.GetString("BinderArguments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot index into a FileRowCollection that allows duplicate FileIds.
        /// </summary>
        internal static string EXP_CannotIndexIntoFileRowCollection {
            get {
                return ResourceManager.GetString("EXP_CannotIndexIntoFileRowCollection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value &apos;{0}&apos; is not a legal identifier and therefore cannot be modularized..
        /// </summary>
        internal static string EXP_CannotModularizeIllegalID {
            get {
                return ResourceManager.GetString("EXP_CannotModularizeIllegalID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot set column &apos;{0}&apos; with value {1} because it is greater than the maximum allowed value for this column, {2}..
        /// </summary>
        internal static string EXP_CannotSetColumnWithValueGreaterThanMaxValue {
            get {
                return ResourceManager.GetString("EXP_CannotSetColumnWithValueGreaterThanMaxValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot set column &apos;{0}&apos; with value {1} because it is less than the minimum allowed value for this column, {2}..
        /// </summary>
        internal static string EXP_CannotSetColumnWithValueLessThanMinValue {
            get {
                return ResourceManager.GetString("EXP_CannotSetColumnWithValueLessThanMinValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A Merge table FileCompression column cannot be set to the invalid value &apos;{0}&apos;..
        /// </summary>
        internal static string EXP_CannotSetMergeTableFileCompressionColumnToInvalidValue {
            get {
                return ResourceManager.GetString("EXP_CannotSetMergeTableFileCompressionColumnToInvalidValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot set column &apos;{0}&apos; with a null value because this is a required field..
        /// </summary>
        internal static string EXP_CannotSetNullOnRequiredField {
            get {
                return ResourceManager.GetString("EXP_CannotSetNullOnRequiredField", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot set number column &apos;{0}&apos; with a value of type &apos;{1}&apos;..
        /// </summary>
        internal static string EXP_CannotSetNumberColumnWithValueOfType {
            get {
                return ResourceManager.GetString("EXP_CannotSetNumberColumnWithValueOfType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot set string column &apos;{0}&apos; with a value of type &apos;{1}&apos;..
        /// </summary>
        internal static string EXP_CannotSetStringColumnWithValueOfType {
            get {
                return ResourceManager.GetString("EXP_CannotSetStringColumnWithValueOfType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Collection has {0} elements. Must have at least one..
        /// </summary>
        internal static string EXP_CollectionMustHaveAtLeastOneElement {
            get {
                return ResourceManager.GetString("EXP_CollectionMustHaveAtLeastOneElement", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not determine ProductCode from transform summary information.
        /// </summary>
        internal static string EXP_CouldnotDetermineProductCodeFromTransformSummaryInfo {
            get {
                return ResourceManager.GetString("EXP_CouldnotDetermineProductCodeFromTransformSummaryInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find a unique identifier for the given resource name..
        /// </summary>
        internal static string EXP_CouldnotFileUniqueIDForResourceName {
            get {
                return ResourceManager.GetString("EXP_CouldnotFileUniqueIDForResourceName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Didn&apos;t find duplicated symbol..
        /// </summary>
        internal static string EXP_DidnotFindDuplicateSymbol {
            get {
                return ResourceManager.GetString("EXP_DidnotFindDuplicateSymbol", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Element must be a subclass of {0}, but was of type {1}..
        /// </summary>
        internal static string EXP_ElementIsSubclassOfDifferentType {
            get {
                return ResourceManager.GetString("EXP_ElementIsSubclassOfDifferentType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Element of type {0} found in enumerator. Must be ChoiceItem or SequenceItem..
        /// </summary>
        internal static string EXP_ElementMustBeChoiceItemOrSequenceItem {
            get {
                return ResourceManager.GetString("EXP_ElementMustBeChoiceItemOrSequenceItem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Element of type {0} is not valid for this collection..
        /// </summary>
        internal static string EXP_ElementOfTypeIsNotValidForThisCollection {
            get {
                return ResourceManager.GetString("EXP_ElementOfTypeIsNotValidForThisCollection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected ComplexReference type..
        /// </summary>
        internal static string EXP_ExpectedComplexReferenceType {
            get {
                return ResourceManager.GetString("EXP_ExpectedComplexReferenceType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found an ActionRow with a non-existent {0} action..
        /// </summary>
        internal static string EXP_FoundActionRowWinNonExistentAction {
            get {
                return ResourceManager.GetString("EXP_FoundActionRowWinNonExistentAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found an ActionRow with no Sequence, Before, or After column set..
        /// </summary>
        internal static string EXP_FoundActionRowWithNoSequenceBeforeOrAfterColumnSet {
            get {
                return ResourceManager.GetString("EXP_FoundActionRowWithNoSequenceBeforeOrAfterColumnSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Illegal arguments passed..
        /// </summary>
        internal static string EXP_IllegalArgumentsPassed {
            get {
                return ResourceManager.GetString("EXP_IllegalArgumentsPassed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid table name passed into GenerateIdentifier..
        /// </summary>
        internal static string EXP_InvalidTableNamePassed {
            get {
                return ResourceManager.GetString("EXP_InvalidTableNamePassed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ISchemaElement with name {0} does not implement ICreateChildren..
        /// </summary>
        internal static string EXP_ISchemaElementDoesnotImplementICreateChildren {
            get {
                return ResourceManager.GetString("EXP_ISchemaElementDoesnotImplementICreateChildren", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ISchemaElement with name {0} does not implement ISetAttributes..
        /// </summary>
        internal static string EXP_ISchemaElementDoesnotImplementISetAttribute {
            get {
                return ResourceManager.GetString("EXP_ISchemaElementDoesnotImplementISetAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A Merge table FileCompression column contains an invalid value &apos;{0}&apos;..
        /// </summary>
        internal static string EXP_MergeTableFileCompressionColumnContainsInvalidValue {
            get {
                return ResourceManager.GetString("EXP_MergeTableFileCompressionColumnContainsInvalidValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple harvester extensions specified..
        /// </summary>
        internal static string EXP_MultipleHarvesterExtensionsSpecified {
            get {
                return ResourceManager.GetString("EXP_MultipleHarvesterExtensionsSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple root elements found in file..
        /// </summary>
        internal static string EXP_MultipleRootElementsFoundInFile {
            get {
                return ResourceManager.GetString("EXP_MultipleRootElementsFoundInFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The other object is not a FileRow..
        /// </summary>
        internal static string EXP_OtherObjectIsNotFileRow {
            get {
                return ResourceManager.GetString("EXP_OtherObjectIsNotFileRow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Transform authored into multiple Media &apos;{0}&apos; and &apos;{1}&apos;..
        /// </summary>
        internal static string EXP_TransformAuthoredIntoMultipleMedia {
            get {
                return ResourceManager.GetString("EXP_TransformAuthoredIntoMultipleMedia", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type {0} is not valid for this collection..
        /// </summary>
        internal static string EXP_TypeIsNotValidForThisCollection {
            get {
                return ResourceManager.GetString("EXP_TypeIsNotValidForThisCollection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected complex reference child type: {0}.
        /// </summary>
        internal static string EXP_UnexpectedComplexReferenceChildType {
            get {
                return ResourceManager.GetString("EXP_UnexpectedComplexReferenceChildType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected entry section type: {0}.
        /// </summary>
        internal static string EXP_UnexpectedEntrySectionType {
            get {
                return ResourceManager.GetString("EXP_UnexpectedEntrySectionType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Encountered an unexpected error while merging &apos;{0}&apos;. More information about the merge and the failure can be found in the merge log: &apos;{1}&apos;.
        /// </summary>
        internal static string EXP_UnexpectedMergerErrorInSourceFile {
            get {
                return ResourceManager.GetString("EXP_UnexpectedMergerErrorInSourceFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Encountered an unexpected merge error of type &apos;{0}&apos; for which there is currently no error message to display.  More information about the merge and the failure can be found in the merge log: &apos;{1}&apos;.
        /// </summary>
        internal static string EXP_UnexpectedMergerErrorWithType {
            get {
                return ResourceManager.GetString("EXP_UnexpectedMergerErrorWithType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown control attribute: &apos;{0}&apos;..
        /// </summary>
        internal static string EXP_UnknowControlAttribute {
            get {
                return ResourceManager.GetString("EXP_UnknowControlAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown column type: {0}.
        /// </summary>
        internal static string EXP_UnknownColumnType {
            get {
                return ResourceManager.GetString("EXP_UnknownColumnType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown compression level type: {0}.
        /// </summary>
        internal static string EXP_UnknownCompressionLevelType {
            get {
                return ResourceManager.GetString("EXP_UnknownCompressionLevelType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown custom column category &apos;{0}&apos;..
        /// </summary>
        internal static string EXP_UnknownCustomColumnCategory {
            get {
                return ResourceManager.GetString("EXP_UnknownCustomColumnCategory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown custom column modularization type &apos;{0}&apos;..
        /// </summary>
        internal static string EXP_UnknownCustomColumnModularizationType {
            get {
                return ResourceManager.GetString("EXP_UnknownCustomColumnModularizationType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown custom column type &apos;{0}&apos;..
        /// </summary>
        internal static string EXP_UnknownCustomColumnType {
            get {
                return ResourceManager.GetString("EXP_UnknownCustomColumnType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown output type..
        /// </summary>
        internal static string EXP_UnknownOutputType {
            get {
                return ResourceManager.GetString("EXP_UnknownOutputType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown permission attribute &apos;{0}&apos;..
        /// </summary>
        internal static string EXP_UnknownPermissionAttribute {
            get {
                return ResourceManager.GetString("EXP_UnknownPermissionAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown platform enumeration &apos;{0}&apos; encountered..
        /// </summary>
        internal static string EXP_UnknownPlatformEnum {
            get {
                return ResourceManager.GetString("EXP_UnknownPlatformEnum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown sequence table..
        /// </summary>
        internal static string EXP_UnknowSequenceTable {
            get {
                return ResourceManager.GetString("EXP_UnknowSequenceTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The table {0} is not supported..
        /// </summary>
        internal static string EXP_UnsupportedTable {
            get {
                return ResourceManager.GetString("EXP_UnsupportedTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to XmlElement with name {0} does not have a corresponding ISchemaElement..
        /// </summary>
        internal static string EXP_XmlElementDoesnotHaveISchemaElement {
            get {
                return ResourceManager.GetString("EXP_XmlElementDoesnotHaveISchemaElement", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}({1}).
        /// </summary>
        internal static string Format_FirstLineNumber {
            get {
                return ResourceManager.GetString("Format_FirstLineNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}.
        /// </summary>
        internal static string Format_InfoMessage {
            get {
                return ResourceManager.GetString("Format_InfoMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}: line {1}.
        /// </summary>
        internal static string Format_LineNumber {
            get {
                return ResourceManager.GetString("Format_LineNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} : {1} {2}{3:0000} : {4}.
        /// </summary>
        internal static string Format_NonInfoMessage {
            get {
                return ResourceManager.GetString("Format_NonInfoMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Source trace:{0}.
        /// </summary>
        internal static string INF_SourceTrace {
            get {
                return ResourceManager.GetString("INF_SourceTrace", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to at {0}{1}.
        /// </summary>
        internal static string INF_SourceTraceLocation {
            get {
                return ResourceManager.GetString("INF_SourceTraceLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Source trace unavailable.{0}.
        /// </summary>
        internal static string INF_SourceTraceUnavailable {
            get {
                return ResourceManager.GetString("INF_SourceTraceUnavailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to error.
        /// </summary>
        internal static string MessageType_Error {
            get {
                return ResourceManager.GetString("MessageType_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to warning.
        /// </summary>
        internal static string MessageType_Warning {
            get {
                return ResourceManager.GetString("MessageType_Warning", resourceCulture);
            }
        }
    }
}
