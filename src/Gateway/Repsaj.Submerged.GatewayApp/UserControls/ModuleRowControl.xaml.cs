﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Repsaj.Submerged.GatewayApp.UserControls
{
    public sealed partial class ModuleRowControl : UserControl
    {
        public ModuleRowControl()
        {
            this.InitializeComponent();
            borMain.DataContext = this;
        }

        #region ModuleName Property
        public string ModuleName
        {
            get { return (string)GetValue(TileModuleNameProperty); }
            set { SetValue(TileModuleNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TileText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TileModuleNameProperty =
            DependencyProperty.Register(nameof(ModuleName), typeof(string), typeof(ModuleRowControl), null);
        #endregion

        #region ModuleState Property
        public string ModuleState
        {
            get { return (string)GetValue(TileModuleStateProperty); }
            set { SetValue(TileModuleStateProperty, value); }
        }

        public static readonly DependencyProperty TileModuleStateProperty =
            DependencyProperty.Register(nameof(ModuleState), typeof(string), typeof(ModuleRowControl), null);
        #endregion
    }
}