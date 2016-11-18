﻿using Level;
using Managers;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponent.Manager
{
    public class AssetManager : BaseManager<AssetManager>
    {
        private Dictionary<Type, Dictionary<string, dynamic>> m_Assets;

        public ContentManager Content { get; set; }

        //---------------------------------------------------------------------------

        protected AssetManager()
        {
            m_Assets = new Dictionary<Type, Dictionary<string, dynamic>>();
        }

        //---------------------------------------------------------------------------

        public void LoadMaps()
        {
            //LoadAll<Map>("Levels");
            //string[] files = Directory.GetFiles("C:/Users/Max/Desktop/Maps");
            //foreach (string file in files)
            //{
            //    Map map = Map.Load(file);
            //    if (map != null)
            //    {
            //        Store(Path.GetFileNameWithoutExtension(file), map);
            //    }
            //}
            string directory = string.Format("{0}/Levels", Content.RootDirectory);
            foreach (string file in Directory.GetFiles(directory))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                Map map = Map.Load(file);
                if (map != null)
                {
                    Store(Path.GetFileNameWithoutExtension(file), map);
                }
            }
        }

        //---------------------------------------------------------------------------

        public void Store<T>(string name, string path)
        {
            if (Content == null)
            {
                throw new Exception("Content in AssetManager is null.");
            }
            Store(name, Content.Load<T>(path));
        }

        //---------------------------------------------------------------------------

        public void Store<T>(string name, T asset)
        {
            if (!m_Assets.ContainsKey(typeof(T)))
            {
                m_Assets.Add(typeof(T), new Dictionary<string, dynamic>());
            }
            if (!m_Assets[typeof(T)].ContainsKey(name))
            {
                m_Assets[typeof(T)].Add(name, asset);
            }
            else
            {
                throw new Exception("Duplicate name usage in AssetManager.");
            }
        }

        //---------------------------------------------------------------------------

        public T Find<T>(string name)
        {
            if (name != null && m_Assets.ContainsKey(typeof(T)))
            {
                if (m_Assets[typeof(T)].ContainsKey(name))
                {
                    return (T)m_Assets[typeof(T)][name];
                }
            }
            return default(T);
        }

        //---------------------------------------------------------------------------

        public void LoadAll<T>(string relativeDirectory)
        {
            string directory = string.Format("{0}/{1}", Content.RootDirectory, relativeDirectory);
            if (!Directory.Exists(directory))
            {
                AssertManager.Get().Show("");
                return;
            }
            foreach (string file in Directory.GetFiles(directory))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                T data = Content.Load<T>(string.Format("{0}/{1}", relativeDirectory, name));
                Store(name, data);
            }
        }
    }
}
