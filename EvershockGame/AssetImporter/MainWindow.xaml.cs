using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Collections.ObjectModel;

namespace AssetImporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool m_IsDarkPreviewActive;
        public bool IsDarkPreviewActive
        {
            get { return m_IsDarkPreviewActive; }
            set
            {
                m_IsDarkPreviewActive = value;
                OnPropertyChanged("IsDarkPreviewActive");
            }
        }

        //---------------------------------------------------------------------------

        public MainWindow()
        {
            InitializeComponent();
            DataContext = AssetManager.Get();

            Closing += OnClosing;

            AssetManager.Get().RegisterMainWindow(this);
            AssetManager.Get().LoadData();
        }

        //---------------------------------------------------------------------------

        private void OnClosing(object sender, EventArgs e)
        {
            AssetManager.Get().StoreData();
        }

        //---------------------------------------------------------------------------

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
