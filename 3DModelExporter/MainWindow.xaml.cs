using HelixToolkit.Wpf;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3DModelExporter
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      DataContext = new MainViewModel();
      VM.PerspectiveCameraSetup(5.3, -12.3, 3.3, -6.3, 11, -6.6);
    }

    private MainViewModel VM { get { return (DataContext as MainViewModel); } }

    private void mContentControl_Loaded(object sender, RoutedEventArgs e)
    {
      VM.LoadHelixViewPort(mContentControl);
    }
  }
}
