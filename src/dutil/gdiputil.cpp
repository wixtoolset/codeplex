//-------------------------------------------------------------------------------------------------
// <copyright file="gdiputil.cpp" company="Microsoft">
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
//    GDI+ helper functions.
// </summary>
//-------------------------------------------------------------------------------------------------

#include "precomp.h"

using namespace Gdiplus;

/********************************************************************
 GdipImageFromResource - read a GDI+ image out of a resource stream

********************************************************************/
extern "C" HRESULT DAPI GdipImageFromResource(
    __in HINSTANCE hinst,
    __in LPSTR szId,
    __out Image **ppImage
    )
{
    HRESULT hr = S_OK;
    LPVOID pvData = NULL;
    DWORD cbData = 0;
    HGLOBAL hGlobal = NULL;;
    LPVOID pv = NULL;
    IStream *pStream = NULL;
    Image *pImage = NULL;
    Status gs = Ok;

    hr = ResReadData(hinst, szId, &pvData, &cbData);
    ExitOnFailure(hr, "Failed to load GDI+ bitmap from resource.");

    // Have to copy the fixed resource data into moveable (heap) memory
    // since that's what GDI+ expects.
    hGlobal = ::GlobalAlloc(GMEM_MOVEABLE, cbData);
    ExitOnNullWithLastError(hGlobal, hr, "Failed to allocate global memory.");

    pv = ::GlobalLock(hGlobal);
    ExitOnNullWithLastError(pv, hr, "Failed to lock global memory.");

    memcpy(pv, pvData, cbData);

    ::GlobalUnlock(pv); // no point taking any more memory than we have already
    pv = NULL;

    hr = ::CreateStreamOnHGlobal(hGlobal, TRUE, &pStream);
    ExitOnFailure(hr, "Failed to allocate stream from global memory.");

    hGlobal = NULL; // we gave the global memory to the stream object so it will close it

    pImage = Image::FromStream(pStream);
    gs = pImage->GetLastStatus();
    ExitOnGdipFailure(gs, hr, "Failed to load bitmap from stream.");

    *ppImage = pImage;
    pImage = NULL;

LExit:
    if (pImage)
    {
        delete pImage;
    }

    ReleaseObject(pStream);

    if (pv)
    {
        ::GlobalUnlock(pv);
    }

    if (hGlobal)
    {
        ::GlobalFree(hGlobal);
    }

    return hr;
}

HRESULT DAPI GdipHresultFromStatus(
    __in Gdiplus::Status gs
    )
{
    switch (gs)
    {
    case Ok:
        return S_OK;

    case GenericError:
        return E_FAIL;

    case InvalidParameter:
        return E_INVALIDARG;

    case OutOfMemory:
        return E_OUTOFMEMORY;

    case ObjectBusy:
        return HRESULT_FROM_WIN32(ERROR_BUSY);

    case InsufficientBuffer:
        return HRESULT_FROM_WIN32(ERROR_INSUFFICIENT_BUFFER);

    case NotImplemented:
        return E_NOTIMPL;

    case Win32Error:
        return E_FAIL;

    case WrongState:
        return E_FAIL;

    case Aborted:
        return E_ABORT;

    case FileNotFound:
        return HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND);

    case ValueOverflow:
        return HRESULT_FROM_WIN32(ERROR_ARITHMETIC_OVERFLOW);

    case AccessDenied:
        return E_ACCESSDENIED;

    case UnknownImageFormat:
        return HRESULT_FROM_WIN32(ERROR_BAD_FORMAT);

    case FontFamilyNotFound: __fallthrough;
    case FontStyleNotFound: __fallthrough;
    case NotTrueTypeFont:
        return E_UNEXPECTED;

    case UnsupportedGdiplusVersion:
        return HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED);

    case GdiplusNotInitialized:
        return E_UNEXPECTED;

    case PropertyNotFound: __fallthrough;
    case PropertyNotSupported:
        return E_FAIL;
    }

    return E_UNEXPECTED;
}