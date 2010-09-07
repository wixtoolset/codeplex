﻿//-------------------------------------------------------------------------------------------------
// <copyright file="BootstrapperApplication.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
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
// <summary>
// The default user experience.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Bootstrapper
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;

    /// <summary>
    /// The default bootstrapper application.
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BootstrapperApplication : MarshalByRefObject, IBootstrapperApplication
    {
        private Engine engine;
        private Command command;

        /// <summary>
        /// Creates a new instance of the <see cref="BootstrapperApplication"/> class.
        /// </summary>
        protected BootstrapperApplication()
        {
        }

        /// <summary>
        /// Fired when the engine is starting up the bootstrapper application.
        /// </summary>
        public event EventHandler<StartupEventArgs> Startup;

        /// <summary>
        /// Fired when the engine is shutting down the bootstrapper application.
        /// </summary>
        public event EventHandler<ShutdownEventArgs> Shutdown;

        /// <summary>
        /// Fired when the overall detection phase has begun.
        /// </summary>
        public event EventHandler<DetectBeginEventArgs> DetectBegin;

        /// <summary>
        /// Fired when the detection for a prior bundle has begun.
        /// </summary>
        public event EventHandler<DetectPriorBundleEventArgs> DetectPriorBundle;

        /// <summary>
        /// Fired when a related bundle has been detected for a bundle.
        /// </summary>
        public event EventHandler<DetectRelatedBundleEventArgs> DetectRelatedBundle;

        /// <summary>
        /// Fired when the detection for a specific package has begun.
        /// </summary>
        public event EventHandler<DetectPackageBeginEventArgs> DetectPackageBegin;

        /// <summary>
        /// Fired when a related MSI package has been detected for a package.
        /// </summary>
        public event EventHandler<DetectRelatedMsiPackageEventArgs> DetectRelatedMsiPackage;

        /// <summary>
        /// Fired when a feature in an MSI package has been detected.
        /// </summary>
        public event EventHandler<DetectMsiFeatureEventArgs> DetectMsiFeature;

        /// <summary>
        /// Fired when the detection for a specific package has completed.
        /// </summary>
        public event EventHandler<DetectPackageCompleteEventArgs> DetectPackageComplete;

        /// <summary>
        /// Fired when the detection phase has completed.
        /// </summary>
        public event EventHandler<DetectCompleteEventArgs> DetectComplete;

        /// <summary>
        /// Fired when the engine has begun planning the installation.
        /// </summary>
        public event EventHandler<PlanBeginEventArgs> PlanBegin;

        /// <summary>
        /// Fired when the engine has begun planning for a related bundle.
        /// </summary>
        public event EventHandler<PlanRelatedBundleEventArgs> PlanRelatedBundle;

        /// <summary>
        /// Fired when the engine has begun planning the installation of a specific package.
        /// </summary>
        public event EventHandler<PlanPackageBeginEventArgs> PlanPackageBegin;

        /// <summary>
        /// Fired when the engine is about to plan a feature in an MSI package.
        /// </summary>
        public event EventHandler<PlanMsiFeatureEventArgs> PlanMsiFeature;

        /// <summary>
        /// Fired when the engine has completed planning the installation of a specific package.
        /// </summary>
        public event EventHandler<PlanPackageCompleteEventArgs> PlanPackageComplete;

        /// <summary>
        /// Fired when the engine has completed planning the installation.
        /// </summary>
        public event EventHandler<PlanCompleteEventArgs> PlanComplete;

        /// <summary>
        /// Fired when the engine has begun installing the bundle.
        /// </summary>
        public event EventHandler<ApplyBeginEventArgs> ApplyBegin;

        /// <summary>
        /// Fired when the engine has begun registering the location and visibility of the bundle.
        /// </summary>
        public event EventHandler<RegisterBeginEventArgs> RegisterBegin;

        /// <summary>
        /// Fired when the engine has completed registering the location and visibility of the bundle.
        /// </summary>
        public event EventHandler<RegisterCompleteEventArgs> RegisterComplete;

        /// <summary>
        /// Fired when the engine has begun removing the registration for the location and visibility of the bundle.
        /// </summary>
        public event EventHandler<UnregisterBeginEventArgs> UnregisterBegin;

        /// <summary>
        /// Fired when the engine has completed removing the registration for the location and visibility of the bundle.
        /// </summary>
        public event EventHandler<UnregisterCompleteEventArgs> UnregisterComplete;

        /// <summary>
        /// Fired when the engine has begun caching the installation sources.
        /// </summary>
        public event EventHandler<CacheBeginEventArgs> CacheBegin;

        /// <summary>
        /// Fired when the engine has begun caching the installation sources.
        /// </summary>
        public event EventHandler<CacheAcquireBeginEventArgs> CacheAcquireBegin;

        /// <summary>
        /// Fired when the engine has progress caching the installation sources.
        /// </summary>
        public event EventHandler<CacheAcquireProgressEventArgs> CacheAcquireProgress;

        /// <summary>
        /// Fired when the engine has complete the caching the installation sources.
        /// </summary>
        public event EventHandler<CacheAcquireCompleteEventArgs> CacheAcquireComplete;

        /// <summary>
        /// Fired after the engine has cached the installation sources.
        /// </summary>
        public event EventHandler<CacheCompleteEventArgs> CacheComplete;

        /// <summary>
        /// Fired when the engine has begun installing packages.
        /// </summary>
        public event EventHandler<ExecuteBeginEventArgs> ExecuteBegin;

        /// <summary>
        /// Fired when the engine has begun installing a specific package.
        /// </summary>
        public event EventHandler<ExecutePackageBeginEventArgs> ExecutePackageBegin;

        /// <summary>
        /// Fired when the engine has encountered an error.
        /// </summary>
        public event EventHandler<ErrorEventArgs> Error;

        /// <summary>
        /// Fired when the engine has changed progress for the bundle installation.
        /// </summary>
        public event EventHandler<ProgressEventArgs> Progress;

        /// <summary>
        /// Fired when Windows Installer sends an installation message.
        /// </summary>
        public event EventHandler<ExecuteMsiMessageEventArgs> ExecuteMsiMessage;

        /// <summary>
        /// Fired when Windows Installer sends a files in use installation message.
        /// </summary>
        public event EventHandler<ExecuteMsiFilesInUseEventArgs> ExecuteMsiFilesInUse;

        /// <summary>
        /// Fired when the engine has completed installing a specific package.
        /// </summary>
        public event EventHandler<ExecutePackageCompleteEventArgs> ExecutePackageComplete;

        /// <summary>
        /// Fired when the engine has completed installing packages.
        /// </summary>
        public event EventHandler<ExecuteCompleteEventArgs> ExecuteComplete;

        /// <summary>
        /// Fired by the engine to request a restart now or inform the user a manual restart is required later.
        /// </summary>
        public event EventHandler<RestartRequiredEventArgs> RestartRequired;

        /// <summary>
        /// Fired when the engine has completed installing the bundle.
        /// </summary>
        public event EventHandler<ApplyCompleteEventArgs> ApplyComplete;

        /// <summary>
        /// Fired by the engine to allow the user experience to change the source using <see cref="Engine.SetLocalSource"/> or <see cref="Engine.SetDownloadSource"/>.
        /// </summary>
        public event EventHandler<ResolveSourceEventArgs> ResolveSource;

        /// <summary>
        /// Fired when the engine has begun caching a specific package.
        /// </summary>
        public event EventHandler<CachePackageBeginEventArgs> CachePackageBegin;

        /// <summary>
        /// Fired when the engine has completed caching a specific package.
        /// </summary>
        public event EventHandler<CachePackageCompleteEventArgs> CachePackageComplete;

        /// <summary>
        /// Fired by the engine when it has begun downloading a specific payload.
        /// </summary>
        public event EventHandler<DownloadPayloadBeginEventArgs> DownloadPayloadBegin;

        /// <summary>
        /// Fired by the engine when it has completed downloading a specific payload.
        /// </summary>
        public event EventHandler<DownloadPayloadCompleteEventArgs> DownloadPayloadComplete;

        /// <summary>
        /// Fired by the engine while downloading payload.
        /// </summary>
        public event EventHandler<DownloadProgressEventArgs> DownloadProgress;

        /// <summary>
        /// Fired by the engine while executing on payload.
        /// </summary>
        public event EventHandler<ExecuteProgressEventArgs> ExecuteProgress;

        /// <summary>
        /// Specifies whether this bootstrapper should run asynchronously. The default is true.
        /// </summary>
        public virtual bool AsyncExecution
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the <see cref="Command"/> information for how the UX should be started.
        /// </summary>
        public Command Command
        {
            get { return this.command; }
            internal set { this.command = value; }
        }

        /// <summary>
        /// Gets the <see cref="Engine"/> for interaction with the Engine.
        /// </summary>
        public Engine Engine
        {
            get { return this.engine; }
            internal set { this.engine = value; }
        }

        /// <summary>
        /// Entry point that is called when the bootstrapper application is ready to run.
        /// </summary>
        protected abstract void Run();

        /// <summary>
        /// Called by the engine on startup of the bootstrapper application.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnStartup(StartupEventArgs args)
        {
            EventHandler<StartupEventArgs> handler = this.Startup;
            if (null != handler)
            {
                handler(this, args);
            }

            if (this.AsyncExecution)
            {
                Thread uiThread = new Thread(this.Run);
                uiThread.Name = "UIThread";
                uiThread.SetApartmentState(ApartmentState.STA);
                uiThread.Start();
            }
            else
            {
                this.Run();
            }
        }

        /// <summary>
        /// Called by the engine to uninitialize the user experience.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnShutdown(ShutdownEventArgs args)
        {
            EventHandler<ShutdownEventArgs> handler = this.Shutdown;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the overall detection phase has begun.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnDetectBegin(DetectBeginEventArgs args)
        {
            EventHandler<DetectBeginEventArgs> handler = this.DetectBegin;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the detection for a prior bundle has begun.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnDetectPriorBundle(DetectPriorBundleEventArgs args)
        {
            EventHandler<DetectPriorBundleEventArgs> handler = this.DetectPriorBundle;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when a related bundle has been detected for a bundle.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnDetectRelatedBundle(DetectRelatedBundleEventArgs args)
        {
            EventHandler<DetectRelatedBundleEventArgs> handler = this.DetectRelatedBundle;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the detection for a specific package has begun.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnDetectPackageBegin(DetectPackageBeginEventArgs args)
        {
            EventHandler<DetectPackageBeginEventArgs> handler = this.DetectPackageBegin;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when a related MSI package has been detected for a package.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnDetectRelatedMsiPackage(DetectRelatedMsiPackageEventArgs args)
        {
            EventHandler<DetectRelatedMsiPackageEventArgs> handler = this.DetectRelatedMsiPackage;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when an MSI feature has been detected for a package.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnDetectMsiFeature(DetectMsiFeatureEventArgs args)
        {
            EventHandler<DetectMsiFeatureEventArgs> handler = this.DetectMsiFeature;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the detection for a specific package has completed.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnDetectPackageComplete(DetectPackageCompleteEventArgs args)
        {
            EventHandler<DetectPackageCompleteEventArgs> handler = this.DetectPackageComplete;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the detection phase has completed.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnDetectComplete(DetectCompleteEventArgs args)
        {
            EventHandler<DetectCompleteEventArgs> handler = this.DetectComplete;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has begun planning the installation.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnPlanBegin(PlanBeginEventArgs args)
        {
            EventHandler<PlanBeginEventArgs> handler = this.PlanBegin;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has begun planning for a prior bundle.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnPlanRelatedBundle(PlanRelatedBundleEventArgs args)
        {
            EventHandler<PlanRelatedBundleEventArgs> handler = this.PlanRelatedBundle;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has begun planning the installation of a specific package.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnPlanPackageBegin(PlanPackageBeginEventArgs args)
        {
            EventHandler<PlanPackageBeginEventArgs> handler = this.PlanPackageBegin;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine is about to plan an MSI feature of a specific package.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnPlanMsiFeature(PlanMsiFeatureEventArgs args)
        {
            EventHandler<PlanMsiFeatureEventArgs> handler = this.PlanMsiFeature;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when then engine has completed planning the installation of a specific package.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnPlanPackageComplete(PlanPackageCompleteEventArgs args)
        {
            EventHandler<PlanPackageCompleteEventArgs> handler = this.PlanPackageComplete;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has completed planning the installation.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnPlanComplete(PlanCompleteEventArgs args)
        {
            EventHandler<PlanCompleteEventArgs> handler = this.PlanComplete;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has begun installing the bundle.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnApplyBegin(ApplyBeginEventArgs args)
        {
            EventHandler<ApplyBeginEventArgs> handler = this.ApplyBegin;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has begun registering the location and visibility of the bundle.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnRegisterBegin(RegisterBeginEventArgs args)
        {
            EventHandler<RegisterBeginEventArgs> handler = this.RegisterBegin;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has completed registering the location and visilibity of the bundle.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnRegisterComplete(RegisterCompleteEventArgs args)
        {
            EventHandler<RegisterCompleteEventArgs> handler = this.RegisterComplete;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has begun removing the registration for the location and visibility of the bundle.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnUnregisterBegin(UnregisterBeginEventArgs args)
        {
            EventHandler<UnregisterBeginEventArgs> handler = this.UnregisterBegin;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has completed removing the registration for the location and visibility of the bundle.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnUnregisterComplete(UnregisterCompleteEventArgs args)
        {
            EventHandler<UnregisterCompleteEventArgs> handler = this.UnregisterComplete;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has begun caching the installation sources.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnCacheBegin(CacheBeginEventArgs args)
        {
            EventHandler<CacheBeginEventArgs> handler = this.CacheBegin;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has begun caching the container or payload.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnCacheAcquireBegin(CacheAcquireBeginEventArgs args)
        {
            EventHandler<CacheAcquireBeginEventArgs> handler = this.CacheAcquireBegin;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has progressed on caching the container or payload.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnCacheAcquireProgress(CacheAcquireProgressEventArgs args)
        {
            EventHandler<CacheAcquireProgressEventArgs> handler = this.CacheAcquireProgress;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has begun caching the container or payload.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnCacheAcquireComplete(CacheAcquireCompleteEventArgs args)
        {
            EventHandler<CacheAcquireCompleteEventArgs> handler = this.CacheAcquireComplete;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called after the engine has cached the installation sources.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnCacheComplete(CacheCompleteEventArgs args)
        {
            EventHandler<CacheCompleteEventArgs> handler = this.CacheComplete;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has begun installing packages.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnExecuteBegin(ExecuteBeginEventArgs args)
        {
            EventHandler<ExecuteBeginEventArgs> handler = this.ExecuteBegin;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has begun installing a specific package.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnExecutePackageBegin(ExecutePackageBeginEventArgs args)
        {
            EventHandler<ExecutePackageBeginEventArgs> handler = this.ExecutePackageBegin;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has encountered an error.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnError(ErrorEventArgs args)
        {
            EventHandler<ErrorEventArgs> handler = this.Error;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has changed progress for the bundle installation.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnProgress(ProgressEventArgs args)
        {
            EventHandler<ProgressEventArgs> handler = this.Progress;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when Windows Installer sends an installation message.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnExecuteMsiMessage(ExecuteMsiMessageEventArgs args)
        {
            EventHandler<ExecuteMsiMessageEventArgs> handler = this.ExecuteMsiMessage;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when Windows Installer sends a file in use installation message.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnExecuteMsiFilesInUse(ExecuteMsiFilesInUseEventArgs args)
        {
            EventHandler<ExecuteMsiFilesInUseEventArgs> handler = this.ExecuteMsiFilesInUse;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has completed installing a specific package.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnExecutePackageComplete(ExecutePackageCompleteEventArgs args)
        {
            EventHandler<ExecutePackageCompleteEventArgs> handler = this.ExecutePackageComplete;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has completed installing packages.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnExecuteComplete(ExecuteCompleteEventArgs args)
        {
            EventHandler<ExecuteCompleteEventArgs> handler = this.ExecuteComplete;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called by the engine to request a restart now or inform the user a manual restart is required later.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnRestartRequired(RestartRequiredEventArgs args)
        {
            EventHandler<RestartRequiredEventArgs> handler = this.RestartRequired;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called when the engine has completed installing the bundle.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnApplyComplete(ApplyCompleteEventArgs args)
        {
            EventHandler<ApplyCompleteEventArgs> handler = this.ApplyComplete;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called by the engine to allow the user experience to change the source using <see cref="Engine.SetSource"/>.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnResolveSource(ResolveSourceEventArgs args)
        {
            EventHandler<ResolveSourceEventArgs> handler = this.ResolveSource;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called by the engine when it has begun caching a specific package.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnCachePackageBegin(CachePackageBeginEventArgs args)
        {
            EventHandler<CachePackageBeginEventArgs> handler = this.CachePackageBegin;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called by the engine when it has completed caching a specific package.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnCachePackageComplete(CachePackageCompleteEventArgs args)
        {
            EventHandler<CachePackageCompleteEventArgs> handler = this.CachePackageComplete;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called by the engine when it has begun downloading a specific payload.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnDownloadPayloadBegin(DownloadPayloadBeginEventArgs args)
        {
            EventHandler<DownloadPayloadBeginEventArgs> handler = this.DownloadPayloadBegin;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called by the engine when it has completed downloading a specific payload.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnDownloadPayloadComplete(DownloadPayloadCompleteEventArgs args)
        {
            EventHandler<DownloadPayloadCompleteEventArgs> handler = this.DownloadPayloadComplete;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called by the engine while downloading payload.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnDownloadProgress(DownloadProgressEventArgs args)
        {
            EventHandler<DownloadProgressEventArgs> handler = this.DownloadProgress;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Called by the engine while executing on payload.
        /// </summary>
        /// <param name="args">Additional arguments for this event.</param>
        protected virtual void OnExecuteProgress(ExecuteProgressEventArgs args)
        {
            EventHandler<ExecuteProgressEventArgs> handler = this.ExecuteProgress;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        #region IBurnUserExperience Members

        void IBootstrapperApplication.OnStartup()
        {
            StartupEventArgs args = new StartupEventArgs();
            this.OnStartup(args);
        }

        Result IBootstrapperApplication.OnShutdown()
        {
            ShutdownEventArgs args = new ShutdownEventArgs();
            this.OnShutdown(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnDetectBegin(int cPackages)
        {
            DetectBeginEventArgs args = new DetectBeginEventArgs(cPackages);
            this.OnDetectBegin(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnDetectRelatedBundle(string wzProductCode, bool fPerMachine, long version, RelatedOperation operation)
        {
            DetectRelatedBundleEventArgs args = new DetectRelatedBundleEventArgs(wzProductCode, fPerMachine, version, operation);
            this.OnDetectRelatedBundle(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnDetectPackageBegin(string wzPackageId)
        {
            DetectPackageBeginEventArgs args = new DetectPackageBeginEventArgs(wzPackageId);
            this.OnDetectPackageBegin(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnDetectRelatedMsiPackage(string wzProductCode, bool fPerMachine, long version, RelatedOperation operation)
        {
            DetectRelatedMsiPackageEventArgs args = new DetectRelatedMsiPackageEventArgs(wzProductCode, fPerMachine, version, operation);
            this.OnDetectRelatedMsiPackage(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnDetectMsiFeature(string wzProductCode, string wzFeatureId, FeatureState state)
        {
            DetectMsiFeatureEventArgs args = new DetectMsiFeatureEventArgs(wzProductCode, wzFeatureId, state);
            this.OnDetectMsiFeature(args);

            return args.Result;
        }

        void IBootstrapperApplication.OnDetectPackageComplete(string wzPackageId, int hrStatus, PackageState state)
        {
            this.OnDetectPackageComplete(new DetectPackageCompleteEventArgs(wzPackageId, hrStatus, state));
        }

        void IBootstrapperApplication.OnDetectComplete(int hrStatus)
        {
            this.OnDetectComplete(new DetectCompleteEventArgs(hrStatus));
        }

        Result IBootstrapperApplication.OnPlanBegin(int cPackages)
        {
            PlanBeginEventArgs args = new PlanBeginEventArgs(cPackages);
            this.OnPlanBegin(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnPlanRelatedBundle(string wzBundleId, ref RequestState pRequestedState)
        {
            PlanRelatedBundleEventArgs args = new PlanRelatedBundleEventArgs(wzBundleId, pRequestedState);
            this.OnPlanRelatedBundle(args);

            pRequestedState = args.State;
            return args.Result;
        }

        Result IBootstrapperApplication.OnPlanPackageBegin(string wzPackageId, ref RequestState pRequestedState)
        {
            PlanPackageBeginEventArgs args = new PlanPackageBeginEventArgs(wzPackageId, pRequestedState);
            this.OnPlanPackageBegin(args);

            pRequestedState = args.State;
            return args.Result;
        }

        Result IBootstrapperApplication.OnPlanMsiFeature(string wzPackageId, string wzFeatureId, ref FeatureState pRequestedState)
        {
            PlanMsiFeatureEventArgs args = new PlanMsiFeatureEventArgs(wzPackageId, wzFeatureId, pRequestedState);
            this.OnPlanMsiFeature(args);

            pRequestedState = args.State;
            return args.Result;
        }

        void IBootstrapperApplication.OnPlanPackageComplete(string wzPackageId, int hrStatus, PackageState state, RequestState requested, ActionState execute, ActionState rollback)
        {
            this.OnPlanPackageComplete(new PlanPackageCompleteEventArgs(wzPackageId, hrStatus, state, requested, execute, rollback));
        }

        void IBootstrapperApplication.OnPlanComplete(int hrStatus)
        {
            this.OnPlanComplete(new PlanCompleteEventArgs(hrStatus));
        }

        Result IBootstrapperApplication.OnApplyBegin()
        {
            ApplyBeginEventArgs args = new ApplyBeginEventArgs();
            this.OnApplyBegin(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnRegisterBegin()
        {
            RegisterBeginEventArgs args = new RegisterBeginEventArgs();
            this.OnRegisterBegin(args);

            return args.Result;
        }

        void IBootstrapperApplication.OnRegisterComplete(int hrStatus)
        {
            this.OnRegisterComplete(new RegisterCompleteEventArgs(hrStatus));
        }

        void IBootstrapperApplication.OnUnregisterBegin()
        {
            this.OnUnregisterBegin(new UnregisterBeginEventArgs());
        }

        void IBootstrapperApplication.OnUnregisterComplete(int hrStatus)
        {
            this.OnUnregisterComplete(new UnregisterCompleteEventArgs(hrStatus));
        }

        Result IBootstrapperApplication.OnCacheBegin()
        {
            CacheBeginEventArgs args = new CacheBeginEventArgs();
            this.OnCacheBegin(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnCacheAcquireBegin(string wzPackageOrContainerId, string wzPayloadId, CacheOperation operation, string wzSource)
        {
            CacheAcquireBeginEventArgs args = new CacheAcquireBeginEventArgs(wzPackageOrContainerId, wzPayloadId, operation, wzSource);
            this.OnCacheAcquireBegin(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnCacheAcquireProgress(string wzPackageOrContainerId, string wzPayloadId, long dw64Progress, long dw64Total, int dwOverallPercentage)
        {
            CacheAcquireProgressEventArgs args = new CacheAcquireProgressEventArgs(wzPackageOrContainerId, wzPayloadId, dw64Progress, dw64Total, dwOverallPercentage);
            this.OnCacheAcquireProgress(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnCacheAcquireComplete(string wzPackageOrContainerId, string wzPayloadId, int hrStatus)
        {
            CacheAcquireCompleteEventArgs args = new CacheAcquireCompleteEventArgs(wzPackageOrContainerId, wzPayloadId, hrStatus);
            this.OnCacheAcquireComplete(args);

            return args.Result;
        }

        void IBootstrapperApplication.OnCacheComplete(int hrStatus)
        {
            this.OnCacheComplete(new CacheCompleteEventArgs(hrStatus));
        }

        Result IBootstrapperApplication.OnExecuteBegin(int cExecutingPackages)
        {
            ExecuteBeginEventArgs args = new ExecuteBeginEventArgs(cExecutingPackages);
            this.OnExecuteBegin(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnExecutePackageBegin(string wzPackageId, bool fExecute)
        {
            ExecutePackageBeginEventArgs args = new ExecutePackageBeginEventArgs(wzPackageId, fExecute);
            this.OnExecutePackageBegin(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnError(string wzPackageId, int dwCode, string wzError, int dwUIHint)
        {
            ErrorEventArgs args = new ErrorEventArgs(wzPackageId, dwCode, wzError, dwUIHint);
            this.OnError(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnProgress(int dwProgressPercentage, int dwOverallPercentage)
        {
            ProgressEventArgs args = new ProgressEventArgs(dwProgressPercentage, dwOverallPercentage);
            this.OnProgress(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnExecuteMsiMessage(string wzPackageId, InstallMessage mt, int uiFlags, string wzMessage)
        {
            ExecuteMsiMessageEventArgs args = new ExecuteMsiMessageEventArgs(wzPackageId, mt, uiFlags, wzMessage);
            this.OnExecuteMsiMessage(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnExecuteMsiFilesInUse(string wzPackageId, int cFiles, string[] rgwzFiles)
        {
            ExecuteMsiFilesInUseEventArgs args = new ExecuteMsiFilesInUseEventArgs(wzPackageId, rgwzFiles);
            this.OnExecuteMsiFilesInUse(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnExecutePackageComplete(string wzPackageId, int hrExitCode, ApplyRestart restart)
        {
            ExecutePackageCompleteEventArgs args = new ExecutePackageCompleteEventArgs(wzPackageId, hrExitCode, restart);
            this.OnExecutePackageComplete(args);

            return args.Result;
        }

        void IBootstrapperApplication.OnExecuteComplete(int hrStatus)
        {
            this.OnExecuteComplete(new ExecuteCompleteEventArgs(hrStatus));
        }

        Result IBootstrapperApplication.OnApplyComplete(int hrStatus, ApplyRestart restart)
        {
            ApplyCompleteEventArgs args = new ApplyCompleteEventArgs(hrStatus, restart);
            this.OnApplyComplete(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnCachePackageBegin(string wzPackageId, int cCachePayloads, long dw64PackageCacheSize)
        {
            CachePackageBeginEventArgs args = new CachePackageBeginEventArgs(wzPackageId, cCachePayloads, dw64PackageCacheSize);
            this.OnCachePackageBegin(args);

            return args.Result;
        }

        void IBootstrapperApplication.OnCachePackageComplete(string wzPackageId, int hrStatus)
        {
            this.OnCachePackageComplete(new CachePackageCompleteEventArgs(wzPackageId, hrStatus));
        }

        Result IBootstrapperApplication.OnExecuteProgress(string wzPackageId, int dwProgressPercentage, int dwOverallPercentage)
        {
            ExecuteProgressEventArgs args = new ExecuteProgressEventArgs(wzPackageId, dwProgressPercentage, dwOverallPercentage);
            this.OnExecuteProgress(args);

            return args.Result;
        }

        Result IBootstrapperApplication.OnResolveSource(string wzPackageOrContainerId, string wzPayloadId, string wzLocalSource, string wzDownloadSource)
        {
            ResolveSourceEventArgs args = new ResolveSourceEventArgs(wzPackageOrContainerId, wzPayloadId, wzLocalSource, wzDownloadSource);
            this.OnResolveSource(args);

            return args.Result;
        }

        #endregion
    }
}
