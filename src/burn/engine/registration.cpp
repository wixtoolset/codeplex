//-------------------------------------------------------------------------------------------------
// <copyright file="registration.cpp" company="Microsoft">
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
//    Module: Core
// </summary>
//-------------------------------------------------------------------------------------------------

#include "precomp.h"

const LPCWSTR BURN_BUNDLE_NAME = L"WixBundleName";


// constants

const LPCWSTR REGISTRY_UNINSTALL_KEY = L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
const LPCWSTR REGISTRY_RUN_KEY = L"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
const LPCWSTR REGISTRY_REBOOT_PENDING_FORMAT = L"%ls.RebootRequired";
const LPCWSTR REGISTRY_BUNDLE_CACHE_PATH = L"BundleCachePath";
const LPCWSTR REGISTRY_BUNDLE_UPGRADE_CODE = L"BundleUpgradeCode";
const LPCWSTR REGISTRY_BUNDLE_ADDON_CODE = L"BundleAddonCode";
const LPCWSTR REGISTRY_BUNDLE_DETECT_CODE = L"BundleDetectCode";
const LPCWSTR REGISTRY_BUNDLE_TAG = L"BundleTag";
const LPCWSTR REGISTRY_BUNDLE_VERSION = L"BundleVersion";

// internal function declarations

static HRESULT GetBundleName(
    __in BURN_REGISTRATION* pRegistration,
    __in BURN_VARIABLES* pVariables,
    __out LPWSTR* psczBundleName
    );
static HRESULT UpdateResumeMode(
    __in BURN_REGISTRATION* pRegistration,
    __in HKEY hkRegistration,
    __in BURN_RESUME_MODE resumeMode,
    __in BOOL fRestartInitiated,
    __in BOOL fPerMachineProcess
    );
static HRESULT ParseRelatedCodes(
    __in BURN_REGISTRATION* pRegistration,
    __in IXMLDOMNode* pixnBundle
    );
static HRESULT InitializeRelatedBundleFromKey(
    __in_z LPCWSTR wzBundleId,
    __in HKEY hkBundleId,
    __in BOOL fPerMachine,
    __inout BURN_RELATED_BUNDLE *pRelatedBundle,
    __out LPWSTR *psczTag
    );
static HRESULT FindMatchingStringBetweenArrays(
    __in_ecount(cValues) LPCWSTR *rgwzStringArray1,
    __in DWORD cStringArray1,
    __in_ecount(cValues) LPCWSTR *rgwzStringArray2,
    __in DWORD cStringArray2
    );
static HRESULT RegistrationDetectRelatedBundlesForKey(
    __in_opt BURN_USER_EXPERIENCE* pUX,
    __in BURN_REGISTRATION* pRegistration,
    __in_opt BOOTSTRAPPER_COMMAND* pCommand,
    __in HKEY hkRoot
    );


// function definitions

/*******************************************************************
 RegistrationParseFromXml - Parses registration information from manifest.

*******************************************************************/
extern "C" HRESULT RegistrationParseFromXml(
    __in BURN_REGISTRATION* pRegistration,
    __in IXMLDOMNode* pixnBundle
    )
{
    HRESULT hr = S_OK;
    IXMLDOMNode* pixnRegistrationNode = NULL;
    IXMLDOMNode* pixnArpNode = NULL;
    LPWSTR scz = NULL;

    // select registration node
    hr = XmlSelectSingleNode(pixnBundle, L"Registration", &pixnRegistrationNode);
    if (S_FALSE == hr)
    {
        hr = E_NOTFOUND;
    }
    ExitOnFailure(hr, "Failed to select registration node.");

    // @Id
    hr = XmlGetAttributeEx(pixnRegistrationNode, L"Id", &pRegistration->sczId);
    ExitOnFailure(hr, "Failed to get @Id.");

    // @Tag
    hr = XmlGetAttributeEx(pixnRegistrationNode, L"Tag", &pRegistration->sczTag);
    ExitOnFailure(hr, "Failed to get @Tag.");

    hr = ParseRelatedCodes(pRegistration, pixnBundle);
    ExitOnFailure(hr, "Failed to parse related bundles");

    // @Version
    hr = XmlGetAttributeEx(pixnRegistrationNode, L"Version", &scz);
    ExitOnFailure(hr, "Failed to get @Version.");

    hr = FileVersionFromStringEx(scz, 0, &pRegistration->qwVersion);
    ExitOnFailure1(hr, "Failed to parse @Version: %ls", scz);

    // @ProviderKey
    hr = XmlGetAttributeEx(pixnRegistrationNode, L"ProviderKey", &pRegistration->sczProviderKey);
    ExitOnFailure(hr, "Failed to get @ProviderKey.");

    // @ExecutableName
    hr = XmlGetAttributeEx(pixnRegistrationNode, L"ExecutableName", &pRegistration->sczExecutableName);
    ExitOnFailure(hr, "Failed to get @ExecutableName.");

    // @PerMachine
    hr = XmlGetYesNoAttribute(pixnRegistrationNode, L"PerMachine", &pRegistration->fPerMachine);
    ExitOnFailure(hr, "Failed to get @PerMachine.");

    // select ARP node
    hr = XmlSelectSingleNode(pixnRegistrationNode, L"Arp", &pixnArpNode);
    if (S_FALSE != hr)
    {
        ExitOnFailure(hr, "Failed to select ARP node.");

        pRegistration->fRegisterArp = TRUE;

        // @DisplayName
        hr = XmlGetAttributeEx(pixnArpNode, L"DisplayName", &pRegistration->sczDisplayName);
        if (E_NOTFOUND != hr)
        {
            ExitOnFailure(hr, "Failed to get @DisplayName.");
        }

        // @DisplayVersion
        hr = XmlGetAttributeEx(pixnArpNode, L"DisplayVersion", &pRegistration->sczDisplayVersion);
        if (E_NOTFOUND != hr)
        {
            ExitOnFailure(hr, "Failed to get @DisplayVersion.");
        }

        // @Publisher
        hr = XmlGetAttributeEx(pixnArpNode, L"Publisher", &pRegistration->sczPublisher);
        if (E_NOTFOUND != hr)
        {
            ExitOnFailure(hr, "Failed to get @Publisher.");
        }

        // @HelpLink
        hr = XmlGetAttributeEx(pixnArpNode, L"HelpLink", &pRegistration->sczHelpLink);
        if (E_NOTFOUND != hr)
        {
            ExitOnFailure(hr, "Failed to get @HelpLink.");
        }

        // @HelpTelephone
        hr = XmlGetAttributeEx(pixnArpNode, L"HelpTelephone", &pRegistration->sczHelpTelephone);
        if (E_NOTFOUND != hr)
        {
            ExitOnFailure(hr, "Failed to get @HelpTelephone.");
        }

        // @AboutUrl
        hr = XmlGetAttributeEx(pixnArpNode, L"AboutUrl", &pRegistration->sczAboutUrl);
        if (E_NOTFOUND != hr)
        {
            ExitOnFailure(hr, "Failed to get @AboutUrl.");
        }

        // @UpdateUrl
        hr = XmlGetAttributeEx(pixnArpNode, L"UpdateUrl", &pRegistration->sczUpdateUrl);
        if (E_NOTFOUND != hr)
        {
            ExitOnFailure(hr, "Failed to get @UpdateUrl.");
        }

        // @Comments
        hr = XmlGetAttributeEx(pixnArpNode, L"Comments", &pRegistration->sczComments);
        if (E_NOTFOUND != hr)
        {
            ExitOnFailure(hr, "Failed to get @Comments.");
        }

        // @Contact
        hr = XmlGetAttributeEx(pixnArpNode, L"Contact", &pRegistration->sczContact);
        if (E_NOTFOUND != hr)
        {
            ExitOnFailure(hr, "Failed to get @Contact.");
        }

        // @DisableModify
        hr = XmlGetAttributeEx(pixnArpNode, L"DisableModify", &scz);
        if (SUCCEEDED(hr))
        {
            if (CSTR_EQUAL == ::CompareStringW(LOCALE_INVARIANT, 0, scz, -1, L"button", -1))
            {
                pRegistration->modify = BURN_REGISTRATION_MODIFY_DISABLE_BUTTON;
            }
            else if (CSTR_EQUAL == ::CompareStringW(LOCALE_INVARIANT, 0, scz, -1, L"yes", -1))
            {
                pRegistration->modify = BURN_REGISTRATION_MODIFY_DISABLE;
            }
            else if (CSTR_EQUAL == ::CompareStringW(LOCALE_INVARIANT, 0, scz, -1, L"no", -1))
            {
                pRegistration->modify = BURN_REGISTRATION_MODIFY_ENABLED;
            }
            else
            {
                hr = E_UNEXPECTED;
                ExitOnRootFailure1(hr, "Invalid modify disabled type: %ls", scz);
            }
        }
        else if (E_NOTFOUND == hr)
        {
            pRegistration->modify = BURN_REGISTRATION_MODIFY_ENABLED;
            hr = S_OK;
        }
        ExitOnFailure(hr, "Failed to get @DisableModify.");

        // @DisableRepair
        hr = XmlGetYesNoAttribute(pixnArpNode, L"DisableRepair", &pRegistration->fNoRepair);
        if (E_NOTFOUND != hr)
        {
            ExitOnFailure(hr, "Failed to get @DisableRepair.");
            pRegistration->fNoRepairDefined = TRUE;
        }

        // @DisableRemove
        hr = XmlGetYesNoAttribute(pixnArpNode, L"DisableRemove", &pRegistration->fNoRemove);
        if (E_NOTFOUND != hr)
        {
            ExitOnFailure(hr, "Failed to get @DisableRemove.");
            pRegistration->fNoRemoveDefined = TRUE;
        }
    }

    hr = S_OK;

LExit:
    ReleaseObject(pixnRegistrationNode);
    ReleaseObject(pixnArpNode);
    ReleaseStr(scz);

    return hr;
}

