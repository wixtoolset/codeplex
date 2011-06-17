//-------------------------------------------------------------------------------------------------
// <copyright file="BalBaseBootstrapperApplication.h" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
//    
//    The use and distribution terms for this software are covered by the
//    Common Public License 1.0 (http://opensource.org/licenses/cpl1.0.php)
//    which can be found in the file CPL.TXT at the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by
//    the terms of this license.
//    
//    You must not remove this notice, or any other, from this software.
// </copyright>
//-------------------------------------------------------------------------------------------------

#include <windows.h>
#include <msiquery.h>

#include "IBootstrapperEngine.h"
#include "IBootstrapperApplication.h"

#include "balutil.h"
#include "balretry.h"

class CBalBaseBootstrapperApplication : public IBootstrapperApplication
{
public: // IUnknown
    virtual STDMETHODIMP QueryInterface(
        __in REFIID riid,
        __out LPVOID *ppvObject
        )
    {
        if (!ppvObject)
        {
            return E_INVALIDARG;
        }

        *ppvObject = NULL;

        if (::IsEqualIID(__uuidof(IBootstrapperApplication), riid))
        {
            *ppvObject = static_cast<IBootstrapperApplication*>(this);
        }
        else if (::IsEqualIID(IID_IUnknown, riid))
        {
            *ppvObject = static_cast<IUnknown*>(this);
        }
        else // no interface for requested iid
        {
            return E_NOINTERFACE;
        }

        AddRef();
        return S_OK;
    }

    virtual STDMETHODIMP_(ULONG) AddRef()
    {
        return ::InterlockedIncrement(&this->m_cReferences);
    }

    virtual STDMETHODIMP_(ULONG) Release()
    {
        long l = ::InterlockedDecrement(&this->m_cReferences);
        if (0 < l)
        {
            return l;
        }

        delete this;
        return 0;
    }

public: // IBurnUserExperience
    virtual STDMETHODIMP OnStartup()
    {
        return S_OK;
    }

