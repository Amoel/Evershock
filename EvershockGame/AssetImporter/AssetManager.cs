﻿using Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows;

namespace AssetImporter
{
    public class AssetManager : BaseManager<AssetManager>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public readonly Dictionary<EAssetType, string> TypeMapping = new Dictionary<EAssetType, string>()
        {
            { EAssetType.Sprite, "Texture2D" },
            { EAssetType.Tileset, "Texture2D" },
            { EAssetType.Light, "Texture2D" },
            { EAssetType.Effect, "Effect" },
            { EAssetType.Font, "SpriteFont" }
        };

        public string RootPath { get; private set; }
        public string AssetsPath { get; private set; }
        public string ProjectPath { get; private set; }
        public ObservableCollection<Asset> Assets { get; set; }

        private string m_FilterText;
        public string FilterText
        {
            get { return m_FilterText; }
            set
            {
                m_FilterText = value;
                OnPropertyChanged("FilterText");
                UpdateTabs();
            }
        }

        private Asset m_Selection;
        public Asset Selection
        {
            get { return m_Selection; }
            set
            {
                m_Selection = value;
                OnPropertyChanged("Selection");
            }
        }

        public Window Main { get; private set; }

        private List<AssetTab> m_Tabs;

        //---------------------------------------------------------------------------

        protected AssetManager()
        {
            RootPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\", "EvershockGame/Content"));
            AssetsPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\", "Assets.json"));
            ProjectPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\", "EvershockGame/Code/Managers/AssetManager.cs"));

            m_Tabs = new List<AssetTab>();
        }

        //---------------------------------------------------------------------------

        public void RegisterTab(AssetTab tab)
        {
            if (!m_Tabs.Contains(tab))
            {
                m_Tabs.Add(tab);
            }
        }

        //---------------------------------------------------------------------------

        public void UpdateTabs()
        {
            foreach (AssetTab tab in m_Tabs)
            {
                tab.RefreshView();
            }
        }

        //---------------------------------------------------------------------------

        public bool IsNameAvailable(string name)
        {
            return !Assets.Any(asset => asset.Name == name);
        }

        //---------------------------------------------------------------------------

        public void Add(string name, string path, EAssetType assetType)
        {
            string absolutePath = Path.Combine(RootPath, path);
            if (File.Exists(absolutePath))
            {
                Assets.Add(new Asset(name, path, assetType));
            }
        }

        //---------------------------------------------------------------------------

