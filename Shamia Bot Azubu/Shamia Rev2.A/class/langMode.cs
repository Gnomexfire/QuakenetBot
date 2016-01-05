using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Shamia_Rev2.A.view;

namespace Shamia_Rev2.A.Class
{
    public static class LangMode
    {
        public enum Lang
        {
            Portuguese,
            English,
            //Chinese,
            Korean
        }
        /// <summary>
        /// is the current language 
        /// </summary>
        public static Lang Elang { get; set; }

        /// <summary>
        /// method update language 
        /// </summary>
        /// <param name="l">enum language</param>
        public static void SetLang(Lang l)
        {
            var s = String.Empty;

            switch (l)
            {
                case Lang.Portuguese:
                    s = "..\\Lang\\LANG_BR.xaml";
                    break;
                case Lang.English:
                    s = "..\\Lang\\LANG_EN.xaml";
                    break;
                //case Lang.Chinese:
                //    s = "..\\Lang\\LANG_CH.xaml";
                //    break;
                case Lang.Korean:
                    s = "..\\Lang\\LANG_KO.xaml";
                    break;
            }
            if (s == String.Empty) { return; }
            Elang = l;
            //((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries.Clear();

            ResourceDictionary d = new ResourceDictionary()
            {
                Source = new Uri(s, UriKind.Relative)
            };
            ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries.Add(d);
        }
        /// <summary>
        /// return object 
        /// </summary>
        /// <returns></returns>
        public static ResourceDictionary Objectlang()
        {
            var s = String.Empty;

            switch (Elang)
            {
                case Lang.Portuguese:
                    s = "..\\Lang\\LANG_BR.xaml";
                    break;
                case Lang.English:
                    s = "..\\Lang\\LANG_EN.xaml";
                    break;
                //case Lang.Chinese:
                //    s = "..\\Lang\\LANG_CH.xaml";
                //    break;
                case Lang.Korean:
                    s = "..\\Lang\\LANG_KO.xaml";
                    break;
            }
            if (s != string.Empty)
            {
                ResourceDictionary d = new ResourceDictionary()
                {
                    Source =  new Uri(s ,UriKind.Relative)
                };
                return d;
            }
            return null;
        }
    }
}
