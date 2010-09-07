﻿//-------------------------------------------------------------------------------------------------
// <copyright file="EventArgs.cs" company="Microsoft">
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
// Base class for EventArgs classes that must return a value.
// </summary>
//-------------------------------------------------------------------------------------------------

namespace Microsoft.Tools.WindowsInstallerXml.Bootstrapper
{
    using System;

    /// <summary>
    /// Base class for <see cref="EventArgs"/> classes that must return a value.
    /// </summary>
    [Serializable]
    public abstract class ResultEventArgs : EventArgs
    {
        private Result result;

        /// <summary>
        /// Creates a new instance of the <see cref="ResultEventArgs"/> class.
        /// </summary>
        public ResultEventArgs()
        {
            this.result = Result.Ok;
        }

        /// <summary>
        /// Gets or sets the <see cref="Result"/> of the operation. This is passed back to the engine.
        /// </summary>
        public Result Result
        {
            get { return this.result; }
            set { this.result = value; }
        }
    }

    /// <summary>
    /// Base class for <see cref="EventArgs"/> classes that receive status from the engine.
    /// </summary>
    [Serializable]
    public abstract class StatusEventArgs : EventArgs
    {
        private int status;

        /// <summary>
        /// Creates a new instance of the <see cref="StatusEventArgs"/> class.
        /// </summary>
        /// <param name="status">The return code of the operation.</param>
        public StatusEventArgs(int status)
        {
            this.status = status;
        }

        /// <summary>
        /// Gets the return code of the operation.
        /// </summary>
        public int Status
        {
            get { return this.status; }
        }
    }

    /// <summary>
    /// Base class for <see cref="EventArgs"/> classes that receive status from the engine and return a result.
    /// </summary>
    [Serializable]
    public abstract class ResultStatusEventArgs : ResultEventArgs
    {
        private int status;

        /// <summary>
        /// Creates a new instance of the <see cref="resultStatusEventArgs"/> class.
        /// </summary>
        /// <param name="status">The return code of the operation.</param>
        public ResultStatusEventArgs(int status)
        {
            this.status = status;
        }

        /// <summary>
        /// Gets the return code of the operation.
        /// </summary>
        public int Status
        {
            get { return this.status; }
        }
    }

