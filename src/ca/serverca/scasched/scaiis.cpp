//-------------------------------------------------------------------------------------------------
// <copyright file="scaiis.h" company="Microsoft">
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
// <summary>
//    IIS functions for CustomActions
// </summary>
//-------------------------------------------------------------------------------------------------

#include "precomp.h"

// globals
LPWSTR vpwzCustomActionData = NULL;
DWORD vdwCustomActionCost = 0;

// prototypes
static HRESULT ScaAddToMetabaseConfiguration(LPCWSTR pwzData, DWORD dwCost);

HRESULT ScaMetabaseTransaction(LPCWSTR wzBackup)
{
    HRESULT hr = S_OK;

    // TODO: These functions have been reported to hang IIS (O11:51709).  They may have been fixed in IIS6, but if not, need to be re-written the hard way

    hr = WcaDoDeferredAction(L"StartMetabaseTransaction", wzBackup, COST_IIS_TRANSACTIONS);
    ExitOnFailure(hr, "Failed to schedule StartMetabaseTransaction");

    hr = WcaDoDeferredAction(L"RollbackMetabaseTransaction", wzBackup, 0);   // rollback cost is irrelevant
    ExitOnFailure(hr, "Failed to schedule RollbackMetabaseTransaction");

    hr = WcaDoDeferredAction(L"CommitMetabaseTransaction", wzBackup, 0);  // commit is free
    ExitOnFailure(hr, "Failed to schedule StartMetabaseTransaction");

LExit:
    return hr;
}


HRESULT ScaCreateWeb(IMSAdminBase* piMetabase, LPCWSTR wzWeb, LPCWSTR wzWebBase)
{
    Assert(piMetabase);

    HRESULT hr = S_OK;
    UINT ui = 0;

    hr = ScaCreateMetabaseKey(piMetabase, wzWebBase, L"");
    ExitOnFailure(hr, "Failed to create web");

    hr = ScaWriteMetabaseValue(piMetabase, wzWebBase, L"", MD_KEY_TYPE, METADATA_NO_ATTRIBUTES, IIS_MD_UT_SERVER, STRING_METADATA, (LPVOID)L"IIsWebServer");
    ExitOnFailure(hr, "Failed to set key type for web");

    hr = ScaCreateMetabaseKey(piMetabase, wzWebBase, L"Root");
    ExitOnFailure(hr, "Failed to create web root");

    hr = ScaWriteMetabaseValue(piMetabase, wzWebBase, L"Root", MD_KEY_TYPE, METADATA_NO_ATTRIBUTES, IIS_MD_UT_SERVER, STRING_METADATA, (LPVOID)L"IIsWebVirtualDir");
    ExitOnFailure(hr, "Failed to set key type for web root");

    ui = 0x4000003e; // 1073741886;    // default directory browsing rights
    hr = ScaWriteMetabaseValue(piMetabase, wzWebBase, L"Root", MD_DIRECTORY_BROWSING, METADATA_INHERIT, IIS_MD_UT_FILE, DWORD_METADATA, (LPVOID)((DWORD_PTR)ui));
    ExitOnFailure(hr, "Failed to set directory browsing for web");

    hr = ScaCreateMetabaseKey(piMetabase, wzWebBase, L"Filters");
    ExitOnFailure(hr, "Failed to create web filters root");

    hr = ScaWriteMetabaseValue(piMetabase, wzWebBase, L"Filters", MD_KEY_TYPE, METADATA_NO_ATTRIBUTES, IIS_MD_UT_SERVER, STRING_METADATA, (LPVOID)L"IIsFilters");
    ExitOnFailure(hr, "Failed to set key for web filters root");

    hr = ScaWriteMetabaseValue(piMetabase, wzWebBase, L"Filters", MD_FILTER_LOAD_ORDER, METADATA_NO_ATTRIBUTES, IIS_MD_UT_SERVER, STRING_METADATA, (LPVOID)L"");
    ExitOnFailure(hr, "Failed to set empty load order for web");

LExit:
    return hr;
}


