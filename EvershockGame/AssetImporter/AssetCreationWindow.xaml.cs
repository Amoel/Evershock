using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AssetImporter
{
    /// <summary>
    /// Interaction logic for AssetCreationWindow.xaml
    /// </summary>
    public partial class AssetCreationWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_Preview;
        public string Preview
        {
            get { return m_Preview; }
            set
            {
                m_Preview = value;
                OnPropertyChanged("Preview");
            }
        }

        private bool m_IsValidName;
        public bool IsValidName
        {
            get { return m_IsValidName; }
            set
            {
                m_IsValidName = value;
                OnPropertyChanged("IsValidName");
            }
        }

        //---------------------------------------------------------------------------

        public AssetCreationWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        //---------------------------------------------------------------------------

        private void OnSearchClicked(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = AssetManager.Get().RootPath;
            dialog.ShowDialog(this);

            if (File.Exists(dialog.FileName))
            {
                RelativePath.Text = dialog.FileName.Substring(AssetManager.Get().RootPath.Length + 1).Replace('\\', '/');
                Preview = dialog.FileName;
            }
        }

        //---------------------------------------------------------------------------

        private void OnCancelClicked(object sender, EventArgs e)
        {
            Close();
        }

        //---------------------------------------------------------------------------

        private void OnAddClicked(object sender, EventArgs e)
        {
            AssetManager.Get().Add(AssetName.Text, RelativePath.Text, (EAssetType)AssetTypeBox.SelectedItem);
            Close();
        }

        //---------------------------------------------------------------------------

        private void OnNameChanged(object sender, EventArgs e)
        {
            IsValidName = (AssetName.Text != string.Empty && AssetManager.Get().IsNameAvailable(AssetName.Text));
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