        public void Generate()
        {
            if (File.Exists(ProjectPath))
            {
                using (FileStream stream = new FileStream(ProjectPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write("/* This file is autogenerated. Do not edit by hand! */\n\nusing System;\nusing System.Collections.Generic;\nusing Microsoft.Xna.Framework.Content;\nusing Microsoft.Xna.Framework.Graphics;\nusing Managers;\n\nnamespace EvershockGame.Code\n{\n");
                        foreach (EAssetType type in Enum.GetValues(typeof(EAssetType)))
                        {
                            if (type == EAssetType.All) continue;
                            GenerateEnum(writer, type);
                        }
                        GenerateClass(writer);
                        writer.Write("}");
                    }
                }
                MessageBox.Show(Main, "Successfully generated file.", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //---------------------------------------------------------------------------

        private void GenerateEnum(StreamWriter writer, EAssetType type)
        {
            writer.Write(string.Format("    public enum E{0}Assets\n    {{\n", type.ToString()));
            foreach (Asset asset in Assets)
            {
                if (asset.AssetType.HasFlag(type))
                {
                    writer.Write(string.Format("        {0},\n", asset.Name));
                }
            }
            writer.Write("    }\n");
            writer.Write("\n    //---------------------------------------------------------------------------\n\n");
        }

        //---------------------------------------------------------------------------

        private void GenerateClass(StreamWriter writer)
        {
            writer.Write("    public class AssetManager : BaseManager<AssetManager>\n    {\n        public ContentManager Content { get; set; }\n\n");
            writer.Write("        //---------------------------------------------------------------------------\n\n");

            foreach (EAssetType type in Enum.GetValues(typeof(EAssetType)))
            {
                if (type == EAssetType.All) continue;
                writer.Write(string.Format("        private Dictionary<E{0}Assets, string> m_{0}Mapping = new Dictionary<E{0}Assets, string>()\n        {{\n", type.ToString()));
                foreach (Asset asset in Assets)
                {
                    if (asset.AssetType.HasFlag(type))
                    {
                        writer.Write(string.Format("            {{ E{0}Assets.{1}, \"{2}\" }},\n", type.ToString(), asset.Name, Path.ChangeExtension(asset.Path, null)));
                    }
                }
                writer.Write("        };\n\n");
                writer.Write(string.Format("        private Dictionary<Type, Dictionary<E{0}Assets, dynamic>> m_{0}Assets;\n\n", type.ToString()));
                writer.Write("        //---------------------------------------------------------------------------\n\n");
            }
            
            GenerateConstructor(writer);
            GenerateFindMethods(writer);
            GenerateLoadAllMethod(writer);
            GenerateLoadMethods(writer);
            //writer.Write("        public string Find<T>(T asset) where T : struct, IConvertible\n        {\n            switch (typeof(T))\n            {\n            }\n        }\n");
            writer.Write("    }\n");
        }

        //---------------------------------------------------------------------------

        private void GenerateConstructor(StreamWriter writer)
        {
            writer.Write("        protected AssetManager()\n        {\n");
            foreach (EAssetType type in Enum.GetValues(typeof(EAssetType)))
            {
                if (type == EAssetType.All) continue;
                writer.Write(string.Format("            m_{0}Assets = new Dictionary<Type, Dictionary<E{0}Assets, dynamic>>();\n", type.ToString()));
            }
            writer.Write("        }\n\n");
        }

        //---------------------------------------------------------------------------

        private void GenerateFindMethods(StreamWriter writer)
        {
            foreach (EAssetType type in Enum.GetValues(typeof(EAssetType)))
            {
                if (type == EAssetType.All) continue;
                writer.Write("        //---------------------------------------------------------------------------\n\n");
                writer.Write(string.Format("        public T Find<T>(E{0}Assets asset)\n        {{\n", type.ToString()));
                writer.Write(string.Format("            if (m_{0}Assets.ContainsKey(typeof(T)))\n            {{\n", type.ToString()));
                writer.Write(string.Format("                if (m_{0}Assets[typeof(T)].ContainsKey(asset))\n                    return (T)m_{0}Assets[typeof(T)][asset];\n", type.ToString()));
                writer.Write("            }\n            return default(T);\n");
                writer.Write("        }\n\n");
            }
        }

        //---------------------------------------------------------------------------

        private void GenerateLoadAllMethod(StreamWriter writer)
        {
            writer.Write("        //---------------------------------------------------------------------------\n\n");
            writer.Write("        public void LoadAll()\n        {\n");
            foreach (EAssetType type in Enum.GetValues(typeof(EAssetType)))
            {
                if (type == EAssetType.All) continue;
                writer.Write(string.Format("            foreach (KeyValuePair<E{0}Assets, string> kvp in m_{0}Mapping)\n            {{\n", type.ToString()));
                writer.Write(string.Format("                Store<{0}>(kvp.Key, kvp.Value);\n            }}\n", TypeMapping[type]));
            }
            writer.Write("        }\n");
        }

        //---------------------------------------------------------------------------

        private void GenerateLoadMethods(StreamWriter writer)
        {
            foreach (EAssetType type in Enum.GetValues(typeof(EAssetType)))
            {
                if (type == EAssetType.All) continue;
                writer.Write("\n        //---------------------------------------------------------------------------\n\n");
                writer.Write(string.Format("        public void Store<T>(E{0}Assets type, string path)\n        {{\n", type.ToString()));
                writer.Write("            T asset = Content.Load<T>(path);\n");
                writer.Write(string.Format("            if (!m_{0}Assets.ContainsKey(typeof(T)))\n            {{\n", type.ToString()));
                writer.Write(string.Format("                m_{0}Assets.Add(typeof(T), new Dictionary<E{0}Assets, dynamic>());\n            }}\n", type.ToString()));
                writer.Write(string.Format("            if (!m_{0}Assets[typeof(T)].ContainsKey(type))\n            {{\n", type.ToString()));
                writer.Write(string.Format("                m_{0}Assets[typeof(T)].Add(type, asset);\n            }}\n        }}\n", type.ToString()));
            }
        }

        //---------------------------------------------------------------------------

        public void StoreData()
        {
            using (FileStream stream = new FileStream(AssetsPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(JsonConvert.SerializeObject(Assets, Formatting.Indented));
                }
            }
            foreach (Asset asset in Assets)
            {
                asset.Save();
            }
        }

        //---------------------------------------------------------------------------

        public void LoadData()
        {
            if (File.Exists(AssetsPath))
            {
                using (FileStream stream = new FileStream(AssetsPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        Assets = JsonConvert.DeserializeObject<ObservableCollection<Asset>>(reader.ReadToEnd());
                        foreach (Asset asset in Assets)
                        {
                            asset.Save();
                        }
                    }
                }
            }
            if (Assets == null) Assets = new ObservableCollection<Asset>();
        }

        //---------------------------------------------------------------------------

        public void RegisterMainWindow(Window main)
        {
            Main = main;
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
