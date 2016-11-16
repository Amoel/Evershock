using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace TilesetViewer
{
    /// <summary>
    /// Interaction logic for LevelCreationWindow.xaml
    /// </summary>
    public partial class LevelCreationWindow : Window
    {
        public LevelCreationWindow()
        {
            InitializeComponent();
        }

        //---------------------------------------------------------------------------

        private void OnCreateLevelClicked(object sender, EventArgs e)
        {
            int width = 0;
            if (!int.TryParse(WidthInput.Text, out width) || width < 3)
            {
                MessageBox.Show("Input for Width is not correct.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                WidthInput.Focus();
                return;
            }

            int height = 0;
            if (!int.TryParse(HeightInput.Text, out height) || height < 3)
            {
                MessageBox.Show("Input for Height is not correct.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                HeightInput.Focus();
                return;
            }
            
            MapManager.Get().Create(width, height);
            MapManager.Get().UpdateImage();
            //LevelManager.Get().Create(width, height);
            Close();
        }
    }
}
