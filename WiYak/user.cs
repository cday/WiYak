using System;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;


namespace WiYak
{
    public class User
    {
        [XmlIgnore]
        public Brush Color_ { get; set; }
        [XmlIgnore]
        public IPEndPoint Address { get; set; }

        public string Name { get; set; }
        public bool Active { get; set; }

        public User()
        {
        }

        public User (string name)
        {
            this.Name = name;

            Color_ = new SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
            Address = null;
            Active = true;
        }

        public User (string name, IPEndPoint addr)
        {
            Random randonGen = new Random();

            this.Name = name;
            this.Address = addr;

            Color_ = new SolidColorBrush(Color.FromArgb((byte)255, (byte)(randonGen.Next(115)+60), (byte)(randonGen.Next(115)+60), (byte)(randonGen.Next(115)+60)));
            Active = true; 
        }

        #region Xml

        [XmlElement("Color")]
        public string Color_Xml
        {
            get
            {
                SolidColorBrush temp = this.Color_ as SolidColorBrush;
                return temp.Color.A.ToString() + ";" + temp.Color.R.ToString() + ";" + temp.Color.G.ToString() + ";" + temp.Color.B.ToString();
            }
            set
            {
                Random randonGen = new Random();

                if (value == null)
                {
                    this.Color_ = new SolidColorBrush(Color.FromArgb((byte)255, (byte)(randonGen.Next(115)+60), (byte)(randonGen.Next(115)+60), (byte)(randonGen.Next(115)+60)));
                }

                string[] colors = value.Split(";".ToCharArray());

                if (colors.Length < 4)
                {
                    this.Color_ = new SolidColorBrush(Color.FromArgb((byte)255, (byte)(randonGen.Next(115) + 60), (byte)(randonGen.Next(115) + 60), (byte)(randonGen.Next(115) + 60)));
                }
                else
                {
                    this.Color_ = new SolidColorBrush(Color.FromArgb((byte)int.Parse(colors[0]), (byte)int.Parse(colors[1]), (byte)int.Parse(colors[2]), (byte)int.Parse(colors[3])));
                }
            }
        }

        [XmlElement("Address")]
        public string AddressXml
        {
            get
            {
                string result = "";

                if (this.Address != null)
                {
                    result = this.Address.Address.ToString() + ":" + this.Address.Port.ToString();
                }

                return result;
            }
            set
            {
                if (value == null)
                {
                    this.Address = null;
                }

                string[] temp = value.Split(":".ToCharArray());

                if (temp.Length >= 2)
                {
                    this.Address = new IPEndPoint(IPAddress.Parse(temp[0]), int.Parse(temp[1]));
                }
                else
                {
                    this.Address = null;
                }
            }
        }

        #endregion
    }
}