/*******************************************************************
 RegistrationUninitialize - 

*******************************************************************/
extern "C" void RegistrationUninitialize(
    __in BURN_REGISTRATION* pRegistration
    )
{
    ReleaseStr(pRegistration->sczId);
    ReleaseStr(pRegistration->sczTag);

    for (DWORD i = 0; i < pRegistration->cDetectCodes; ++i)
    {
        ReleaseStr(pRegistration->rgsczDetectCodes[i]);
    }
    ReleaseMem(pRegistration->rgsczDetectCodes);

    for (DWORD i = 0; i < pRegistration->cUpgradeCodes; ++i)
    {
        ReleaseStr(pRegistration->rgsczUpgradeCodes[i]);
    }
    ReleaseMem(pRegistration->rgsczUpgradeCodes);

    for (DWORD i = 0; i < pRegistration->cAddonCodes; ++i)
    {
        ReleaseStr(pRegistration->rgsczAddonCodes[i]);
    }
    ReleaseMem(pRegistration->rgsczAddonCodes);

    ReleaseStr(pRegistration->sczProviderKey);
    ReleaseStr(pRegistration->sczExecutableName);

    ReleaseStr(pRegistration->sczRegistrationKey);
    ReleaseStr(pRegistration->sczCacheExecutablePath);
    ReleaseStr(pRegistration->sczResumeCommandLine);
    ReleaseStr(pRegistration->sczStateFile);

    ReleaseStr(pRegistration->sczDisplayName);
    ReleaseStr(pRegistration->sczDisplayVersion);
    ReleaseStr(pRegistration->sczPublisher);
    ReleaseStr(pRegistration->sczHelpLink);
    ReleaseStr(pRegistration->sczHelpTelephone);
    ReleaseStr(pRegistration->sczAboutUrl);
    ReleaseStr(pRegistration->sczUpdateUrl);
    ReleaseStr(pRegistration->sczComments);
    ReleaseStr(pRegistration->sczContact);

    if (pRegistration->rgRelatedBundles)
    {
        for (DWORD i = 0; i < pRegistration->cRelatedBundles; ++i)
        {
            PackageUninitialize(&pRegistration->rgRelatedBundles[i].package);
        }

        MemFree(pRegistration->rgRelatedBundles);
    }

    // clear struct
    memset(pRegistration, 0, sizeof(BURN_REGISTRATION));
}

/*******************************************************************
 RegistrationSetVariables - Initializes bundle variables that map to
                            registration entities.

*******************************************************************/
extern "C" HRESULT RegistrationSetVariables(
    __in BURN_REGISTRATION* pRegistration,
    __in BURN_VARIABLES* pVariables
    )
{
    HRESULT hr = S_OK;
    LPWSTR scz = NULL;

    // Ensure the registration bundle name is updated.
    hr = GetBundleName(pRegistration, pVariables, &scz);
    ExitOnFailure(hr, "Failed to intitialize bundle name.");

    hr = AddBuiltInVariable(pVariables, L"WixBundleProviderKey", InitializeVariableString, (DWORD_PTR)pRegistration->sczProviderKey);
    ExitOnFailure(hr, "Failed to initialize the WixBundleProviderKey built-in variable.");

    hr = AddBuiltInVariable(pVariables, L"WixBundleTag", InitializeVariableString, (DWORD_PTR)pRegistration->sczTag);
    ExitOnFailure(hr, "Failed to initialize the WixBundleTag built-in variable.");

LExit:
    ReleaseStr(scz);
    return hr;
}

/*******************************************************************
 RegistrationSetPaths - Initializes file system paths to registration entities.

*******************************************************************/
extern "C" HRESULT RegistrationSetPaths(
    __in BURN_REGISTRATION* pRegistration
    )
{
    HRESULT hr = S_OK;
    LPWSTR sczCacheDirectory = NULL;

    // save registration key root
    pRegistration->hkRoot = pRegistration->fPerMachine ? HKEY_LOCAL_MACHINE : HKEY_CURRENT_USER;

    // build uninstall registry key path
    hr = StrAllocFormatted(&pRegistration->sczRegistrationKey, L"%s\\%s", REGISTRY_UNINSTALL_KEY, pRegistration->sczId);
    ExitOnFailure(hr, "Failed to build uninstall registry key path.");

    // build cache directory
    hr = CacheGetCompletedPath(pRegistration->fPerMachine, pRegistration->sczId, &sczCacheDirectory);
    ExitOnFailure(hr, "Failed to build cache directory.");

    // build cached executable path
    hr = PathConcat(sczCacheDirectory, pRegistration->sczExecutableName, &pRegistration->sczCacheExecutablePath);
    ExitOnFailure(hr, "Failed to build cached executable path.");

    // build state file path
    hr = StrAllocFormatted(&pRegistration->sczStateFile, L"%s\\state.rsm", sczCacheDirectory);
    ExitOnFailure(hr, "Failed to build state file path.");

LExit:
    ReleaseStr(sczCacheDirectory);
    return hr;
}

