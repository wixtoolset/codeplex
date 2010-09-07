//-------------------------------------------------------------------------------------------------
// <copyright file="BurnUnitTest.h" company="Microsoft">
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
//    Base class for Burn Unit tests.
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
    using namespace System;
    using namespace Microsoft::VisualStudio::TestTools::UnitTesting;

    [TestClass]
    public ref class BurnUnitTest
    {
    private:
        TestContext^ testContextInstance;

    public: 
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        property Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ TestContext
        {
            Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ get()
            {
                return testContextInstance;
            }
            System::Void set(Microsoft::VisualStudio::TestTools::UnitTesting::TestContext^ value)
            {
                testContextInstance = value;
            }
        };

        //Use ClassInitialize to run code before running the first test in the class
        BurnUnitTest ()
        {
            HRESULT hr = XmlInitialize();
            TestThrowOnFailure(hr, L"Failed to initialize XML support.");

            hr = RegInitialize();
            TestThrowOnFailure(hr, L"Failed to initialize Regutil.");
        }

        //Use ClassCleanup to run code after all tests in a class have run
        ~BurnUnitTest()
        {
            XmlUninitialize();
            RegUninitialize();
        }

        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        void TestInitialize()
        {
            HRESULT hr = S_OK;

            LogInitialize(::GetModuleHandleW(NULL));

            hr = LogOpen(NULL, L"BurnUnitTest", NULL, L"txt", FALSE, FALSE, NULL);
            TestThrowOnFailure(hr, L"Failed to open log.");

            PlatformInitialize();
        }

        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        void TestCleanup() 
        {
            LogUninitialize(FALSE);
        }
    }; 
}
}
}
}
}
