//-------------------------------------------------------------------------------------------------
// <copyright file="mspengine.cpp" company="Microsoft">
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
//
// <summary>
//    Module: MSP Engine
// </summary>
//-------------------------------------------------------------------------------------------------

#include "precomp.h"


// constants


// structs


// internal function declarations

static HRESULT AddDetectedTargetProduct(
    BURN_PACKAGE* pPackage,
    MSIINSTALLCONTEXT context,
    DWORD dwOrder,
    LPCWSTR wzProductCode
    );
static HRESULT PlanTargetProduct(
    __in BOOL fRollback,
    __in BURN_PLAN* pPlan,
    __in BOOTSTRAPPER_ACTION_STATE actionState,
    __in BURN_PACKAGE* pPackage,
    __in BURN_MSPTARGETPRODUCT* pTargetProduct
    );


// function definitions

extern "C" HRESULT MspEngineParsePackageFromXml(
    __in IXMLDOMNode* pixnMspPackage,
    __in BURN_PACKAGE* pPackage
    )
{
    HRESULT hr = S_OK;

    // @PatchCode
    hr = XmlGetAttributeEx(pixnMspPackage, L"PatchCode", &pPackage->Msp.sczPatchCode);
    ExitOnFailure(hr, "Failed to get @PatchCode.");

    // @PatchXml
    hr = XmlGetAttributeEx(pixnMspPackage, L"PatchXml", &pPackage->Msp.sczApplicabilityXml);
    ExitOnFailure(hr, "Failed to get @PatchXml.");

    // Read properties.
    hr = MsiEngineParsePropertiesFromXml(pixnMspPackage, &pPackage->Msp.rgProperties, &pPackage->Msp.cProperties);
    ExitOnFailure(hr, "Failed to parse properties from XML.");

LExit:

    return hr;
}

extern "C" void MspEnginePackageUninitialize(
    __in BURN_PACKAGE* pPackage
    )
{
    ReleaseStr(pPackage->Msp.sczPatchCode);
    ReleaseStr(pPackage->Msp.sczApplicabilityXml);

    // free properties
    if (pPackage->Msp.rgProperties)
    {
        for (DWORD i = 0; i < pPackage->Msp.cProperties; ++i)
        {
            BURN_MSIPROPERTY* pProperty = &pPackage->Msp.rgProperties[i];

            ReleaseStr(pProperty->sczId);
            ReleaseStr(pProperty->sczValue);
            ReleaseStr(pProperty->sczRollbackValue);
        }
        MemFree(pPackage->Msp.rgProperties);
    }

    // free target products
    ReleaseMem(pPackage->Msp.rgTargetProducts);

    // clear struct
    memset(&pPackage->Msp, 0, sizeof(pPackage->Msp));
}

extern "C" HRESULT MspEngineDetectInitialize(
    __in BURN_PACKAGES* pPackages
    )
{
    AssertSz(pPackages->cPatchInfo, "MspEngineDetectInitialize() should only be called if there are MSP packages.");

    static MSIINSTALLCONTEXT rgContexts[] = { MSIINSTALLCONTEXT_USERUNMANAGED, MSIINSTALLCONTEXT_USERMANAGED, MSIINSTALLCONTEXT_MACHINE };

    HRESULT hr = S_OK;
    WCHAR wzProductCode[MAX_GUID_CHARS + 1];

    // Erase any previously detected product information.
    for (DWORD i = 0; i < pPackages->cPatchInfo; ++i)
    {
        BURN_PACKAGE* pPackage = pPackages->rgPatchInfoToPackage[i];
        ReleaseNullMem(pPackage->Msp.rgTargetProducts);
    }

    // Loop through all products on the machine, testing the collective patch applicability
    // against each product in all contexts. Store the result with the appropriate patch package.
    for (DWORD iProduct = 0; SUCCEEDED(hr = WiuEnumProducts(iProduct, wzProductCode)); ++iProduct)
    {
        for (DWORD i = 0; i < countof(rgContexts); ++i)
        {
            hr = WiuDeterminePatchSequence(wzProductCode, NULL, rgContexts[i], pPackages->rgPatchInfo, pPackages->cPatchInfo);
            if (SUCCEEDED(hr))
            {
                for (DWORD iPatchInfo = 0; iPatchInfo < pPackages->cPatchInfo; ++iPatchInfo)
                {
                    if (ERROR_SUCCESS == pPackages->rgPatchInfo[iPatchInfo].uStatus)
                    {
                        BURN_PACKAGE* pPackage = pPackages->rgPatchInfoToPackage[iPatchInfo];

                        // Note that we do add superseded and obsolete MSP packages. Package Detect and Plan will sort them out later.
                        hr = AddDetectedTargetProduct(pPackage, rgContexts[i], pPackages->rgPatchInfo[iPatchInfo].dwOrder, wzProductCode);
                        ExitOnFailure1(hr, "Failed to add target product code to package: %ls", pPackage->sczId);
                    }
                    // TODO: should we log something for this error case?
                }
            }
            // TODO: should we log something for this error case?
        }
    }

    if (E_NOMOREITEMS == hr)
    {
        hr = S_OK;
    }

    ExitOnFailure(hr, "Failed to test patches applicability against all products on the machine.");

LExit:

    return hr;
}