/*******************************************************************
 RegistrationSetResumeCommand - Initializes resume command string

*******************************************************************/
extern "C" HRESULT RegistrationSetResumeCommand(
    __in BURN_REGISTRATION* pRegistration,
    __in BOOTSTRAPPER_COMMAND* pCommand,
    __in BURN_LOGGING* pLog
    )
{
    HRESULT hr = S_OK;
    LPWSTR sczLogAppend = NULL;

    // build the resume command-line.
    hr = StrAllocFormatted(&pRegistration->sczResumeCommandLine, L"\"%ls\"", pRegistration->sczCacheExecutablePath);
    ExitOnFailure(hr, "Failed to copy executable path to resume command-line.");

    if (pLog->sczPath)
    {
        hr = StrAllocFormatted(&sczLogAppend, L" /%ls \"%ls\"", BURN_COMMANDLINE_SWITCH_LOG_APPEND, pLog->sczPath);
        ExitOnFailure(hr, "Failed to format append log command-line for resume command-line.");

        hr = StrAllocConcat(&pRegistration->sczResumeCommandLine, sczLogAppend, 0);
        ExitOnFailure(hr, "Failed to append log command-line to resume command-line");
    }

    switch (pCommand->action)
    {
    case BOOTSTRAPPER_ACTION_REPAIR:
        hr = StrAllocConcat(&pRegistration->sczResumeCommandLine, L" /repair", 0);
        break;
    case BOOTSTRAPPER_ACTION_UNINSTALL:
        hr = StrAllocConcat(&pRegistration->sczResumeCommandLine, L" /uninstall", 0);
        break;
    }
    ExitOnFailure(hr, "Failed to append action state to resume command-line");

    switch (pCommand->display)
    {
    case BOOTSTRAPPER_DISPLAY_NONE:
        hr = StrAllocConcat(&pRegistration->sczResumeCommandLine, L" /quiet", 0);
        break;
    case BOOTSTRAPPER_DISPLAY_PASSIVE:
        hr = StrAllocConcat(&pRegistration->sczResumeCommandLine, L" /passive", 0);
        break;
    }
    ExitOnFailure(hr, "Failed to append display state to resume command-line");

    switch (pCommand->restart)
    {
    case BOOTSTRAPPER_RESTART_ALWAYS:
        hr = StrAllocConcat(&pRegistration->sczResumeCommandLine, L" /forcerestart", 0);
        break;
    case BOOTSTRAPPER_RESTART_NEVER:
        hr = StrAllocConcat(&pRegistration->sczResumeCommandLine, L" /norestart", 0);
        break;
    }
    ExitOnFailure(hr, "Failed to append restart state to resume command-line");

    if (pCommand->wzCommandLine && *pCommand->wzCommandLine)
    {
        hr = StrAllocConcat(&pRegistration->sczResumeCommandLine, L" ", 0);
        ExitOnFailure(hr, "Failed to append space to resume command-line.");

        hr = StrAllocConcat(&pRegistration->sczResumeCommandLine, pCommand->wzCommandLine, 0);
        ExitOnFailure(hr, "Failed to append command-line to resume command-line.");
    }

LExit:
    ReleaseStr(sczLogAppend);
    return hr;
}

/*******************************************************************
 RegistrationDetectResumeMode - Detects registration information onthe system
                                to determine if a resume is taking place.

*******************************************************************/
extern "C" HRESULT RegistrationDetectResumeType(
    __in BURN_REGISTRATION* pRegistration,
    __out BOOTSTRAPPER_RESUME_TYPE* pResumeType
    )
{
    HRESULT hr = S_OK;
    LPWSTR sczRebootRequiredKey = NULL;
    HKEY hkRebootRequired = NULL;
    HKEY hkRegistration = NULL;
    DWORD dwResume = 0;

    // Check to see if a restart is pending for this bundle.
    hr = StrAllocFormatted(&sczRebootRequiredKey, REGISTRY_REBOOT_PENDING_FORMAT, pRegistration->sczRegistrationKey);
    ExitOnFailure(hr, "Failed to format pending restart registry key to read.");

    hr = RegOpen(pRegistration->hkRoot, sczRebootRequiredKey, KEY_QUERY_VALUE, &hkRebootRequired);
    if (SUCCEEDED(hr))
    {
        *pResumeType = BOOTSTRAPPER_RESUME_TYPE_REBOOT_PENDING;
        ExitFunction1(hr = S_OK);
    }

    // open registration key
    hr = RegOpen(pRegistration->hkRoot, pRegistration->sczRegistrationKey, KEY_QUERY_VALUE, &hkRegistration);
    if (E_FILENOTFOUND == hr || E_PATHNOTFOUND == hr)
    {
        *pResumeType = BOOTSTRAPPER_RESUME_TYPE_NONE;
        ExitFunction1(hr = S_OK);
    }
    ExitOnFailure(hr, "Failed to open registration key.");

    // read Resume value
    hr = RegReadNumber(hkRegistration, L"Resume", &dwResume);
    if (E_FILENOTFOUND == hr)
    {
        *pResumeType = BOOTSTRAPPER_RESUME_TYPE_INVALID;
        ExitFunction1(hr = S_OK);
    }
    ExitOnFailure(hr, "Failed to read Resume value.");

    switch (dwResume)
    {
    case BURN_RESUME_MODE_ACTIVE:
        // a previous run was interrupted
        *pResumeType = BOOTSTRAPPER_RESUME_TYPE_UNEXPECTED;
        break;

    case BURN_RESUME_MODE_SUSPEND:
        *pResumeType = BOOTSTRAPPER_RESUME_TYPE_SUSPEND;
        break;

    case BURN_RESUME_MODE_ARP:
        *pResumeType = BOOTSTRAPPER_RESUME_TYPE_ARP;
        break;

    case BURN_RESUME_MODE_REBOOT_PENDING:
        // The volatile pending registry doesn't exist (checked above) which means
        // the system was successfully restarted.
        *pResumeType = BOOTSTRAPPER_RESUME_TYPE_REBOOT;
        break;

    default:
        // the value stored in the registry is not valid
        *pResumeType = BOOTSTRAPPER_RESUME_TYPE_INVALID;
        break;
    }

LExit:
    ReleaseRegKey(hkRegistration);
    ReleaseRegKey(hkRebootRequired);
    ReleaseStr(sczRebootRequiredKey);

    return hr;
}

/*******************************************************************
 RegistrationDetectRelatedBundles - finds the bundles with same upgrade code.

*******************************************************************/
extern "C" HRESULT RegistrationDetectRelatedBundles(
    __in BOOL fElevated,
    __in BURN_USER_EXPERIENCE* pUX,
    __in BURN_REGISTRATION* pRegistration,
    __in_opt BOOTSTRAPPER_COMMAND* pCommand
    )
{
    HRESULT hr = S_OK;

    if (fElevated)
    {
        hr = RegistrationDetectRelatedBundlesForKey(pUX, pRegistration, pCommand, HKEY_LOCAL_MACHINE);
        ExitOnFailure(hr, "Failed to detect related bundles in HKLM for elevated process");
    }
    else
    {
        hr = RegistrationDetectRelatedBundlesForKey(pUX, pRegistration, pCommand, HKEY_CURRENT_USER);
        ExitOnFailure(hr, "Failed to detect related bundles in HKCU for non-elevated process");

        hr = RegistrationDetectRelatedBundlesForKey(pUX, pRegistration, pCommand, HKEY_LOCAL_MACHINE);
        ExitOnFailure(hr, "Failed to detect related bundles in HKLM for non-elevated process");
    }

LExit:
    return hr;
}

