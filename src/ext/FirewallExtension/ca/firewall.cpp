//-------------------------------------------------------------------------------------------------
// <copyright file="firewall.cpp" company="Microsoft">
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
//    Firewall custom action code.
// </summary>
//-------------------------------------------------------------------------------------------------

#include "precomp.h"

LPCWSTR vcsFirewallExceptionQuery =
    L"SELECT `Name`, `RemoteAddresses`, `Port`, `Protocol`, `Program`, `Attributes`, `Component_` FROM `WixFirewallException`";
enum eFirewallExceptionQuery { feqName = 1, feqRemoteAddresses, feqPort, feqProtocol, feqProgram, feqAttributes, feqComponent };
enum eFirewallExceptionTarget { fetPort = 1, fetApplication, fetUnknown };
enum eFirewallExceptionAttributes { feaIgnoreFailures = 1 };



/******************************************************************
 SchedFirewallExceptions - immediate custom action worker to 
   register and remove firewall exceptions.

********************************************************************/
static UINT SchedFirewallExceptions(
    __in MSIHANDLE hInstall,
    WCA_TODO todoSched
    )
{
    HRESULT hr = S_OK;
    UINT er = ERROR_SUCCESS;
    int cFirewallExceptions = 0;

    PMSIHANDLE hView = NULL;
    PMSIHANDLE hRec = NULL;

    LPWSTR pwzCustomActionData = NULL;
    LPWSTR pwzName = NULL;
    LPWSTR pwzRemoteAddresses = NULL;
    int iPort = 0;
    int iProtocol = 0;
    int iAttributes = 0;
    LPWSTR pwzProgram = NULL;
    LPWSTR pwzComponent = NULL;
    LPWSTR pwzFormattedFile = NULL;

    // initialize
    hr = WcaInitialize(hInstall, "SchedFirewallExceptions");
    ExitOnFailure(hr, "failed to initialize");

    // anything to do?
    if (S_OK != WcaTableExists(L"WixFirewallException"))
    {
        WcaLog(LOGMSG_STANDARD, "WixFirewallException table doesn't exist, so there are no firewall exceptions to configure.");
        ExitFunction();
    }

    // query and loop through all the firewall exceptions
    hr = WcaOpenExecuteView(vcsFirewallExceptionQuery, &hView);
    ExitOnFailure(hr, "failed to open view on WixFirewallException table");

    while (S_OK == (hr = WcaFetchRecord(hView, &hRec)))
    {
        hr = WcaGetRecordFormattedString(hRec, feqName, &pwzName);
        ExitOnFailure(hr, "failed to get firewall exception name");

        hr = WcaGetRecordFormattedString(hRec, feqRemoteAddresses, &pwzRemoteAddresses);
        ExitOnFailure(hr, "failed to get firewall exception remote addresses");

        hr = WcaGetRecordFormattedInteger(hRec, feqPort, &iPort);
        ExitOnFailure(hr, "failed to get firewall exception port");

        hr = WcaGetRecordInteger(hRec, feqProtocol, &iProtocol);
        ExitOnFailure(hr, "failed to get firewall exception protocol");

        hr = WcaGetRecordFormattedString(hRec, feqProgram, &pwzProgram);
        ExitOnFailure(hr, "failed to get firewall exception program");

        hr = WcaGetRecordInteger(hRec, feqAttributes, &iAttributes);
        ExitOnFailure(hr, "failed to get firewall exception attributes");

        hr = WcaGetRecordString(hRec, feqComponent, &pwzComponent);
        ExitOnFailure(hr, "failed to get firewall exception component");

        // figure out what we're doing for this exception, treating reinstall the same as install
        WCA_TODO todoComponent = WcaGetComponentToDo(pwzComponent);
        if ((WCA_TODO_REINSTALL == todoComponent ? WCA_TODO_INSTALL : todoComponent) != todoSched)
        {
            WcaLog(LOGMSG_STANDARD, "Component '%S' action state (%d) doesn't match request (%d)", pwzComponent, todoComponent, todoSched);
            continue;
        }

        // action :: name :: remoteaddresses :: attributes :: target :: {port::protocol | path}
        ++cFirewallExceptions;
        hr = WcaWriteIntegerToCaData(todoComponent, &pwzCustomActionData);
        ExitOnFailure(hr, "failed to write exception action to custom action data");

        hr = WcaWriteStringToCaData(pwzName, &pwzCustomActionData);
        ExitOnFailure(hr, "failed to write exception name to custom action data");

        hr = WcaWriteStringToCaData(pwzRemoteAddresses, &pwzCustomActionData);
        ExitOnFailure(hr, "failed to write exception remote addresses to custom action data");

        hr = WcaWriteIntegerToCaData(iAttributes, &pwzCustomActionData);
        ExitOnFailure(hr, "failed to write exception attributes to custom action data");

        if (MSI_NULL_INTEGER == iPort || MSI_NULL_INTEGER == iProtocol)
        {
            // without port and protocol, we have an application exception.
            hr = WcaWriteIntegerToCaData(fetApplication, &pwzCustomActionData);
            ExitOnFailure(hr, "failed to write exception target (application) to custom action data");

            hr = WcaWriteStringToCaData(pwzProgram, &pwzCustomActionData);
            ExitOnFailure(hr, "failed to write application path to custom action data");
        }
        else
        {
            // we have a port-based exception
            hr = WcaWriteIntegerToCaData(fetPort, &pwzCustomActionData);
            ExitOnFailure(hr, "failed to write exception target (port) to custom action data");

            hr = WcaWriteIntegerToCaData(iPort, &pwzCustomActionData);
            ExitOnFailure(hr, "failed to write exception port to custom action data");

            hr = WcaWriteIntegerToCaData(iProtocol, &pwzCustomActionData);
            ExitOnFailure(hr, "failed to write exception protocol to custom action data");
        }
    }

    // reaching the end of the list is actually a good thing, not an error
    if (E_NOMOREITEMS == hr)
    {
        hr = S_OK;
    }
    ExitOnFailure(hr, "failure occured while processing WixFirewallException table");

    // schedule ExecFirewallExceptions if there's anything to do
    if (pwzCustomActionData && *pwzCustomActionData)
    {
        WcaLog(LOGMSG_STANDARD, "Scheduling firewall exception (%S)", pwzCustomActionData);

        if (WCA_TODO_INSTALL == todoSched)
        {
            hr = WcaDoDeferredAction(L"WixRollbackFirewallExceptionsInstall", pwzCustomActionData, cFirewallExceptions * COST_FIREWALL_EXCEPTION);
            ExitOnFailure(hr, "failed to schedule firewall install exceptions rollback");
            hr = WcaDoDeferredAction(L"WixExecFirewallExceptionsInstall", pwzCustomActionData, cFirewallExceptions * COST_FIREWALL_EXCEPTION);
            ExitOnFailure(hr, "failed to schedule firewall install exceptions execution");
        }
        else
        {
            hr = WcaDoDeferredAction(L"WixRollbackFirewallExceptionsUninstall", pwzCustomActionData, cFirewallExceptions * COST_FIREWALL_EXCEPTION);
            ExitOnFailure(hr, "failed to schedule firewall uninstall exceptions rollback");
            hr = WcaDoDeferredAction(L"WixExecFirewallExceptionsUninstall", pwzCustomActionData, cFirewallExceptions * COST_FIREWALL_EXCEPTION);
            ExitOnFailure(hr, "failed to schedule firewall uninstall exceptions execution");
        }
    }
    else
    {
        WcaLog(LOGMSG_STANDARD, "No firewall exceptions scheduled");
    }

LExit:
    ReleaseStr(pwzCustomActionData);
    ReleaseStr(pwzName);
    ReleaseStr(pwzRemoteAddresses);
    ReleaseStr(pwzProgram);
    ReleaseStr(pwzComponent);
    ReleaseStr(pwzFormattedFile);

    return WcaFinalize(er = FAILED(hr) ? ERROR_INSTALL_FAILURE : er);
}