extern "C" HRESULT MspEngineDetectPackage(
    __in BURN_PACKAGE* pPackage,
    __in BURN_USER_EXPERIENCE* pUserExperience
    )
{
    HRESULT hr = S_OK;
    int nResult = IDOK;
    LPWSTR sczState = NULL;

    if (0 == pPackage->Msp.cTargetProductCodes)
    {
        pPackage->currentState = BOOTSTRAPPER_PACKAGE_STATE_ABSENT;
    }
    else
    {
        // Start the package state at the the highest state then loop through all the
        // target product codes and end up setting the current state to the lowest
        // package state applied to the the target proudct codes.
        pPackage->currentState = BOOTSTRAPPER_PACKAGE_STATE_OBSOLETE;

        for (DWORD i = 0; i < pPackage->Msp.cTargetProductCodes; ++i)
        {
            BURN_MSPTARGETPRODUCT* pTargetProduct = pPackage->Msp.rgTargetProducts + i;

            hr = WiuGetPatchInfoEx(pPackage->Msp.sczPatchCode, pTargetProduct->wzTargetProductCode, NULL, pTargetProduct->context, INSTALLPROPERTY_PATCHSTATE, &sczState);
            if (SUCCEEDED(hr))
            {
                switch (*sczState)
                {
                case '1':
                    pTargetProduct->patchPackageState = BOOTSTRAPPER_PACKAGE_STATE_PRESENT;
                    break;

                case '2':
                    pTargetProduct->patchPackageState = BOOTSTRAPPER_PACKAGE_STATE_SUPERSEDED;
                    break;

                case '4':
                    pTargetProduct->patchPackageState = BOOTSTRAPPER_PACKAGE_STATE_OBSOLETE;
                    break;

                default:
                    pTargetProduct->patchPackageState = BOOTSTRAPPER_PACKAGE_STATE_ABSENT;
                    break;
                }
            }
            else if (HRESULT_FROM_WIN32(ERROR_UNKNOWN_PATCH) == hr)
            {
                pTargetProduct->patchPackageState = BOOTSTRAPPER_PACKAGE_STATE_ABSENT;
                hr = S_OK;
            }
            ExitOnFailure2(hr, "Failed to get patch information for patch code: %ls, target product code: %ls", pPackage->Msp.sczPatchCode, pTargetProduct->wzTargetProductCode);

            if (pPackage->currentState > pTargetProduct->patchPackageState)
            {
                pPackage->currentState = pTargetProduct->patchPackageState;
            }

            nResult = pUserExperience->pUserExperience->OnDetectTargetMsiPackage(pPackage->sczId, pTargetProduct->wzTargetProductCode, pTargetProduct->patchPackageState);
            hr = HRESULT_FROM_VIEW(nResult);
            ExitOnRootFailure(hr, "UX aborted detect target MSI package.");
        }
    }

LExit:
    ReleaseStr(sczState);

    return hr;
}

