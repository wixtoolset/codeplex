﻿//-----------------------------------------------------------------------
// <copyright file="MaintenanceModePage.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//    
//    The use and distribution terms for this software are covered by the
//    Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
//    which can be found in the file CPL.TXT at the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by
//    the terms of this license.
//    
//    You must not remove this notice, or any other, from this software.
// </copyright>
// <summary>window for uninstall, repair and change</summary>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace BurnSampleWPFUI
{
    /// <summary>
    /// Interaction logic for MaintenanceModePage.xaml
    /// </summary>
    public partial class MaintenanceModePage : UserControl
    {
        private BurnSetup m_MainWindow;

        public MaintenanceModePage(BurnSetup mainWindow)
        {
            InitializeComponent();
            m_MainWindow = mainWindow;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            m_MainWindow.ShowCancelWindow(ManagedSetupUX.UIPage.Welcome);
        }

        private void btnRepair_Click(object sender, RoutedEventArgs e)
        {
            m_MainWindow.m_Mode = ManagedSetupUX.INSTALL_MODE.REPAIR_FULL_DISPLAY;

            if (File.Exists(m_MainWindow.m_ItemPlanFilePath))
                File.Delete(m_MainWindow.m_ItemPlanFilePath);

            m_MainWindow.PostActionMessage();
        }

        private void btnUninstall_Click(object sender, RoutedEventArgs e)
        {
            m_MainWindow.m_Mode = ManagedSetupUX.INSTALL_MODE.UNINSTALL_FULL_DISPLAY;

            if (File.Exists(m_MainWindow.m_ItemPlanFilePath))
                File.Delete(m_MainWindow.m_ItemPlanFilePath);

            m_MainWindow.PostActionMessage();
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            m_MainWindow.m_Mode = ManagedSetupUX.INSTALL_MODE.UNINSTALL_FULL_DISPLAY;
            m_MainWindow.LoadWelcomePage();
        }       
    }
}
