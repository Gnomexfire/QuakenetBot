using System;
using System.Collections.Generic;
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
using MahApps.Metro.Controls;
using Shamia_Rev2.A.Class;

namespace Shamia_Rev2.A.view
{
    /// <summary>
    /// Interaction logic for adjust.xaml
    /// </summary>
    public partial class Adjust
    {
        public Adjust()
        {
            InitializeComponent();
        }

        private void Adjust_OnLoaded(object sender, RoutedEventArgs e)
        {
            _translate();
        }
        /// <summary>
        /// used translate 
        /// </summary>
        internal void _translate()
        {
            var response = LangMode.Objectlang();
            Resources.MergedDictionaries.Add(response);
        }
    }
}
