//-------------------------------------------------------------------------------------------------
// <copyright file="logutil.cpp" company="Microsoft">
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
//    Logging helper functions.
// </summary>
//-------------------------------------------------------------------------------------------------

#include "precomp.h"

// globals
static HMODULE LogUtil_hModule = NULL;
static BOOL LogUtil_fDisabled = FALSE;
static HANDLE LogUtil_hLog = INVALID_HANDLE_VALUE;
static LPWSTR LogUtil_sczLogPath = NULL;
static LPSTR LogUtil_sczPreInitBuffer = NULL;
static REPORT_LEVEL LogUtil_rlCurrent = REPORT_STANDARD;
static CRITICAL_SECTION LogUtil_csLog = { };
static BOOL LogUtil_fInitializedCriticalSection = FALSE;

// Customization of certain parts of the string, within a line
static LPSTR LogUtil_sczSpecialBeginLine = NULL;
static LPSTR LogUtil_sczSpecialEndLine = NULL;
static LPSTR LogUtil_sczSpecialAfterTimeStamp = NULL;

static LPCSTR LOGUTIL_UNKNOWN = "unknown";
static LPCSTR LOGUTIL_STANDARD = "standard";
static LPCSTR LOGUTIL_VERBOSE = "verbose";
static LPCSTR LOGUTIL_DEBUG = "debug";
static LPCSTR LOGUTIL_NONE = "none";

// prototypes
static HRESULT LogIdWork(
    __in_opt HMODULE hModule,
    __in DWORD dwLogId,
    __in va_list args,
    __in BOOL fLOGUTIL_NEWLINE
    );
static HRESULT LogStringWorkArgs(
    __in_z __format_string LPCSTR szFormat,
    __in va_list args,
    __in BOOL fLOGUTIL_NEWLINE
    );
static HRESULT LogStringWork(
    __in_z LPCSTR szString,
    __in BOOL fLOGUTIL_NEWLINE
    );

// Hook to allow redirecting LogStringWorkRaw function calls
static PFN_LOGSTRINGWORKRAW s_vpfLogStringWorkRaw = NULL;
static LPVOID s_vpvLogStringWorkRawContext = NULL;


/********************************************************************
 IsLogInitialized - Checks if log is currently initialized.
********************************************************************/
extern "C" BOOL DAPI IsLogInitialized()
{
    return LogUtil_fInitializedCriticalSection;
}

/********************************************************************
 IsLogOpen - Checks if log is currently initialized and open.
********************************************************************/
extern "C" BOOL DAPI IsLogOpen()
{
    return (INVALID_HANDLE_VALUE != LogUtil_hLog && NULL != LogUtil_sczLogPath);
}


/********************************************************************
 LogInitialize - initializes the logutil API

********************************************************************/
extern "C" void DAPI LogInitialize(
    __in HMODULE hModule
    )
{
    AssertSz(INVALID_HANDLE_VALUE == LogUtil_hLog && !LogUtil_sczLogPath, "LogInitialize() or LogOpen() - already called.");

    LogUtil_hModule = hModule;
    LogUtil_fDisabled = FALSE;

    ::InitializeCriticalSection(&LogUtil_csLog);
    LogUtil_fInitializedCriticalSection = TRUE;
}