/******************************************************************
 SchedFirewallExceptionsInstall - immediate custom action entry
   point to register firewall exceptions.

********************************************************************/
extern "C" UINT __stdcall SchedFirewallExceptionsInstall(
    __in MSIHANDLE hInstall
    )
{
    return SchedFirewallExceptions(hInstall, WCA_TODO_INSTALL);
}

/******************************************************************
 SchedFirewallExceptionsUninstall - immediate custom action entry
   point to remove firewall exceptions.

********************************************************************/
extern "C" UINT __stdcall SchedFirewallExceptionsUninstall(
    __in MSIHANDLE hInstall
    )
{
    return SchedFirewallExceptions(hInstall, WCA_TODO_UNINSTALL);
}



/******************************************************************
 GetFirewallProfile - get the active firewall profile as an
   INetFwProfile, which owns the lists of exceptions we're 
   updating.
********************************************************************/
static HRESULT GetFirewallProfile(
    __in BOOL fIgnoreFailures,
    __out INetFwProfile** ppfwProfile
    )
{
    HRESULT hr = S_OK;
    INetFwMgr* pfwMgr = NULL;
    INetFwPolicy* pfwPolicy = NULL;
    INetFwProfile* pfwProfile = NULL;
    
    do
    {
        ReleaseNullObject(pfwPolicy);
        ReleaseNullObject(pfwMgr);
        ReleaseNullObject(pfwProfile);

        if (SUCCEEDED(hr = ::CoCreateInstance(__uuidof(NetFwMgr), NULL, CLSCTX_INPROC_SERVER, __uuidof(INetFwMgr), (void**)&pfwMgr)) &&
            SUCCEEDED(hr = pfwMgr->get_LocalPolicy(&pfwPolicy)) &&
            SUCCEEDED(hr = pfwPolicy->get_CurrentProfile(&pfwProfile)))
        {
            break;
        }
        else if (fIgnoreFailures)
        {
            ExitFunction1(hr = S_FALSE);
        }
        else
        {
            WcaLog(LOGMSG_STANDARD, "Failed to connect to Windows Firewall");
            UINT er = WcaErrorMessage(msierrFirewallCannotConnect, hr, INSTALLMESSAGE_ERROR | MB_ABORTRETRYIGNORE, 0);
            switch (er)
            {
            case IDABORT: // exit with the current HRESULT
                ExitFunction();
            case IDRETRY: // clean up and retry the loop
                hr = S_FALSE;
                break;
            case IDIGNORE: // pass S_FALSE back to the caller, who knows how to ignore the failure
                ExitFunction1(hr = S_FALSE);
            }
        }
    } while (S_FALSE == hr);

    *ppfwProfile = pfwProfile;
    pfwProfile = NULL;
    
LExit:
    ReleaseObject(pfwPolicy);
    ReleaseObject(pfwMgr);
    ReleaseObject(pfwProfile);

    return hr;
}

