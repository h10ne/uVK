﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace uVK
{
    /// <summary>
    /// Логика взаимодействия для InputBoxWindow.xaml
    /// </summary>
    public partial class InputBoxWindow : Window
    {
        public InputBoxWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("someFile.tempdat", tbCode.Text);
            Close();
        }
    }
}
