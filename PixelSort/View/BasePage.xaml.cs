﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PixelSort.View
{
    /// <summary>
    /// Interaction logic for BasePage.xaml
    /// </summary>
    public partial class BasePage : UserControl
    {
        public BasePage()
        {
            InitializeComponent();
        }
    }

    /*
     * Standard TextBox, but the only text that is allows is numberic
     */
    public class NumericTextBox : TextBox
    {
        private static readonly Regex regex = new Regex("^[0-9.]+$");

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (!regex.IsMatch(e.Text))
                e.Handled = true;
            base.OnPreviewTextInput(e);
        }
    }
}
