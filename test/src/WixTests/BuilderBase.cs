﻿//-----------------------------------------------------------------------
// <copyright file="BuilderBase.cs" company="Outercurve Foundation">
//   Copyright (c) 2004, Outercurve Foundation.
//   This software is released under Microsoft Reciprocal License (MS-RL).
//   The license and further copyright text can be found in the file
//   LICENSE.TXT at the root directory of the distribution.
// </copyright>
// <summary>
//     Provides methods for building an MSI.
// </summary>
//-----------------------------------------------------------------------

namespace WixTest.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Base class for builders.
    /// </summary>
    public abstract class BuilderBase<T> where T : BuilderBase<T>
    {
        private static Stack<BuiltItem> BuiltItems = new Stack<BuiltItem>();

        protected WixTests test;

        public BuilderBase(WixTests test) :
            this(test, test.TestContext.TestName)
        {
        }

        public BuilderBase(WixTests test, string name)
        {
            this.test = test;
            this.Name = name;

            this.AdditionalSourceFiles = new string[0];
            this.Extensions = new string[0];
            this.PreprocessorVariables = new Dictionary<string, string>();
            this.BindPaths = new Dictionary<string, string>();
        }

        /// <summary>
        /// Name of the output, defaults to the name of the test.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Primary source file to build, defaults to the TestDataDirectory + Name of the test + ".wxs".
        /// </summary>
        public string SourceFile { get; set; }

        /// <summary>
        /// Optional colleciton of additional source files to build.
        /// </summary>
        public string[] AdditionalSourceFiles { get; set; }

        /// <summary>
        /// Optional collection of extensions needed for the build.
        /// </summary>
        public string[] Extensions { get; set; }

        /// <summary>
        /// Indicates this package is only built, never installed so don't try to clean it up.
        /// </summary>
        /// <remarks>Typically this is set for packages that are the "upgrade target" for a patch/</remarks>
        public bool NeverGetsInstalled { get; set; }

        /// <summary>
        /// Optional key/value colleciton used for preprocessor variables.
        /// </summary>
        public IDictionary<string, string> PreprocessorVariables { get; set; }

        /// <summary>
        /// Optional key/value colleciton used for bindpath values.
        /// </summary>
        public IDictionary<string, string> BindPaths { get; set; }

        /// <summary>
        /// Gets the last built output.
        /// </summary>
        public string Output { get; protected set; }

        /// <summary>
        /// Builds the target.
        /// </summary>
        /// <returns>The path to the built target.</returns>
        public T Build()
        {
            T t = this.BuildItem();
            Assert.IsFalse(String.IsNullOrEmpty(t.Output), "A builder must specify its output.");

            if (!t.NeverGetsInstalled)
            {
                BuiltItems.Push(new BuiltItem() { Builder = this, Path = t.Output, TestName = this.test.TestContext.TestName });
            }

            return t;
        }

        /// <summary>
        /// Removes any built items from the machine.
        /// </summary>
        public static void CleanupByUninstalling()
        {
            while (BuiltItems.Count > 0)
            {
                BuiltItem item = BuiltItems.Pop();
                item.Builder.UninstallItem(item);
            }
        }

        /// <summary>
        /// Override to build an item of type T.
        /// </summary>
        /// <returns></returns>
        protected abstract T BuildItem();

        /// <summary>
        /// Override to uninstalls an individual built item of type T.
        /// </summary>
        /// <param name="item">Built item.</param>
        protected abstract void UninstallItem(BuiltItem item);

        /// <summary>
        /// Private structure that tracks items that are built.
        /// </summary>
        protected struct BuiltItem
        {
            public BuilderBase<T> Builder;
            public string Path;
            public string TestName;
        }
    }
}