HRESULT ScaDeleteApp(IMSAdminBase* piMetabase, LPCWSTR wzWebRoot)
{
    Assert(piMetabase);
    Unused(piMetabase);

    HRESULT hr = S_OK;
    WCHAR wzKey[METADATA_MAX_NAME_LEN];
    BOOL fRecursive = FALSE;

    WCHAR* pwzCustomActionData = NULL;

    hr = WcaWriteIntegerToCaData(MBA_DELETEAPP, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase delete app directive to CustomActionData");

    hr = ::StringCchCopyW(wzKey, countof(wzKey), wzWebRoot);
    ExitOnFailure(hr, "Failed to copy webroot string to key");
    hr = WcaWriteStringToCaData(wzKey, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase key to CustomActionData");

    hr = ScaAddToMetabaseConfiguration(pwzCustomActionData, COST_IIS_DELETEAPP);
    ExitOnFailure2(hr, "Failed to add ScaDeleteApp action data: %S cost: %d", pwzCustomActionData, COST_IIS_DELETEAPP);

LExit:
    ReleaseStr(pwzCustomActionData);

    return hr;
}


HRESULT ScaCreateApp(IMSAdminBase* piMetabase, LPCWSTR wzWebRoot, 
                     DWORD dwIsolation)
{
    Assert(piMetabase);
    Unused(piMetabase);

    HRESULT hr = S_OK;
    WCHAR wzKey[METADATA_MAX_NAME_LEN];
    BOOL fInProc = FALSE;

    WCHAR* pwzCustomActionData = NULL;

    hr = WcaWriteIntegerToCaData(MBA_CREATEAPP, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase create app directive to CustomActionData");

    hr = ::StringCchCopyW(wzKey, countof(wzKey), wzWebRoot);
    ExitOnFailure(hr, "Failed to copy webroot string to key");
    hr = WcaWriteStringToCaData(wzKey, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase key to CustomActionData");

    if (0 == dwIsolation)
        fInProc = TRUE;
    else
        fInProc = FALSE;

    hr = WcaWriteIntegerToCaData(fInProc, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add isolation value to CustomActionData");

    hr = ScaAddToMetabaseConfiguration(pwzCustomActionData, COST_IIS_CREATEAPP);
    ExitOnFailure2(hr, "Failed to add ScaCreateApp action data: %S cost: %d", pwzCustomActionData, COST_IIS_CREATEAPP);

LExit:
    ReleaseStr(pwzCustomActionData);

    return hr;
}


HRESULT ScaCreateMetabaseKey(IMSAdminBase* piMetabase, LPCWSTR wzRootKey, 
                             LPCWSTR wzSubKey)
{
    Assert(piMetabase);
    Unused(piMetabase);

    HRESULT hr = S_OK;
    WCHAR wzKey[METADATA_MAX_NAME_LEN];
    WCHAR* pwzCustomActionData = NULL;

    hr = ::StringCchCopyW(wzKey, countof(wzKey), wzRootKey);
    ExitOnFailure(hr, "Failed to copy root key string to key");
    if (L'/' != *(wzKey + lstrlenW(wzRootKey)))
    {
        hr = ::StringCchCatW(wzKey, countof(wzKey), L"/");
        ExitOnFailure(hr, "Failed to concatenate / to key string");
    }
    if (wzSubKey && *wzSubKey)
    {
        if (L'/' == *wzSubKey)
        {
            hr = ::StringCchCatW(wzKey, countof(wzKey), wzSubKey + 1);
            ExitOnFailure(hr, "Failed to concatenate subkey (minus slash) to key string");
        }
        else
        {
            hr = ::StringCchCatW(wzKey, countof(wzKey), wzSubKey);
            ExitOnFailure(hr, "Failed to concatenate subkey to key string");
        }
    }

    hr = WcaWriteIntegerToCaData(MBA_CREATEKEY, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase delete key directive to CustomActionData");

    hr = WcaWriteStringToCaData(wzKey, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase key to CustomActionData");

    hr = ScaAddToMetabaseConfiguration(pwzCustomActionData, COST_IIS_CREATEKEY);
    ExitOnFailure2(hr, "Failed to add ScaCreateMetabaseKey action data: %S cost: %d", pwzCustomActionData, COST_IIS_CREATEKEY);

LExit:
    ReleaseStr(pwzCustomActionData);

    return hr;
}


HRESULT ScaDeleteMetabaseKey(IMSAdminBase* piMetabase, LPCWSTR wzRootKey, 
                             LPCWSTR wzSubKey)
{
    Assert(piMetabase);
    Unused(piMetabase);

    HRESULT hr = S_OK;
    WCHAR wzKey[METADATA_MAX_NAME_LEN];
    WCHAR* pwzCustomActionData = NULL;

    hr = ::StringCchCopyW(wzKey, countof(wzKey), wzRootKey);
    ExitOnFailure(hr, "Failed to copy root key string to key");
    if (L'/' != *(wzKey + lstrlenW(wzRootKey)))
    {
        hr = ::StringCchCatW(wzKey, countof(wzKey), L"/");
        ExitOnFailure(hr, "Failed to concatenate / to key string");
    }
    if (*wzSubKey)
    {
        if (L'/' == *wzSubKey)
        {
            hr = ::StringCchCatW(wzKey, countof(wzKey), wzSubKey + 1);
            ExitOnFailure(hr, "Failed to concatenate subkey (minus slash) to key string");
        }
        else
        {
            hr = ::StringCchCatW(wzKey, countof(wzKey), wzSubKey);
            ExitOnFailure(hr, "Failed to concatenate subkey to key string");
        }
    }

    hr = WcaWriteIntegerToCaData(MBA_DELETEKEY, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase delete key directive to CustomActionData");

    hr = WcaWriteStringToCaData(wzKey, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase key to CustomActionData");

    hr = ScaAddToMetabaseConfiguration(pwzCustomActionData, COST_IIS_DELETEKEY);
    ExitOnFailure2(hr, "Failed to add ScaDeleteMetabaseKey action data: %S cost: %d", pwzCustomActionData, COST_IIS_DELETEKEY);

LExit:
    ReleaseStr(pwzCustomActionData);

    return hr;
}


HRESULT ScaDeleteMetabaseValue(IMSAdminBase* piMetabase, LPCWSTR wzRootKey, 
                              LPCWSTR wzSubKey, DWORD dwIdentifier,
                              DWORD dwDataType)
{
    Assert(piMetabase);
    Unused(piMetabase);

    HRESULT hr = S_OK;
    WCHAR wzKey[METADATA_MAX_NAME_LEN];
    WCHAR* pwzCustomActionData = NULL;

    hr = ::StringCchCopyW(wzKey, countof(wzKey), wzRootKey);
    ExitOnFailure(hr, "Failed to copy root key string to key");
    if (L'/' != *(wzKey + lstrlenW(wzRootKey)))
    {
        hr = ::StringCchCatW(wzKey, countof(wzKey), L"/");
        ExitOnFailure(hr, "Failed to concatenate / to key string");
    }

    if (wzSubKey && *wzSubKey)
    {
        if (L'/' == *wzSubKey)
        {
            hr = ::StringCchCatW(wzKey, countof(wzKey), wzSubKey + 1);
            ExitOnFailure(hr, "Failed to concatenate subkey (minus slash) to key string");
        }
        else
        {
            hr = ::StringCchCatW(wzKey, countof(wzKey), wzSubKey);
            ExitOnFailure(hr, "Failed to concatenate subkey to key string");
        }
    }

    hr = WcaWriteIntegerToCaData(MBA_DELETEVALUE, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase write value directive to CustomActionData");

    hr = WcaWriteStringToCaData(wzKey, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase key to CustomActionData");

    hr = WcaWriteIntegerToCaData(dwIdentifier, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase identifier to CustomActionData");

    hr = WcaWriteIntegerToCaData(dwDataType, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase data type to CustomActionData");

    hr = ScaAddToMetabaseConfiguration(pwzCustomActionData, COST_IIS_DELETEVALUE);
    ExitOnFailure2(hr, "Failed to add ScaDeleteMetabaseValue action data: %S, cost: %d", pwzCustomActionData, COST_IIS_DELETEVALUE);

LExit:
    ReleaseStr(pwzCustomActionData);

    return hr;
}


#pragma prefast(push)
#pragma prefast(disable:25120) // Disable "requires count parameter" warning - we do have a way to distinguish buffer sizes in all situations,
                               // it just depends on the dwDataType, and there's no way to annotate the situation in SAL
HRESULT ScaWriteMetabaseValue(IMSAdminBase* piMetabase, LPCWSTR wzRootKey, 
                              LPCWSTR wzSubKey, DWORD dwIdentifier, 
                              DWORD dwAttributes, DWORD dwUserType, 
                              DWORD dwDataType, LPVOID pvData)
#pragma prefast(pop)
{
    Assert(piMetabase && (pvData || (DWORD_METADATA == dwDataType)));    // pvData may be 0 if it is DWORD data
    Unused(piMetabase);

    HRESULT hr = S_OK;
    WCHAR wzKey[METADATA_MAX_NAME_LEN];
    WCHAR* pwzCustomActionData = NULL;

    if (BINARY_METADATA == dwDataType)
    {
        ExitOnNull(pvData, hr, E_INVALIDARG, "Failed to write binary metadata - no data available to write");
    }

    hr = ::StringCchCopyW(wzKey, countof(wzKey), wzRootKey);
    ExitOnFailure(hr, "Failed to copy root key string to key");
    if (L'/' != *(wzKey + lstrlenW(wzRootKey)))
    {
        hr = ::StringCchCatW(wzKey, countof(wzKey), L"/");
        ExitOnFailure(hr, "Failed to concatenate / to key string");
    }
    if (wzSubKey && *wzSubKey)
    {
        if (L'/' == *wzSubKey)
        {
            hr = ::StringCchCatW(wzKey, countof(wzKey), wzSubKey + 1);
            ExitOnFailure(hr, "Failed to concatenate subkey (minus slash) to key string");
        }
        else
        {
            hr = ::StringCchCatW(wzKey, countof(wzKey), wzSubKey);
            ExitOnFailure(hr, "Failed to concatenate subkey to key string");
        }
    }

    hr = WcaWriteIntegerToCaData(MBA_WRITEVALUE, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase write value directive to CustomActionData");

    hr = WcaWriteStringToCaData(wzKey, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase key to CustomActionData");

    hr = WcaWriteIntegerToCaData(dwIdentifier, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase identifier to CustomActionData");

    hr = WcaWriteIntegerToCaData(dwAttributes, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase attributes to CustomActionData");

    hr = WcaWriteIntegerToCaData(dwUserType, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase user type to CustomActionData");

    hr = WcaWriteIntegerToCaData(dwDataType, &pwzCustomActionData);
    ExitOnFailure(hr, "Failed to add metabase data type to CustomActionData");

    switch (dwDataType)
    {
    case DWORD_METADATA:
        hr = WcaWriteIntegerToCaData((DWORD)((DWORD_PTR)pvData), &pwzCustomActionData);
        break;
    case STRING_METADATA:
        hr = WcaWriteStringToCaData((LPCWSTR)pvData, &pwzCustomActionData);
        break;
    case MULTISZ_METADATA:
        {
        // change NULLs to unprintable character  to create a 'safe' MULTISZ string
        LPWSTR pwz = (LPWSTR)pvData;
        for (;;)
        {
            if ('\0' == *pwz)
            {
                *pwz = MAGIC_MULTISZ_CHAR;
                if ('\0' == *(pwz + 1))    // second null back to back means end of string
                    break;
            }

            pwz++;
        }

        hr = WcaWriteStringToCaData((LPCWSTR)pvData, &pwzCustomActionData);
        }
        break;
    case BINARY_METADATA:
        hr = WcaWriteStreamToCaData(((BLOB*) pvData)->pBlobData, ((BLOB*) pvData)->cbSize, &pwzCustomActionData);
        break;
    default:
        hr = E_UNEXPECTED;
    }
    ExitOnFailure(hr, "Failed to add metabase data to CustomActionData");

    // TODO: maybe look the key up and make sure we're not just writing the same value that already there

    hr = ScaAddToMetabaseConfiguration(pwzCustomActionData, COST_IIS_WRITEVALUE);
    ExitOnFailure2(hr, "Failed to add ScaWriteMetabaseValue action data: %S, cost: %d", pwzCustomActionData, COST_IIS_WRITEVALUE);

LExit:
    ReleaseStr(pwzCustomActionData);

    return hr;
}


static HRESULT ScaAddToMetabaseConfiguration(LPCWSTR pwzData, DWORD dwCost)
{
    HRESULT hr = S_OK;

    hr = WcaWriteStringToCaData(pwzData, &vpwzCustomActionData);
    ExitOnFailure1(hr, "failed to add to metabase configuration data string: %S", pwzData);

    vdwCustomActionCost += dwCost;

LExit:
    return hr;
}


HRESULT ScaWriteMetabaseConfigurationScript(__in LPCWSTR pwzCaScriptKey)
{
    HRESULT hr = S_OK;
    WCA_CASCRIPT_HANDLE hWriteMetabase = NULL;

    // Create CaScript for communication with WriteMetabaseChanges
    hr = WcaCaScriptCreate(WCA_ACTION_INSTALL, WCA_CASCRIPT_SCHEDULED, FALSE, pwzCaScriptKey, FALSE, &hWriteMetabase);
    ExitOnFailure(hr, "Failed to write ca script for WriteMetabaseChanges script.");

    if (vpwzCustomActionData && *vpwzCustomActionData)
    {
        // Write the actual custom action data to the ca script
        WcaCaScriptWriteString(hWriteMetabase, vpwzCustomActionData);

        WcaLog(LOGMSG_TRACEONLY, "Custom action data being written to ca script: %S", vpwzCustomActionData);
    }
    else
        hr = S_FALSE;

LExit:
    // Release the string
    ReleaseStr(vpwzCustomActionData);

    // Flush the ca script to disk as best we can
    WcaCaScriptFlush(hWriteMetabase);

    WcaCaScriptClose(hWriteMetabase, WCA_CASCRIPT_CLOSE_PRESERVE);

    return hr;
}


HRESULT ScaLoadMetabase(IMSAdminBase** ppiMetabase)
{
    HRESULT hr = S_OK;
    UINT er = ERROR_SUCCESS;

    // if IIS was uninstalled (thus no IID_IMSAdminBase) allow the
    // user to still uninstall this package by clicking "Ignore"
    do
    {
        hr = ::CoCreateInstance(CLSID_MSAdminBase, NULL, CLSCTX_ALL, IID_IMSAdminBase, (void**)ppiMetabase); 
        if (FAILED(hr))
        {
            WcaLog(LOGMSG_STANDARD, "failed to get IID_IMSAdminBase Object");
            er = WcaErrorMessage(msierrIISCannotConnect, hr, INSTALLMESSAGE_ERROR | MB_ABORTRETRYIGNORE, 0);
            switch (er)
            {
            case IDABORT:
                ExitFunction();   // bail with the error result from the CoCreate to kick off a rollback
            case IDRETRY:
                hr = S_FALSE;   // hit me, baby, one more time
                break;
            case IDIGNORE:
                hr = S_OK;  // pretend everything is okay and bail
            }
        }
    } while (S_FALSE == hr);

LExit:
    return hr;
}