extern "C" HRESULT RegistrationLoadRelatedBundle(
    __in BURN_REGISTRATION* pRegistration,
    __in_z LPCWSTR sczBundleId,
    __out BURN_RELATION_TYPE *pRelationType,
    __out LPWSTR *psczTag
    )
{
    HRESULT hr = S_OK;
    BOOL fRelated = FALSE;
    LPWSTR sczBundleKey = NULL;
    LPWSTR sczId = NULL;
    HKEY hkBundleId = NULL;
    LPWSTR *rgsczUpgradeCodes = NULL;
    DWORD cUpgradeCodes = 0;
    LPWSTR *rgsczAddonCodes = NULL;
    DWORD cAddonCodes = 0;
    LPWSTR *rgsczDetectCodes = NULL;
    DWORD cDetectCodes = 0;
    LPWSTR sczCachePath = NULL;
    LPWSTR sczTag = NULL;

    hr = StrAllocFormatted(&sczBundleKey, L"%ls\\%ls", REGISTRY_UNINSTALL_KEY, sczBundleId);
    ExitOnFailure(hr, "Failed to allocate path to bundle registry key.");

    hr = RegOpen(pRegistration->hkRoot, sczBundleKey, KEY_READ, &hkBundleId);
    if (E_FILENOTFOUND == hr)
    {
        hr = S_OK;
    }
    ExitOnFailure1(hr, "Failed to open bundle registry key: %ls", sczBundleKey);

    hr = RegReadStringArray(hkBundleId, REGISTRY_BUNDLE_UPGRADE_CODE, &rgsczUpgradeCodes, &cUpgradeCodes);
    if (HRESULT_FROM_WIN32(ERROR_INVALID_DATATYPE) == hr)
    {
        TraceError(hr, "Failed to read upgrade code as REG_MULTI_SZ - trying again as REG_SZ in case of older products");

        rgsczUpgradeCodes = reinterpret_cast<LPWSTR *>(MemAlloc(sizeof(LPWSTR), TRUE));
        ExitOnNull(rgsczUpgradeCodes, hr, E_OUTOFMEMORY, "Failed to allocate list for a single upgrade code from older registry format");

        hr = RegReadString(hkBundleId, REGISTRY_BUNDLE_UPGRADE_CODE, &rgsczUpgradeCodes[0]);
        if (SUCCEEDED(hr))
        {
            cUpgradeCodes = 1;
        }
    }
    if (SUCCEEDED(hr))
    {
        // Both them and us must have the same upgrade code to cause an upgrade relation
        hr = FindMatchingStringBetweenArrays(const_cast<LPCWSTR *>(rgsczUpgradeCodes), cUpgradeCodes, const_cast<LPCWSTR *>(pRegistration->rgsczUpgradeCodes), pRegistration->cUpgradeCodes);
        if (HRESULT_FROM_WIN32(ERROR_NO_MATCH) == hr)
        {
            hr = S_OK;
        }
        else
        {
            ExitOnFailure(hr, "Failed to do array search for upgrade code match");

            fRelated = TRUE;
            *pRelationType = BURN_RELATION_UPGRADE;
            goto Finish;
        }
    }

    hr = RegReadStringArray(hkBundleId, REGISTRY_BUNDLE_ADDON_CODE, &rgsczAddonCodes, &cAddonCodes);
    if (SUCCEEDED(hr))
    {
        // Addon relation only occurs when their addon code matches our detect code
        hr = FindMatchingStringBetweenArrays(const_cast<LPCWSTR *>(rgsczAddonCodes), cAddonCodes, const_cast<LPCWSTR *>(pRegistration->rgsczDetectCodes), pRegistration->cDetectCodes);
        if (HRESULT_FROM_WIN32(ERROR_NO_MATCH) == hr)
        {
            hr = S_OK;
        }
        else
        {
            ExitOnFailure(hr, "Failed to do array search for addon code match");

            fRelated = TRUE;
            *pRelationType = BURN_RELATION_ADDON;
            goto Finish;
        }
    }

    // Ignore failure
    RegReadStringArray(hkBundleId, REGISTRY_BUNDLE_DETECT_CODE, &rgsczDetectCodes, &cDetectCodes);

    // Detect relation occurs when one of our detect codes matches any of their codes.
    // Since we already matched their addon codes to our detect codes, we just have to check the other two
    // detection types: their detect codes and their upgrade codes
    hr = FindMatchingStringBetweenArrays(const_cast<LPCWSTR *>(rgsczDetectCodes), cDetectCodes, const_cast<LPCWSTR *>(pRegistration->rgsczDetectCodes), pRegistration->cDetectCodes);
    if (HRESULT_FROM_WIN32(ERROR_NO_MATCH) == hr)
    {
        hr = S_OK;
    }
    else
    {
        ExitOnFailure(hr, "Failed to do array search for detect code match");

        fRelated = TRUE;
        *pRelationType = BURN_RELATION_DETECT;
        goto Finish;
    }

    // Here we check against their upgrades, as described in the larger comment above
    hr = FindMatchingStringBetweenArrays(const_cast<LPCWSTR *>(rgsczUpgradeCodes), cUpgradeCodes, const_cast<LPCWSTR *>(pRegistration->rgsczDetectCodes), pRegistration->cDetectCodes);
    if (HRESULT_FROM_WIN32(ERROR_NO_MATCH) == hr)
    {
        hr = S_OK;
    }
    else
    {
        ExitOnFailure(hr, "Failed to do array search for detect code match");

        fRelated = TRUE;
        *pRelationType = BURN_RELATION_DETECT;
        goto Finish;
    }

Finish:
    if (fRelated)
    {
        hr = MemEnsureArraySize(reinterpret_cast<LPVOID*>(&pRegistration->rgRelatedBundles), pRegistration->cRelatedBundles + 1, sizeof(BURN_RELATED_BUNDLE), 5);
        ExitOnFailure(hr, "Failed to ensure there is space for related bundles.");

        BURN_RELATED_BUNDLE* pRelatedBundle = pRegistration->rgRelatedBundles + pRegistration->cRelatedBundles;

        hr = StrAllocString(&sczId, sczBundleId, 0);
        ExitOnFailure(hr, "Failed to copy related bundle id.");

        hr = InitializeRelatedBundleFromKey(sczBundleId, hkBundleId, pRegistration->fPerMachine, pRelatedBundle, &sczTag);
        ExitOnFailure1(hr, "Failed to initialize package from bundle id: %ls", sczBundleId);

        pRelatedBundle->package.sczId = sczId;
        sczId = NULL;
        *psczTag = sczTag;
        sczTag = NULL;
        pRelatedBundle->package.fPerMachine = pRegistration->fPerMachine;
        ++pRegistration->cRelatedBundles;
    }
    else
    {
        hr = E_NOTFOUND;
    }

LExit:
    ReleaseStr(sczCachePath);
    ReleaseStr(sczId);
    ReleaseStrArray(rgsczUpgradeCodes, cUpgradeCodes);
    ReleaseStrArray(rgsczAddonCodes, cAddonCodes);
    ReleaseStrArray(rgsczDetectCodes, cDetectCodes);
    ReleaseRegKey(hkBundleId);
    ReleaseStr(sczBundleKey);

    return hr;
}

