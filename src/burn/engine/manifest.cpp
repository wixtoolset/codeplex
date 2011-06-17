//-------------------------------------------------------------------------------------------------
// <copyright file="manifest.cpp" company="Microsoft">
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
//    Module: Core
// </summary>
//-------------------------------------------------------------------------------------------------

#include "precomp.h"


// function definitions

extern "C" HRESULT ManifestLoadXmlFromBuffer(
    __in_bcount(cbBuffer) BYTE* pbBuffer,
    __in SIZE_T cbBuffer,
    __in BURN_ENGINE_STATE* pEngineState
    )
{
    HRESULT hr = S_OK;
    IXMLDOMDocument* pixdDocument = NULL;
    IXMLDOMElement* pixeBundle = NULL;
    IXMLDOMNode* pixnLog = NULL;
    IXMLDOMNode* pixnChain = NULL;

    // load xml document
    hr = XmlLoadDocumentFromBuffer(pbBuffer, cbBuffer, &pixdDocument);
    ExitOnFailure(hr, "Failed to load manifest as XML document.");

    // get bundle element
    hr = pixdDocument->get_documentElement(&pixeBundle);
    ExitOnFailure(hr, "Failed to get bundle element.");

    // parse the log element, if present.
    hr = XmlSelectSingleNode(pixeBundle, L"Log", &pixnLog);
    ExitOnFailure(hr, "Failed to get Log element.");

    if (S_OK == hr)
    {
        hr = XmlGetAttributeEx(pixnLog, L"PathVariable", &pEngineState->log.sczPathVariable);
        if (E_NOTFOUND != hr)
        {
            ExitOnFailure(hr, "Failed to get Log/@PathVariable.");
        }

        hr = XmlGetAttributeEx(pixnLog, L"Prefix", &pEngineState->log.sczPrefix);
        ExitOnFailure(hr, "Failed to get Log/@Prefix attribute.");

        hr = XmlGetAttributeEx(pixnLog, L"Extension", &pEngineState->log.sczExtension);
        ExitOnFailure(hr, "Failed to get Log/@Extension attribute.");
    }

    // get the chain element
    hr = XmlSelectSingleNode(pixeBundle, L"Chain", &pixnChain);
    ExitOnFailure(hr, "Failed to get chain element.");

    if (S_OK == hr)
    {
        // parse disable rollback
        hr = XmlGetYesNoAttribute(pixnChain, L"DisableRollback", &pEngineState->fDisableRollback);
        if (E_NOTFOUND != hr)
        {
            ExitOnFailure(hr, "Failed to to get Chain/@DisableRollback");
        }

        // parse parallel cache
        hr = XmlGetYesNoAttribute(pixnChain, L"ParallelCache", &pEngineState->fParallelCacheAndExecute);
        if (E_NOTFOUND != hr)
        {
            ExitOnFailure(hr, "Failed to to get Chain/@ParallelCache");
        }
    }

    // parse built-in condition 
    hr = ConditionGlobalParseFromXml(&pEngineState->condition, pixeBundle);
    ExitOnFailure(hr, "Failed to parse global condition.");

    // parse variables
    hr = VariablesParseFromXml(&pEngineState->variables, pixeBundle);
    ExitOnFailure(hr, "Failed to parse variables.");

    // parse searches
    hr = SearchesParseFromXml(&pEngineState->searches, pixeBundle); // TODO: Modularization
    ExitOnFailure(hr, "Failed to parse searches.");

    // parse user experience
    hr = UserExperienceParseFromXml(&pEngineState->userExperience, pixeBundle);
    ExitOnFailure(hr, "Failed to parse user experience.");

    // parse catalog files
    hr = CatalogsParseFromXml(&pEngineState->catalogs, pixeBundle);
    ExitOnFailure(hr, "Failed to parse catalog files.");

    // parse registration
    hr = RegistrationParseFromXml(&pEngineState->registration, pixeBundle);
    ExitOnFailure(hr, "Failed to parse registration.");

    // Set a useful variable we got as part of registration
    if (NULL != pEngineState->registration.sczTag)
    {
        hr = VariableSetString(&pEngineState->variables, L"BundleTag", pEngineState->registration.sczTag);
        ExitOnFailure(hr, "Failed to set Tag variable from registration information");
    }

    // parse containers
    hr = ContainersParseFromXml(&pEngineState->containers, pixeBundle);
    ExitOnFailure(hr, "Failed to parse containers.");

    // parse payloads
    hr = PayloadsParseFromXml(&pEngineState->payloads, &pEngineState->containers, &pEngineState->catalogs, pixeBundle);
    ExitOnFailure(hr, "Failed to parse payloads.");

    // parse packages
    hr = PackagesParseFromXml(&pEngineState->packages, &pEngineState->payloads, pixeBundle);
    ExitOnFailure(hr, "Failed to parse packages.");

LExit:
    ReleaseObject(pixnChain);
    ReleaseObject(pixnLog);
    ReleaseObject(pixeBundle);
    ReleaseObject(pixdDocument);
    return hr;
}