/********************************************************************
 LogOpen - creates an application log file

 NOTE: if wzExt is null then wzLog is path to desired log else wzLog and wzExt are used to generate log name
********************************************************************/
extern "C" HRESULT DAPI LogOpen(
    __in_z_opt LPCWSTR wzDirectory,
    __in_z LPCWSTR wzLog,
    __in_z_opt LPCWSTR wzPostfix,
    __in_z_opt LPCWSTR wzExt,
    __in BOOL fAppend,
    __in BOOL fHeader,
    __out_z_opt LPWSTR* psczLogPath
    )
{
    HRESULT hr = S_OK;
    BOOL fEnteredCriticalSection = FALSE;

    ::EnterCriticalSection(&LogUtil_csLog);
    fEnteredCriticalSection = TRUE;

    if (wzExt && *wzExt)
    {
        hr = PathCreateTimeBasedTempFile(wzDirectory, wzLog, wzPostfix, wzExt, &LogUtil_sczLogPath, &LogUtil_hLog);
        ExitOnFailure(hr, "Failed to create log based on current system time.");
    }
    else
    {
        hr = PathConcat(wzDirectory, wzLog, &LogUtil_sczLogPath);
        ExitOnFailure(hr, "Failed to combine the log path.");

        LogUtil_hLog = ::CreateFileW(LogUtil_sczLogPath, GENERIC_WRITE, FILE_SHARE_READ, NULL, (fAppend) ? OPEN_ALWAYS : CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
        if (INVALID_HANDLE_VALUE == LogUtil_hLog)
        {
            ExitOnLastError1(hr, "failed to create log file: %ls", LogUtil_sczLogPath);
        }

        if (fAppend)
        {
            ::SetFilePointer(LogUtil_hLog, 0, 0, FILE_END);
        }
    }

    if (fHeader)
    {
        LogHeader();
    }

    if (NULL != LogUtil_sczPreInitBuffer)
    {
        // Log anything that was logged before LogOpen() was called.
        LogStringWork(LogUtil_sczPreInitBuffer, FALSE);
        ReleaseNullStr(LogUtil_sczPreInitBuffer);
    }

    if (psczLogPath)
    {
        hr = StrAllocString(psczLogPath, LogUtil_sczLogPath, 0);
        ExitOnFailure(hr, "Failed to copy log path.");
    }

    LogUtil_fDisabled = FALSE;

LExit:
    if (fEnteredCriticalSection)
    {
        ::LeaveCriticalSection(&LogUtil_csLog);
    }

    return hr;
}


/********************************************************************
 LogDisable - closes any open files and disables in memory logging.

********************************************************************/
void DAPI LogDisable()
{
    ::EnterCriticalSection(&LogUtil_csLog);

    LogUtil_fDisabled = TRUE;

    ReleaseFileHandle(LogUtil_hLog);
    ReleaseNullStr(LogUtil_sczLogPath);
    ReleaseNullStr(LogUtil_sczPreInitBuffer);

    ::LeaveCriticalSection(&LogUtil_csLog);
}


/********************************************************************
 LogRedirect - Redirects all logging strings to the specified
               function - or set NULL to disable the hook
********************************************************************/
void DAPI LogRedirect(
    __in_opt PFN_LOGSTRINGWORKRAW vpfLogStringWorkRaw,
    __in_opt LPVOID pvContext
    )
{
    s_vpfLogStringWorkRaw = vpfLogStringWorkRaw;
    s_vpvLogStringWorkRawContext = pvContext;
}


/********************************************************************
 LogRename - Renames a logfile, moving its contents to a new path,
             and re-opening the file for appending at the new
             location
********************************************************************/
HRESULT DAPI LogRename(
    __in_z LPCWSTR wzNewPath
    )
{
    HRESULT hr = S_OK;
    BOOL fEnteredCriticalSection = FALSE;

    ::EnterCriticalSection(&LogUtil_csLog);
    fEnteredCriticalSection = TRUE;

    ReleaseFileHandle(LogUtil_hLog);

    hr = FileEnsureMove(LogUtil_sczLogPath, wzNewPath, TRUE, TRUE);
    ExitOnFailure1(hr, "Failed to move logfile to new location: %ls", wzNewPath);

    hr = StrAllocString(&LogUtil_sczLogPath, wzNewPath, 0);
    ExitOnFailure1(hr, "Failed to store new logfile path: %ls", wzNewPath);

    LogUtil_hLog = ::CreateFileW(LogUtil_sczLogPath, GENERIC_WRITE, FILE_SHARE_READ, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
    if (INVALID_HANDLE_VALUE == LogUtil_hLog)
    {
        ExitOnLastError1(hr, "failed to create log file: %ls", LogUtil_sczLogPath);
    }

    // Enable "append" mode by moving file pointer to the end
    ::SetFilePointer(LogUtil_hLog, 0, 0, FILE_END);

LExit:
    if (fEnteredCriticalSection)
    {
        ::LeaveCriticalSection(&LogUtil_csLog);
    }

    return hr;
}


extern "C" void DAPI LogUninitialize(
    __in BOOL fFooter
    )
{
    if (INVALID_HANDLE_VALUE != LogUtil_hLog && LogUtil_sczLogPath)
    {
        if (fFooter)
        {
            LogFooter();
        }
    }

    if (LogUtil_fInitializedCriticalSection)
    {
        ::DeleteCriticalSection(&LogUtil_csLog);
        LogUtil_fInitializedCriticalSection = FALSE;
    }

    LogUtil_hModule = NULL;
    LogUtil_fDisabled = FALSE;

    ReleaseFileHandle(LogUtil_hLog);
    ReleaseNullStr(LogUtil_sczLogPath);
    ReleaseNullStr(LogUtil_sczPreInitBuffer);
    ReleaseNullStr(LogUtil_sczSpecialBeginLine);
    ReleaseNullStr(LogUtil_sczSpecialAfterTimeStamp);
    ReleaseNullStr(LogUtil_sczSpecialEndLine);
}


/********************************************************************
 LogIsOpen - returns whether log file is open or note

********************************************************************/
extern "C" BOOL DAPI LogIsOpen()
{
    return INVALID_HANDLE_VALUE != LogUtil_hLog;
}


/********************************************************************
 LogSetSpecialParams - sets a special beginline string, endline
                       string, post-timestamp string, etc.
********************************************************************/
HRESULT DAPI LogSetSpecialParams(
    __in_z LPCSTR wzSpecialBeginLine,
    __in_z LPCSTR wzSpecialAfterTimeStamp,
    __in_z LPCSTR wzSpecialEndLine
    )
{
    HRESULT hr = S_OK;

    // Handle special string to be prepended before every full line
    if (NULL == wzSpecialBeginLine)
    {
        ReleaseNullStr(LogUtil_sczSpecialBeginLine);
    }
    else
    {
        hr = StrAnsiAllocConcat(&LogUtil_sczSpecialBeginLine, wzSpecialBeginLine, 0);
        ExitOnFailure(hr, "Failed to allocate copy of special beginline string");
    }

    // Handle special string to be appended to every time stamp
    if (NULL == wzSpecialAfterTimeStamp)
    {
        ReleaseNullStr(LogUtil_sczSpecialAfterTimeStamp);
    }
    else
    {
        hr = StrAnsiAllocConcat(&LogUtil_sczSpecialAfterTimeStamp, wzSpecialAfterTimeStamp, 0);
        ExitOnFailure(hr, "Failed to allocate copy of special post-timestamp string");
    }

    // Handle special string to be appended before every full line
    if (NULL == wzSpecialEndLine)
    {
        ReleaseNullStr(LogUtil_sczSpecialEndLine);
    }
    else
    {
        hr = StrAnsiAllocConcat(&LogUtil_sczSpecialEndLine, wzSpecialEndLine, 0);
        ExitOnFailure(hr, "Failed to allocate copy of special endline string");
    }

LExit:
    return hr;
}

/********************************************************************
 LogSetLevel - sets the logging level

 NOTE: returns previous logging level
********************************************************************/
extern "C" REPORT_LEVEL DAPI LogSetLevel(
    __in REPORT_LEVEL rl,
    __in BOOL fLogChange
    )
{
    AssertSz(REPORT_ERROR != rl, "REPORT_ERROR is not a valid logging level to set");

    REPORT_LEVEL rlPrev = LogUtil_rlCurrent;

    if (LogUtil_rlCurrent != rl)
    {
        LogUtil_rlCurrent = rl;

        if (fLogChange)
        {
            LPCSTR szLevel = LOGUTIL_UNKNOWN;
            switch (LogUtil_rlCurrent)
            {
            case REPORT_STANDARD:
                szLevel = LOGUTIL_STANDARD;
                break;
            case REPORT_VERBOSE:
                szLevel = LOGUTIL_VERBOSE;
                break;
            case REPORT_DEBUG:
                szLevel = LOGUTIL_DEBUG;
                break;
            case REPORT_NONE:
                szLevel = LOGUTIL_NONE;
                break;
            }

            LogLine(REPORT_STANDARD, "--- logging level: %s ---", szLevel);
        }
    }

    return rlPrev;
}


/********************************************************************
 LogGetLevel - gets the current logging level

********************************************************************/
extern "C" REPORT_LEVEL DAPI LogGetLevel()
{
    return LogUtil_rlCurrent;
}


/********************************************************************
 LogGetPath - gets the current log path

********************************************************************/
extern "C" HRESULT DAPI LogGetPath(
    __out_ecount_z(cchLogPath) LPWSTR pwzLogPath,
    __in DWORD cchLogPath
    )
{
    Assert(pwzLogPath);

    HRESULT hr = S_OK;

    if (NULL == LogUtil_sczLogPath)        // they can't have a path if there isn't one!
    {
        ExitFunction1(hr = E_UNEXPECTED);
    }

    hr = ::StringCchCopyW(pwzLogPath, cchLogPath, LogUtil_sczLogPath);

LExit:
    return hr;
}


/********************************************************************
 LogGetHandle - gets the current log file handle

********************************************************************/
extern "C" HANDLE DAPI LogGetHandle()
{
    return LogUtil_hLog;
}


/********************************************************************
 LogString - write a string to the log

 NOTE: use printf formatting ("%ls", "%d", etc.)
********************************************************************/
extern "C" HRESULT DAPIV LogString(
    __in REPORT_LEVEL rl,
    __in_z __format_string LPCSTR szFormat,
    ...
    )
{
    HRESULT hr = S_OK;
    va_list args;

    va_start(args, szFormat);
    hr = LogStringArgs(rl, szFormat, args);
    va_end(args);

    return hr;
}

extern "C" HRESULT DAPI LogStringArgs(
    __in REPORT_LEVEL rl,
    __in_z __format_string LPCSTR szFormat,
    __in va_list args
    )
{
    AssertSz(REPORT_NONE != rl, "REPORT_NONE is not a valid logging level");
    HRESULT hr = S_OK;

    if (REPORT_ERROR != rl && LogUtil_rlCurrent < rl)
    {
        ExitFunction1(hr = S_FALSE);
    }

    hr = LogStringWorkArgs(szFormat, args, FALSE);

LExit:
    return hr;
}

/********************************************************************
 LogStringLine - write a string plus LOGUTIL_NEWLINE to the log

 NOTE: use printf formatting ("%ls", "%d", etc.)
********************************************************************/
extern "C" HRESULT DAPIV LogStringLine(
    __in REPORT_LEVEL rl,
    __in_z __format_string LPCSTR szFormat,
    ...
    )
{
    HRESULT hr = S_OK;
    va_list args;

    va_start(args, szFormat);
    hr = LogStringLineArgs(rl, szFormat, args);
    va_end(args);

    return hr;
}

extern "C" HRESULT DAPI LogStringLineArgs(
    __in REPORT_LEVEL rl,
    __in_z __format_string LPCSTR szFormat,
    __in va_list args
    )
{
    AssertSz(REPORT_NONE != rl, "REPORT_NONE is not a valid logging level");
    HRESULT hr = S_OK;

    if (REPORT_ERROR != rl && LogUtil_rlCurrent < rl)
    {
        ExitFunction1(hr = S_FALSE);
    }

    hr = LogStringWorkArgs(szFormat, args, TRUE);

LExit:
    return hr;
}

/********************************************************************
 LogIdModuleArgs - write a string embedded in a MESSAGETABLE to the log

 NOTE: uses format string from MESSAGETABLE resource
********************************************************************/

extern "C" HRESULT DAPI LogIdModuleArgs(
    __in REPORT_LEVEL rl,
    __in DWORD dwLogId,
    __in_opt HMODULE hModule,
    __in va_list args
    )
{
    AssertSz(REPORT_NONE != rl, "REPORT_NONE is not a valid logging level");
    HRESULT hr = S_OK;

    if (REPORT_ERROR != rl && LogUtil_rlCurrent < rl)
    {
        ExitFunction1(hr = S_FALSE);
    }

    hr = LogIdWork((hModule) ? hModule : LogUtil_hModule, dwLogId, args, TRUE);

LExit:
    return hr;
}

extern "C" HRESULT DAPI LogIdModule(
    __in REPORT_LEVEL rl,
    __in DWORD dwLogId,
    __in HMODULE hModule,
    ...
    )
{
    AssertSz(REPORT_NONE != rl, "REPORT_NONE is not a valid logging level");
    HRESULT hr = S_OK;
    va_list args;

    if (REPORT_ERROR != rl && LogUtil_rlCurrent < rl)
    {
        ExitFunction1(hr = S_FALSE);
    }

    va_start(args, hModule);
    hr = LogIdWork((hModule) ? hModule : LogUtil_hModule, dwLogId, args, TRUE);
    va_end(args);

LExit:
    return hr;
}




/********************************************************************
 LogError - write an error to the log

 NOTE: use printf formatting ("%ls", "%d", etc.)
********************************************************************/
extern "C" HRESULT DAPIV LogErrorString(
    __in HRESULT hrError,
    __in_z __format_string LPCSTR szFormat,
    ...
    )
{
    HRESULT hr  = S_OK;

    va_list args;
    va_start(args, szFormat);
    hr = LogErrorStringArgs(hrError, szFormat, args);
    va_end(args);

    return hr;
}

extern "C" HRESULT DAPI LogErrorStringArgs(
    __in HRESULT hrError,
    __in_z __format_string LPCSTR szFormat,
    __in va_list args
    )
{
    HRESULT hr  = S_OK;
    LPSTR sczMessage = NULL;

    hr = StrAnsiAllocFormattedArgs(&sczMessage, szFormat, args);
    ExitOnFailure1(hr, "failed to format error message: \"%s\"", szFormat);

    hr = LogLine(REPORT_ERROR, "Error 0x%x: %s", hrError, sczMessage);

LExit:
    ReleaseStr(sczMessage);

    return hr;
}


/********************************************************************
 LogErrorIdModule - write an error string embedded in a MESSAGETABLE to the log

 NOTE:  uses format string from MESSAGETABLE resource
        can log no more than three strings in the error message
********************************************************************/
extern "C" HRESULT DAPI LogErrorIdModule(
    __in HRESULT hrError,
    __in DWORD dwLogId,
    __in_opt HMODULE hModule,
    __in_z_opt LPCWSTR wzString1,
    __in_z_opt LPCWSTR wzString2,
    __in_z_opt LPCWSTR wzString3
    )
{
    HRESULT hr = S_OK;
    WCHAR wzError[11];
    WORD cStrings = 1; // guaranteed wzError is in the list

    hr = ::StringCchPrintfW(wzError, countof(wzError), L"0x%08x", hrError);
    ExitOnFailure1(hr, "failed to format error code: \"0%08x\"", hrError);

    cStrings += wzString1 ? 1 : 0;
    cStrings += wzString2 ? 1 : 0;
    cStrings += wzString3 ? 1 : 0;

    hr = LogIdModule(REPORT_ERROR, dwLogId, hModule, wzError, wzString1, wzString2, wzString3);
    ExitOnFailure(hr, "Failed to log id module.");

LExit:
    return hr;
}

/********************************************************************
 LogHeader - write a standard header to the log

********************************************************************/
extern "C" HRESULT DAPI LogHeader()
{
    HRESULT hr = S_OK;
    WCHAR wzComputerName[MAX_PATH];
    DWORD cchComputerName = countof(wzComputerName);
    WCHAR wzPath[MAX_PATH];
    DWORD dwMajorVersion = 0;
    DWORD dwMinorVersion = 0;
    LPCSTR szLevel = LOGUTIL_UNKNOWN;
    LPWSTR sczCurrentDateTime = NULL;

    //
    // get the interesting data
    //
    if (!::GetModuleFileNameW(NULL, wzPath, countof(wzPath)))
    {
        memset(wzPath, 0, sizeof(wzPath));
    }

    hr = FileVersion(wzPath, &dwMajorVersion, &dwMinorVersion);
    if (FAILED(hr))
    {
        dwMajorVersion = 0;
        dwMinorVersion = 0;
    }

    if (!::GetComputerNameW(wzComputerName, &cchComputerName))
    {
        ::SecureZeroMemory(wzComputerName, sizeof(wzComputerName));
    }

    TimeCurrentDateTime(&sczCurrentDateTime, FALSE);

    //
    // write data to the log
    //
    LogLine(REPORT_STANDARD, "=== Logging started: %ls ===", sczCurrentDateTime);
    LogLine(REPORT_STANDARD, "Executable: %ls v%d.%d.%d.%d", wzPath, dwMajorVersion >> 16, dwMajorVersion & 0xFFFF, dwMinorVersion >> 16, dwMinorVersion & 0xFFFF);
    LogLine(REPORT_STANDARD, "Computer  : %ls", wzComputerName);
    switch (LogUtil_rlCurrent)
    {
    case REPORT_STANDARD:
        szLevel = LOGUTIL_STANDARD;
        break;
    case REPORT_VERBOSE:
        szLevel = LOGUTIL_VERBOSE;
        break;
    case REPORT_DEBUG:
        szLevel = LOGUTIL_DEBUG;
        break;
    case REPORT_NONE:
        szLevel = LOGUTIL_NONE;
        break;
    }
    LogLine(REPORT_STANDARD, "--- logging level: %s ---", szLevel);

    hr = S_OK;

    ReleaseStr(sczCurrentDateTime);

    return hr;
}


/********************************************************************
 LogFooterWork - write a standard footer to the log

********************************************************************/

static HRESULT LogFooterWork(
    __in_z __format_string LPCSTR szFormat,
    ...
    )
{
    HRESULT hr = S_OK;

    va_list args;
    va_start(args, szFormat);
    hr = LogStringWorkArgs(szFormat, args, TRUE);
    va_end(args);

    return hr;
}

extern "C" HRESULT DAPI LogFooter()
{
    HRESULT hr = S_OK;
    LPWSTR sczCurrentDateTime = NULL;
    TimeCurrentDateTime(&sczCurrentDateTime, FALSE);
    hr = LogFooterWork("=== Logging stopped: %ls ===", sczCurrentDateTime);
    ReleaseStr(sczCurrentDateTime);
    return hr;
}

/********************************************************************
 LogStringWorkRaw - Write a raw, unformatted string to the log

********************************************************************/
extern "C" HRESULT LogStringWorkRaw(
    __in_z LPCSTR szLogData
    )
{
    Assert(szLogData && *szLogData);

    HRESULT hr = S_OK;
    DWORD cbLogData = 0;
    DWORD cbTotal = 0;
    DWORD cbWrote = 0;

    cbLogData = lstrlenA(szLogData);

    // If the log hasn't been initialized yet, store it in a buffer
    if (INVALID_HANDLE_VALUE == LogUtil_hLog)
    {
        hr = StrAnsiAllocConcat(&LogUtil_sczPreInitBuffer, szLogData, 0);
        ExitOnFailure(hr, "Failed to concatenate string to pre-init buffer");

        ExitFunction1(hr = S_OK);
    }

    // write the string
    while (cbTotal < cbLogData)
    {
        if (!::WriteFile(LogUtil_hLog, reinterpret_cast<const BYTE*>(szLogData) + cbTotal, cbLogData - cbTotal, &cbWrote, NULL))
        {
            ExitOnLastError2(hr, "Failed to write output to log: %ls - %ls", LogUtil_sczLogPath, szLogData);
        }

        cbTotal += cbWrote;
    }

LExit:
    return hr;
}

//
// private worker functions
//
static HRESULT LogIdWork(
    __in_opt HMODULE hModule,
    __in DWORD dwLogId,
    __in va_list args,
    __in BOOL fLOGUTIL_NEWLINE
    )
{
    HRESULT hr = S_OK;
    LPSTR psz = NULL;
    DWORD cch = 0;

    // get the string for the id
#pragma prefast(push)
#pragma prefast(disable:25028)
    cch = ::FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_HMODULE,
                           static_cast<LPCVOID>(hModule), dwLogId, 0, reinterpret_cast<LPSTR>(&psz), 0, &args);
#pragma prefast(pop)

    if (0 == cch)
    {
        ExitOnLastError1(hr, "failed to log id: %d", dwLogId);
    }

    if (2 <= cch && '\r' == psz[cch-2] && '\n' == psz[cch-1])
    {
        psz[cch-2] = L'\0'; // remove newline from message table
    }

    LogStringWork(psz, fLOGUTIL_NEWLINE);

LExit:
    if (psz)
    {
        ::LocalFree(psz);
    }

    return hr;
}


static HRESULT LogStringWorkArgs(
    __in_z __format_string LPCSTR szFormat,
    __in va_list args,
    __in BOOL fLOGUTIL_NEWLINE
    )
{
    Assert(szFormat && *szFormat);

    HRESULT hr = S_OK;
    LPSTR scz = NULL;

    // format the string
    hr = StrAnsiAllocFormattedArgs(&scz, szFormat, args);
    ExitOnFailure1(hr, "Failed to format message: \"%s\"", szFormat);

    hr = LogStringWork(scz, fLOGUTIL_NEWLINE);
    ExitOnFailure1(hr, "Failed to write formatted string to log:%s", scz);

LExit:
    ReleaseStr(scz);

    return hr;
}


static HRESULT LogStringWork(
    __in_z LPCSTR szString,
    __in BOOL fLOGUTIL_NEWLINE
    )
{
    Assert(szString && *szString);

    HRESULT hr = S_OK;
    BOOL fEnteredCriticalSection = FALSE;
    DWORD dwProcessId = 0;
    DWORD dwThreadId = 0;
    SYSTEMTIME st = { };
    TIME_ZONE_INFORMATION tzi = { };
    DWORD dwAbsBias = 0;
    LPSTR scz = NULL;
    LPCSTR szLogData = NULL;

    // If logging is disabled, just bail.
    if (LogUtil_fDisabled)
    {
        ExitFunction();
    }

    ::EnterCriticalSection(&LogUtil_csLog);
    fEnteredCriticalSection = TRUE;

    if (fLOGUTIL_NEWLINE)
    {
        // get the process and thread id.
        dwProcessId = ::GetCurrentProcessId();
        dwThreadId = ::GetCurrentThreadId();

        // get the time relative to GMT.
        ::GetLocalTime(&st);
        ::GetTimeZoneInformation(&tzi);
        dwAbsBias = abs(tzi.Bias);

        // add line prefix and trailing newline
        hr = StrAnsiAllocFormatted(&scz, "%s[%04X:%04X][%04hu-%02hu-%02huT%02hu:%02hu:%02hu.%03hu%c%02u:%02u]:%s %s%s", LogUtil_sczSpecialBeginLine ? LogUtil_sczSpecialBeginLine : "",
            dwProcessId, dwThreadId, st.wYear, st.wMonth, st.wDay, st.wHour, st.wMinute, st.wSecond, st.wMilliseconds, 0 >= tzi.Bias ? L'+' : L'-', dwAbsBias / 60, dwAbsBias % 60,
            LogUtil_sczSpecialAfterTimeStamp ? LogUtil_sczSpecialAfterTimeStamp : "", szString, LogUtil_sczSpecialEndLine ? LogUtil_sczSpecialEndLine : "\r\n");
        ExitOnFailure(hr, "Failed to format line prefix.");
    }

    szLogData = scz ? scz : szString;

    if (s_vpfLogStringWorkRaw)
    {
        hr = s_vpfLogStringWorkRaw(szLogData, s_vpvLogStringWorkRawContext);
        ExitOnFailure1(hr, "Failed to write string to log using redirected function: %ls", szString);
    }
    else
    {
        hr = LogStringWorkRaw(szLogData);
        ExitOnFailure1(hr, "Failed to write string to log using default function: %ls", szString);
    }

LExit:
    if (fEnteredCriticalSection)
    {
        ::LeaveCriticalSection(&LogUtil_csLog);
    }

    ReleaseStr(scz);

    return hr;
}
