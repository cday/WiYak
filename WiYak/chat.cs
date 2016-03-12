using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace WiYak
{
    public class Chat : IDisposable
    {
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Thread> Threads { get; set; }
        public Thread Lobby { get; set; }
        public User Me { get; set; }
        public Controls Settings { get; set; }

        public Channel Connection;

        private const string DefaultName = "User";
        private const string HostAddress = "224.0.1.11";
        private const int PortNum = 54329;

        public Chat ()
        {
            this.Threads = new ObservableCollection<Thread> ();
            this.Lobby = new Thread (new ObservableCollection<User> ());
            this.Users = new ObservableCollection<User>();
            this.Connection = new Channel (HostAddress, PortNum);
            this.Me = new User (DefaultName);
            this.Settings = new Controls();

            this.Settings.LobbyMessages_ = false;
            this.Settings.PrivateMessages_ = true;
            this.Settings.Joined_ = false;

            this.Connection.Open();

            RegisterEvents();
            StartKeepAlive();
        }

        public void Dispose()
        {
            UnregisterEvents();
            StopkeepAlive();
        }

        #region Public Functions

        public void Join ()
        {
            if (!Settings.Joined_ && this.Connection.IsJoined)
            {
                this.Settings.Joined_ = true;
                this.Connection.ChannelOn = true;
                this.Connection.Send(Commands.JoinFormat, this.Me.Name);

                Display.MessageBoxShow("joined the chat.", "Join", false);
            }
            else
            {
                if (this.Settings.Joined_)
                {
                    Display.MessageBoxShow("already joined to the chat.", "Join", false);
                }
                else
                {
                    Display.MessageBoxShow("cannot join the chat, problem with wi-fi network.", "Join", false);
                }
            }
        }

        public void Disconnect()
        {
            if (this.Settings.Joined_ && this.Connection.IsJoined)
            {
                this.Connection.Send(Commands.LeaveFormat, this.Me.Name);
                this.Connection.ChannelOn = false;
                this.Settings.Joined_ = false;

                foreach (User user in this.Users)
                {
                    user.Active = false;
                }

                Display.MessageBoxShow("disconnected from the chat.", "Disconnect", false);
            }
            else
            {
                Display.MessageBoxShow("not joined to the chat.", "Disconnect", false);
            }
        }

        public void DisconnectClean()
        {
            if (this.Settings.Joined_ && this.Connection.IsJoined)
            {
                this.Connection.Send(Commands.LeaveFormat, this.Me.Name);
                this.Connection.ChannelOn = false;
                this.Settings.Joined_ = false;

                foreach (User user in this.Users)
                {
                    user.Active = false;
                }
            }
        }

        public void ChangeName (string name)
        {
            if (this.Me == null)
            {
                this.Me = new User(name);
            }
            else
            {
                name = this.CleanString(name);

                if (name != "")
                {
                    this.Connection.Send(Commands.ChangeFormat, this.Me.Name, name);
                    this.Me.Name = name;
                }
            }
        }

        public void SendMessage (string message)
        {
            if (this.Settings.Joined_ && this.Connection.IsJoined)
            {
                Message new_message = new Message(this.Me, this.CleanString(message), true);
                this.Lobby.Messages.Add(new_message);
                this.Connection.Send(Commands.LobbyMessageFormat, this.Me.Name, message);
            }
            else
            {
                Display.MessageBoxShow("unable to send message, not joined to the chat.", "Error", false);
            }
        }

        public void SendMessageTo (Thread current_thread, string message)
        {
            if (this.Settings.Joined_ && this.Connection.IsJoined)
            {
                Message new_message = new Message(this.Me, this.CleanString(message), true);
                current_thread.Messages.Add(new_message);
                foreach (User user in current_thread.Users)
                {
                    this.Connection.SendTo(user.Address, Commands.PrivateMessageFormat, this.Me.Name, current_thread.Guid, message);
                }
            }
            else
            {
                Display.MessageBoxShow("unable to send message, not joined to the chat.", "Error", false);
            }
        }

        public void AddThread(ObservableCollection<User> users)
        {
            if (this.Settings.Joined_ && (users.Count > 0) && this.Connection.IsJoined)
            {
                Thread thread = new Thread(users);
                this.Threads.Add(thread);

                foreach (User user in users)
                {
                    this.Connection.SendTo(user.Address, Commands.PrivateRequestFormat, this.Me.Name, thread.Guid, thread.ToStringCommand());
                }
            }
            else
            {
                Display.MessageBoxShow("unable to create thread.", "Error", false);
            }
        }

        public void DeleteThread (Thread current_thread)
        {
            foreach (User user in current_thread.Users)
            {
                this.Connection.SendTo(user.Address, Commands.PrivateLeaveFormat, this.Me.Name, current_thread.Guid);
            }

            this.Threads.Remove(current_thread);
        }

        public void Reopen()
        {
            this.Connection = new Channel(HostAddress, PortNum);
            this.Connection.Open();

            RegisterEvents();
        }

        #endregion

        #region Channel Event Handlers

        private void RegisterEvents()
        {
            this.Connection.Joined += new EventHandler(Chat_Joined);
            this.Connection.BeforeClose += new EventHandler(Chat_BeforeClose);
            this.Connection.PacketReceived += new EventHandler<Packet>(Chat_PacketReceived);
        }

        private void UnregisterEvents()
        {
            if (this.Connection != null)
            {
                this.Connection.Joined -= new EventHandler(Chat_Joined);
                this.Connection.BeforeClose -= new EventHandler(Chat_BeforeClose);
                this.Connection.PacketReceived -= new EventHandler<Packet>(Chat_PacketReceived);
            }
        }

        void Chat_Joined(object sender, EventArgs e)
        {
        }

        void Chat_BeforeClose(object sender, EventArgs e)
        {
            this.Connection.Send(Commands.LeaveFormat, this.Me.Name);
        }

        void Chat_PacketReceived(object sender, Packet e)
        {
            string message = e.Message.Trim('\0');
            string[] messageParts = message.Split(Commands.CommandDelimeter.ToCharArray());

            if (messageParts.Length >= 2)
            {
                switch (messageParts[0])
                {
                    case Commands.Join:
                        OnJoin(new User(messageParts[1], e.Source));
                        break;
                    case Commands.Leave:
                        OnLeave(messageParts[1]);
                        break;
                    case Commands.LobbyMessage:
                        OnMessage(messageParts[1], messageParts[2]);
                        break;
                    case Commands.Invalid:
                        OnInvalid(messageParts[1]);
                        break;
                    case Commands.WhoAmI:
                        OnWhoAmI(new User(messageParts[1], e.Source));
                        break;
                    case Commands.Ready:
                        OnReady(messageParts[1]);
                        break;
                    case Commands.Change:
                        OnChange(messageParts[1], messageParts[2]);
                        break;
                    case Commands.PrivateMessage:
                        if (this.Settings.PrivateMessages_)
                            OnPrivate(messageParts[1], messageParts[2], messageParts[3]);
                        break;
                    case Commands.PrivateLeave:
                        if (this.Settings.PrivateMessages_)
                            OnPrivateLeave(messageParts[1], messageParts[2]);
                        break;
                    case Commands.PrivateRequest:
                        if (this.Settings.PrivateMessages_)
                            OnPrivateRequest(messageParts[1], messageParts[2], messageParts[3]);
                        break;
                    case Commands.PrivateResponse:
                        if (this.Settings.PrivateMessages_)
                            OnPrivateResponse(messageParts[1], messageParts[2], messageParts[3]);
                        break;
                    case Commands.PrivateFinalize:
                        if (this.Settings.PrivateMessages_)
                            OnPrivateFinalize(messageParts[1], messageParts[2]);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Command Handlers

        private void OnJoin(User user_info)
        {
            bool add = true;

            foreach (User user in this.Users)
            {
                if (user.Name == user_info.Name)
                {
                    if (user.Active)
                    {
                        //same name and user is active invalid join request
                        add = false;
                        this.Connection.SendTo(user_info.Address, Commands.InvalidFormat, Errors.NameTaken);
                    }
                    else
                    {
                        if (user.Address.ToString() == user_info.Address.ToString())
                        {
                            //disconnected user rejoining
                            user.Active = true;
                            add = false;
                            this.Connection.Send(Commands.WhoAmIFormat, this.Me.Name);
                            this.JoinNotice(user_info.Name);
                        }
                        else
                        {
                            //protect the names of disconnected users
                            add = false;
                            this.Connection.SendTo(user_info.Address, Commands.InvalidFormat, Errors.NameTaken);
                        }
                    }
                }
                else
                {
                    if (user.Address.ToString() == user_info.Address.ToString())
                    {
                        //disconnected user rejoining with new name
                        user.Name = user_info.Name;
                        user.Active = true;
                        add = false;
                        this.Connection.Send(Commands.WhoAmIFormat, this.Me.Name);
                        this.JoinNotice(user_info.Name);
                    }
                }
            }

            if (user_info.Name == Me.Name)
            {
                //protect ourselves
                add = false;
                this.Connection.SendTo(user_info.Address, Commands.InvalidFormat, Errors.NameTaken);
            }

            if (add)
            {
                //new user
                this.Users.Add(user_info);
                this.Lobby.AddUser(user_info);
                this.Connection.Send(Commands.WhoAmIFormat, this.Me.Name);
                this.JoinNotice(user_info.Name);
            }
        }

        private void OnWhoAmI(User user_info)
        {
            bool add = true;

            foreach (User user in this.Users)
            {
                if (!user.Active)
                {
                    if (user.Name == user_info.Name)
                    {
                        user.Active = true;
                        add = false;
                    }
                    else
                    {
                        if (user.Address.ToString() == user_info.Address.ToString())
                        {
                            user.Active = true;
                            user.Name = user_info.Name;
                            add = false;
                        }
                    }
                }
                else
                {
                    if (user.Name == user_info.Name)
                    {
                        add = false;
                    }
                    else
                    {
                        if (user.Address.ToString() == user_info.Address.ToString())
                        {
                            user.Name = user_info.Name;
                            add = false;
                        }
                    }
                }
            }

            if (add)
            {
                this.Users.Add(user_info);
                this.Lobby.AddUser(user_info);
            }
        }

        private void OnLeave(string user_name)
        {
            User current_user = this.GetUser(user_name);

            if (current_user != null)
            {
                this.DisconnectNotice(current_user.Name);
                current_user.Active = false;
            }
        }

        private void OnReady(string user)
        {           
        }

        private void OnInvalid(string error)
        {
            switch (error)
            {
                case Errors.NameTaken:
                    this.Disconnect();
                    Display.MessageBoxShow("disconnecting, username is taken.", "Disconnect", false);
                    break;
                default:
                    break;
            }
        }

        private void OnMessage(string user_name, string message = null)
        {
            if (message != null)
            {
                User current_user = this.GetUser(user_name);

                if (current_user != null)
                {
                    Message new_message = new Message(current_user, message, false);
                    this.Lobby.Messages.Add(new_message);
                }
            }
        }

        private void OnChange(string old_name, string new_name = null)
        {
            if (new_name != null)
            {
                User current_user = this.GetUser(old_name);

                if (current_user != null)
                {
                    current_user.Name = new_name;
                }
            }
        }

        private void OnPrivate (string user_name, string guid = null, string message = null)
        {
            if ((guid != null) && (message != null))
            {
                User current_user = this.GetUser(user_name);
                Thread current_thread = this.GetThread(guid);

                if ((current_user != null) && (current_thread != null))
                {
                    Message new_message = new Message(current_user, message, false);
                    current_thread.Messages.Add(new_message);
                    current_thread.UnreadCount++;
                }
            }
        }

        private void OnPrivateLeave (string user_name, string guid = null)
        {
            if (guid != null)
            {
                User current_user = this.GetUser(user_name);
                Thread current_thread = this.GetThread(guid);

                if ((current_user != null) && (current_thread != null))
                {
                    current_thread.Users.Remove(current_user);
                    current_thread.Messages.Add(new Message(new User(""), user_name + " disconnected.", false));
                }
            }
        }

        private void OnPrivateRequest(string user_name, string guid = null, string users = null)
        {
            if ((guid != null) && (users != null))
            {
                ObservableCollection<User> user_list = this.GetUserlist(user_name, users);

                bool result = Display.MessageBoxShow("You are being invited to a thread with " + this.ToString(user_list) + "." + Environment.NewLine + "Ok to accept, Cancel to reject.", "Thread Request", true);

                if (result)
                {
                    this.Threads.Add(new Thread(user_list, guid));

                    foreach (User user in user_list)
                    {
                        this.Connection.SendTo(user.Address, Commands.PrivateResponseFormat, this.Me.Name, guid, Commands.True);
                    }
                }
                else
                {
                    foreach (User user in user_list)
                    {
                        this.Connection.SendTo(user.Address, Commands.PrivateResponseFormat, this.Me.Name, guid, Commands.False);
                    }
                }
            }
        }

        private void OnPrivateResponse(string user_name, string guid = null, string response = null)
        {
            if ((guid != null) && (response != null))
            {
                User current_user = this.GetUser(user_name);
                Thread current_thread = this.GetThread(guid);

                if ((current_user != null) && (current_thread != null))
                {
                    if (response == Commands.True)
                    {
                        current_thread.Messages.Add(new Message(new User(""), user_name + " joined.", false));
                    }
                    else
                    {
                        current_thread.Users.Remove(current_user);
                        Display.MessageBoxShow(current_user.Name + " ignored your thread request.", "Thread Request", false);
                    }

                    current_thread.Finalize--;

                    if (current_thread.Finalize == 0)
                    {
                        foreach (User user in current_thread.Users)
                        {
                            this.Connection.SendTo(user.Address, Commands.PrivateFinalizeFormat, current_thread.Guid, current_thread.ToStringCommand() + ";" + this.Me.Name);
                        }
                    }

                    if (current_thread.Users.Count == 0)
                    {
                        this.Threads.Remove(current_thread);
                    }
                }
            }
        }

        private void OnPrivateFinalize(string guid, string users = null)
        {
            if (users != null)
            {
                Thread current_thread = this.GetThread(guid);

                if (current_thread != null)
                {
                    ObservableCollection<User> user_list = this.GetUserlist("", users);
                    current_thread.Users = user_list;
                }
            }
        }

        #endregion

        #region Keep Alive

        DispatcherTimer _dt;

        private void StartKeepAlive()
        {
            if (_dt == null)
            {
                _dt = new DispatcherTimer();
                _dt.Interval = new TimeSpan(0, 0, 5);
                _dt.Tick +=
                            delegate(object s, EventArgs args)
                            {
                                if (this.Connection != null && this.Settings.Joined_)
                                {
                                    this.Connection.Send(Commands.ReadyFormat, Me.Name);
                                }
                            };
            }
            _dt.Start();

        }

        private void StopkeepAlive()
        {
            if (_dt != null)
                _dt.Stop();
        }

        #endregion

        #region Helpers

        private void JoinNotice (string name)
        {
            if (this.Settings.LobbyMessages_)
            {
                this.Lobby.Messages.Add(new Message(new User(""), name + " joined.", false));
            }
        }

        private void DisconnectNotice (string name)
        {
            if (this.Settings.LobbyMessages_)
            {
                this.Lobby.Messages.Add(new Message(new User(""), name + " disconnected.", false));
            }
        }

        public User GetUser(string name)
        {
            return this.Users.Where(user => user.Name == name).SingleOrDefault();
        }

        private Thread GetThread(string guid)
        {
            return this.Threads.Where(thread => thread.Guid == guid).SingleOrDefault();
        }

        private ObservableCollection<User> GetUserlist(string starter, string users)
        {
            ObservableCollection<User> result = new ObservableCollection<User>();

            users += starter;

            string[] split_users = users.Split(";".ToCharArray());

            foreach (string name in split_users)
            {
                if (this.Me.Name != name)
                {
                    User temp = this.GetUser(name);

                    if (temp != null)
                    {
                        result.Add(temp);
                    }
                }
            }

            return result;
        }

        private string ToString(ObservableCollection<User> users)
        {
            string result = "";

            foreach (User user in users)
            {
                result += user.Name;
                result += "; ";
            }

            return result;
        }

        private string CleanString(string input)
        {
            return Regex.Replace(input, Commands.CommandDelimeter, "");
        }

        #endregion
    }
}
