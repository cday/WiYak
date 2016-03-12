using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace WiYak
{
    public class Storage
    {
        public static void SaveState()
        {
            TextWriter writer = null;
            IsolatedStorageFileStream file = null;
            XmlSerializer serialize = null;

            try
            {
                IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();

                //save users
                file = isoStorage.OpenFile("users.xml", FileMode.Create);
                writer = new StreamWriter(file);
                serialize = new XmlSerializer(typeof(ObservableCollection<User>));
                serialize.Serialize(writer, App.State.Users);
                writer.Close();

                //save lobby
                file = isoStorage.OpenFile("lobby.xml", FileMode.Create);
                writer = new StreamWriter(file);
                serialize = new XmlSerializer(typeof(Thread));
                serialize.Serialize(writer, App.State.Lobby);
                writer.Close();

                //save me
                file = isoStorage.OpenFile("whoami.xml", FileMode.Create);
                writer = new StreamWriter(file);
                serialize = new XmlSerializer(typeof(User));
                serialize.Serialize(writer, App.State.Me);
                writer.Close();

                //save settings
                file = isoStorage.OpenFile("settings.xml", FileMode.Create);
                writer = new StreamWriter(file);
                serialize = new XmlSerializer(typeof(Controls));
                serialize.Serialize(writer, App.State.Settings);
                writer.Close();

                //save threads
                file = isoStorage.OpenFile("threads.xml", FileMode.Create);
                writer = new StreamWriter(file);
                serialize = new XmlSerializer(typeof(ObservableCollection<Thread>));
                serialize.Serialize(writer, App.State.Threads);
                writer.Close();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
            }
        }

        public static void LoadState()
        {
            TextReader reader = null;
            IsolatedStorageFileStream file = null;
            XmlSerializer deserialize = null;

            try
            {
                IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();

                //load users
                file = isoStorage.OpenFile("users.xml", FileMode.OpenOrCreate);
                reader = new StreamReader(file);
                deserialize = new XmlSerializer(typeof(ObservableCollection<User>));
                App.State.Users = deserialize.Deserialize(reader) as ObservableCollection<User>;
                reader.Close();

                //load me
                file = isoStorage.OpenFile("whoami.xml", FileMode.OpenOrCreate);
                reader = new StreamReader(file);
                deserialize = new XmlSerializer(typeof(User));
                App.State.Me = deserialize.Deserialize(reader) as User;
                reader.Close();

                //load settings
                file = isoStorage.OpenFile("settings.xml", FileMode.OpenOrCreate);
                reader = new StreamReader(file);
                deserialize = new XmlSerializer(typeof(Controls));
                App.State.Settings = deserialize.Deserialize(reader) as Controls;
                reader.Close();

                //load lobby
                file = isoStorage.OpenFile("lobby.xml", FileMode.OpenOrCreate);
                reader = new StreamReader(file);
                deserialize = new XmlSerializer(typeof(Thread));
                App.State.Lobby = deserialize.Deserialize(reader) as Thread;
                reader.Close();

                //load threads
                file = isoStorage.OpenFile("threads.xml", FileMode.OpenOrCreate);
                reader = new StreamReader(file);
                deserialize = new XmlSerializer(typeof(ObservableCollection<Thread>));
                App.State.Threads = deserialize.Deserialize(reader) as ObservableCollection<Thread>;
                reader.Close();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
            }
        }

        public static void SaveSettings()
        {
            TextWriter writer = null;
            IsolatedStorageFileStream file = null;
            XmlSerializer serialize = null;

            try
            {
                IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();

                //save me
                file = isoStorage.OpenFile("whoami.xml", FileMode.Create);
                writer = new StreamWriter(file);
                serialize = new XmlSerializer(typeof(User));
                serialize.Serialize(writer, App.State.Me);
                writer.Close();

                //save settings
                file = isoStorage.OpenFile("settings.xml", FileMode.Create);
                writer = new StreamWriter(file);
                serialize = new XmlSerializer(typeof(Controls));
                serialize.Serialize(writer, App.State.Settings);
                writer.Close();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();
            }
        }

        public static void LoadSettings()
        {
            TextReader reader = null;
            IsolatedStorageFileStream file = null;
            XmlSerializer deserialize = null;

            try
            {
                IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();

                //load me
                file = isoStorage.OpenFile("whoami.xml", FileMode.OpenOrCreate);
                reader = new StreamReader(file);
                deserialize = new XmlSerializer(typeof(User));
                App.State.Me = deserialize.Deserialize(reader) as User;
                reader.Close();

                //load settings
                file = isoStorage.OpenFile("settings.xml", FileMode.OpenOrCreate);
                reader = new StreamReader(file);
                deserialize = new XmlSerializer(typeof(Controls));
                App.State.Settings = deserialize.Deserialize(reader) as Controls;
                reader.Close();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
            }
        }
    }
}
