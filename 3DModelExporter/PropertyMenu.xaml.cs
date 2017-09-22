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
using System.Windows.Shapes;

namespace _3DModelExporter
{
    /// <summary>
    /// Interaction logic for PropertyMenu.xaml
    /// </summary>
    public partial class PropertyMenu : UserControl
    {
        public PropertyMenu()
        {
            InitializeComponent();
        }

        private PropertyMenuViewModel VM { get { return (DataContext as PropertyMenuViewModel); } }


        private void OnClick(object sender, RoutedEventArgs e)
        {
            VM.OpenModel();
        }
    }
}
