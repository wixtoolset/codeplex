//-------------------------------------------------------------------------------------------------
// <copyright file="VariableHelpers.cpp" company="Microsoft">
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
//    Variable helper functions for unit tests for Burn.
// </summary>
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


void VariableSetStringHelper(BURN_VARIABLES* pVariables, LPCWSTR wzVariable, LPCWSTR wzValue);
void VariableSetNumericHelper(BURN_VARIABLES* pVariables, LPCWSTR wzVariable, LONGLONG llValue);
void VariableSetVersionHelper(BURN_VARIABLES* pVariables, LPCWSTR wzVariable, DWORD64 qwValue);
System::String^ VariableGetStringHelper(BURN_VARIABLES* pVariables, LPCWSTR wzVariable);
__int64 VariableGetNumericHelper(BURN_VARIABLES* pVariables, LPCWSTR wzVariable);
unsigned __int64 VariableGetVersionHelper(BURN_VARIABLES* pVariables, LPCWSTR wzVariable);
System::String^ VariableGetFormattedHelper(BURN_VARIABLES* pVariables, LPCWSTR wzVariable);
System::String^ VariableFormatStringHelper(BURN_VARIABLES* pVariables, LPCWSTR wzIn);
System::String^ VariableEscapeStringHelper(LPCWSTR wzIn);
bool EvaluateConditionHelper(BURN_VARIABLES* pVariables, LPCWSTR wzCondition);
bool EvaluateFailureConditionHelper(BURN_VARIABLES* pVariables, LPCWSTR wzCondition);
bool VariableExistsHelper(BURN_VARIABLES* pVariables, LPCWSTR wzVariable);
int VariableGetTypeHelper(BURN_VARIABLES* pVariables, LPCWSTR wzVariable);


}
}
}
}
}
