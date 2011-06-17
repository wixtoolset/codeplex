//-------------------------------------------------------------------------------------------------
// <copyright file="BurnTestException.h" company="Microsoft">
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

#pragma once


namespace Microsoft
{
namespace Tools
{
namespace WindowsInstallerXml
{
namespace Test
{
namespace Bootstrapper
{
    using namespace System;
    using namespace Microsoft::VisualStudio::TestTools::UnitTesting;

    public ref struct BurnTestException : public System::Exception
    {
    public:
        BurnTestException(HRESULT error)
        {
            this->HResult = error;
        }

        BurnTestException(HRESULT error, String^ message)
            : Exception(message)
        {
            this->HResult = error;
        }

        property Int32 ErrorCode
        {
            Int32 get()
            {
              return this->HResult;
            }
        }

    };
}
}
}
}
}

// this class is used by __TestThrowOnFailure_Format() below to deallocate
// the string created after the function call has returned
class __TestThrowOnFailure_StringFree
{
    LPWSTR m_scz;

public:
    __TestThrowOnFailure_StringFree(LPWSTR scz)
    {
        m_scz = scz;
    }

    ~__TestThrowOnFailure_StringFree()
    {
        ReleaseStr(m_scz);
    }

    operator LPCWSTR()
    {
        return m_scz;
    }
};

// used by the TestThrowOnFailure macros to format the error string and
// return an LPCWSTR that can be used to initialize a System::String
#pragma warning (push)
#pragma warning (disable : 4793)
inline __TestThrowOnFailure_StringFree __TestThrowOnFailure_Format(LPCWSTR wzFormat, ...)
{
    Assert(ppwz && wzFormat && *wzFormat);

    HRESULT hr = S_OK;
    LPWSTR scz = NULL;
    va_list args;

    va_start(args, wzFormat);
    hr = StrAllocFormattedArgs(&scz, wzFormat, args);
    va_end(args);
    ExitOnFailure(hr, "Failed to format message string.");

LExit:
    return scz;
}
#pragma warning (pop)

#define TestThrowOnFailure(hr, s) if (FAILED(hr)) { throw gcnew Microsoft::Tools::WindowsInstallerXml::Test::Bootstrapper::BurnTestException(hr, gcnew System::String(s)); }
#define TestThrowOnFailure1(hr, s, p) if (FAILED(hr)) { throw gcnew Microsoft::Tools::WindowsInstallerXml::Test::Bootstrapper::BurnTestException(hr, gcnew System::String(__TestThrowOnFailure_Format(s, p))); }
#define TestThrowOnFailure2(hr, s, p1, p2) if (FAILED(hr)) { throw gcnew Microsoft::Tools::WindowsInstallerXml::Test::Bootstrapper::BurnTestException(hr, gcnew System::String(__TestThrowOnFailure_Format(s, p1, p2))); }