/******************************************************************
 AddApplicationException

********************************************************************/
static HRESULT AddApplicationException(
    __in LPCWSTR wzFile, 
    __in LPCWSTR wzName, 
    __in_opt LPCWSTR wzRemoteAddresses,
    __in BOOL fIgnoreFailures
    )
{
    HRESULT hr = S_OK;
    BSTR bstrFile = NULL;
    BSTR bstrName = NULL;
    BSTR bstrRemoteAddresses = NULL;
    INetFwProfile* pfwProfile = NULL;
    INetFwAuthorizedApplications* pfwApps = NULL;
    INetFwAuthorizedApplication* pfwApp = NULL;

    // convert to BSTRs to make COM happy
    bstrFile = ::SysAllocString(wzFile);
    ExitOnNull(bstrFile, hr, E_OUTOFMEMORY, "failed SysAllocString for path");
    bstrName = ::SysAllocString(wzName);
    ExitOnNull(bstrName, hr, E_OUTOFMEMORY, "failed SysAllocString for name");
    bstrRemoteAddresses = ::SysAllocString(wzRemoteAddresses);
    ExitOnNull(bstrRemoteAddresses, hr, E_OUTOFMEMORY, "failed SysAllocString for remote addresses");

    // get the firewall profile, which is our entry point for adding exceptions
    hr = GetFirewallProfile(fIgnoreFailures, &pfwProfile);
    ExitOnFailure(hr, "failed to get firewall profile");
    if (S_FALSE == hr) // user or package author chose to ignore missing firewall
    {
        ExitFunction();
    }

    // first, let's see if the app is already on the exception list
    hr = pfwProfile->get_AuthorizedApplications(&pfwApps);
    ExitOnFailure(hr, "failed to get list of authorized apps");

    // try to find it (i.e., support reinstall)
    hr = pfwApps->Item(bstrFile, &pfwApp);
    if (HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND) == hr)
    {
        // not found, so we get to add it
        hr = ::CoCreateInstance(__uuidof(NetFwAuthorizedApplication), NULL, CLSCTX_INPROC_SERVER, __uuidof(INetFwAuthorizedApplication), reinterpret_cast<void**>(&pfwApp));
        ExitOnFailure(hr, "failed to create authorized app");

        // set the display name
        hr = pfwApp->put_Name(bstrName);
        ExitOnFailure(hr, "failed to set authorized app name");

        // set path
        hr = pfwApp->put_ProcessImageFileName(bstrFile);
        ExitOnFailure(hr, "failed to set authorized app path");

        // set the allowed remote addresses
        if (bstrRemoteAddresses && *bstrRemoteAddresses)
        {
            hr = pfwApp->put_RemoteAddresses(bstrRemoteAddresses);
            ExitOnFailure(hr, "failed to set authorized app remote addresses");
        }

        // add it to the list of authorized apps
        hr = pfwApps->Add(pfwApp);
        ExitOnFailure(hr, "failed to add app to the authorized apps list");
    }
    else
    {
        // we found an existing app exception (if we succeeded, that is)
        ExitOnFailure(hr, "failed trying to find existing app");

        // enable it (just in case it was disabled)
        pfwApp->put_Enabled(VARIANT_TRUE);
    }

