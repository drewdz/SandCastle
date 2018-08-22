using Android.Content;

using MvvmCross.Platform.Droid;
using PE.Framework.Serialization;

using System.Collections.Generic;
using System.IO;

namespace PE.Plugins.LocalStorage.Droid
{
    public class LocalStorageService : ILocalStorageService
    {
        #region Constants

        private const string SETTINGS = "Settings";
        private LocalStorageConfiguration _Configuration;

        #endregion Constants

        #region Fields

        private Dictionary<string, string> _Settings;
        private IMvxAndroidGlobals _Globals;
        private Java.IO.File _Root;

        #endregion Fields

        #region Constructors

        public LocalStorageService(LocalStorageConfiguration configuration)
        {
            //Initialize();
            _Configuration = configuration;
        }

        #endregion Constructors

        #region Put

        public void Put<TEntity>(TEntity entity)
        {
            Put(typeof(TEntity).Name, entity);
        }

        public void Put<TEntity>(string name, TEntity entity)
        {
            if (_Settings == null)
            {
                Initialize();
            }

            name = GetPath(name);
            var data = System.Text.Encoding.UTF8.GetBytes(Serializer.Serialize(entity));

            if (File.Exists(name))
            {
                File.Delete(name);
            }

            using (var stream = File.Open(name, FileMode.OpenOrCreate, FileAccess.Write))
            {
                stream.Write(data, 0, data.Length);
            }
        }

        #endregion Put

        #region Get

        public TEntity Get<TEntity>()
        {
            return Get<TEntity>(typeof(TEntity).Name);
        }

        public TEntity Get<TEntity>(string name)
        {
            try
            {
                lock (this)
                {
                    //  open the file
                    name = GetPath(name);
                    if (!File.Exists(name)) return default(TEntity);

                    using (Stream stream = File.Open(name, FileMode.Open, FileAccess.Read))
                    {
                        using (var ms = new MemoryStream())
                        {
                            while (true)
                            {
                                byte[] d = new byte[1024];
                                int result = stream.Read(d, 0, 1024);
                                ms.Write(d, 0, result);
                                if (result < 1024) break;
                            }
                            //  deserialize
                            ms.Seek(0, SeekOrigin.Begin);
                            byte[] data = ms.ToArray();
                            return Serializer.Deserialize<TEntity>(System.Text.Encoding.UTF8.GetString(data, 0, data.Length));
                        }
                    }
                }
            }
            catch (FileNotFoundException fex)
            {
                return default(TEntity);
            }
        }

        #endregion Get

        #region Delete

        public void Delete(string name)
        {
            if (_Settings == null)
            {
                Initialize();
            }

            try
            {
                name = GetPath(name);
                if (File.Exists(name)) File.Delete(name);
            } 
            catch { /* do nothing */ }
        }

        #endregion Delete

        #region Exists

        public bool Exists(string key)
        {
            if (_Settings == null)
            {
                Initialize();
            }

            try
            {
                //  open the file
                key = GetPath(key);
                return (!File.Exists(key)) ? false : true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        #endregion Exists

        #region Other Operations

        public void Write(string key, string value)
        {
            if (_Settings == null)
            {
                Initialize();
            }

            if (_Settings.ContainsKey(key))
                _Settings[key] = value;
            else
                _Settings.Add(key, value);
        }

        public string Read(string key)
        {
            if (_Settings == null)
            {
                Initialize();
            }

            //  check if the setting exists
            return (_Settings.ContainsKey(key)) ? _Settings[key] : string.Empty;
        }

        #endregion Other Operations

        #region Private Methods

        private string GetPath(string file)
        {
            if (_Root == null)
            {
                _Root = _Configuration.ApplicationContext.GetDir("data", FileCreationMode.Private);
            }
            return string.Format("{0}/{1}", _Root, file);
        }

        private void Initialize()
        {
            _Settings = Get<Dictionary<string, string>>(SETTINGS);
            if (_Settings == null) _Settings = new Dictionary<string, string>();
        }
        #endregion Private Methods
    }
}