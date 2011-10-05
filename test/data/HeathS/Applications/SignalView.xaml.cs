﻿//-------------------------------------------------------------------------------------------------
// <copyright file="SignalView.xaml.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// <summary>
// Code-behind file for SignalView.xaml.
// </summary>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;

namespace Signal
{
    /// <summary>
    /// Interaction logic for SignalView.xaml
    /// </summary>
    public partial class SignalView : Window
    {
        public SignalView()
        {
            InitializeComponent();

            // Keep the ListBox scrolled to the bottom.
            this.Messages.ItemContainerGenerator.ItemsChanged += (s, e) =>
                {
                    if (NotifyCollectionChangedAction.Add == e.Action && 0 < this.Messages.Items.Count)
                    {
                        object item = this.Messages.Items[this.Messages.Items.Count - 1];
                        this.Messages.ScrollIntoView(item);
                    }

                };
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            HwndSource source = HwndSource.FromVisual(this) as HwndSource;
            if (null != source)
            {
                SignalViewModel viewModel = this.DataContext as SignalViewModel;
                if (null != viewModel)
                {
                    source.AddHook(viewModel.WindowProc);
                }
            }
        }

        private void OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (null != checkBox)
            {
                checkBox.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateTarget();
            }
        }
    }
}