LExit:
    ReleaseBSTR(bstrRemoteAddresses);
    ReleaseBSTR(bstrName);
    ReleaseBSTR(bstrFile);
    ReleaseObject(pfwApp);
    ReleaseObject(pfwApps);
    ReleaseObject(pfwProfile);

    return fIgnoreFailures ? S_OK : hr;
}

/******************************************************************
 RemoveApplicationException

********************************************************************/
static HRESULT RemoveApplicationException(
    __in LPCWSTR wzFile, 
    __in BOOL fIgnoreFailures
    )
{
    HRESULT hr = S_OK;
    INetFwProfile* pfwProfile = NULL;
    INetFwAuthorizedApplications* pfwApps = NULL;

    // convert to BSTRs to make COM happy
    BSTR bstrFile = ::SysAllocString(wzFile);
    ExitOnNull(bstrFile, hr, E_OUTOFMEMORY, "failed SysAllocString for path");

    // get the firewall profile, which is our entry point for removing exceptions
    hr = GetFirewallProfile(fIgnoreFailures, &pfwProfile);
    ExitOnFailure(hr, "failed to get firewall profile");
    if (S_FALSE == hr) // user or package author chose to ignore missing firewall
    {
        ExitFunction();
    }

    // now get the list of app exceptions and remove the one
    hr = pfwProfile->get_AuthorizedApplications(&pfwApps);
    ExitOnFailure(hr, "failed to get list of authorized apps");

    hr = pfwApps->Remove(bstrFile);
    ExitOnFailure(hr, "failed to remove authorized app");

LExit:
    ReleaseBSTR(bstrFile);
    ReleaseObject(pfwApps);
    ReleaseObject(pfwProfile);

    return fIgnoreFailures ? S_OK : hr;
}