    /// <summary>
    /// Additional arguments used when startup has begun.
    /// </summary>
    [Serializable]
    public class StartupEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="StartupEventArgs"/> class.
        /// </summary>
        public StartupEventArgs()
        {
        }
    }

    /// <summary>
    /// Additional arguments used when shutdown has begun.
    /// </summary>
    [Serializable]
    public class ShutdownEventArgs : ResultEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ShutdownEventArgs"/> class.
        /// </summary>
        public ShutdownEventArgs()
        {
        }
    }

    /// <summary>
    /// Additional arguments used when the overall detection phase has begun.
    /// </summary>
    [Serializable]
    public class DetectBeginEventArgs : ResultEventArgs
    {
        private int packageCount;

        /// <summary>
        /// Creates a new instance of the <see cref="DetectBeginEventArgs"/> class.
        /// </summary>
        /// <param name="packageCount">The number of packages to detect.</param>
        public DetectBeginEventArgs(int packageCount)
        {
            this.packageCount = packageCount;
        }

        /// <summary>
        /// Gets the number of packages to detect.
        /// </summary>
        public int PackageCount
        {
            get { return this.packageCount; }
        }
    }

    /// <summary>
    /// Additional arguments used when the detection for a prior bundle has begun.
    /// </summary>
    [Serializable]
    public class DetectPriorBundleEventArgs : ResultEventArgs
    {
        private string bundleId;

        /// <summary>
        /// Creates a new instance of the <see cref="DetectPriorBundleEventArgs"/> class.
        /// </summary>
        /// <param name="bundleId">The identity of the bundle to detect.</param>
        public DetectPriorBundleEventArgs(string bundleId)
        {
            this.bundleId = bundleId;
        }

        /// <summary>
        /// Gets the identity of the bundle to detect.
        /// </summary>
        public string BundleId
        {
            get { return this.bundleId; }
        }
    }

    /// <summary>
    /// Additional arguments used when a related bundle has been detected for a bundle.
    /// </summary>
    [Serializable]
    public class DetectRelatedBundleEventArgs : ResultEventArgs
    {
        private string productCode;
        private bool perMachine;
        private Version version;
        private RelatedOperation operation;

        /// <summary>
        /// Creates a new instance of the <see cref="DetectRelatedBundleEventArgs"/> class.
        /// </summary>
        /// <param name="productCode">The identity of the related package bundle.</param>
        /// <param name="perMachine">Whether the detected bundle is per machine.</param>
        /// <param name="version">The version of the related bundle detected.</param>
        /// <param name="operation">The operation that will be taken on the detected bundle.</param>
        public DetectRelatedBundleEventArgs(string productCode, bool perMachine, long version, RelatedOperation operation)
        {
            this.productCode = productCode;
            this.perMachine = perMachine;
            this.version = new Version((int)(version >> 48 & 0xFFFF), (int)(version >> 32 & 0xFFFF), (int)(version >> 16 & 0xFFFF), (int)(version & 0xFFFF));
            this.operation = operation;
        }

        /// <summary>
        /// Gets the identity of the related bundle detected.
        /// </summary>
        public string ProductCode
        {
            get { return this.productCode; }
        }

        /// <summary>
        /// Gets whether the detected bundle is per machine.
        /// </summary>
        public bool PerMachine
        {
            get { return this.perMachine; }
        }

        /// <summary>
        /// Gets the version of the related bundle detected.
        /// </summary>
        public Version Version
        {
            get { return this.version; }
        }

        /// <summary>
        /// Gets the operation that will be taken on the detected bundle.
        /// </summary>
        public RelatedOperation Operation
        {
            get { return this.operation; }
        }
    }

    /// <summary>
    /// Additional arguments used when the detection for a specific package has begun.
    /// </summary>
    [Serializable]
    public class DetectPackageBeginEventArgs : ResultEventArgs
    {
        private string packageId;

        /// <summary>
        /// Creates a new instance of the <see cref="DetectPackageBeginEventArgs"/> class.
        /// </summary>
        /// <param name="packageId">The identity of the package to detect.</param>
        public DetectPackageBeginEventArgs(string packageId)
        {
            this.packageId = packageId;
        }

        /// <summary>
        /// Gets the identity of the package to detect.
        /// </summary>
        public string PackageId
        {
            get { return this.packageId; }
        }
    }

    /// <summary>
    /// Additional arguments used when a related MSI package has been detected for a package.
    /// </summary>
    [Serializable]
    public class DetectRelatedMsiPackageEventArgs : ResultEventArgs
    {
        private string productCode;
        private bool perMachine;
        private Version version;
        private RelatedOperation operation;

        /// <summary>
        /// Creates a new instance of the <see cref="DetectRelatedMsiPackageEventArgs"/> class.
        /// </summary>
        /// <param name="productCode">The identity of the related package detected.</param>
        /// <param name="perMachine">Whether the detected package is per machine.</param>
        /// <param name="version">The version of the related package detected.</param>
        /// <param name="operation">The operation that will be taken on the detected package.</param>
        public DetectRelatedMsiPackageEventArgs(string productCode, bool perMachine, long version, RelatedOperation operation)
        {
            this.productCode = productCode;
            this.perMachine = perMachine;
            this.version = new Version((int)(version >> 48 & 0xFFFF), (int)(version >> 32 & 0xFFFF), (int)(version >> 16 & 0xFFFF), (int)(version & 0xFFFF));
            this.operation = operation;
        }

        /// <summary>
        /// Gets the identity of the related package detected.
        /// </summary>
        public string ProductCode
        {
            get { return this.productCode; }
        }

        /// <summary>
        /// Gets whether the detected package is per machine.
        /// </summary>
        public bool PerMachine
        {
            get { return this.perMachine; }
        }

        /// <summary>
        /// Gets the version of the related package detected.
        /// </summary>
        public Version Version
        {
            get { return this.version; }
        }

        /// <summary>
        /// Gets the operation that will be taken on the detected package.
        /// </summary>
        public RelatedOperation Operation
        {
            get { return this.operation; }
        }
    }

    /// <summary>
    /// Additional arguments used when a feature in an MSI package has been detected.
    /// </summary>
    public class DetectMsiFeatureEventArgs : ResultEventArgs
    {
        private string packageId;
        private string featureId;
        private FeatureState state;

        /// <summary>
        /// Creates a new instance of the <see cref="DetectMsiFeatureEventArgs"/> class.
        /// </summary>
        /// <param name="packageId">Detected package identifier.</param>
        /// <param name="featureId">Detected feature identifier.</param>
        /// <param name="state">Feature state detected.</param>
        public DetectMsiFeatureEventArgs(string packageId, string featureId, FeatureState state)
        {
            this.packageId = packageId;
            this.featureId = featureId;
            this.state = state;
        }

        /// <summary>
        /// Gets the identity of the feature's package detected.
        /// </summary>
        public string PackageId
        {
            get { return this.packageId; }
        }

        /// <summary>
        /// Gets the identity of the feature detected.
        /// </summary>
        public string FeatureId
        {
            get { return this.featureId; }
        }

        /// <summary>
        /// Gets the detected feature state.
        /// </summary>
        public FeatureState State
        {
            get { return this.state; }
        }
    }

    /// <summary>
    /// Additional arguments used when the detection for a specific package has completed.
    /// </summary>
    [Serializable]
    public class DetectPackageCompleteEventArgs : StatusEventArgs
    {
        private string packageId;
        private PackageState state;

        /// <summary>
        /// Creates a new instance of the <see cref="DetectPackageCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="packageId">The identity of the package detected.</param>
        /// <param name="status">The return code of the operation.</param>
        /// <param name="state">The state of the specified package.</param>
        public DetectPackageCompleteEventArgs(string packageId, int status, PackageState state)
            : base(status)
        {
            this.packageId = packageId;
            this.state = state;
        }

        /// <summary>
        /// Gets the identity of the package detected.
        /// </summary>
        public string PackageId
        {
            get { return this.packageId; }
        }

        /// <summary>
        /// Gets the state of the specified package.
        /// </summary>
        public PackageState State
        {
            get { return this.state; }
        }
    }

    /// <summary>
    /// Additional arguments used when the detection phase has completed.
    /// </summary>
    [Serializable]
    public class DetectCompleteEventArgs : StatusEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DetectCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="status">The return code of the operation.</param>
        public DetectCompleteEventArgs(int status)
            : base(status)
        {
        }
    }

    /// <summary>
    /// Additional arguments used when the engine has begun planning the installation.
    /// </summary>
    [Serializable]
    public class PlanBeginEventArgs : ResultEventArgs
    {
        private int packageCount;

        /// <summary>
        /// Creates a new instance of the <see cref="PlanBeginEventArgs"/> class.
        /// </summary>
        /// <param name="packageCount">The number of packages to plan for.</param>
        public PlanBeginEventArgs(int packageCount)
        {
            this.packageCount = packageCount;
        }

        /// <summary>
        /// Gets the number of packages to plan for.
        /// </summary>
        public int PackageCount
        {
            get { return this.packageCount; }
        }
    }

    /// <summary>
    /// Additional arguments used when the engine has begun planning for a related bundle.
    /// </summary>
    [Serializable]
    public class PlanRelatedBundleEventArgs : ResultEventArgs
    {
        private string bundleId;
        private RequestState state;

        /// <summary>
        /// Creates a new instance of the <see cref="PlanRelatedBundleEventArgs"/> class.
        /// </summary>
        /// <param name="bundleId">The identity of the bundle to plan for.</param>
        /// <param name="state">The requested state for the bundle.</param>
        public PlanRelatedBundleEventArgs(string bundleId, RequestState state)
        {
            this.bundleId = bundleId;
            this.state = state;
        }

        /// <summary>
        /// Gets the identity of the bundle to plan for.
        /// </summary>
        public string BundleId
        {
            get { return this.bundleId; }
        }

        /// <summary>
        /// Gets the requested state for the bundle.
        /// </summary>
        public RequestState State
        {
            get { return this.state; }
        }
    }

    /// <summary>
    /// Additional arguments used when the engine has begun planning the installation of a specific package.
    /// </summary>
    [Serializable]
    public class PlanPackageBeginEventArgs : ResultEventArgs
    {
        private string packageId;
        private RequestState state;

        /// <summary>
        /// Creates a new instance of the <see cref="PlanPackageBeginEventArgs"/> class.
        /// </summary>
        /// <param name="packageId">The identity of the package to plan for.</param>
        /// <param name="state">The requested state for the package.</param>
        public PlanPackageBeginEventArgs(string packageId, RequestState state)
        {
            this.packageId = packageId;
            this.state = state;
        }

        /// <summary>
        /// Gets the identity of the package to plan for.
        /// </summary>
        public string PackageId
        {
            get { return this.packageId; }
        }

        /// <summary>
        /// Gets or sets the requested state for the package.
        /// </summary>
        public RequestState State
        {
            get { return this.state; }
            set { this.state = value; }
        }
    }

    /// <summary>
    /// Additional arguments used when engine is about to plan a feature in an MSI package.
    /// </summary>
    [Serializable]
    public class PlanMsiFeatureEventArgs : ResultEventArgs
    {
        private string packageId;
        private string featureId;
        private FeatureState state;

        /// <summary>
        /// Creates a new instance of the <see cref="PlanMsiFeatureEventArgs"/> class.
        /// </summary>
        /// <param name="packageId">Package identifier being planned.</param>
        /// <param name="featureId">Feature identifier being planned.</param>
        /// <param name="state">Feature state being planned.</param>
        public PlanMsiFeatureEventArgs(string packageId, string featureId, FeatureState state)
        {
            this.packageId = packageId;
            this.featureId = featureId;
            this.state = state;
        }

        /// <summary>
        /// Gets the identity of the feature's package to plan.
        /// </summary>
        public string PackageId
        {
            get { return this.packageId; }
        }

        /// <summary>
        /// Gets the identity of the feature to plan.
        /// </summary>
        public string FeatureId
        {
            get { return this.featureId; }
        }

        /// <summary>
        /// Gets or sets the feature state to use by planning.
        /// </summary>
        public FeatureState State
        {
            get { return this.state; }
            set { this.state = value; }
        }
    }

    /// <summary>
    /// Additional arguments used when then engine has completed planning the installation of a specific package.
    /// </summary>
    [Serializable]
    public class PlanPackageCompleteEventArgs : StatusEventArgs
    {
        private string packageId;
        private PackageState state;
        private RequestState requested;
        private ActionState execute;
        private ActionState rollback;

        /// <summary>
        /// Creates a new instance of the <see cref="PlanPackageCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="packageId">The identity of the package planned for.</param>
        /// <param name="status">The return code of the operation.</param>
        /// <param name="state">The current state of the package.</param>
        /// <param name="requested">The requested state for the package</param>
        /// <param name="execute">The execution action to take.</param>
        /// <param name="rollback">The rollback action to take.</param>
        public PlanPackageCompleteEventArgs(string packageId, int status, PackageState state, RequestState requested, ActionState execute, ActionState rollback)
            : base(status)
        {
            this.packageId = packageId;
            this.state = state;
            this.requested = requested;
            this.execute = execute;
            this.rollback = rollback;
        }

        /// <summary>
        /// Gets the identity of the package planned for.
        /// </summary>
        public string PackageId
        {
            get { return this.packageId; }
        }

        /// <summary>
        /// Gets the current state of the package.
        /// </summary>
        public PackageState State
        {
            get { return this.state; }
        }

        /// <summary>
        /// Gets the requested state for the package.
        /// </summary>
        public RequestState Requested
        {
            get { return this.requested; }
        }

        /// <summary>
        /// Gets the execution action to take.
        /// </summary>
        public ActionState Execute
        {
            get { return this.execute; }
        }

        /// <summary>
        /// Gets the rollback action to take.
        /// </summary>
        public ActionState Rollback
        {
            get { return this.rollback; }
        }
    }

    /// <summary>
    /// Additional arguments used when the engine has completed planning the installation.
    /// </summary>
    [Serializable]
    public class PlanCompleteEventArgs : StatusEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PlanCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="status">The return code of the operation.</param>
        public PlanCompleteEventArgs(int status)
            : base(status)
        {
        }
    }

    /// <summary>
    /// Additional arguments used when the engine has begun installing the bundle.
    /// </summary>
    [Serializable]
    public class ApplyBeginEventArgs : ResultEventArgs
    {
    }

    /// <summary>
    /// Additional arguments used when the engine has begun registering the location and visibility of the bundle.
    /// </summary>
    [Serializable]
    public class RegisterBeginEventArgs : ResultEventArgs
    {
    }

    /// <summary>
    /// Additional arguments used when the engine has completed registering the location and visilibity of the bundle.
    /// </summary>
    [Serializable]
    public class RegisterCompleteEventArgs : StatusEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="RegisterCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="status">The return code of the operation.</param>
        public RegisterCompleteEventArgs(int status)
            : base(status)
        {
        }
    }

    /// <summary>
    /// Additional arguments used when the engine has begun removing the registration for the location and visibility of the bundle.
    /// </summary>
    [Serializable]
    public class UnregisterBeginEventArgs : EventArgs
    {
    }

    /// <summary>
    /// Additional arguments used when the engine has completed removing the registration for the location and visibility of the bundle.
    /// </summary>
    [Serializable]
    public class UnregisterCompleteEventArgs : StatusEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="UnregisterCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="status">The return code of the operation.</param>
        public UnregisterCompleteEventArgs(int status)
            : base(status)
        {
        }
    }

    /// <summary>
    /// Additional arguments used when the engine has begun caching the installation sources.
    /// </summary>
    [Serializable]
    public class CacheBeginEventArgs : ResultEventArgs
    {
    }

    /// <summary>
    /// Additional arguments used when the engine begins to acquire containers or payloads.
    /// </summary>
    [Serializable]
    public class CacheAcquireBeginEventArgs : ResultEventArgs
    {
        private string packageOrContainerId;
        private string payloadId;
        private CacheOperation operation;
        private string source;

        /// <summary>
        /// Creates a new instance of the <see cref="CacheAcquireBeginEventArgs"/> class.
        /// </summary>
        public CacheAcquireBeginEventArgs(string packageOrContainerId, string payloadId, CacheOperation operation, string source)
        {
            this.packageOrContainerId = packageOrContainerId;
            this.payloadId = payloadId;
            this.operation = operation;
            this.source = source;
        }

        /// <summary>
        /// Gets the identifier of the container or package.
        /// </summary>
        public string PackageOrContainerId
        {
            get { return this.packageOrContainerId; }
        }

        /// <summary>
        /// Gets the identifier of the payload (if acquiring a payload).
        /// </summary>
        public string PayloadId
        {
            get { return this.payloadId; }
        }

        /// <summary>
        /// Gets the cache acquire operation.
        /// </summary>
        public CacheOperation Operation
        {
            get { return this.operation; }
        }

        /// <summary>
        /// Gets the source of the container or payload.
        /// </summary>
        public string Source
        {
            get { return this.source; }
        }
    }

    /// <summary>
    /// Additional arguments used when the engine acquires some part of a container or payload.
    /// </summary>
    [Serializable]
    public class CacheAcquireProgressEventArgs : ResultEventArgs
    {
        private string packageOrContainerId;
        private string payloadId;
        private long progress;
        private long total;
        private int overallPercentage;

        /// <summary>
        /// Creates a new instance of the <see cref="CacheAcquireBeginEventArgs"/> class.
        /// </summary>
        public CacheAcquireProgressEventArgs(string packageOrContainerId, string payloadId, long progress, long total, int overallPercentage)
        {
            this.packageOrContainerId = packageOrContainerId;
            this.payloadId = payloadId;
            this.progress = progress;
            this.total = total;
            this.overallPercentage = overallPercentage;
        }

        /// <summary>
        /// Gets the identifier of the container or package.
        /// </summary>
        public string PackageOrContainerId
        {
            get { return this.packageOrContainerId; }
        }

        /// <summary>
        /// Gets the identifier of the payload (if acquiring a payload).
        /// </summary>
        public string PayloadId
        {
            get { return this.payloadId; }
        }

        /// <summary>
        /// Gets the number of bytes cached thus far.
        /// </summary>
        public long Progress
        {
            get { return this.progress; }
        }

        /// <summary>
        /// Gets the total bytes to cache.
        /// </summary>
        public long Total
        {
            get { return this.total; }
        }

        /// <summary>
        /// Gets the overall percentage of progress of caching.
        /// </summary>
        public int OverallPercentage
        {
            get { return this.overallPercentage; }
        }
    }

    /// <summary>
    /// Additional arguments used when the engine complets the acquisition of a container or payload.
    /// </summary>
    [Serializable]
    public class CacheAcquireCompleteEventArgs : ResultStatusEventArgs
    {
        private string packageOrContainerId;
        private string payloadId;

        /// <summary>
        /// Creates a new instance of the <see cref="CacheAcquireCompleteEventArgs"/> class.
        /// </summary>
        public CacheAcquireCompleteEventArgs(string packageOrContainerId, string payloadId, int status)
            : base(status)
        {
            this.packageOrContainerId = packageOrContainerId;
            this.payloadId = payloadId;
        }

        /// <summary>
        /// Gets the identifier of the container or package.
        /// </summary>
        public string PackageOrContainerId
        {
            get { return this.packageOrContainerId; }
        }

        /// <summary>
        /// Gets the identifier of the payload (if acquiring a payload).
        /// </summary>
        public string PayloadId
        {
            get { return this.payloadId; }
        }
    }

    /// <summary>
    /// Additional arguments used after the engine has cached the installation sources.
    /// </summary>
    [Serializable]
    public class CacheCompleteEventArgs : StatusEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CacheCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="status">The return code of the operation.</param>
        public CacheCompleteEventArgs(int status)
            : base(status)
        {
        }
    }

    /// <summary>
    /// Additional arguments used when the engine has begun installing packages.
    /// </summary>
    [Serializable]
    public class ExecuteBeginEventArgs : ResultEventArgs
    {
        private int packageCount;

        /// <summary>
        /// Creates a new instance of the <see cref="ExecuteBeginEventArgs"/> class.
        /// </summary>
        /// <param name="packageCount">The number of packages to act on.</param>
        public ExecuteBeginEventArgs(int packageCount)
        {
            this.packageCount = packageCount;
        }

        /// <summary>
        /// Gets the number of packages to act on.
        /// </summary>
        public int PackageCount
        {
            get { return this.packageCount; }
        }
    }

    /// <summary>
    /// Additional arguments used when the engine has begun installing a specific package.
    /// </summary>
    [Serializable]
    public class ExecutePackageBeginEventArgs : ResultEventArgs
    {
        private string packageId;
        private bool shouldExecute;

        /// <summary>
        /// Creates a new instance of the <see cref="ExecutePackageBeginEventArgs"/> class.
        /// </summary>
        /// <param name="packageId">The identity of the package to act on.</param>
        /// <param name="shouldExecute">Whether the package should really be acted on.</param>
        public ExecutePackageBeginEventArgs(string packageId, bool shouldExecute)
        {
            this.packageId = packageId;
            this.shouldExecute = shouldExecute;
        }

        /// <summary>
        /// Gets the identity of the package to act on.
        /// </summary>
        public string PackageId
        {
            get { return this.packageId; }
        }

        /// <summary>
        /// Gets whether the package should really be acted on.
        /// </summary>
        public bool ShouldExecute
        {
            get { return this.shouldExecute; }
        }
    }

    /// <summary>
    /// Additional arguments used when the engine has encountered an error.
    /// </summary>
    [Serializable]
    public class ErrorEventArgs : ResultEventArgs
    {
        private string packageId;
        private int errorCode;
        private string errorMessage;
        private int uiHint;

        /// <summary>
        /// Creates a new instance of the <see cref="ErrorEventArgs"/> class.
        /// </summary>
        /// <param name="packageId">The identity of the package that yielded the error.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="uiHint">Recommended display flags for an error dialog.</param>
        public ErrorEventArgs(string packageId, int errorCode, string errorMessage, int uiHint)
        {
            this.packageId = packageId;
            this.errorCode = errorCode;
            this.errorMessage = errorMessage;
            this.uiHint = uiHint;
        }

        /// <summary>
        /// Gets the identity of the package that yielded the error.
        /// </summary>
        public string PackageId
        {
            get { return this.packageId; }
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public int ErrorCode
        {
            get { return this.errorCode; }
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage
        {
            get { return this.errorMessage; }
        }

        /// <summary>
        /// Gets the recommended display flags for an error dialog.
        /// </summary>
        public int UIHint
        {
            get { return this.uiHint; }
        }
    }

    /// <summary>
    /// Additional arguments used when the engine has changed progress for the bundle installation.
    /// </summary>
    [Serializable]
    public class ProgressEventArgs : ResultEventArgs
    {
        private int progressPercentage;
        private int overallPercentage;

        /// <summary>
        /// Creates an new instance of the <see cref="ProgressEventArgs"/> class.
        /// </summary>
        /// <param name="progressPercentage">The percentage from 0 to 100 completed for a package.</param>
        /// <param name="overallPercentage">The percentage from 0 to 100 completed for the bundle.</param>
        public ProgressEventArgs(int progressPercentage, int overallPercentage)
        {
            this.progressPercentage = progressPercentage;
            this.overallPercentage = overallPercentage;
        }

        /// <summary>
        /// Gets the percentage from 0 to 100 completed for a package.
        /// </summary>
        public int ProgressPercentage
        {
            get { return this.progressPercentage; }
        }

        /// <summary>
        /// Gets the percentage from 0 to 100 completed for the bundle.
        /// </summary>
        public int OverallPercentage
        {
            get { return this.overallPercentage; }
        }
    }

    /// <summary>
    /// Additional arguments used when Windows Installer sends an installation message.
    /// </summary>
    [Serializable]
    public class ExecuteMsiMessageEventArgs : ResultEventArgs
    {
        private string packageId;
        private InstallMessage messageType;
        private int displayParameters;
        private string message;

        /// <summary>
        /// Creates a new instance of the <see cref="ExecuteMsiMessageEventArgs"/> class.
        /// </summary>
        /// <param name="packageId">The identity of the package that yielded this message.</param>
        /// <param name="messageType">The type of this message.</param>
        /// <param name="displayParameters">Recommended display flags for this message.</param>
        /// <param name="message">The message.</param>
        public ExecuteMsiMessageEventArgs(string packageId, InstallMessage messageType, int displayParameters, string message)
        {
            this.packageId = packageId;
            this.messageType = messageType;
            this.displayParameters = displayParameters;
            this.message = message;
        }

        /// <summary>
        /// Gets the identity of the package that yielded this message.
        /// </summary>
        public string PackageId
        {
            get { return this.packageId; }
        }

        /// <summary>
        /// Gets the type of this message.
        /// </summary>
        public InstallMessage MessageType
        {
            get { return this.messageType; }
        }

        /// <summary>
        /// Gets the recommended display flags for this message.
        /// </summary>
        public int DisplayParameters
        {
            get { return this.displayParameters; }
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message
        {
            get { return this.message; }
        }
    }

    /// <summary>
    /// Additional arugments used when Windows Installer sends a file in use installation message.
    /// </summary>
    [Serializable]
    public class ExecuteMsiFilesInUseEventArgs : ResultEventArgs
    {
        private string packageId;
        private string[] files;

        /// <summary>
        /// Creates a new instance of the <see cref="ExecuteMsiFilesInUseEventArgs"/> class.
        /// </summary>
        /// <param name="packageId">The identity of the package that yielded the files in use message.</param>
        /// <param name="files">The list of files in use.</param>
        public ExecuteMsiFilesInUseEventArgs(string packageId, string[] files)
        {
            this.packageId = packageId;
            this.files = files;
        }

        /// <summary>
        /// Gets the identity of the package that yielded the files in use message.
        /// </summary>
        public string PackageId
        {
            get { return this.packageId; }
        }

        /// <summary>
        /// Gets the list of files in use.
        /// </summary>
        public string[] Files
        {
            get { return this.files; }
        }
    }

    /// <summary>
    /// Additional arguments used when the engine has completed installing a specific package.
    /// </summary>
    [Serializable]
    public class ExecutePackageCompleteEventArgs : ResultStatusEventArgs
    {
        private string packageId;
        private ApplyRestart restart;

        /// <summary>
        /// Creates a new instance of the <see cref="ExecutePackageCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="packageId">The identity of the packaged that was acted on.</param>
        /// <param name="status">The return code of the operation.</param>
        public ExecutePackageCompleteEventArgs(string packageId, int status, ApplyRestart restart)
            : base(status)
        {
            this.packageId = packageId;
            this.restart = restart;
        }

        /// <summary>
        /// Gets the identity of the package that was acted on.
        /// </summary>
        public string PackageId
        {
            get { return this.packageId; }
        }

        /// <summary>
        /// Gets the package restart state after being applied.
        /// </summary>
        public ApplyRestart Restart
        {
            get { return this.restart; }
        }
    }

    /// <summary>
    /// Additional arguments used when the engine has completed installing packages.
    /// </summary>
    [Serializable]
    public class ExecuteCompleteEventArgs : StatusEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ExecuteCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="status">The return code of the operation.</param>
        public ExecuteCompleteEventArgs(int status)
            : base(status)
        {
        }
    }

    /// <summary>
    /// Additional arguments used by the engine to request a restart now or inform the user a manual restart is required later.
    /// </summary>
    [Serializable]
    public class RestartRequiredEventArgs : EventArgs
    {
        private bool restart;

        /// <summary>
        /// Creates a new instance of the <see cref="RestartRequiredEventArgs"/> class.
        /// </summary>
        public RestartRequiredEventArgs()
        {
        }

        /// <summary>
        /// Gets or sets whether the engine should restart now. The default is false.
        /// </summary>
        public bool Restart
        {
            get { return this.restart; }
            set { this.restart = value; }
        }
    }

    /// <summary>
    /// Additional arguments used when the engine has completed installing the bundle.
    /// </summary>
    [Serializable]
    public class ApplyCompleteEventArgs : ResultStatusEventArgs
    {
        private ApplyRestart restart;

        /// <summary>
        /// Creates a new instance of the <see cref="ApplyCompleteEventArgs"/> clas.
        /// </summary>
        /// <param name="status">The return code of the operation.</param>
        public ApplyCompleteEventArgs(int status, ApplyRestart restart)
            : base(status)
        {
            this.restart = restart;
        }

        /// <summary>
        /// Gets the apply restart state when complete.
        /// </summary>
        public ApplyRestart Restart
        {
            get { return this.restart; }
        }
    }

    /// <summary>
    /// Additional arguments used by the engine to allow the user experience to change the source using <see cref="Engine.SetSource"/>.
    /// </summary>
    [Serializable]
    public class ResolveSourceEventArgs : ResultEventArgs
    {
        private string packageOrContainerId;
        private string payloadId;
        private string localSource;
        private string downloadSource;

        /// <summary>
        /// Creates a new instance of the <see cref="ResolveSourceEventArgs"/> class.
        /// </summary>
        /// <param name="packageOrContainerId">The identity of the package or container that requires source.</param>
        /// <param name="payloadId">The identity of the payload that requires source.</param>
        /// <param name="path">The current path used for source resolution.</param>
        /// <param name="downloadSource">Optional URL to download container or payload.</param>
        public ResolveSourceEventArgs(string packageOrContainerId, string payloadId, string localSource, string downloadSource)
        {
            this.packageOrContainerId = packageOrContainerId;
            this.payloadId = payloadId;
            this.localSource = localSource;
            this.downloadSource = downloadSource;
        }

        /// <summary>
        /// Gets the identity of the package or container that requires source.
        /// </summary>
        public string PackageOrContainerId
        {
            get { return this.packageOrContainerId; }
        }

        /// <summary>
        /// Gets the identity of the payload that requires source.
        /// </summary>
        public string PayloadId
        {
            get { return this.payloadId; }
        }

        /// <summary>
        /// Gets the current path used for source resolution.
        /// </summary>
        public string LocalSource
        {
            get { return this.localSource; }
        }

        /// <summary>
        /// Gets the optional URL to download container or payload.
        /// </summary>
        public string DownloadSource
        {
            get { return this.downloadSource; }
        }
    }

    /// <summary>
    /// Additional arguments used by the engine when it has begun caching a specific package.
    /// </summary>
    [Serializable]
    public class CachePackageBeginEventArgs : ResultEventArgs
    {
        private string packageId;
        private int cachePayloads;
        private long packageCacheSize;

        /// <summary>
        /// Creates a new instance of the <see cref="CachePackageBeginEventArgs"/> class.
        /// </summary>
        /// <param name="packageId">The identity of the package that is being cached.</param>
        /// <param name="cachePayloads">Number of payloads to be cached.</param>
        /// <param name="packageCacheSize">The size on disk required by the specific package.</param>
        public CachePackageBeginEventArgs(string packageId, int cachePayloads, long packageCacheSize)
        {
            this.packageId = packageId;
            this.cachePayloads = cachePayloads;
            this.packageCacheSize = packageCacheSize;
        }

        /// <summary>
        /// Gets the identity of the package that is being cached.
        /// </summary>
        public string PackageId
        {
            get { return this.packageId; }
        }

        /// <summary>
        /// Gets number of payloads to be cached.
        /// </summary>
        public long CachePayloads
        {
            get { return this.cachePayloads; }
        }
        /// <summary>
        /// Gets the size on disk required by the specific package.
        /// </summary>
        public long PackageCacheSize
        {
            get { return this.packageCacheSize; }
        }
    }

    /// <summary>
    /// Additional arguments passed by the engine when it has completed caching a specific package.
    /// </summary>
    [Serializable]
    public class CachePackageCompleteEventArgs : StatusEventArgs
    {
        private string packageId;

        /// <summary>
        /// Creates a new instance of the <see cref="CachePackageCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="packageId">The identity of the package that was cached.</param>
        /// <param name="status">The return code of the operation.</param>
        public CachePackageCompleteEventArgs(string packageId, int status)
            : base(status)
        {
            this.packageId = packageId;
        }

        /// <summary>
        /// Gets the identity of the package that was cached.
        /// </summary>
        public string PackageId
        {
            get { return this.packageId; }
        }
    }

    /// <summary>
    /// Additional arguments passed by the engine when it has begun downloading a specific payload.
    /// </summary>
    [Serializable]
    public class DownloadPayloadBeginEventArgs : ResultEventArgs
    {
        private string payloadId;
        private string payloadFileName;

        /// <summary>
        /// Creates a new instance of the <see cref="DownloadPayloadBeginEventArgs"/> class.
        /// </summary>
        /// <param name="payloadId">The identifier of the payload being downloaded.</param>
        /// <param name="payloadFileName">The file name of the payload being downloaded.</param>
        public DownloadPayloadBeginEventArgs(string payloadId, string payloadFileName)
        {
            this.payloadId = payloadId;
            this.payloadFileName = payloadFileName;
        }

        /// <summary>
        /// Gets the identifier of the payload being downloaded.
        /// </summary>
        public string PayloadId
        {
            get { return this.payloadId; }
        }

        /// <summary>
        /// Gets the file name of the payload being downloaded.
        /// </summary>
        public string PayloadFileName
        {
            get { return this.payloadFileName; }
        }
    }

    /// <summary>
    /// Additional arguments passed by the engine when it has completed downloading a specific payload.
    /// </summary>
    [Serializable]
    public class DownloadPayloadCompleteEventArgs : ResultStatusEventArgs
    {
        private string payloadId;
        private string payloadFileName;

        /// <summary>
        /// Creates a new instance of the <see cref="DownloadPayloadCompleteEventArgs"/> class.
        /// </summary>
        /// <param name="payloadId">The identifier of the payload that was downloaded.</param>
        /// <param name="payloadFileName">The file name of the payload that was downloaded.</param>
        /// <param name="status">The return code of the operation.</param>
        public DownloadPayloadCompleteEventArgs(string payloadId, string payloadFileName, int status)
            : base(status)
        {
            this.payloadId = payloadId;
            this.payloadFileName = payloadFileName;
        }

        /// <summary>
        /// Gets the identifier of the payload that was downloaded.
        /// </summary>
        public string PayloadId
        {
            get { return this.payloadId; }
        }

        /// <summary>
        /// Gets the file name of the payload that was downloaded.
        /// </summary>
        public string PayloadFileName
        {
            get { return this.payloadFileName; }
        }
    }

    /// <summary>
    /// Additional arguments passed by the engine while downloading payload.
    /// </summary>
    [Serializable]
    public class DownloadProgressEventArgs : ResultEventArgs
    {
        private int progressPercentage;
        private int overallPercentage;

        /// <summary>
        /// Creates a new instance of the <see cref="DownloadProgressEventArgs"/> class.
        /// </summary>
        /// <param name="progressPercentage">The percentage from 0 to 100 of the download progress for a single payload.</param>
        /// <param name="overallPercentage">The percentage from 0 to 100 of the download progress for all payload.</param>
        public DownloadProgressEventArgs(int progressPercentage, int overallPercentage)
        {
            this.progressPercentage = progressPercentage;
            this.overallPercentage = overallPercentage;
        }

        /// <summary>
        /// Gets the percentage from 0 to 100 of the download progress for a single payload.
        /// </summary>
        public int ProgressPercentage
        {
            get { return this.progressPercentage; }
        }

        /// <summary>
        /// Gets the percentage from 0 to 100 of the download progress for all payload.
        /// </summary>
        public int OverallPercentage
        {
            get { return this.overallPercentage; }
        }
    }

    /// <summary>
    /// Additional arguments passed by the engine while executing on payload.
    /// </summary>
    [Serializable]
    public class ExecuteProgressEventArgs : ResultEventArgs
    {
        private string packageId;
        private int progressPercentage;
        private int overallPercentage;

        /// <summary>
        /// Creates a new instance of the <see cref="ExecuteProgressEventArgs"/> class.
        /// </summary>
        /// <param name="packageId">The identifier of the package being executed.</param>
        /// <param name="progressPercentage">The percentage from 0 to 100 of the execution progress for a single payload.</param>
        /// <param name="overallPercentage">The percentage from 0 to 100 of the execution progress for all payload.</param>
        public ExecuteProgressEventArgs(string packageId, int progressPercentage, int overallPercentage)
        {
            this.packageId = packageId;
            this.progressPercentage = progressPercentage;
            this.overallPercentage = overallPercentage;
        }

        /// <summary>
        /// Gets the identity of the package that was executed.
        /// </summary>
        public string PackageId
        {
            get { return this.packageId; }
        }

        /// <summary>
        /// Gets the percentage from 0 to 100 of the execution progress for a single payload.
        /// </summary>
        public int ProgressPercentage
        {
            get { return this.progressPercentage; }
        }

        /// <summary>
        /// Gets the percentage from 0 to 100 of the execution progress for all payload.
        /// </summary>
        public int OverallPercentage
        {
            get { return this.overallPercentage; }
        }
    }
}
