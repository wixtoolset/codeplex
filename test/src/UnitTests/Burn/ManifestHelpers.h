//-------------------------------------------------------------------------------------------------
// <copyright file="ManifestHelpers.h" company="Outercurve Foundation">
//   Copyright (c) 2004, Outercurve Foundation.
//   This software is released under Microsoft Reciprocal License (MS-RL).
//   The license and further copyright text can be found in the file LICENSE.TXT
//   LICENSE.TXT at the root directory of the distribution.
// </copyright>
//
// <summary>
//    Manifest helper functions for unit tests for Burn.
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


void LoadBundleXmlHelper(LPCWSTR wzDocument, IXMLDOMElement** ppixeBundle);


}
}
}
}
}