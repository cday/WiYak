using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace WiYak
{
    public class Thread
    {
        [XmlIgnore]
        public ObservableCollection<User> Users { get; set; }

        public ObservableCollection<Message> Messages { get; set; }
        public int UnreadCount { get; set; }
        public string Guid { get; set; }
        public int Finalize { get; set; }

        public Thread()
        {
        }

        public Thread (ObservableCollection<User> users)
        {
            this.Users = users;
            this.Messages = new ObservableCollection<Message>();
            this.UnreadCount = 0;
            this.Guid = System.Guid.NewGuid().ToString();
            this.Finalize = this.Users.Count;
        }

        public Thread(ObservableCollection<User> users, string guid)
        {
            this.Users = users;
            this.Messages = new ObservableCollection<Message>();
            this.UnreadCount = 0;
            this.Guid = guid;
            this.Finalize = this.Users.Count;
        }

        public override string ToString()
        {
            string temp = "";

            foreach (User user in this.Users)
            {
                temp += user.Name;
                temp += "; ";
            }

            return temp;
        }

        public string ToStringCommand()
        {
            string temp = "";

            foreach (User user in this.Users)
            {
                temp += user.Name;
                temp += ";";
            }

            return temp;
        }

        public string ToPlainText()
        {
            StringWriter writer = new StringWriter();

            foreach (Message message in this.Messages)
            {
                writer.WriteLine(message.User.Name + " [" + message.Time + "]: " + message.Text);
            }

            return writer.ToString();
        }

        public void AddUser(User new_user)
        {
            this.Users.Add(new_user);
        }

        #region Xml

        [XmlElement("Users")]
        public string UsersXml
        {
            get
            {
                return this.ToStringCommand();
            }
            set
            {
                this.Users = new ObservableCollection<User>();

                if (value != null)
                {
                    string[] temp = value.Split(";".ToCharArray());

                    foreach (string user in temp)
                    {
                        User temp_user = App.State.GetUser(user);

                        if (temp_user != null)
                        {
                            this.Users.Add(temp_user);
                        }
                    }
                }
            }
        }

        #endregion
    }
}