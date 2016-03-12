using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace WiYak
{
    public class Message
    {
        [XmlIgnore]
        public User User { get; set; }
        public string Text { get; set; }
        public string Time { get; set; }
        public bool Me_ { get; set; }

        public Message()
        {
        }

        public Message (User user, string text, bool me)
        {
            this.User = user;
            this.Text = text;
            this.Me_ = me;
            DateTime now = DateTime.Now;
            this.Time = now.ToString(DateTimeFormatInfo.CurrentInfo.LongTimePattern);
        }

        #region Xml

        [XmlElement("User")]
        public string UserXml
        {
            get
            {
                return this.User.Name;
            }
            set
            {
                if (value == App.State.Me.Name)
                {
                    this.User = App.State.Me;
                }
                else if (value == "")
                {
                    this.User = new User("");
                }
                else
                {
                    this.User = App.State.GetUser(value);
                }
            }
        }

        #endregion
    }
}
