using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Shamia_Rev2.A.view;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace Shamia_Rev2.A.Class
{
    /// <summary>
    /// class represent core shamia
    /// </summary>
    public partial class Score 
    {
        #region delegate

        public delegate void PingPongCallBack(string s);

#pragma warning disable 67
        public static event PingPongCallBack PCallback;
#pragma warning restore 67
        

        #endregion

        #region declare 
        /// <summary>
        /// used in buffer
        /// </summary>
        public string Buff { get; set; }

        /// <summary>
        /// declaration struct
        /// </summary>
        public Sconf Conf;

        private TextReader _input;
        private TextWriter _output;
        public System.Net.Sockets.TcpClient Ssock;
        public Thread UThread;

        #endregion
        public struct Sconf
        {
            /// <summary>
            /// port server default 6667
            /// </summary>
            public int Port { get; set; }
            /// <summary>
            /// nick user used SSL
            /// </summary>
            public string Nick { get; set; }
            /// <summary>
            /// owner nick in chat
            /// </summary>
            public string Owner { get; set; }
            /// <summary>
            /// server irc 
            /// </summary>
            public string Server { get; set; }
            /// <summary>
            /// channel to connect
            /// </summary>
            public string Chan { get; set; }
            /// <summary>
            /// used for auth ssl if 1 true
            /// </summary>
            public int AuthSsl { get; set; }
            /// <summary>
            /// password used Ssl
            /// </summary>
            public string Pwssl { get; set; }
            /// <summary>
            /// object element webauth
            /// </summary>
            public string Loginnickname { get; set; }

            /// <summary>
            /// consecutive message send(s) prevent flood
            /// </summary>
            public int MaxFlood { get; set; }
            /// <summary>
            /// flag language PT US KO
            /// </summary>
            public string Langflag { get; set; }
        }
        /// <summary>
        /// constructor class
        /// </summary>
        public Score()
        {
            // load config => struct Sconf
            // example json load config file 
            string json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory +
                                           "\\conf.json");
            TemplateSconf conf = new JavaScriptSerializer().Deserialize<TemplateSconf>(json);

            Conf.Port = conf.Port; // set port json file => struct
            Conf.Nick = conf.Nick; // set nick  json file => struct
            Conf.Owner = conf.Owner; // set owner json file => struct
            Conf.Server = conf.Server; // set server json file => struct
            Conf.Chan = conf.Chan; // set channel json file => struct
            Conf.AuthSsl = conf.AuthSsl; // if 1 auth ssl
            Conf.Pwssl = conf.Pwssl; // password used ssl
            Conf.Loginnickname = conf.Loginnickname ; // object element login auth
            Conf.MaxFlood = conf.MaxFlood; // set maxime consecutive message user send chat default 3 

            Console.Clear();
            Console.WriteLine(@"port :" + Conf.Port + Environment.NewLine +
                              @"nick :" + Conf.Nick + Environment.NewLine +
                              @"owner :" + Conf.Owner + Environment.NewLine +
                              @"server :" + Conf.Server + Environment.NewLine +
                              @"channel :" + Conf.Chan + Environment.NewLine +
                              @"use ssl :" + Conf.AuthSsl + Environment.NewLine +
                              @"success load config.json");
        }

        public void ScoreLoad()
        {
            // example json load config file 
            string json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory +
                                           "\\conf.json");
            TemplateSconf conf = new JavaScriptSerializer().Deserialize<TemplateSconf>(json);

            Conf.Port = conf.Port; // set port json file => struct
            Conf.Nick = conf.Nick; // set nick  json file => struct
            Conf.Owner = conf.Owner; // set owner json file => struct
            Conf.Server = conf.Server; // set server json file => struct
            Conf.Chan = conf.Chan; // set channel json file => struct
            Conf.AuthSsl = conf.AuthSsl; // if 1 auth ssl
            Conf.Pwssl = conf.Pwssl; // password used ssl
            Conf.Loginnickname = conf.Loginnickname; // object element login auth
            Conf.MaxFlood = conf.MaxFlood; // set maxime consecutive message user send chat default 3 
            Conf.Langflag = conf.Langflag; // set lang flag
            Console.Clear();
            Console.WriteLine(@"port :" + Conf.Port + Environment.NewLine +
                              @"nick :" + Conf.Nick + Environment.NewLine +
                              @"owner :" + Conf.Owner + Environment.NewLine +
                              @"server :" + Conf.Server + Environment.NewLine +
                              @"channel :" + Conf.Chan + Environment.NewLine +
                              @"use ssl :" + Conf.AuthSsl + Environment.NewLine +
                              @"success load config.json");
        }


        /// <summary>
        /// this method step 1 connect to server and channel
        /// </summary>
        /// <returns></returns>
        public void Connect()
        {
            try
            {
                // if thread is alive abort thread creat new 
                //if (!Thread.IsAlive)
                //{
                //    Thread.Abort();
                //}
                UThread?.Abort();

                UThread = new Thread(ThreadIrc) { IsBackground = true };
                UThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return ;
            }
        }

        /// <summary>
        /// this method destroy thread if created
        /// </summary>
        public void Disconnect()
        {
            try
            {
                //if thread is alive abort thread creat new
                if (UThread.IsAlive)
                {
                    MainWindow.Core.SendToIrc("/QUIT",true); // send QUIT command IRC

                    //_output.Close();
                    //_input.Close();
                    //Ssock.Close();
                    Sdelegates.OnUpStatus(((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["Shamialetstart"].ToString());
                    ((MainWindow)Application.Current.MainWindow).Listchat.Items.Clear();
                    Console.Clear();
                    Console.WriteLine(@"disconnected by user");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);    
            }
        }

        internal void ThreadIrc()
        {
            try
            {
                Ssock = new TcpClient();
                Ssock.Connect(Conf.Server, Conf.Port);
                if (!Ssock.Connected)
                {
                    Console.WriteLine(@"Failed to connect socket [ internal void ThreadIrc() ]");
                    return;
                }
                // continue :
                Console.Clear();
                Console.WriteLine(@"continue:");


                _input = new StreamReader(Ssock.GetStream());
                _output = new StreamWriter(Ssock.GetStream());

                // step 1
                _output.Write(
                    "USER " + Conf.Nick + " 0 * :" + Conf.Owner + "\r\n" +
                    "NICK " + Conf.Owner + "\r\n");
                _output.Flush();

                // step 2
                if (Conf.AuthSsl == 1)
                {
                    Console.WriteLine(@"AuthSsl: true");
                    // auth ssl add privileged for user moderate channel
                    LoginSsl(Conf.Nick, Conf.Pwssl);

                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Normal, (Action) delegate
                        {
                            Sdelegates.OnUpStatus(
                                ((MainWindow) Application.Current.MainWindow).Resources.MergedDictionaries[0]["AuthSslEnable"].ToString());

                           // open page login quakenet auth
                            //System.Diagnostics.Process.Start("https://auth.quakenet.org/webchat_ssl");
                        });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Normal, (Action) delegate
                        {
                            Sdelegates.OnUpStatus(
                                ((MainWindow) Application.Current.MainWindow).Resources.MergedDictionaries[0]["AuthSslDisable"].ToString());
                        });
                    Console.WriteLine(@"AuthSsl: false");
                }
                Console.WriteLine();
                //if(Buff != null)
                // process each line received from irc server
                for (Buff = _input.ReadLine();; Buff = _input.ReadLine())
                {
                    PCallback += OutPCallback;
                    if(Buff == null) { return;}
                    OutPCallback(Buff);

                    //send pong reply to any ping messages
                    if (Buff != null && Buff.StartsWith("PING"))
                    {
                        _output.Write(Buff.Replace("PING","PONG") +"\r\n");
                        _output.Flush();
                    }
                    if(Buff != null && Buff[0] != ':') continue;

                    if (Buff != null && Buff.Split(' ')[1] == "001")
                    {
                        var b = Buff;
                        //_output.Write("MODE " + Conf.Nick + "+i +o +m +x +B +n \r\n" +

                        _output.Write("MODE " + Conf.Nick + "+o \r\n" +

                                      "JOIN " + Conf.Chan + "\r\n");
                        _output.Flush();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.Clear();
                if(ex.HResult != -2146233040)
                Console.WriteLine(ex.StackTrace);
            }
            


        }
        /// <summary>
        /// this method auth user to moderate channel
        /// </summary>
        /// <param name="user">user past by email</param>
        /// <param name="pw">pw past by email</param>
        internal void LoginSsl(string user, string pw)
        {
            _output = new StreamWriter(Ssock.GetStream());
            _output.Write("OPER " + user + " " + pw + " " + "\r\n");
            //_output.Write("OPER " + user + " " + pw + " " + "\r\n");
            _output.Flush();

            //// test show window auth https://webchat.quakenet.org/
            //var auth = new Webauth()
            //{
            //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
            //    ShowTitleBar = false,
            //    BorderThickness = new Thickness(1),
            //    BorderBrush = new SolidColorBrush(Colors.White)
            //};
            //auth.Show();
        }

        /// <summary>
        /// destructor
        /// </summary>
        ~Score()
        {
            
        }

        private static void OutPCallback(string s)
        {
                Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal, (Action) delegate
                {
                    /*
                    
                        message body of message IRC
                        PART - leave channel
                        JOIN - enter channel

                        ex: string receive server IRC
                        ":teste!webchat@000.00.000.000 JOIN #azubu.channel"

                        ex:
                        step 1 split parts[]
                        part[0] -teste

                    */
                    if(s != null)
                    Console.WriteLine(s);
                    
                    // check Part or Join
                    if (s.Contains("PART") || s.Contains("JOIN"))
                    {
                        string[] part = s.Split(new string[] {":", "!"}, StringSplitOptions.None); // nick user

                        // auto kick check list contains user
                        Sdelegates.OnAutoKick(part[1]);

                        // leave channel
                        if (s.Contains("PART"))
                        {
                            Sdelegates.OnPartJoinCall(Sdelegates.ParJon.Leave,
                                part[1]);
                        }
                        // enter channel
                        else if (s.Contains("JOIN"))
                        {
                            Sdelegates.OnPartJoinCall(Sdelegates.ParJon.Enter,
                                part[1]);

                            
                        }

                    }
                    // if user send message to channel

                    /*
                    ex:
                    ":teste!webchat@000.00.000.000 PRIVMSG #azubu.rafael_teste :dasda"

                    */
                    else if (s.Contains("PRIVMSG") && !s.Contains("CPRIVMSG"))
                    {
                        string[] msg = s.Split(new string[] {":",@"\"}, StringSplitOptions.None);
                        // msg[2] == content message
                        // msg.count() -1 ex content messa irc  remove(\u00038\u0003)

                        /*
                        paullascs!azubu@aa11cbb1.flash.quakenet.org PRIVMSG #azubu.henrytado.br "Henry o que você acha da história do Aruan?\u00038\u0003 


                        */
                        for (int i = 0; i < msg.Count() ; i++)
                        {
                            if (i > 2)
                            {
                                msg[2] += msg[i];
                            }
                        }


                        string[] user = s.Split(new string[] {":","!"}, StringSplitOptions.None);
                        // user[1] == user nick
                        
                        
                        Sdelegates.OnChatCallback(user[1] + ": ",
                            msg[2]);
                        //msg[2]    );
                    }
                    // welcome to channel
                    else if (s.Contains("CPRIVMSG"))
                    {
                        /*
                        ":servercentral.il.us.quakenet.org 005 teste WHOX WALLCHOPS WALLVOICES USERIP CPRIVMSG CNOTICE SILENCE=15 MODES=6 MAXCHANNELS=20 MAXBANS=45 NICKLEN=15 :are supported by this server"
                        */
                        string[] chan = s.Split(new string[] {" "}, StringSplitOptions.None);
                        //Console.WriteLine(chan[1]); Application.Current.MainWindow.Resources.MergedDictionaries[0]["Welcomechat"].ToString()
                        Sdelegates.OnUpStatus(Application.Current.MainWindow.Resources.MergedDictionaries[0]["Welcomechat"] + @" " + MainWindow.Core.Conf.Chan);

                        // disable progress ring 
                        Application.Current.Dispatcher.Invoke(
                            DispatcherPriority.Normal, (Action) delegate
                            {
                                ((MainWindow)Application.Current.MainWindow)._progressring(false);
                                // auth for privilege(s) to channel
                                MainWindow.Core.SendToIrc(@"/AUTH " + MainWindow.Core.Conf.Nick + " " + MainWindow.Core.Conf.Pwssl,true);

                            });
                    }

                });
        }

        /// <summary>
        /// method send message to irc server
        /// </summary>
        /// <param name="s">string content message</param>
        /// <param name="iscommand"></param>
        public void SendToIrc(string s,bool iscommand=false)
        {

            if (UThread == null) { return; }
            //if (!UThread.IsAlive) { return;}
            string[] r = s.Split(new string[] {" ","/"}, StringSplitOptions.None);

            _output = new StreamWriter(Ssock.GetStream());



            try
            {
            // send command
            if (iscommand)
            {
                // auth privilege
                if (r[1] == "AUTH")
                {
                    string[] content = s.Split(new string[] {" ", "/"}, StringSplitOptions.None);
                    _output.Write(@"AUTH " + content[2] + @" " + content[3] + "\r\n");
                    _output.Flush();

                    // display message listviewchat system
                    Sdelegates.OnChatCallback(@"system :",
                        Application.Current.MainWindow.Resources.MergedDictionaries[0]["Authsucess"].ToString());
                }
                // kick user
                else if (r[1] == "KICK")
                {
                    // command kick ex :
                    // KICK <#channel> <nick> <comment banned>
                    string[] k = s.Split(new string[] {"/"," "}, StringSplitOptions.None);
                    _output.Write(@"KICK " + MainWindow.Core.Conf.Chan + " " + k[2] + " " + k[3] + "\r\n");
                    _output.Flush();
                }
                
                else if (r[1] == "BANTIME")
                {
                    // created simple list auto kick user if join channel if banned for TIMEOUT
                    SBanlist.AddBanList(r[2]);
                        // add command to send in listviewchat
                        Sdelegates.OnChatCallback(@"system: ",
                                                  @"BANTIME => " + r[2]);
                        // kick user
                        // auto kick check list contains user
                        Sdelegates.OnAutoKick(r[2]);
                    }
                else if (r[1] == "UNBANTIME")
                {
                    SBanlist.RemoveBanList(r[2]);
                        // add command to send in listviewchat
                        Sdelegates.OnChatCallback(@"system: ",
                                                  @"UNBANTIME => " + r[2]);
                }
                else if (r[1] == "CLEARBANTIME")
                {
                    SBanlist.ClearList();
                        // add command to send in listviewchat
                        Sdelegates.OnChatCallback(@"system: ",
                                                  @"CLEARBANTIME => CLEAN");
                    }
                else if (r[1] == "BANDAYS")
                {
                    
                }
                // clear server channel chat
                else if (r[1] == "CLEAR")
                {
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Normal, (Action) delegate
                        {
                            ((MainWindow)Application.Current.MainWindow).Listchat.Items.Clear();
                            Sdelegates.OnChatCallback(@"system: ",
                                                      @"clean client chat");
                        });
                }
                // send pure command to server
                else if (r[1] == "CMD")
                {
                    // send pure command 
                     _output.Write(s.Replace("/CMD", string.Empty) + "\r\n");
                     _output.Flush();
                }
                // Q PERMBAN <#channel> <banmask> [<reason>]
                else if (r[1] == "BANFOREVER")
                {
                            
                }
                // show debug console
                else if (r[1] == "DEBUG")
                {
                    if (Showconsole.ConsoleIsShow())
                    {
                        Showconsole.ShowConsole();
                    }
                    else
                    {
                        Showconsole.ShowConsole(false);
                    }
                }
                // quit
                else if (r[1] == "QUIT")
                {
                    _output.Write(@"QUIT" + "\r\n");
                    _output.Flush();
                }


                    // add command to send in listviewchat
                    //Sdelegates.OnChatCallback(MainWindow.Core.Conf.Nick,
                    //                          r[1].ToString());
                    Console.WriteLine(MainWindow.Core.Conf.Nick + @" " +r[1]);
                    //return;
                }
                else
            {
                //// send pure command 
                //_output.Write(s + "\r\n");
                //_output.Flush();
                //return;
                //

                // send simple message
                _output.Write("PRIVMSG " + MainWindow.Core.Conf.Chan + " : " + s + "\r\n");
                _output.Flush();
                // add message to send in listviewchat
                Sdelegates.OnChatCallback(MainWindow.Core.Conf.Owner,
                                          s);
            }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Sdelegates.OnChatCallback(@"error: ",
                                          ex.HResult.ToString());
            }

        }
    }
    /// <summary>
    /// class represent json file => conf.json
    /// </summary>
    public class TemplateSconf
    {
        /// <summary>
        /// port server default 6667
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// nick user used SSL
        /// </summary>
        public string Nick { get; set; }
        /// <summary>
        /// owner nick in chat
        /// </summary>
        public string Owner { get; set; }
        /// <summary>
        /// server irc 
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// channel to connect
        /// </summary>
        public string Chan { get; set; }
        /// <summary>
        /// used for auth ssl if 1 true
        /// </summary>
        public int AuthSsl { get; set; }
        /// <summary>
        /// password used Ssl
        /// </summary>
        public string Pwssl { get; set; }
        /// <summary>
        /// object element auth login
        /// </summary>
        public string Loginnickname { get; set; }
        /// <summary>
        /// consecutive message send(s) prevent flood
        /// </summary>
        public int MaxFlood { get; set; }

        /// <summary>
        /// lang flag
        /// </summary>
        public string Langflag { get; set; }
    }

    /// <summary>
    /// this class represent template listview
    /// </summary>
    public class TemplateUichat
    {
        /// <summary>
        /// user or system send message
        /// </summary>
        public string Yuser { get; set; }
        /// <summary>
        /// body message 
        /// </summary>
        public string Ycontent { get; set; }
    }
}
