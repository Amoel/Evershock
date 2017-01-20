using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    public partial class AssetCreationWindow : Window
    {
        public AssetCreationWindow()
        {
            InitializeComponent();
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
            AssetManager.Get().Add(AssetName.Text, RelativePath.Text, EAssetType.Sprite);
            Close();
        }
    }
}