//
// Plan - calculates the execute and rollback state for the requested package state.
//
extern "C" HRESULT MspEnginePlanPackage(
    __in DWORD /*dwPackageSequence*/,
    __in_opt DWORD* /*pdwInsertSequence*/,
    __in BURN_PACKAGE* pPackage,
    __in BURN_PLAN* pPlan,
    __in BURN_LOGGING* /*pLog*/,
    __in BURN_VARIABLES* /*pVariables*/,
    __in_opt HANDLE hCacheEvent,
    __in BOOL fPlanPackageCacheRollback,
    __in BURN_USER_EXPERIENCE* pUserExperience,
    __out BOOTSTRAPPER_ACTION_STATE* pExecuteAction,
    __out BOOTSTRAPPER_ACTION_STATE* pRollbackAction
    )
{
    HRESULT hr = S_OK;
    int nResult = IDNOACTION;

    // TODO: need to handle the case where this patch adds itself to an earlier patch's list of target products. That would
    //       essentially bump this patch earlier in the plan and we need to make sure this patch is downloaded.
    // add wait for cache
    if (hCacheEvent)
    {
        hr = PlanExecuteCacheSyncAndRollback(pPlan, pPackage, hCacheEvent, fPlanPackageCacheRollback);
        ExitOnFailure(hr, "Failed to plan package cache syncpoint");
    }

    for (DWORD i = 0; i < pPackage->Msp.cTargetProductCodes; ++i)
    {
        BURN_MSPTARGETPRODUCT* pTargetProduct = pPackage->Msp.rgTargetProducts + i;

        BOOTSTRAPPER_REQUEST_STATE requested = pPackage->requested;
        BOOTSTRAPPER_ACTION_STATE execute = BOOTSTRAPPER_ACTION_STATE_NONE;
        BOOTSTRAPPER_ACTION_STATE rollback = BOOTSTRAPPER_ACTION_STATE_NONE;

        nResult = pUserExperience->pUserExperience->OnPlanTargetMsiPackage(pPackage->sczId, pTargetProduct->wzTargetProductCode, &requested);
        hr = HRESULT_FROM_VIEW(nResult);
        ExitOnRootFailure(hr, "UX aborted plan target MSI package.");

        // Calculate the execute action.
        switch (pTargetProduct->patchPackageState)
        {
        case BOOTSTRAPPER_PACKAGE_STATE_PRESENT:
            switch (requested)
            {
            case BOOTSTRAPPER_REQUEST_STATE_REPAIR:
                execute = BOOTSTRAPPER_ACTION_STATE_INSTALL;
                break;

            case BOOTSTRAPPER_REQUEST_STATE_ABSENT: __fallthrough;
            case BOOTSTRAPPER_REQUEST_STATE_CACHE:
                execute = pPackage->fUninstallable ? BOOTSTRAPPER_ACTION_STATE_UNINSTALL : BOOTSTRAPPER_ACTION_STATE_NONE;
                break;

            default:
                execute = BOOTSTRAPPER_ACTION_STATE_NONE;
                break;
            }
            break;

        case BOOTSTRAPPER_PACKAGE_STATE_ABSENT:
            switch (requested)
            {
            case BOOTSTRAPPER_REQUEST_STATE_PRESENT: __fallthrough;
            case BOOTSTRAPPER_REQUEST_STATE_REPAIR:
                execute = BOOTSTRAPPER_ACTION_STATE_INSTALL;
                break;

            default:
                execute = BOOTSTRAPPER_ACTION_STATE_NONE;
                break;
            }
            break;
        }

        // Calculate the rollback action.
        switch (BOOTSTRAPPER_PACKAGE_STATE_UNKNOWN != pPackage->expected ? pPackage->expected : pPackage->currentState)
        {
        case BOOTSTRAPPER_PACKAGE_STATE_PRESENT:
            switch (requested)
            {
            case BOOTSTRAPPER_REQUEST_STATE_ABSENT:
                rollback = BOOTSTRAPPER_ACTION_STATE_INSTALL;
                break;

            default:
                rollback = BOOTSTRAPPER_ACTION_STATE_NONE;
                break;
            }
            break;

        case BOOTSTRAPPER_PACKAGE_STATE_ABSENT: __fallthrough;
        case BOOTSTRAPPER_PACKAGE_STATE_CACHED:
            switch (requested)
            {
            case BOOTSTRAPPER_REQUEST_STATE_PRESENT: __fallthrough;
            case BOOTSTRAPPER_REQUEST_STATE_REPAIR:
                rollback = pPackage->fUninstallable ? BOOTSTRAPPER_ACTION_STATE_UNINSTALL : BOOTSTRAPPER_ACTION_STATE_NONE;
                break;

            default:
                rollback = BOOTSTRAPPER_ACTION_STATE_NONE;
                break;
            }
            break;

        default:
            rollback = BOOTSTRAPPER_ACTION_STATE_NONE;
            break;
        }

        if (BOOTSTRAPPER_ACTION_STATE_NONE != execute)
        {
            hr = PlanTargetProduct(FALSE, pPlan, execute, pPackage, pTargetProduct);
            ExitOnFailure(hr, "Failed to plan target product.");
        }

        if (BOOTSTRAPPER_ACTION_STATE_NONE != rollback)
        {
            hr = PlanTargetProduct(TRUE, pPlan, rollback, pPackage, pTargetProduct);
            ExitOnFailure(hr, "Failed to plan rollack target product.");
        }

        // The highest aggregate action state found will be returned.
        if (*pExecuteAction < execute)
        {
            *pExecuteAction = execute;
        }

        if (*pRollbackAction < rollback)
        {
            *pRollbackAction = rollback;
        }
    }

LExit:

    return hr;
}

