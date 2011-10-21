//-------------------------------------------------------------------------------------------------
// <copyright file="locutil.h" company="Microsoft">
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
//    Header for localization helper functions.
// </summary>
//-------------------------------------------------------------------------------------------------
#pragma once

#ifdef __cplusplus
extern "C" {
#endif

struct LOC_STRING
{
    LPWSTR wzId;
    LPWSTR wzText;
    BOOL bOverridable;
};

struct LOC_STRINGSET
{
    DWORD cLocStrings;
    LOC_STRING* rgLocStrings;
};

typedef int (WINAPI *PFN_GETUSERDEFAULTLOCALENAME)(
    __out_ecount(cchLocaleName) LPWSTR lpLocaleName,
    __in int cchLocaleName
    );

typedef int (WINAPI *PFN_GETSYSTEMDEFAULTLOCALENAME)(
    __out_ecount(cchLocaleName) LPWSTR lpLocaleName,
    __in int cchLocaleName
    );

typedef LCID (WINAPI *PFN_LOCALENAMETOLCID)(
    __in LPCWSTR lpName,
    __in DWORD dwFlags
    );

HRESULT DAPI LocInitialize(
    );
void DAPI LocUninitialize(
    );

/********************************************************************
 LocProbeForFile - Searches for a localization file on disk.

*******************************************************************/
HRESULT DAPI LocProbeForFile(
    __in_z LPCWSTR wzBasePath,
    __in_z LPCWSTR wzLocFileName,
    __in_z_opt LPCWSTR wzLanguage,
    __inout LPWSTR* psczPath
    );

/********************************************************************
 LocLoadFromFile - Loads a localization file

*******************************************************************/
HRESULT DAPI LocLoadFromFile(
    __in_z LPCWSTR wzWxlFile,
    __out LOC_STRINGSET** ppLocStringSet
    );

/********************************************************************
 LocLoadFromResource - loads a localization file from a module's data
                       resource.

 NOTE: The resource data must be UTF-8 encoded.
*******************************************************************/
HRESULT DAPI LocLoadFromResource(
    __in HMODULE hModule,
    __in_z LPCSTR szResource,
    __out LOC_STRINGSET** ppLocStringSet
    );

/********************************************************************
 LocFree - free memory allocated when loading a localization file

*******************************************************************/
void DAPI LocFree(
    __in_opt LOC_STRINGSET* pLocStringSet
    );

/********************************************************************
 LocLocalizeString - replace any #(loc.id) in a string with the
                    correct sub string
*******************************************************************/
HRESULT DAPI LocLocalizeString(
    __in const LOC_STRINGSET* pLocStringSet,
    __inout LPWSTR* psczInput
    );

#ifdef __cplusplus
}
#endif