/******************************************************************
 AddPortException

********************************************************************/
static HRESULT AddPortException(
    __in LPCWSTR wzName,
    __in_opt LPCWSTR wzRemoteAddresses,
    __in BOOL fIgnoreFailures,
    __in int iPort,
    __in int iProtocol
    )
{
    HRESULT hr = S_OK;
    BSTR bstrName = NULL;
    BSTR bstrRemoteAddresses = NULL;
    INetFwProfile* pfwProfile = NULL;
    INetFwOpenPorts* pfwPorts = NULL;
    INetFwOpenPort* pfwPort = NULL;

    // convert to BSTRs to make COM happy
    bstrName = ::SysAllocString(wzName);
    ExitOnNull(bstrName, hr, E_OUTOFMEMORY, "failed SysAllocString for name");
    bstrRemoteAddresses = ::SysAllocString(wzRemoteAddresses);
    ExitOnNull(bstrRemoteAddresses, hr, E_OUTOFMEMORY, "failed SysAllocString for remote addresses");

    // create and initialize a new open port object
    hr = ::CoCreateInstance(__uuidof(NetFwOpenPort), NULL, CLSCTX_INPROC_SERVER, __uuidof(INetFwOpenPort), reinterpret_cast<void**>(&pfwPort));
    ExitOnFailure(hr, "failed to create new open port");

    hr = pfwPort->put_Port(iPort);
    ExitOnFailure(hr, "failed to set exception port");

    hr = pfwPort->put_Protocol(static_cast<NET_FW_IP_PROTOCOL>(iProtocol));
    ExitOnFailure(hr, "failed to set exception protocol");

    if (bstrRemoteAddresses && *bstrRemoteAddresses)
    {
        hr = pfwPort->put_RemoteAddresses(bstrRemoteAddresses);
        ExitOnFailure1(hr, "failed to set exception remote addresses '%S'", bstrRemoteAddresses);
    }

    hr = pfwPort->put_Name(bstrName);
    ExitOnFailure(hr, "failed to set exception name");

    // get the firewall profile, its current list of open ports, and add ours
    hr = GetFirewallProfile(fIgnoreFailures, &pfwProfile);
    ExitOnFailure(hr, "failed to get firewall profile");
    if (S_FALSE == hr) // user or package author chose to ignore missing firewall
    {
        ExitFunction();
    }

    hr = pfwProfile->get_GloballyOpenPorts(&pfwPorts);
    ExitOnFailure(hr, "failed to get open ports");

    hr = pfwPorts->Add(pfwPort);
    ExitOnFailure(hr, "failed to add exception to global list");

LExit:
    ReleaseBSTR(bstrRemoteAddresses);
    ReleaseBSTR(bstrName);
    ReleaseObject(pfwProfile);
    ReleaseObject(pfwPorts);
    ReleaseObject(pfwPort);

    return fIgnoreFailures ? S_OK : hr;
}

/******************************************************************
 RemovePortException

********************************************************************/
static HRESULT RemovePortException(
    __in int iPort,
    __in int iProtocol,
    __in BOOL fIgnoreFailures
    )
{
    HRESULT hr = S_OK;
    INetFwProfile* pfwProfile = NULL;
    INetFwOpenPorts* pfwPorts = NULL;

    // get the firewall profile, which is our entry point for adding exceptions
    hr = GetFirewallProfile(fIgnoreFailures, &pfwProfile);
    ExitOnFailure(hr, "failed to get firewall profile");
    if (S_FALSE == hr) // user or package author chose to ignore missing firewall
    {
        ExitFunction();
    }

    hr = pfwProfile->get_GloballyOpenPorts(&pfwPorts);
    ExitOnFailure(hr, "failed to get open ports");

    hr = pfwPorts->Remove(iPort, static_cast<NET_FW_IP_PROTOCOL>(iProtocol));
    ExitOnFailure2(hr, "failed to remove open port %d, protocol %d", iPort, iProtocol);

LExit:
    return fIgnoreFailures ? S_OK : hr;
}



