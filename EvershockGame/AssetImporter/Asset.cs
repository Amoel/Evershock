using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AssetImporter
{
    [Flags]
    public enum EAssetType
    {
        All = 0,
        Sprite = 1,
        Tileset = 2,
        Light = 4,
        Effect = 8
    }

    //---------------------------------------------------------------------------

    public class Asset : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonIgnore]
        private bool m_HasUnsavedChanges;
        [JsonIgnore]
        public bool HasUnsavedChanges
        {
            get { return m_HasUnsavedChanges; }
            set
            {
                m_HasUnsavedChanges = value;
                OnPropertyChanged("HasUnsavedChanges");
            }
        }

        private Uri m_Thumbnail;
        public Uri Thumbnail
        {
            get { return m_Thumbnail; }
            set
            {
                m_Thumbnail = value;
                OnPropertyChanged("Thumbnail");
            }
        }

        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set
            {
                m_Name = value;
                OnPropertyChanged("Name");
                HasUnsavedChanges = true;
            }
        }

        private string m_Path;
        public string Path
        {
            get { return m_Path; }
            set
            {
                m_Path = value;
                OnPropertyChanged("Path");
                UpdateThumbnail();
                HasUnsavedChanges = true;
            }
        }

        private bool m_IncludeInDebug;
        public bool IncludeInDebug
        {
            get { return m_IncludeInDebug; }
            set
            {
                m_IncludeInDebug = value;
                OnPropertyChanged("IncludeInDebug");
                HasUnsavedChanges = true;
            }
        }

        private bool m_IncludeInRelease;
        public bool IncludeInRelease
        {
            get { return m_IncludeInRelease; }
            set
            {
                m_IncludeInRelease = value;
                OnPropertyChanged("IncludeInRelease");
                HasUnsavedChanges = true;
            }
        }

        private EAssetType m_AssetType;
        public EAssetType AssetType
        {
            get { return m_AssetType; }
            set
            {
                m_AssetType = value;
                OnPropertyChanged("AssetType");
                HasUnsavedChanges = true;
            }
        }

        //---------------------------------------------------------------------------

        public Asset(string name, string path, EAssetType assetType)
        {
            Save();
            Name = name;
            Path = path;
            IncludeInDebug = false;
            IncludeInRelease = false;
            UpdateThumbnail();
            AssetType = EAssetType.All | assetType;
        }

        //---------------------------------------------------------------------------

        public void Save()
        {
            HasUnsavedChanges = false;
        }

        //---------------------------------------------------------------------------

        private void UpdateThumbnail()
        {
            Uri temp = null;
            if (Uri.TryCreate(System.IO.Path.Combine(AssetManager.Get().RootPath, Path), UriKind.Absolute, out temp))
            {
                Thumbnail = temp;
            }
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