extern "C" HRESULT MspEngineExecutePackage(
    __in BURN_EXECUTE_ACTION* pExecuteAction,
    __in BURN_VARIABLES* pVariables,
    __in BOOL fRollback,
    __in PFN_MSIEXECUTEMESSAGEHANDLER pfnMessageHandler,
    __in LPVOID pvContext,
    __out BOOTSTRAPPER_APPLY_RESTART* pRestart
    )
{
    HRESULT hr = S_OK;
    WIU_MSI_EXECUTE_CONTEXT context = { };
    WIU_RESTART restart = WIU_RESTART_NONE;

    LPWSTR sczCachedDirectory = NULL;
    LPWSTR sczMspPath = NULL;
    LPWSTR sczPatches = NULL;
    LPWSTR sczProperties = NULL;

    // default to "verbose" logging
    DWORD dwLogMode = WIU_LOG_DEFAULT | INSTALLLOGMODE_VERBOSE;

    // get cached MSP paths
    for (DWORD i = 0; i < pExecuteAction->mspTarget.cOrderedPatches; ++i)
    {
        LPCWSTR wzAppend = NULL;
        BURN_PACKAGE* pMspPackage = pExecuteAction->mspTarget.rgOrderedPatches[i].pPackage;
        AssertSz(BURN_PACKAGE_TYPE_MSP == pMspPackage->type, "Invalid package type added to ordered patches.");

        // TODO: send UX message that we are adding this patch to the execution.

        if (BOOTSTRAPPER_ACTION_STATE_INSTALL == pExecuteAction->mspTarget.action)
        {
            hr = CacheGetCompletedPath(pMspPackage->fPerMachine, pMspPackage->sczCacheId, &sczCachedDirectory);
            ExitOnFailure1(hr, "Failed to get cached path for MSP package: %ls", pMspPackage->sczId);

            hr = PathConcat(sczCachedDirectory, pMspPackage->rgPayloads[0].pPayload->sczFilePath, &sczMspPath);
            ExitOnFailure(hr, "Failed to build MSP path.");

            wzAppend = sczMspPath;
        }
        else // uninstall
        {
            wzAppend = pMspPackage->Msp.sczPatchCode;
        }

        if (NULL != sczPatches)
        {
            hr = StrAllocConcat(&sczPatches, L";", 0);
            ExitOnFailure(hr, "Failed to semi-colon delimit patches.");
        }

        hr = StrAllocConcat(&sczPatches, wzAppend, 0);
        ExitOnFailure(hr, "Failed to append patch.");
    }

    // Wire up the external UI handler and logging.
    hr = WiuInitializeExternalUI(pfnMessageHandler, pvContext, fRollback, &context);
    ExitOnFailure(hr, "Failed to initialize external UI handler.");

    //if (BURN_LOGGING_LEVEL_DEBUG == logLevel)
    //{
    //    dwLogMode | INSTALLLOGMODE_EXTRADEBUG;
    //}

    if (pExecuteAction->mspTarget.sczLogPath && *pExecuteAction->mspTarget.sczLogPath)
    {
        hr = WiuEnableLog(dwLogMode, pExecuteAction->mspTarget.sczLogPath, 0);
        ExitOnFailure2(hr, "Failed to enable logging for package: %ls to: %ls", pExecuteAction->mspTarget.pPackage->sczId, pExecuteAction->mspTarget.sczLogPath);
    }

    // set up properties
    hr = MsiEngineConcatProperties(pExecuteAction->mspTarget.pPackage->Msp.rgProperties, pExecuteAction->mspTarget.pPackage->Msp.cProperties, pVariables, fRollback, &sczProperties);
    ExitOnFailure(hr, "Failed to add properties to argument string.");

    LogId(REPORT_STANDARD, MSG_APPLYING_PACKAGE, pExecuteAction->mspTarget.pPackage->sczId, LoggingActionStateToString(pExecuteAction->mspTarget.action), sczMspPath, sczProperties);

    //
    // Do the actual action.
    //
    switch (pExecuteAction->mspTarget.action)
    {
    case BOOTSTRAPPER_ACTION_STATE_INSTALL:
        hr = StrAllocConcat(&sczProperties, L" PATCH=\"", 0);
        ExitOnFailure(hr, "Failed to add PATCH property on install.");

        hr = StrAllocConcat(&sczProperties, sczPatches, 0);
        ExitOnFailure(hr, "Failed to add patches to PATCH property on install.");

        hr = StrAllocConcat(&sczProperties, L"\" REBOOT=ReallySuppress", 0);
        ExitOnFailure(hr, "Failed to add reboot suppression property on install.");

        hr = WiuConfigureProductEx(pExecuteAction->mspTarget.sczTargetProductCode, INSTALLLEVEL_DEFAULT, INSTALLSTATE_DEFAULT, sczProperties, &restart);
        ExitOnFailure(hr, "Failed to install MSP package.");
        break;

    case BOOTSTRAPPER_ACTION_STATE_UNINSTALL:
        hr = StrAllocConcat(&sczProperties, L" REBOOT=ReallySuppress", 0);
        ExitOnFailure(hr, "Failed to add reboot suppression property on uninstall.");

        hr = WiuRemovePatches(sczPatches, pExecuteAction->mspTarget.sczTargetProductCode, sczProperties, &restart);
        ExitOnFailure(hr, "Failed to uninstall MSP package.");
        break;
    }

LExit:
    WiuUninitializeExternalUI(&context);

    ReleaseStr(sczCachedDirectory);
    ReleaseStr(sczMspPath);
    ReleaseStr(sczProperties);
    ReleaseStr(sczPatches);

    switch (restart)
    {
        case WIU_RESTART_NONE:
            *pRestart = BOOTSTRAPPER_APPLY_RESTART_NONE;
            break;

        case WIU_RESTART_REQUIRED:
            *pRestart = BOOTSTRAPPER_APPLY_RESTART_REQUIRED;
            break;

        case WIU_RESTART_INITIATED:
            *pRestart = BOOTSTRAPPER_APPLY_RESTART_INITIATED;
            break;
    }

    return hr;
}


