//-------------------------------------------------------------------------------------------------
// <copyright file="platform.cpp" company="Outercurve Foundation">
//   Copyright (c) 2004, Outercurve Foundation.
//   This software is released under Microsoft Reciprocal License (MS-RL).
//   The license and further copyright text can be found in the file
//   LICENSE.TXT at the root directory of the distribution.
// </copyright>
//
// <summary>
//    Module: Core
// </summary>
//-------------------------------------------------------------------------------------------------

#include "precomp.h"


// variables

PFN_INITIATESYSTEMSHUTDOWNEXW vpfnInitiateSystemShutdownExW;


// function definitions

extern "C" void PlatformInitialize()
{
    vpfnInitiateSystemShutdownExW = ::InitiateSystemShutdownExW;
}