/*******************************************************************
 RegistrationSessionBegin - Registers a run session on the system.

*******************************************************************/
extern "C" HRESULT RegistrationSessionBegin(
    __in BURN_REGISTRATION* pRegistration,
    __in BURN_VARIABLES* pVariables,
    __in BURN_USER_EXPERIENCE* pUserExperience,
    __in BOOTSTRAPPER_ACTION action,
    __in DWORD64 /* qwEstimatedSize */,
    __in BOOL fPerMachineProcess
    )
{
    HRESULT hr = S_OK;
    HKEY hkRegistration = NULL;
    LPWSTR sczExecutablePath = NULL;
    LPWSTR sczDisplayName = NULL;

    // alter registration in the correct process
    if (pRegistration->fPerMachine == fPerMachineProcess)
    {
        // On install, cache executable
        if (BOOTSTRAPPER_ACTION_INSTALL == action)
        {
            hr = PathForCurrentProcess(&sczExecutablePath, NULL);
            ExitOnFailure(hr, "Failed to get path for current executing process.");

            hr = CacheBundle(pRegistration, pUserExperience, sczExecutablePath);
            ExitOnFailure1(hr, "Failed to cache bundle from path: %ls", sczExecutablePath);
        }

        // create registration key
        hr = RegCreate(pRegistration->hkRoot, pRegistration->sczRegistrationKey, KEY_WRITE, &hkRegistration);
        ExitOnFailure(hr, "Failed to create registration key.");

        // ARP registration
        if (pRegistration->fRegisterArp)
        {
            // on initial install, or repair, write any ARP values
            if (BOOTSTRAPPER_ACTION_INSTALL == action || BOOTSTRAPPER_ACTION_REPAIR == action)
            {
                // Upgrade information
                hr = RegWriteString(hkRegistration, REGISTRY_BUNDLE_CACHE_PATH, pRegistration->sczCacheExecutablePath);
                ExitOnFailure(hr, "Failed to write BundleUpgradeCommand value.");

                hr = RegWriteStringArray(hkRegistration, REGISTRY_BUNDLE_UPGRADE_CODE, pRegistration->rgsczUpgradeCodes, pRegistration->cUpgradeCodes);
                ExitOnFailure(hr, "Failed to write BundleUpgradeCode value.");

                hr = RegWriteStringArray(hkRegistration, REGISTRY_BUNDLE_ADDON_CODE, pRegistration->rgsczAddonCodes, pRegistration->cAddonCodes);
                ExitOnFailure(hr, "Failed to write BundleAddonCode value.");

                hr = RegWriteStringArray(hkRegistration, REGISTRY_BUNDLE_DETECT_CODE, pRegistration->rgsczDetectCodes, pRegistration->cDetectCodes);
                ExitOnFailure(hr, "Failed to write BundleDetectCode value.");

                hr = RegWriteStringFormatted(hkRegistration, REGISTRY_BUNDLE_VERSION, L"%hu.%hu.%hu.%hu", (WORD)(pRegistration->qwVersion >> 48), (WORD)(pRegistration->qwVersion >> 32), (WORD)(pRegistration->qwVersion >> 16), (WORD)(pRegistration->qwVersion));
                ExitOnFailure(hr, "Failed to write BundleVersion value.");

                if (pRegistration->sczTag)
                {
                    hr = RegWriteString(hkRegistration, REGISTRY_BUNDLE_TAG, pRegistration->sczTag);
                    ExitOnFailure(hr, "Failed to write BundleTag value.");
                }

                // DisplayIcon: [path to exe] and ",0" to refer to the first icon in the executable.
                hr = RegWriteStringFormatted(hkRegistration, L"DisplayIcon", L"%s,0", pRegistration->sczCacheExecutablePath);
                ExitOnFailure(hr, "Failed to write DisplayIcon value.");

                // DisplayName: provided by UI
                hr = GetBundleName(pRegistration, pVariables, &sczDisplayName);
                hr = RegWriteString(hkRegistration, L"DisplayName", SUCCEEDED(hr) ? sczDisplayName : pRegistration->sczDisplayName);
                ExitOnFailure(hr, "Failed to write DisplayName value.");

                // DisplayVersion: provided by UI
                if (pRegistration->sczDisplayVersion)
                {
                    hr = RegWriteString(hkRegistration, L"DisplayVersion", pRegistration->sczDisplayVersion);
                    ExitOnFailure(hr, "Failed to write DisplayVersion value.");
                }

                // Publisher: provided by UI
                if (pRegistration->sczPublisher)
                {
                    hr = RegWriteString(hkRegistration, L"Publisher", pRegistration->sczPublisher);
                    ExitOnFailure(hr, "Failed to write Publisher value.");
                }

                // HelpLink: provided by UI
                if (pRegistration->sczHelpLink)
                {
                    hr = RegWriteString(hkRegistration, L"HelpLink", pRegistration->sczHelpLink);
                    ExitOnFailure(hr, "Failed to write HelpLink value.");
                }

                // HelpTelephone: provided by UI
                if (pRegistration->sczHelpTelephone)
                {
                    hr = RegWriteString(hkRegistration, L"HelpTelephone", pRegistration->sczHelpTelephone);
                    ExitOnFailure(hr, "Failed to write HelpTelephone value.");
                }

                // URLInfoAbout, provided by UI
                if (pRegistration->sczAboutUrl)
                {
                    hr = RegWriteString(hkRegistration, L"URLInfoAbout", pRegistration->sczAboutUrl);
                    ExitOnFailure(hr, "Failed to write URLInfoAbout value.");
                }

                // URLUpdateInfo, provided by UI
                if (pRegistration->sczUpdateUrl)
                {
                    hr = RegWriteString(hkRegistration, L"URLUpdateInfo", pRegistration->sczUpdateUrl);
                    ExitOnFailure(hr, "Failed to write URLUpdateInfo value.");
                }

                // Comments, provided by UI
                if (pRegistration->sczComments)
                {
                    hr = RegWriteString(hkRegistration, L"Comments", pRegistration->sczComments);
                    ExitOnFailure(hr, "Failed to write Comments value.");
                }

                // Contact, provided by UI
                if (pRegistration->sczContact)
                {
                    hr = RegWriteString(hkRegistration, L"Contact", pRegistration->sczContact);
                    ExitOnFailure(hr, "Failed to write Contact value.");
                }

                // InstallLocation: provided by UI
                // TODO: need to figure out what "InstallLocation" means in a chainer. <smile/>

                // NoModify
                if (BURN_REGISTRATION_MODIFY_DISABLE == pRegistration->modify)
                {
                    hr = RegWriteNumber(hkRegistration, L"NoModify", 1);
                    ExitOnFailure(hr, "Failed to set NoModify value.");
                }
                else if (BURN_REGISTRATION_MODIFY_DISABLE_BUTTON != pRegistration->modify) // if support modify (aka: did not disable anything)
                {
                    // ModifyPath: [path to exe] /modify
                    hr = RegWriteStringFormatted(hkRegistration, L"ModifyPath", L"\"%ls\" /modify", pRegistration->sczCacheExecutablePath);
                    ExitOnFailure(hr, "Failed to write ModifyPath value.");

                    // NoElevateOnModify: 1
                    hr = RegWriteNumber(hkRegistration, L"NoElevateOnModify", 1);
                    ExitOnFailure(hr, "Failed to set NoElevateOnModify value.");
                }

                // NoRepair
                if (pRegistration->fNoRepairDefined)
                {
                    hr = RegWriteNumber(hkRegistration, L"NoRepair", (DWORD)pRegistration->fNoRepair);
                    ExitOnFailure(hr, "Failed to set NoRepair value.");
                }

                // NoRemove: should this be allowed?
                if (pRegistration->fNoRemoveDefined)
                {
                    hr = RegWriteNumber(hkRegistration, L"NoRemove", (DWORD)pRegistration->fNoRemove);
                    ExitOnFailure(hr, "Failed to set NoRemove value.");
                }

                // QuietUninstallString: [path to exe] /uninstall /quiet
                hr = RegWriteStringFormatted(hkRegistration, L"QuietUninstallString", L"\"%ls\" /uninstall /quiet", pRegistration->sczCacheExecutablePath);
                ExitOnFailure(hr, "Failed to write QuietUninstallString value.");

                // UninstallString, [path to exe]
                // If the modify button is to be disabled, we'll add "/modify" to the uninstall string because the button is "Uninstall/Change". Otherwise,
                // it's just the "Uninstall" button so we add "/uninstall" to make the program just go away.
                LPCWSTR wzUninstallParameters = (BURN_REGISTRATION_MODIFY_DISABLE_BUTTON == pRegistration->modify) ? L"/modify" : L" /uninstall";
                hr = RegWriteStringFormatted(hkRegistration, L"UninstallString", L"\"%ls\" %ls", pRegistration->sczCacheExecutablePath, wzUninstallParameters);
                ExitOnFailure(hr, "Failed to write UninstallString value.");
            }

            // TODO: if we are not uninstalling, update estimated size
            //if (BOOTSTRAPPER_ACTION_UNINSTALL != action)
            //{
            //}
        }

        // Register the bundle dependency key.
        if (BOOTSTRAPPER_ACTION_INSTALL == action || BOOTSTRAPPER_ACTION_REPAIR == action)
        {
            hr = DependencyRegister(pRegistration);
            ExitOnFailure(hr, "Failed to register the bundle dependency key.");
        }
    }

    // update resume mode
    hr = UpdateResumeMode(pRegistration, hkRegistration, BURN_RESUME_MODE_ACTIVE, FALSE, fPerMachineProcess);
    ExitOnFailure(hr, "Failed to update resume mode.");

LExit:
    ReleaseStr(sczDisplayName);
    ReleaseStr(sczExecutablePath);
    ReleaseRegKey(hkRegistration);

    return hr;
}


