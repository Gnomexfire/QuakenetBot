/*
                    GNU GENERAL PUBLIC LICENSE
                     Version 3, 29 June 2007

         Copyright (C) 2007 Free Software Foundation, Inc. <http://fsf.org/>
         Everyone is permitted to copy and distribute verbatim copies
         of this license document, but changing it is not allowed.

                                    Preamble

          The GNU General Public License is a free, copyleft license for
        software and other kinds of works.

        this file Sdelegates.cs part project Shamia 
*/
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Color = System.Windows.Media.Brushes;

namespace Shamia_Rev2.A.Class
{
    /// <summary>
    /// this class contais methos and events delegate(s)
    /// </summary>
    public static class Sdelegates
    {
        #region declare
        /// <summary>
        /// enumeration methos Part or Join
        /// </summary>
        public enum ParJon
        {
            Leave,
            Enter
        }
        #endregion

        #region delegate(s)

        /// <summary>
        /// delegate
        /// </summary>
        /// <param name="parJon"></param>
        /// <param name="nick"></param>
        private delegate void PartJoinCallback(ParJon parJon,
                                              string nick);

#pragma warning disable 67
        /// <summary>
        /// declaration delegate
        /// </summary>
        private static event PartJoinCallback PartJoinCall;
#pragma warning restore 67

        /// <summary>
        /// methos used update text send text finaly
        /// </summary>
        /// <param name="content">message to update control chat</param>
        public delegate void UpdateChatCallback(string content);
#pragma warning disable 67
        public static event UpdateChatCallback ChatCallback;
#pragma warning restore 67

        /// <summary>
        /// delegate update status label mainwindow
        /// </summary>
        /// <param name="content">content message display</param>
        public delegate void UpdateStatus(string content);
        
#pragma warning disable 67
        public static event UpdateStatus OnUpdateStatus;
#pragma warning restore 67

        public delegate void CheckFloodMax();
#pragma warning disable 67
        public static event CheckFloodMax Flood;
#pragma warning restore 67

        public delegate void AutoKickTemp(string user);

        public static event AutoKickTemp AutoKick;
        #endregion


        /// <summary>
        /// this method used user PART or JOIN channel
        /// </summary>
        /// <param name="parjon">enum Join(enter) Part(Leave)</param>
        /// <param name="nick">user nick</param>
        public static void OnPartJoinCall(ParJon parjon, string nick)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal, (Action) delegate
                {
                    // debug console
                    Console.WriteLine(@"system :" + nick + @" " + parjon.ToString() + @" " + DateTime.Today.ToString("f"));


                    // call method and nick user and event JOIN or PART add chat log disabled
                    //OnChatCallback(@"system :",
                    //               parjon.ToString() + @" " + nick) ;
                });
        }

        /// <summary>
        /// event update chat content
        /// </summary>
        /// <param name="whosent">user or system</param>
        /// <param name="content">message content update  chat </param>
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public static void OnChatCallback(string whosent, 
            string content)
        {

            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal, (Action) delegate
                {
                    List<string> unicodes = new List<string>()
                    {
                        "\u00030\u0003",
                        "\u00031\u0003",
                        "\u00032\u0003",
                        "\u00033\u0003",
                        "\u00034\u0003",
                        "\u00035\u0003",
                        "\u00036\u0003",
                        "\u00036\u0003",
                        "\u00038\u0003",
                        "\u00039\u0003",

                    };

                    if (content.Length > 60)
                    {
                        int l = content.Length / 2;
                        content = content.Substring(0, l) + Environment.NewLine +
                                content.Substring(l, content.Length - l);
                    }

                    string[] c = content.Split(new string[] { "\u0003" }, StringSplitOptions.None);
                    content = c[0];

                    ((MainWindow) Application.Current.MainWindow).Listchat.Items.Add(new TemplateUichat()
                    {
                        Yuser = whosent,
                        Ycontent = content
                    });

                    

                    if (((MainWindow) Application.Current.MainWindow).Chkauto != null &&
                        ((MainWindow) Application.Current.MainWindow).Chkauto.IsChecked.Value)
                    {
                        ((MainWindow)Application.Current.MainWindow).Listchat.ScrollIntoView(
                            ((MainWindow)Application.Current.MainWindow).Listchat.Items[((MainWindow)Application.Current.MainWindow).Listchat.Items.Count -1]
                            );
                    }

                    // check massive flood
                    OnFlood(whosent);


                });
        }

        /// <summary>
        /// this method update control status display message
        /// </summary>
        /// <param name="content"></param>
        public static void OnUpStatus(string content)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal, (Action) delegate
                {
                    //((MainWindow) Application.Current.MainWindow).LblChan.Content = content;
                        ((MainWindow) Application.Current.MainWindow).Effectfade(
                        ((MainWindow) Application.Current.MainWindow).LblChan,
                        MainWindow.Efeitos.Surgir,
                        content,
                        5,
                        Color.White);

                    ((MainWindow) Application.Current.MainWindow).Title =
                        ((MainWindow) Application.Current.MainWindow).LblChan.Content.ToString();
                });
        }

        /// <summary>
        /// massive flood
        /// </summary>
        /// <param name="user">user nick</param>
        private static void OnFlood(string user)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal, (Action) delegate
                {
                    var total = ((MainWindow) Application.Current.MainWindow).Listchat.Items.Count ;
                    if(total < MainWindow.Core.Conf.MaxFlood) { return;}
                    int message = 0;
                    for (int i = 0; i < MainWindow.Core.Conf.MaxFlood; i++) // check the last 3 message
                    {
                        var obj = ((MainWindow) Application.Current.MainWindow).Listchat.Items[total -1] as TemplateUichat ?? new TemplateUichat();
                        if (obj.Yuser == user && obj.Yuser != MainWindow.Core.Conf.Owner) // comparation my user :) no kick shamia bot
                        {
                            message++;
                        }
                        total--;
                    }
                    if (message == MainWindow.Core.Conf.MaxFlood) 
                    {
                        // user flooder
                        Console.WriteLine(user.Replace(":", string.Empty) + @" flood");
                        // send kick command to irc to user flood
                        MainWindow.Core.SendToIrc(@"/KICK " + user.Replace(":",string.Empty) + " flood",true);

                        // add message chat list
                        Console.WriteLine(user.Replace(":",string.Empty) + @" kicked flood");
                        Sdelegates.OnChatCallback(@"system: ",
                                                  user.Replace(":", string.Empty) + @" kicked flood");
                    }
                });
        }
        /// <summary>
        /// tempore list user auto kick BANTIME
        /// </summary>
        public static void OnAutoKick(string user)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal, (Action) delegate
                {
                    if (SBanlist.UserInList(user))
                    {
                        // kick
                        // send kick commando to Irc
                        MainWindow.Core.SendToIrc(@"/KICK " + user.Replace(":",string.Empty) + " autokick",true);
                    }
                });
        }
    }
}