// internal helper functions

static HRESULT AddDetectedTargetProduct(
    BURN_PACKAGE* pPackage,
    MSIINSTALLCONTEXT context,
    DWORD dwOrder,
    LPCWSTR wzProductCode
    )
{
    HRESULT hr = S_OK;

    hr = MemEnsureArraySize(reinterpret_cast<LPVOID*>(&pPackage->Msp.rgTargetProducts), pPackage->Msp.cTargetProductCodes + 1, sizeof(BURN_MSPTARGETPRODUCT), 5);
    ExitOnFailure(hr, "Failed to ensure enough target product codes were allocated.");

    hr = ::StringCchCopyW(pPackage->Msp.rgTargetProducts[pPackage->Msp.cTargetProductCodes].wzTargetProductCode, countof(pPackage->Msp.rgTargetProducts[pPackage->Msp.cTargetProductCodes].wzTargetProductCode), wzProductCode);
    ExitOnFailure(hr, "Failed to copy target product code.");

    pPackage->Msp.rgTargetProducts[pPackage->Msp.cTargetProductCodes].context = context;
    pPackage->Msp.rgTargetProducts[pPackage->Msp.cTargetProductCodes].dwOrder = dwOrder;
    ++pPackage->Msp.cTargetProductCodes;

LExit:
    return hr;
}


