using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AssetImporter
{
    /// <summary>
    /// Interaction logic for AssetTab.xaml
    /// </summary>
    public partial class AssetTab : UserControl
    {
        public static readonly DependencyProperty AssetTypeProperty = DependencyProperty.Register
            (
                "AssetType", typeof(EAssetType), typeof(AssetTab), new FrameworkPropertyMetadata()
            );
        
        public EAssetType AssetType
        {
            get { return (EAssetType)GetValue(AssetTypeProperty); }
            set { SetValue(AssetTypeProperty, value); }
        }

        //public ICollectionView FilteredAssets
        //{
        //    get
        //    {
        //        var filteredAssets = CollectionViewSource.GetDefaultView(AssetManager.Get().Assets);
        //        filteredAssets.Filter = a => (a as Asset).AssetType.HasFlag(AssetType);
        //        return filteredAssets;
        //    }
        //}

        //---------------------------------------------------------------------------

        public AssetTab()
        {
            InitializeComponent();
            DataContext = AssetManager.Get();

            AssetManager.Get().RegisterTab(this);
        }

        //---------------------------------------------------------------------------

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AssetManager.Get().Selection = Container.SelectedItem as Asset;
        }

        //---------------------------------------------------------------------------

        public void RefreshView()
        {
            ((CollectionViewSource)BaseGrid.Resources["FilteredAssets"]).View.Refresh();
        }

        //---------------------------------------------------------------------------

        private void OnFilter(object sender, FilterEventArgs e)
        {
            var item = e.Item as Asset;
            e.Accepted = (item.AssetType.HasFlag(AssetType));
            if (e.Accepted && AssetManager.Get().FilterText != null && AssetManager.Get().FilterText != string.Empty)
            {
                string filter = AssetManager.Get().FilterText.ToLower();
                if (!item.Name.ToLower().Contains(filter) && !item.Path.ToLower().Contains(filter)) e.Accepted = false;
            }
        }
    }
}
