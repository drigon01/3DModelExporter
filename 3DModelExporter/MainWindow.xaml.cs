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
            VM.PropertyChanged += VM_PropertyChanged;
            VM.PerspectiveCameraSetup(5.3, -12.3, 3.3, -6.3, 11, -6.6);
            mViewPort.Camera = VM.PerspectiveCamera;
        }

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            mViewPort.Children.Clear();
            AddContours(VM.Model, VM.NumberOfDetails, VM.NumberOfDetails, VM.NumberOfDetails);
        }

        private MainViewModel VM { get { return (DataContext as MainViewModel); } }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            VM.OpenModel();
        }

        private Plane3D ContourPlane;

        private void AddContours(Visual3D model, int o, int m, int n)
        {
            var bounds = Visual3DHelper.FindBounds(model, Transform3D.Identity);
            for (int i = 1; i < n; i++)
            {
                this.ContourPlane = new Plane3D(new Point3D(0, 0, bounds.Location.Z + bounds.Size.Z * i / n), new Vector3D(0, 0, 1));
                Visual3DHelper.Traverse<GeometryModel3D>(model, this.AddContours);
            }
            for (int i = 1; i < m; i++)
            {
                this.ContourPlane = new Plane3D(new Point3D(0, bounds.Location.Y + bounds.Size.Y * i / m, 0), new Vector3D(0, 1, 0));
                Visual3DHelper.Traverse<GeometryModel3D>(model, this.AddContours);
            }
            for (int i = 1; i < o; i++)
            {
                this.ContourPlane = new Plane3D(new Point3D(bounds.Location.X + bounds.Size.X * i / o, 0, 0), new Vector3D(1, 0, 0));
                Visual3DHelper.Traverse<GeometryModel3D>(model, this.AddContours);
            }
        }

        private void AddContours(GeometryModel3D model, Transform3D transform)
        {
            var wPosition = ContourPlane.Position;
            var wNormal = ContourPlane.Normal;
            var wSegments = MeshGeometryHelper.GetContourSegments(model.Geometry as MeshGeometry3D, wPosition, wNormal).ToList();
            foreach (var wContour in MeshGeometryHelper.CombineSegments(wSegments, 1e-6).ToList())
            {
                if (wContour.Count == 0)
                    continue;
                mViewPort.Children.Add(new TubeVisual3D
                {
                    Diameter = 0.03,
                    Path = new Point3DCollection(wContour),
                    Fill = new SolidColorBrush(VM.WireframeColor)
                });
            }
        }
    }
}
