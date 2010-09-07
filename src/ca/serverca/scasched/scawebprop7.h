#pragma once
//-------------------------------------------------------------------------------------------------
// <copyright file="scawebprop7.h" company="Microsoft">
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
//    IIS Web Directory Property functions for CustomActions
// </summary>
//-------------------------------------------------------------------------------------------------

#include "scauser.h"

// global sql queries provided for optimization
extern LPCWSTR vcsWebDirPropertiesQuery;


// prototypes
HRESULT ScaGetWebDirProperties7(
    __in_z LPCWSTR pwzProperties,
    __inout SCA_WEB_PROPERTIES* pswp
    );

HRESULT ScaWriteWebDirProperties7(
    __in_z LPCWSTR wzwWebName,
    __in_z LPCWSTR wzRootOfWeb,
    const SCA_WEB_PROPERTIES* pswp
    );

