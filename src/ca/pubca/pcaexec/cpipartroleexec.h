#pragma once
//-------------------------------------------------------------------------------------------------
// <copyright file="cpipartroleexec.h" company="Outercurve Foundation">
//   Copyright (c) 2004, Outercurve Foundation.
//   This software is released under Microsoft Reciprocal License (MS-RL).
//   The license and further copyright text can be found in the file
//   LICENSE.TXT at the root directory of the distribution.
// </copyright>
// 
// <summary>
//    COM+ partition role functions for CustomActions
// </summary>
//-------------------------------------------------------------------------------------------------


// function prototypes

HRESULT CpiConfigureUsersInPartitionRoles(
	LPWSTR* ppwzData,
	HANDLE hRollbackFile
	);
HRESULT CpiRollbackConfigureUsersInPartitionRoles(
	LPWSTR* ppwzData,
	CPI_ROLLBACK_DATA* pRollbackDataList
	);