static HRESULT PlanTargetProduct(
    __in BOOL fRollback,
    __in BURN_PLAN* pPlan,
    __in BOOTSTRAPPER_ACTION_STATE actionState,
    __in BURN_PACKAGE* pPackage,
    __in BURN_MSPTARGETPRODUCT* pTargetProduct
    )
{
    HRESULT hr = S_OK;
    BURN_EXECUTE_ACTION* rgActions = fRollback ? pPlan->rgRollbackActions : pPlan->rgExecuteActions;
    DWORD cActions = fRollback ? pPlan->cRollbackActions : pPlan->cExecuteActions;
    BURN_EXECUTE_ACTION* pAction = NULL;

    // Try to find another MSP action with the exact same action (install or uninstall) targeting
    // the same product in the same machine context (per-user or per-machine).
    for (DWORD i = 0; i < cActions; ++i)
    {
        pAction = rgActions + i;

        if (BURN_EXECUTE_ACTION_TYPE_MSP_TARGET == pAction->type &&
            pAction->mspTarget.action == actionState &&
            pAction->mspTarget.fPerMachineTarget == (MSIINSTALLCONTEXT_MACHINE == pTargetProduct->context) &&
            CSTR_EQUAL == ::CompareStringW(LOCALE_NEUTRAL, 0, pAction->mspTarget.sczTargetProductCode, -1, pTargetProduct->wzTargetProductCode, -1))
        {
            break;
        }

        pAction = NULL;
    }

    // If we didn't find an MSP target action already updating the product, create a new action.
    if (!pAction)
    {
        if (fRollback)
        {
            hr = PlanAppendRollbackAction(pPlan, &pAction);
        }
        else
        {
            hr = PlanAppendExecuteAction(pPlan, &pAction);
        }
        ExitOnFailure(hr, "Failed to plan action for target product.");

        pAction->type = BURN_EXECUTE_ACTION_TYPE_MSP_TARGET;
        pAction->mspTarget.action = actionState;
        pAction->mspTarget.pPackage = pPackage;
        pAction->mspTarget.fPerMachineTarget = (MSIINSTALLCONTEXT_MACHINE == pTargetProduct->context);
        hr = StrAllocString(&pAction->mspTarget.sczTargetProductCode, pTargetProduct->wzTargetProductCode, 0);
        ExitOnFailure(hr, "Failed to copy target product code.");

        // If this is a per-machine target product, then the plan needs to be per-machine as well.
        if (pAction->mspTarget.fPerMachineTarget)
        {
            pPlan->fPerMachine = TRUE;
        }
    }

    // Add our target product to the array and sort based on their order determined during detection.
    hr = MemEnsureArraySize(reinterpret_cast<LPVOID*>(&pAction->mspTarget.rgOrderedPatches), pAction->mspTarget.cOrderedPatches + 1, sizeof(BURN_ORDERED_PATCHES), 2);
    ExitOnFailure(hr, "Failed grow array of ordered patches.");

    pAction->mspTarget.rgOrderedPatches[pAction->mspTarget.cOrderedPatches].dwOrder = pTargetProduct->dwOrder;
    pAction->mspTarget.rgOrderedPatches[pAction->mspTarget.cOrderedPatches].pPackage = pPackage;
    ++pAction->mspTarget.cOrderedPatches;

    // Insertion sort to keep the patches ordered.
    for (DWORD i = pAction->mspTarget.cOrderedPatches - 1; i > 0; --i)
    {
        if (pAction->mspTarget.rgOrderedPatches[i].dwOrder < pAction->mspTarget.rgOrderedPatches[i - 1].dwOrder)
        {
            BURN_ORDERED_PATCHES temp = pAction->mspTarget.rgOrderedPatches[i - 1];
            pAction->mspTarget.rgOrderedPatches[i - 1] = pAction->mspTarget.rgOrderedPatches[i];
            pAction->mspTarget.rgOrderedPatches[i] = temp;
        }
        else // no swap necessary, we're done.
        {
            break;
        }
    }

LExit:
    return hr;
}