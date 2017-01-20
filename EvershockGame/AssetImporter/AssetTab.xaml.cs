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
        }

        //---------------------------------------------------------------------------

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AssetManager.Get().Selection = Container.SelectedItem as Asset;
        }

        //---------------------------------------------------------------------------

        private void OnFilter(object sender, FilterEventArgs e)
        {
            var item = e.Item as Asset;
            e.Accepted = (item.AssetType.HasFlag(AssetType));
        }
    }
}