    virtual STDMETHODIMP_(int) OnShutdown()
    {
        return IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnDetectBegin(
        __in DWORD /*cPackages*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnDetectPriorBundle(
        __in_z LPCWSTR /*wzBundleId*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnDetectPackageBegin(
        __in_z LPCWSTR /*wzPackageId*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnDetectRelatedBundle(
        __in_z LPCWSTR /*wzBundleId*/,
        __in_z LPCWSTR /*wzBundleTag*/,
        __in BOOL /*fPerMachine*/,
        __in DWORD64 /*dw64Version*/,
        __in BOOTSTRAPPER_RELATED_OPERATION operation
        )
    {
        return BOOTSTRAPPER_RELATED_OPERATION_DOWNGRADE == operation || CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnDetectRelatedMsiPackage(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzProductCode*/,
        __in BOOL /*fPerMachine*/,
        __in DWORD64 /*dw64Version*/,
        __in BOOTSTRAPPER_RELATED_OPERATION operation
        ) 
    {
        return BOOTSTRAPPER_RELATED_OPERATION_DOWNGRADE == operation || CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnDetectTargetMsiPackage(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzProductCode*/,
        __in BOOTSTRAPPER_PACKAGE_STATE /*patchState*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnDetectMsiFeature(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzFeatureId*/,
        __in BOOTSTRAPPER_FEATURE_STATE /*state*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(void) OnDetectPackageComplete(
        __in_z LPCWSTR /*wzPackageId*/,
        __in HRESULT /*hrStatus*/,
        __in BOOTSTRAPPER_PACKAGE_STATE /*state*/
        )
    {
    }

    virtual STDMETHODIMP_(void) OnDetectComplete(
        __in HRESULT /*hrStatus*/
        )
    {
    }

    virtual STDMETHODIMP_(int) OnPlanBegin(
        __in DWORD /*cPackages*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnPlanRelatedBundle(
        __in_z LPCWSTR /*wzBundleId*/,
        __inout BOOTSTRAPPER_REQUEST_STATE* /*pRequestedState*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnPlanPackageBegin(
        __in_z LPCWSTR /*wzPackageId*/, 
        __inout BOOTSTRAPPER_REQUEST_STATE* /*pRequestState*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnPlanTargetMsiPackage(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzProductCode*/,
        __inout BOOTSTRAPPER_REQUEST_STATE* /*pRequestedState*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnPlanMsiFeature(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzFeatureId*/,
        __inout BOOTSTRAPPER_FEATURE_STATE* /*pRequestedState*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(void) OnPlanPackageComplete(
        __in_z LPCWSTR /*wzPackageId*/,
        __in HRESULT /*hrStatus*/,
        __in BOOTSTRAPPER_PACKAGE_STATE /*state*/,
        __in BOOTSTRAPPER_REQUEST_STATE /*requested*/,
        __in BOOTSTRAPPER_ACTION_STATE /*execute*/,
        __in BOOTSTRAPPER_ACTION_STATE /*rollback*/
        )
    {
    }

    virtual STDMETHODIMP_(void) OnPlanComplete(
        __in HRESULT /*hrStatus*/
        )
    {
    }

    virtual STDMETHODIMP_(int) OnApplyBegin()
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnElevate()
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnRegisterBegin()
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(void) OnRegisterComplete(
        __in HRESULT /*hrStatus*/
        )
    {
        return;
    }

    virtual STDMETHODIMP_(void) OnUnregisterBegin()
    {
        return;
    }

    virtual STDMETHODIMP_(void) OnUnregisterComplete(
        __in HRESULT /*hrStatus*/
        )
    {
        return;
    }

    virtual STDMETHODIMP_(int) OnApplyComplete(
        __in HRESULT /*hrStatus*/,
        __in BOOTSTRAPPER_APPLY_RESTART restart
        )
    {
        return BOOTSTRAPPER_APPLY_RESTART_REQUIRED == restart ? IDRESTART : CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnCacheBegin()
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnCachePackageBegin(
        __in_z LPCWSTR /*wzPackageId*/,
        __in DWORD /*cCachePayloads*/,
        __in DWORD64 /*dw64PackageCacheSize*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnCacheAcquireBegin(
        __in_z LPCWSTR /*wzPackageOrContainerId*/,
        __in_z_opt LPCWSTR /*wzPayloadId*/,
        __in BOOTSTRAPPER_CACHE_OPERATION /*operation*/,
        __in_z LPCWSTR /*wzSource*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnCacheAcquireProgress(
        __in_z LPCWSTR /*wzPackageOrContainerId*/,
        __in_z_opt LPCWSTR /*wzPayloadId*/,
        __in DWORD64 /*dw64Progress*/,
        __in DWORD64 /*dw64Total*/,
        __in DWORD /*dwOverallPercentage*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnCacheAcquireComplete(
        __in_z LPCWSTR /*wzPackageOrContainerId*/,
        __in_z_opt LPCWSTR /*wzPayloadId*/,
        __in HRESULT /*hrStatus*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnCacheVerifyBegin(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzPayloadId*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnCacheVerifyComplete(
        __in_z LPCWSTR /*wzPackageId*/,
        __in_z LPCWSTR /*wzPayloadId*/,
        __in HRESULT /*hrStatus*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(void) OnCachePackageComplete(
        __in_z LPCWSTR /*wzPackageId*/,
        __in HRESULT /*hrStatus*/
        )
    {
    }

    virtual STDMETHODIMP_(void) OnCacheComplete(
        __in HRESULT /*hrStatus*/
        )
    {
    }

    virtual STDMETHODIMP_(int) OnExecuteBegin(
        __in DWORD /*cExecutingPackages*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnExecutePackageBegin(
        __in_z LPCWSTR wzPackageId,
        __in BOOL fExecute
        )
    {
        // Only track retry on execution (not rollback).
        if (fExecute)
        {
            BalRetryStartPackage(wzPackageId);
        }

        m_fRollingBack = !fExecute;
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnError(
        __in_z LPCWSTR wzPackageId,
        __in DWORD dwCode,
        __in_z LPCWSTR /*wzError*/,
        __in DWORD /*dwUIHint*/
        )
    {
        BalRetryOnError(wzPackageId, dwCode);
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnProgress(
        __in DWORD /*dwProgressPercentage*/,
        __in DWORD /*dwOverallProgressPercentage*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnDownloadPayloadBegin(
        __in_z LPCWSTR /*wzPayloadId*/,
        __in_z LPCWSTR /*wzPayloadFileName*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnDownloadPayloadComplete(
        __in_z LPCWSTR /*wzPayloadId*/,
        __in_z LPCWSTR /*wzPayloadFileName*/,
        __in HRESULT /*hrStatus*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnDownloadProgress(
        __in DWORD /*dwProgressPercentage*/,
        __in DWORD /*dwOverallProgressPercentage*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int)  OnExecuteProgress(
        __in_z LPCWSTR /*wzPackageId*/,
        __in DWORD /*dwProgressPercentage*/,
        __in DWORD /*dwOverallProgressPercentage*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnExecuteMsiMessage(
        __in_z LPCWSTR /*wzPackageId*/,
        __in INSTALLMESSAGE /*mt*/,
        __in UINT /*uiFlags*/,
        __in_z LPCWSTR /*wzMessage*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnExecuteFilesInUse(
        __in_z LPCWSTR /*wzPackageId*/,
        __in DWORD /*cFiles*/,
        __in_ecount_z(cFiles) LPCWSTR* /*rgwzFiles*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

    virtual STDMETHODIMP_(int) OnExecutePackageComplete(
        __in_z LPCWSTR wzPackageId,
        __in HRESULT hrExitCode,
        __in BOOTSTRAPPER_APPLY_RESTART /*restart*/
        )
    {
        return CheckCanceled() ? IDCANCEL : BalRetryEndPackage(wzPackageId, hrExitCode);
    }

    virtual STDMETHODIMP_(void) OnExecuteComplete(
        __in HRESULT /*hrStatus*/
        )
    {
    }

    virtual STDMETHODIMP_(int) OnResolveSource(
        __in_z LPCWSTR /*wzPackageOrContainerId*/,
        __in_z_opt LPCWSTR /*wzPayloadId*/,
        __in_z LPCWSTR /*wzLocalSource*/,
        __in_z_opt LPCWSTR /*wzDownloadSource*/
        )
    {
        return CheckCanceled() ? IDCANCEL : IDNOACTION;
    }

protected:
    //
    // PromptCancel - prompts the user to close (if not forced).
    //
    virtual BOOL PromptCancel(
        __in HWND hWnd,
        __in BOOL fForceCancel,
        __in_z LPCWSTR wzMessage,
        __in_z LPCWSTR wzCaption
        )
    {
        ::EnterCriticalSection(&m_csCanceled);

        // Only prompt the user to close if we have not canceled already.
        if (!m_fCanceled)
        {
            if (fForceCancel)
            {
                m_fCanceled = TRUE;
            }
            else
            {
                m_fCanceled = (IDYES == ::MessageBoxW(hWnd, wzMessage, wzCaption, MB_YESNO | MB_ICONEXCLAMATION));
            }
        }

        ::LeaveCriticalSection(&m_csCanceled);

        return m_fCanceled;
    }

    //
    // CheckCanceled - waits if the cancel dialog is up and checks to see if the user canceled the operation.
    //
    BOOL CheckCanceled()
    {
        ::EnterCriticalSection(&m_csCanceled);
        ::LeaveCriticalSection(&m_csCanceled);
        return m_fRollingBack ? FALSE : m_fCanceled;
    }

    CBalBaseBootstrapperApplication(
        __in IBootstrapperEngine* /*pEngine*/,
        __in BOOTSTRAPPER_RESTART restart,
        __in DWORD dwRetryCount = 0,
        __in DWORD dwRetryTimeout = 1000
        )
    {
        m_cReferences = 1;
        m_restart = restart;

        ::InitializeCriticalSection(&m_csCanceled);
        m_fCanceled = FALSE;
        m_fRollingBack = FALSE;

        BalRetryInitialize(dwRetryCount, dwRetryTimeout);
    }

    virtual ~CBalBaseBootstrapperApplication()
    {
        BalRetryUninitialize();
        ::DeleteCriticalSection(&m_csCanceled);
    }

protected:
    long m_cReferences;
    BOOTSTRAPPER_RESTART m_restart;

    CRITICAL_SECTION m_csCanceled;
    BOOL m_fCanceled;
    BOOL m_fRollingBack;
};