//-------------------------------------------------------------------------------------------------
// <copyright file="preq.cpp" company="Microsoft">
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
// MBA prerequisite bootstrapper application.
// </summary>
//-------------------------------------------------------------------------------------------------

#include "precomp.h"

HINSTANCE vhInstance = NULL;

extern "C" BOOL WINAPI DllMain(
    IN HINSTANCE hInstance,
    IN DWORD dwReason,
    IN LPVOID /* pvReserved */
    )
{
    switch(dwReason)
    {
    case DLL_PROCESS_ATTACH:
        ::DisableThreadLibraryCalls(hInstance);
        vhInstance = hInstance;
        break;

    case DLL_PROCESS_DETACH:
        vhInstance = NULL;
        break;
    }

    return TRUE;
}


extern "C" HRESULT WINAPI BootstrapperApplicationCreate(
    __in IBootstrapperEngine* pEngine,
    __in const BOOTSTRAPPER_COMMAND* pCommand,
    __out IBootstrapperApplication** ppApplication
    )
{
    HRESULT hr = S_OK;
    INITCOMMONCONTROLSEX icc = { };

    // Initialize common controls.
    icc.dwSize = sizeof(icc);
    icc.dwICC = ICC_PROGRESS_CLASS /* | ICC_STANDARD_CLASSES (requires 0x0501 == _WIN32_WINNT*/;
    ::InitCommonControlsEx(&icc);

    // initialize theme util
    hr = ThemeInitialize(vhInstance);
    ExitOnFailure(hr, "Failed to initialize theme manager.");

    // create user experience interface
    hr = CreateBootstrapperApplication(vhInstance, pEngine, pCommand, ppApplication);
    ExitOnFailure(hr, "Failed to create bootstrapper application interface.");

LExit:
    return hr;
}


extern "C" void WINAPI BootstrapperApplicationDestroy()
{
    ThemeUninitialize();
}