/*******************************************************************
 RegistrationSessionResume - Resumes a previous run session.

*******************************************************************/
extern "C" HRESULT RegistrationSessionResume(
    __in BURN_REGISTRATION* pRegistration,
    __in BOOTSTRAPPER_ACTION /* action */,
    __in BOOL fPerMachineProcess
    )
{
    HRESULT hr = S_OK;
    HKEY hkRegistration = NULL;

    // alter registration in the correct process
    if (pRegistration->fPerMachine == fPerMachineProcess)
    {
        // open registration key
        hr = RegOpen(pRegistration->hkRoot, pRegistration->sczRegistrationKey, KEY_WRITE, &hkRegistration);
        ExitOnFailure(hr, "Failed to open registration key.");
    }

    // update resume mode
    hr = UpdateResumeMode(pRegistration, hkRegistration, BURN_RESUME_MODE_ACTIVE, FALSE, fPerMachineProcess);
    ExitOnFailure(hr, "Failed to update resume mode.");

LExit:
    ReleaseRegKey(hkRegistration);

    return hr;
}


/*******************************************************************
 RegistrationSessionEnd - Unregisters a run session from the system.

 *******************************************************************/
extern "C" HRESULT RegistrationSessionEnd(
    __in BURN_REGISTRATION* pRegistration,
    __in BOOTSTRAPPER_ACTION action,
    __in BOOL fRollback,
    __in BOOL fSuspend,
    __in BOOTSTRAPPER_APPLY_RESTART restart,
    __in BOOL fPerMachineProcess,
    __out_opt BURN_RESUME_MODE* pResumeMode
    )
{
    HRESULT hr = S_OK;
    BURN_RESUME_MODE resumeMode = BURN_RESUME_MODE_NONE;
    LPWSTR sczRebootRequiredKey = NULL;
    HKEY hkRebootRequired = NULL;
    HKEY hkRegistration = NULL;

    // Calculate the correct resume mode. If a restart has been initiated, that trumps all other
    // modes. If the user chose to suspend the install then we'll use that as the resume mode.
    // Barring those special cases, if the bundle is supposed to be registered in ARP and the
    // install was successful or the uninstall was unsuccessful, then set resume mode to "ARP".
    // Finally, (the unspoken case) if we are not registering in ARP or we failed to install
    // or we successfully uninstalled then the resume mode is none and everything gets cleaned
    // up.
    if (BOOTSTRAPPER_APPLY_RESTART_INITIATED == restart)
    {
        resumeMode = BURN_RESUME_MODE_REBOOT_PENDING;
    }
    else if (fSuspend)
    {
        resumeMode = BURN_RESUME_MODE_SUSPEND;
    }
    else if (pRegistration->fRegisterArp && !((BOOTSTRAPPER_ACTION_INSTALL == action && fRollback) || (BOOTSTRAPPER_ACTION_UNINSTALL == action && !fRollback)))
    {
        resumeMode = BURN_RESUME_MODE_ARP;
    }

    // Alter registration in the correct process.
    if (pRegistration->fPerMachine == fPerMachineProcess)
    {
        // If a restart is required for any reason, write a volatile registry key to track of
        // of that fact until the reboot has taken place.
        if (BOOTSTRAPPER_APPLY_RESTART_NONE != restart)
        {
            // We'll write the volatile registry key right next to the bundle ARP registry key
            // because that's easy. This is all best effort since the worst case just means in
            // the rare case the user launches the same install again before taking the restart
            // the BA won't know a restart was still required.
            hr = StrAllocFormatted(&sczRebootRequiredKey, REGISTRY_REBOOT_PENDING_FORMAT, pRegistration->sczRegistrationKey);
            if (SUCCEEDED(hr))
            {
                hr = RegCreateEx(pRegistration->hkRoot, sczRebootRequiredKey, KEY_WRITE, TRUE, NULL, &hkRebootRequired, NULL);
            }

            if (FAILED(hr))
            {
                ExitTrace(hr, "Failed to write volatile reboot required registry key.");
                hr = S_OK;
            }
        }

        // If no resume mode, then remove the bundle registration.
        if (BURN_RESUME_MODE_NONE == resumeMode)
        {
            // Remove the bundle dependency key.
            hr = DependencyUnregister(pRegistration);
            ExitOnFailure(hr, "Failed to remove the bundle dependency key.");

            // Delete registration key.
            hr = RegDelete(pRegistration->hkRoot, pRegistration->sczRegistrationKey, REG_KEY_DEFAULT, FALSE);
            if (E_FILENOTFOUND != hr)
            {
                ExitOnFailure1(hr, "Failed to delete registration key: %ls", pRegistration->sczRegistrationKey);
            }

            hr = CacheRemoveBundle(pRegistration->fPerMachine, pRegistration->sczId);
            ExitOnFailure(hr, "Failed to remove bundle from cache.");
        }
        else // the mode needs to be updated so open the registration key.
        {
            // Open registration key.
            hr = RegOpen(pRegistration->hkRoot, pRegistration->sczRegistrationKey, KEY_WRITE, &hkRegistration);
            ExitOnFailure(hr, "Failed to open registration key.");
        }
    }

    // Update resume mode.
    hr = UpdateResumeMode(pRegistration, hkRegistration, resumeMode, BOOTSTRAPPER_APPLY_RESTART_INITIATED == restart, fPerMachineProcess);
    ExitOnFailure(hr, "Failed to update resume mode.");

    // Return resume mode.
    if (pResumeMode)
    {
        *pResumeMode = resumeMode;
    }

LExit:
    ReleaseRegKey(hkRegistration);
    ReleaseRegKey(hkRebootRequired);
    ReleaseStr(sczRebootRequiredKey);

    return hr;
}

/*******************************************************************
 RegistrationSaveState - Saves an engine state BLOB for retreval after a resume.

*******************************************************************/
extern "C" HRESULT RegistrationSaveState(
    __in BURN_REGISTRATION* pRegistration,
    __in_bcount(cbBuffer) BYTE* pbBuffer,
    __in SIZE_T cbBuffer
    )
{
    HRESULT hr = S_OK;

    // write data to file
    hr = FileWrite(pRegistration->sczStateFile, FILE_ATTRIBUTE_NORMAL, pbBuffer, cbBuffer, NULL);
    ExitOnFailure1(hr, "Failed to write state to file: %ls", pRegistration->sczStateFile);

LExit:
    return hr;
}

/*******************************************************************
 RegistrationLoadState - Loads a previously stored engine state BLOB.

*******************************************************************/
extern "C" HRESULT RegistrationLoadState(
    __in BURN_REGISTRATION* pRegistration,
    __out_bcount(*pcbBuffer) BYTE** ppbBuffer,
    __out DWORD* pcbBuffer
    )
{
    // read data from file
    HRESULT hr = FileRead(ppbBuffer, pcbBuffer, pRegistration->sczStateFile);
    return hr;
}


