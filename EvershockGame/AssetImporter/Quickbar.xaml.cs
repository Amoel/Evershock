using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AssetImporter
{
    /// <summary>
    /// Interaction logic for Quickbar.xaml
    /// </summary>
    public partial class Quickbar : UserControl
    {
        public Quickbar()
        {
            InitializeComponent();
        }

        //---------------------------------------------------------------------------

        private void OnSaveClicked(object sender, EventArgs e)
        {
            AssetManager.Get().StoreData();
        }

        //---------------------------------------------------------------------------

        private void OnAddClicked(object sender, EventArgs e)
        {
            AssetCreationWindow window = new AssetCreationWindow();
            window.Owner = AssetManager.Get().Main;
            window.ShowDialog();
        }

        //---------------------------------------------------------------------------

        private void OnGenerateClicked(object sender, EventArgs e)
        {
            AssetManager.Get().Generate();
        }

        //---------------------------------------------------------------------------

        private void OnSettingsClicked(object sender, EventArgs e)
        {

        }

        //---------------------------------------------------------------------------

        private void OnFilterTextChanged(object sender, EventArgs e)
        {
            AssetManager.Get().FilterText = Filter.Text;
        }
    }
}