/******************************************************************
 ExecFirewallExceptions - deferred custom action entry point to 
   register and remove firewall exceptions.

********************************************************************/
extern "C" UINT __stdcall ExecFirewallExceptions(
    __in MSIHANDLE hInstall
    )
{
    HRESULT hr = S_OK;
    LPWSTR pwz = NULL;
    LPWSTR pwzCustomActionData = NULL;
    int iTodo = WCA_TODO_UNKNOWN;
    LPWSTR pwzName = NULL;
    LPWSTR pwzRemoteAddresses = NULL;
    int iAttributes = 0;
    int iTarget = fetUnknown;
    LPWSTR pwzFile = NULL;
    int iPort = 0;
    int iProtocol = 0;

    // initialize
    hr = WcaInitialize(hInstall, "ExecFirewallExceptions");
    ExitOnFailure(hr, "failed to initialize");

    hr = WcaGetProperty( L"CustomActionData", &pwzCustomActionData);
    ExitOnFailure(hr, "failed to get CustomActionData");
    WcaLog(LOGMSG_TRACEONLY, "CustomActionData: %S", pwzCustomActionData);

    hr = ::CoInitialize(NULL);
    ExitOnFailure(hr, "failed to initialize COM");

    // loop through all the passed in data
    pwz = pwzCustomActionData;
    while (pwz && *pwz)
    {
        // extract the custom action data and if rolling back, swap INSTALL and UNINSTALL
        hr = WcaReadIntegerFromCaData(&pwz, &iTodo);
        ExitOnFailure(hr, "failed to read todo from custom action data");
        if (::MsiGetMode(hInstall, MSIRUNMODE_ROLLBACK))
        {
            if (WCA_TODO_INSTALL == iTodo)
            {
                iTodo = WCA_TODO_UNINSTALL;
            }
            else if (WCA_TODO_UNINSTALL == iTodo)
            {
                iTodo = WCA_TODO_INSTALL;
            }
        }

        hr = WcaReadStringFromCaData(&pwz, &pwzName);
        ExitOnFailure(hr, "failed to read name from custom action data");

        hr = WcaReadStringFromCaData(&pwz, &pwzRemoteAddresses);
        ExitOnFailure(hr, "failed to read remote addresses from custom action data");

        hr = WcaReadIntegerFromCaData(&pwz, &iAttributes);
        ExitOnFailure(hr, "failed to read attributes from custom action data");
        BOOL fIgnoreFailures = feaIgnoreFailures == (iAttributes & feaIgnoreFailures);

        hr = WcaReadIntegerFromCaData(&pwz, &iTarget);
        ExitOnFailure(hr, "failed to read target from custom action data");

        switch (iTarget)
        {
        case fetPort:
            hr = WcaReadIntegerFromCaData(&pwz, &iPort);
            ExitOnFailure(hr, "failed to read port from custom action data");
            hr = WcaReadIntegerFromCaData(&pwz, &iProtocol);
            ExitOnFailure(hr, "failed to read protocol from custom action data");

            switch (iTodo)
            {
            case WCA_TODO_INSTALL:
            case WCA_TODO_REINSTALL:
                WcaLog(LOGMSG_STANDARD, "Installing firewall exception %S on port %d, protocol %d", pwzName, iPort, iProtocol);
                hr = AddPortException(pwzName, pwzRemoteAddresses, fIgnoreFailures, iPort, iProtocol);
                ExitOnFailure3(hr, "failed to add/update port exception for name '%S' on port %d, protocol %d", pwzName, iPort, iProtocol);
                break;

            case WCA_TODO_UNINSTALL:
                WcaLog(LOGMSG_STANDARD, "Uninstalling firewall exception %S on port %d, protocol %d", pwzName, iPort, iProtocol);
                hr = RemovePortException(iPort, iProtocol, fIgnoreFailures);
                ExitOnFailure3(hr, "failed to remove port exception for name '%S' on port %d, protocol %d", pwzName, iPort, iProtocol);
                break;
            }
            break;

        case fetApplication:
            hr = WcaReadStringFromCaData(&pwz, &pwzFile);
            ExitOnFailure(hr, "failed to read file path from custom action data");

            switch (iTodo)
            {
            case WCA_TODO_INSTALL:
            case WCA_TODO_REINSTALL:
                WcaLog(LOGMSG_STANDARD, "Installing firewall exception %S (%S)", pwzName, pwzFile);
                hr = AddApplicationException(pwzFile, pwzName, pwzRemoteAddresses, fIgnoreFailures);
                ExitOnFailure2(hr, "failed to add/update application exception for name '%S', file '%S'", pwzName, pwzFile);
                break;

            case WCA_TODO_UNINSTALL:
                WcaLog(LOGMSG_STANDARD, "Uninstalling firewall exception %S (%S)", pwzName, pwzFile);
                hr = RemoveApplicationException(pwzFile, fIgnoreFailures);
                ExitOnFailure2(hr, "failed to remove application exception for name '%S', file '%S'", pwzName, pwzFile);
                break;
            }
            break;
        }
    }

LExit:
    ReleaseStr(pwzCustomActionData);
    ReleaseStr(pwzName);
    ReleaseStr(pwzRemoteAddresses);
    ReleaseStr(pwzFile);
    ::CoUninitialize();

    return WcaFinalize(FAILED(hr) ? ERROR_INSTALL_FAILURE : ERROR_SUCCESS);
}