// internal helper functions

static HRESULT GetBundleName(
    __in BURN_REGISTRATION* pRegistration,
    __in BURN_VARIABLES* pVariables,
    __out LPWSTR* psczBundleName
    )
{
    HRESULT hr = S_OK;

    hr = VariableGetString(pVariables, BURN_BUNDLE_NAME, psczBundleName);
    if (E_NOTFOUND == hr)
    {
        hr = VariableSetString(pVariables, BURN_BUNDLE_NAME, pRegistration->sczDisplayName);
        ExitOnFailure(hr, "Failed to set bundle name.");

        hr = StrAllocString(psczBundleName, pRegistration->sczDisplayName, 0);
    }
    ExitOnFailure(hr, "Failed to get bundle name.");

LExit:
    return hr;
}

static HRESULT UpdateResumeMode(
    __in BURN_REGISTRATION* pRegistration,
    __in HKEY hkRegistration,
    __in BURN_RESUME_MODE resumeMode,
    __in BOOL fRestartInitiated,
    __in BOOL fPerMachineProcess
    )
{
    HRESULT hr = S_OK;
    DWORD er = ERROR_SUCCESS;
    HKEY hkRebootRequired = NULL;
    HKEY hkRun = NULL;

    // write resume information
    if (hkRegistration)
    {
        // write Resume value
        hr = RegWriteNumber(hkRegistration, L"Resume", (DWORD)resumeMode);
        ExitOnFailure(hr, "Failed to write Resume value.");
    }

    // update run key, this always happens in the per-user process
    if (!fPerMachineProcess)
    {
        // If the engine is active write the run key so we resume if there is an unexpected
        // power loss. Also, if a restart was initiated in the middle of the chain then
        // ensure the run key exists (it should since going active would have written it).
        if (BURN_RESUME_MODE_ACTIVE == resumeMode || fRestartInitiated)
        {
            // write run key
            hr = RegCreate(HKEY_CURRENT_USER, REGISTRY_RUN_KEY, KEY_WRITE, &hkRun);
            ExitOnFailure(hr, "Failed to create run key.");

            hr = RegWriteString(hkRun, pRegistration->sczId, pRegistration->sczResumeCommandLine);
            ExitOnFailure(hr, "Failed to write run key value.");
        }
        else // delete run key value
        {
            hr = RegOpen(HKEY_CURRENT_USER, REGISTRY_RUN_KEY, KEY_WRITE, &hkRun);
            if (E_FILENOTFOUND == hr || E_PATHNOTFOUND == hr)
            {
                hr = S_OK;
            }
            else
            {
                ExitOnWin32Error(er, hr, "Failed to open run key.");

                er = ::RegDeleteValueW(hkRun, pRegistration->sczId);
                ExitOnWin32Error(er, hr, "Failed to delete run key value.");
            }
        }
    }

LExit:
    ReleaseRegKey(hkRebootRequired);
    ReleaseRegKey(hkRun);

    return hr;
}

static HRESULT ParseRelatedCodes(
    __in BURN_REGISTRATION* pRegistration,
    __in IXMLDOMNode* pixnBundle
    )
{
    HRESULT hr = S_OK;
    IXMLDOMNodeList* pixnNodes = NULL;
    IXMLDOMNode* pixnElement = NULL;
    LPWSTR sczAction = NULL;
    LPWSTR sczId = NULL;
    DWORD cElements = 0;

    hr = XmlSelectNodes(pixnBundle, L"RelatedBundle", &pixnNodes);
    ExitOnFailure(hr, "Failed to get RelatedBundle nodes");

    hr = pixnNodes->get_length((long*)&cElements);
    ExitOnFailure(hr, "Failed to get RelatedBundle element count.");

    for (DWORD i = 0; i < cElements; ++i)
    {
        hr = XmlNextElement(pixnNodes, &pixnElement, NULL);
        ExitOnFailure(hr, "Failed to get next RelatedBundle element.");

        hr = XmlGetAttributeEx(pixnElement, L"Action", &sczAction);
        ExitOnFailure(hr, "Failed to get @Action.");

        hr = XmlGetAttributeEx(pixnElement, L"Id", &sczId);
        ExitOnFailure(hr, "Failed to get @Id.");

        if (CSTR_EQUAL == ::CompareStringW(LOCALE_INVARIANT, 0, sczAction, -1, L"Detect", -1))
        {
            hr = MemEnsureArraySize(reinterpret_cast<LPVOID*>(&pRegistration->rgsczDetectCodes), pRegistration->cDetectCodes + 1, sizeof(LPWSTR), 5);
            ExitOnFailure(hr, "Failed to resize Detect code array in registration");

            pRegistration->rgsczDetectCodes[pRegistration->cDetectCodes] = sczId;
            sczId = NULL;
            ++pRegistration->cDetectCodes;
        }
        else if (CSTR_EQUAL == ::CompareStringW(LOCALE_INVARIANT, 0, sczAction, -1, L"Upgrade", -1))
        {
            hr = MemEnsureArraySize(reinterpret_cast<LPVOID*>(&pRegistration->rgsczUpgradeCodes), pRegistration->cUpgradeCodes + 1, sizeof(LPWSTR), 5);
            ExitOnFailure(hr, "Failed to resize Upgrade code array in registration");

            pRegistration->rgsczUpgradeCodes[pRegistration->cUpgradeCodes] = sczId;
            sczId = NULL;
            ++pRegistration->cUpgradeCodes;
        }
        else if (CSTR_EQUAL == ::CompareStringW(LOCALE_INVARIANT, 0, sczAction, -1, L"Addon", -1))
        {
            hr = MemEnsureArraySize(reinterpret_cast<LPVOID*>(&pRegistration->rgsczAddonCodes), pRegistration->cAddonCodes + 1, sizeof(LPWSTR), 5);
            ExitOnFailure(hr, "Failed to resize Addon code array in registration");

            pRegistration->rgsczAddonCodes[pRegistration->cAddonCodes] = sczId;
            sczId = NULL;
            ++pRegistration->cAddonCodes;
        }
        else
        {
            hr = E_INVALIDARG;
            ExitOnFailure1(hr, "Invalid value for @Action: %ls", sczAction);
        }
    }

LExit:
    ReleaseObject(pixnNodes);
    ReleaseObject(pixnElement);
    ReleaseStr(sczAction);
    ReleaseStr(sczId);

    return hr;
}

