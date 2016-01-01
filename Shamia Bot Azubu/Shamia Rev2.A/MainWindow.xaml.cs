using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Shamia_Rev2.A.Class;
using Shamia_Rev2.A.view;

namespace Shamia_Rev2.A
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            GlowBrush = new SolidColorBrush(Colors.White);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            // example add user in list
            //for (int i = 0; i < 10; i++)
            //{
            //    ListViewUser.Items.Add(new Ui()
            //    {
            //        Uicon = new BitmapImage(new Uri(
            //            AppDomain.CurrentDomain.BaseDirectory + "\\icon\\icon1.jpg")),
            //        Uiuser = @"user " + i
            //    });
            //}

            // set default language US
            LangMode.SetLang(LangMode.Lang.Portuguese);
        }

        private void Cmdfly_OnClick(object sender, RoutedEventArgs e)
        {
            Myfly.IsOpen = true;
            ListViewUser.SelectionMode = SelectionMode.Single;



        }

        private void ListViewUser_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if(ListViewUser.SelectedItem == null) { return;}

            var i = ListViewUser.SelectedItem as Ui ?? new Ui();

            Console.WriteLine(i.Uiuser);
        }



        private void ListViewUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            object obj = GetListViewItemObject(ListViewUser, e.OriginalSource);
            if (obj.GetType() == typeof(Ui))
            {
                Ui myObject = (Ui)obj;
                // Add the rest of your logic here.
                //MessageBox.Show(myObject.Uiuser);
            }
        }

        private void OnListViewItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            

        }

        private object GetListViewItemObject(ListView LV, object originalSource)
        {
            DependencyObject dep = (DependencyObject)originalSource;
            while ((dep != null) && !(dep.GetType() == typeof(ListViewItem)))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            if (dep == null)
                return null;
            object obj = (Object)LV.ItemContainerGenerator.ItemFromContainer(dep);
            return obj;
        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        private void CmdConf_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var conf = new Adjust()
                {
                    ShowTitleBar = false,
                    IsWindowDraggable = false,
                    TitleCaps = false,
                    GlowBrush = new SolidColorBrush(Colors.White)
                };
                conf.ShowDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);   
            }
        }

       

        private void Cmdabout_OnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
