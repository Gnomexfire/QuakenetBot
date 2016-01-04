using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using mshtml;
using MahApps.Metro.Controls;
using Shamia_Rev2.A.Class;
using Shamia_Rev2.A.view;
using Color = System.Windows.Media.Brushes;

namespace Shamia_Rev2.A
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region declare
        public static Score Core = new Score();
        
        /// <summary>
        /// enumeration used effectfade
        /// </summary>
        public enum Efeitos
        {
            Surgir,
            Desaparecer,
        }
        /// <summary>
        /// flag set irc connect or disconnect
        /// </summary>
        public static bool ShamiaIsAlive { get; set; }
        #endregion


        public MainWindow()
        {
            InitializeComponent();
            GlowBrush = new SolidColorBrush(Colors.White);
            //Background = new SolidColorBrush(Colors.Transparent);
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

            // set ShamiaIsAlive false
            ShamiaIsAlive = false;

            // set default language US
            LangMode.SetLang(LangMode.Lang.Portuguese);

            // example add chat content
            //Listchat.Items.Add(new TemplateUichat
            //{
            //    Yuser = @"System :",
            //    Ycontent = @"Teste message "  
            //});

            //Core.Connect();

            //Sdelegates.OnUpStatus(@"shamia , come on ?");
            Sdelegates.OnUpStatus(Application.Current.MainWindow.Resources.MergedDictionaries[0]["Shamialetstart"].ToString());

            // hide listviewchat and checkbox
            Listchat.Visibility = Visibility.Collapsed;
            Chkauto.Visibility = Visibility.Collapsed;

            // update mainwindow title message
            Title = LblChan.Content.ToString();

            // check flag gist enbled application
            _gist();

            // hide console
            Showconsole.ShowConsole(false);
        }

        public void _gist()
        {
            Webgist.Navigate(new Uri("https://gist.githubusercontent.com/Gnomexfire/06265e4c82e39f6ac8e3/raw/25b99f7d5c0ddcf186cc8cb037d74973e52271c1/shamia_tester", UriKind.Absolute));
            Webgist.LoadCompleted += Webgist_LoadCompleted;
        }

        private void Webgist_LoadCompleted(object sender, NavigationEventArgs e)
        {
            dynamic c = Webgist.Document;
            string h = c.documentElement.innerHtml;
            if (h.Contains("disabled"))
            {
                Effectfade(LblChan, Efeitos.Surgir, @"application disabled", 5, Brushes.Red);
                Cmdconnect.IsEnabled = false;
            }

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

        private object GetListViewItemObject(ListView lv, object originalSource)
        {
            DependencyObject dep = (DependencyObject)originalSource;
            while ((dep != null) && !(dep.GetType() == typeof(ListViewItem)))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            if (dep == null)
                return null;
            object obj = (Object)lv.ItemContainerGenerator.ItemFromContainer(dep);
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

        private void Cmdconnect_OnClick(object sender, RoutedEventArgs e)
        {
            // not connected
            if (!ShamiaIsAlive)
            {
                Core.Connect();
                ShamiaIsAlive = true; // set ShamiaIsAlive true
                // progress ring
                _progressring();
                // update title control
                Textblockcmdconnect.Text =
                    Application.Current.MainWindow.Resources.MergedDictionaries[0]["Disconnectazubu"].ToString();
            }
            // connected
            else
            {
                Core.Disconnect();
                ShamiaIsAlive = false; // set ShamiaIsAlive false
                // update title control
                Textblockcmdconnect.Text =
                    Application.Current.MainWindow.Resources.MergedDictionaries[0]["Connectazubu"].ToString();

                // hide listviewchat and checkbox
                Listchat.Visibility = Visibility.Collapsed;
                Chkauto.Visibility = Visibility.Collapsed;

                // disble thread
               //Core.UThread.Abort();

            }


        }
        /// <summary>
        /// enable and disable progress ring
        /// </summary>
        internal void _progressring(bool enabled =true)
        {
            if (enabled)
            {
                Listchat.Visibility = Visibility.Collapsed;
                Chkauto.Visibility = Visibility.Collapsed;
                CanvasProgressring.Visibility = Visibility.Visible;
                RingP.IsActive = true;
                return;    
            }

            // disable progress ring
            Listchat.Visibility = Visibility.Visible;
            Chkauto.Visibility = Visibility.Visible;
            CanvasProgressring.Visibility = Visibility.Collapsed;
            RingP.IsActive = false;
        }
        #region effectfade 
        /// <summary>
        /// animacao label
        /// </summary>
        /// <param name="obj">label</param>
        /// <param name="show">1 sumindo 0 aparece</param>
        /// <param name="message"></param>
        /// <param name="time"></param>
        /// <param name="color"></param>
        public void Effectfade(Label obj, Efeitos show = 0, string message = null, int time = 5, Brush color = null)
        {
            obj.Foreground = color ?? Color.White;
            if (message != null)
            {
                obj.Content = message;
            }

            // Create a storyboard to contain the animations.
            Storyboard storyboard = new Storyboard();
            TimeSpan duration = new TimeSpan(0, 0, time);

            // Create a DoubleAnimation to fade the not selected option control
            DoubleAnimation animation = new DoubleAnimation();

            // Oculta
            if (show == Efeitos.Desaparecer)
            {
                obj.Opacity = 0;
                animation.From = 1.0;
                animation.To = 0.0;
            }
            // Show
            else
            {
                animation.From = 0.0;
                animation.To = 1.0;
            }

            animation.Duration = new Duration(duration);
            // Configure the animation to target de property Opacity
            Storyboard.SetTargetName(animation, obj.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));
            // Add the animation to the storyboard
            storyboard.Children.Add(animation);

            // Begin the storyboard
            storyboard.Begin(this);

        }

        #endregion

        private void Txtsend_KeyDown(object sender, KeyEventArgs e)
        {
            // send simple message
            if (e.Key == Key.Enter && Txtsend.Text.Trim() != string.Empty != Txtsend.Text.StartsWith("/"))
            {
                Core.SendToIrc(Txtsend.Text);
                Txtsend.Text = string.Empty;
            }
            // send command
            else if (e.Key == Key.Enter && Txtsend.Text.Trim() != string.Empty && Txtsend.Text.StartsWith("/"))
            {
                Core.SendToIrc(Txtsend.Text,true);
                Txtsend.Text = string.Empty;
            }


        }
        /// <summary>
        /// enabled auto scroll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chkauto_OnChecked(object sender, RoutedEventArgs e)
        {
           
        }
        /// <summary>
        /// disable auto scroll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chkauto_OnUnchecked(object sender, RoutedEventArgs e)
        {

        }

       
    }
}