static HRESULT InitializeRelatedBundleFromKey(
    __in_z LPCWSTR wzBundleId,
    __in HKEY hkBundleId,
    __in BOOL fPerMachine,
    __inout BURN_RELATED_BUNDLE *pRelatedBundle,
    __out LPWSTR *psczTag
    )
{
    HRESULT hr = S_OK;
    LPWSTR sczCachePath = NULL;

    hr = RegReadVersion(hkBundleId, REGISTRY_BUNDLE_VERSION, &pRelatedBundle->qwVersion);
    ExitOnFailure1(hr, "Failed to read version from registry for bundle: %ls", wzBundleId);

    hr = RegReadString(hkBundleId, REGISTRY_BUNDLE_CACHE_PATH, &sczCachePath);
    ExitOnFailure1(hr, "Failed to read cache path from registry for bundle: %ls", wzBundleId);

    hr = RegReadString(hkBundleId, REGISTRY_BUNDLE_TAG, psczTag);
    if (E_FILENOTFOUND == hr)
    {
        hr = S_OK;
    }
    ExitOnFailure1(hr, "Failed to read tag from registry for bundle: %ls", wzBundleId);

    // Initialize the single payload, and fill out all the necessary fields
    pRelatedBundle->package.rgPayloads = (BURN_PACKAGE_PAYLOAD *)MemAlloc(sizeof(BURN_PACKAGE_PAYLOAD), TRUE); 
    ExitOnNull(pRelatedBundle->package.rgPayloads, hr, E_OUTOFMEMORY, "Failed to allocate space for burn package payload inside of related bundle struct");
    pRelatedBundle->package.cPayloads = 1;

    pRelatedBundle->package.rgPayloads->fCached = TRUE;

    pRelatedBundle->package.rgPayloads->pPayload = (BURN_PAYLOAD *)MemAlloc(sizeof(BURN_PAYLOAD), TRUE); 
    ExitOnNull(pRelatedBundle->package.rgPayloads, hr, E_OUTOFMEMORY, "Failed to allocate space for burn payload inside of related bundle struct");
    pRelatedBundle->package.rgPayloads->pPayload->packaging = BURN_PAYLOAD_PACKAGING_EXTERNAL;

    hr = FileSize(sczCachePath, reinterpret_cast<LONGLONG *>(&pRelatedBundle->package.rgPayloads->pPayload->qwFileSize));
    ExitOnFailure1(hr, "Failed to get size of related bundle: %ls", sczCachePath);

    pRelatedBundle->package.rgPayloads->pPayload->sczFilePath = sczCachePath;
    sczCachePath = NULL;

    pRelatedBundle->package.type = BURN_PACKAGE_TYPE_EXE;
    pRelatedBundle->package.fPerMachine = fPerMachine;
    pRelatedBundle->package.currentState = BOOTSTRAPPER_PACKAGE_STATE_PRESENT;
    pRelatedBundle->package.fUninstallable = TRUE;
    pRelatedBundle->package.fVital = FALSE;
    
    hr = StrAllocString(&pRelatedBundle->package.Exe.sczInstallArguments, L"/quiet", 0);
    ExitOnFailure(hr, "Failed to copy install arguments for related bundle package");

    hr = StrAllocString(&pRelatedBundle->package.Exe.sczRepairArguments, L"/repair /quiet", 0);
    ExitOnFailure(hr, "Failed to copy repair arguments for related bundle package");

    hr = StrAllocString(&pRelatedBundle->package.Exe.sczUninstallArguments, L"/uninstall /quiet", 0);
    ExitOnFailure(hr, "Failed to copy uninstall arguments for related bundle package");

    pRelatedBundle->package.Exe.fRepairable = TRUE;
    pRelatedBundle->package.Exe.protocol = BURN_EXE_PROTOCOL_TYPE_BURN;

LExit:
    ReleaseStr(sczCachePath);

    return hr;
}

static HRESULT FindMatchingStringBetweenArrays(
    __in_ecount(cValues) LPCWSTR *rgwzStringArray1,
    __in DWORD cStringArray1,
    __in_ecount(cValues) LPCWSTR *rgwzStringArray2,
    __in DWORD cStringArray2
    )
{
    HRESULT hr = S_OK;

    for (DWORD i = 0; i < cStringArray1; ++i)
    {
        for (DWORD j = 0; j < cStringArray2; ++j)
        {
            if (CSTR_EQUAL == ::CompareStringW(LOCALE_NEUTRAL, NORM_IGNORECASE, rgwzStringArray2[j], -1, rgwzStringArray1[i], -1))
            {
                ExitFunction1(hr = S_OK);
            }
        }
    }

    ExitFunction1(hr = HRESULT_FROM_WIN32(ERROR_NO_MATCH));

LExit:
    return hr;
}

static HRESULT RegistrationDetectRelatedBundlesForKey(
    __in_opt BURN_USER_EXPERIENCE* pUX,
    __in BURN_REGISTRATION* pRegistration,
    __in_opt BOOTSTRAPPER_COMMAND* pCommand,
    __in HKEY hkRoot
    )
{
    HRESULT hr = S_OK;
    BURN_RELATION_TYPE relationType = BURN_RELATION_NONE;
    HKEY hkUninstallKey = NULL;
    LPWSTR sczBundleId = NULL;
    LPWSTR sczTag = NULL;

    hr = RegOpen(hkRoot, REGISTRY_UNINSTALL_KEY, KEY_READ, &hkUninstallKey);
    if (HRESULT_FROM_WIN32(ERROR_PATH_NOT_FOUND) == hr || HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND) == hr)
    {
        ExitFunction1(hr = S_OK);
    }
    ExitOnFailure(hr, "Failed to open uninstall registry key.");

    for (DWORD dwIndex = 0; /* exit via break below */; ++dwIndex)
    {
        hr = RegKeyEnum(hkUninstallKey, dwIndex, &sczBundleId);
        if (E_NOMOREITEMS == hr)
        {
            hr = S_OK;
            break;
        }
        ExitOnFailure(hr, "Failed to enumerate uninstall key.");

        // If we did not find ourself, try to load the subkey as a related bundle.
        if (CSTR_EQUAL != ::CompareStringW(LOCALE_NEUTRAL, NORM_IGNORECASE, sczBundleId, -1, pRegistration->sczId, -1))
        {
            // Ignore failures here since we'll often find products that aren't actually
            // related bundles (or even bundles at all).
            relationType = BURN_RELATION_NONE;

            hr = RegistrationLoadRelatedBundle(pRegistration, sczBundleId, &relationType, &sczTag);
            if (SUCCEEDED(hr))
            {
                BURN_RELATED_BUNDLE* pRelatedBundle = pRegistration->rgRelatedBundles + pRegistration->cRelatedBundles - 1;

                pRelatedBundle->relationType = relationType;

                if (pUX)
                {
                    BOOTSTRAPPER_RELATED_OPERATION operation = BOOTSTRAPPER_RELATED_OPERATION_NONE;

                    switch (relationType)
                    {
                    case BURN_RELATION_UPGRADE:
                        if (BOOTSTRAPPER_ACTION_INSTALL == pCommand->action)
                        {
                            if (pRegistration->qwVersion > pRelatedBundle->qwVersion)
                            {
                                operation = BOOTSTRAPPER_RELATED_OPERATION_MAJOR_UPGRADE;
                            }
                            else if (pRegistration->qwVersion < pRelatedBundle->qwVersion)
                            {
                                operation = BOOTSTRAPPER_RELATED_OPERATION_DOWNGRADE;
                            }
                        }
                        break;

                    case BURN_RELATION_ADDON:
                        if (pCommand)
                        {
                            if (BOOTSTRAPPER_ACTION_UNINSTALL == pCommand->action)
                            {
                                operation = BOOTSTRAPPER_RELATED_OPERATION_REMOVE;
                            }
                            else if (BOOTSTRAPPER_ACTION_REPAIR == pCommand->action)
                            {
                                operation = BOOTSTRAPPER_RELATED_OPERATION_REPAIR;
                            }
                        }
                        break;

                    case BURN_RELATION_DETECT:
                        break;

                    default:
                        hr = E_FAIL;
                        ExitOnFailure1(hr, "Unexpected relation type encountered: %d", relationType);
                        break;
                    }

                    LogId(REPORT_STANDARD, MSG_DETECTED_RELATED_BUNDLE, pRelatedBundle->package.sczId, LoggingPerMachineToString(pRelatedBundle->package.fPerMachine), LoggingVersionToString(pRelatedBundle->qwVersion), LoggingRelatedOperationToString(operation));

                    int nResult = pUX->pUserExperience->OnDetectRelatedBundle(pRelatedBundle->package.sczId, sczTag, pRelatedBundle->package.fPerMachine, pRelatedBundle->qwVersion, operation);
                    hr = HRESULT_FROM_VIEW(nResult);
                    ExitOnRootFailure(hr, "UX aborted detect related bundle.");
                }
            }
        }
    }

LExit:
    ReleaseStr(sczBundleId);
    ReleaseStr(sczTag);
    ReleaseRegKey(hkUninstallKey);

    return hr;
}
