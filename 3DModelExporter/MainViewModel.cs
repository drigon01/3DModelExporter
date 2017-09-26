using HelixToolkit.Wpf;
using Microsoft.Win32;
using System.Windows.Media.Media3D;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Linq;

namespace _3DModelExporter
{
  class MainViewModel : PropertyChangedAware
  {
    public MainViewModel()
    {
      Properties = new PropertyMenuViewModel(ExportToPNG, SetPresetCameraPosition);
      Properties.PropertyChanged += OnPropertyChanged;
      Properties.RedrawAction = DrawHelix;
    }

    private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == Properties.NameOf(p => p.ViewMode))
      {
        if (Properties.ViewMode == ViewMode.HelixToolkit)
          Properties.RedrawAction = DrawHelix;
        else
          Properties.RedrawAction = DrawOGL;
      }
    }

    private void DrawHelix()
    {
      if (mHelixViewPort != null)
      {
        mHelixViewPort.Children.Clear();
        AddContours(Properties.Model, Properties.NumberOfDetailsX, Properties.NumberOfDetailsY, Properties.NumberOfDetailsZ);
      }
    }

    private void DrawOGL()
    {

    }

    HelixViewport3D mHelixViewPort;
    HelixViewport3D HelixViewPort
    {
      get { return mHelixViewPort; }
      set
      {
        mHelixViewPort = value;
        NotifyPropertyChanged(this.NameOf(p => p.HelixViewPort));
      }
    }

    public void LoadHelixViewPort(FrameworkElement control)
    {
      HelixViewPort = control.FindChild<HelixViewport3D>("mHelixViewPort");
      if (HelixViewPort != null)
      {
        HelixViewPort.Camera = PerspectiveCamera;
      }
    }

    private PropertyMenuViewModel mPropertyVM;
    public PropertyMenuViewModel Properties { get { return mPropertyVM; } internal set { mPropertyVM = value; } }// NotifyPropertyChanged("Properties"); } }

    private PerspectiveCamera mPerspectiveCamera = new PerspectiveCamera();
    public PerspectiveCamera PerspectiveCamera { get; set; }

    private Plane3D ContourPlane;

    private void AddContours(Visual3D model, int o, int m, int n)
    {
      var wBounds = Visual3DHelper.FindBounds(model, Transform3D.Identity);
      for (int i = 1; i < n; i++)
      {
        this.ContourPlane = new Plane3D(new Point3D(0, 0, wBounds.Location.Z + wBounds.Size.Z * i / n), new Vector3D(0, 0, 1));
        Visual3DHelper.Traverse<GeometryModel3D>(model, this.AddContours);
      }
      for (int i = 1; i < m; i++)
      {
        this.ContourPlane = new Plane3D(new Point3D(0, wBounds.Location.Y + wBounds.Size.Y * i / m, 0), new Vector3D(0, 1, 0));
        Visual3DHelper.Traverse<GeometryModel3D>(model, this.AddContours);
      }
      for (int i = 1; i < o; i++)
      {
        this.ContourPlane = new Plane3D(new Point3D(wBounds.Location.X + wBounds.Size.X * i / o, 0, 0), new Vector3D(1, 0, 0));
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
        HelixViewPort.Children.Add(new TubeVisual3D
        {
          Diameter = Properties.LineDiameter / 100,
          Path = new Point3DCollection(wContour),
          Fill = new SolidColorBrush(Properties.WireframeColor)
        });
      }
    }

    public void SetPresetCameraPosition(string presetName)
    {
      ///PerspectiveCamera.UpDirection*3
      var wBounds = Visual3DHelper.FindBounds(Properties.Model, null);
      switch (presetName)
      {// Need to add proper calc to reposition over model's center
        case "Top": PerspectiveCameraSetup(0, -0.1, wBounds.SizeZ + 5, 0, -0.1, -(wBounds.SizeZ + 5)); break;

        //Both should scale better with the model to enclose everything in the scene
        case "Front": PerspectiveCameraSetup(0, -(wBounds.SizeY + 5), 0, 0, wBounds.SizeY + 5, 0); break;
        case "Side": PerspectiveCameraSetup(wBounds.SizeX + 15, 0, 0, -(wBounds.SizeX + 15), 0, 0); break;
      }
    }

    private void ExportToPNG()
    {
      var wViewPort = Application.Current.MainWindow.FindChild<HelixViewport3D>("mHelixViewPort");

      RenderTargetBitmap bmp = new RenderTargetBitmap((int)1920, (int)1080, Properties.DotsPerInch, Properties.DotsPerInch, PixelFormats.Pbgra32);
      bmp.Render(wViewPort);

      var wPng = new PngBitmapEncoder();
      wPng.Frames.Add(BitmapFrame.Create(bmp));

      var wDialog = new SaveFileDialog();
      wDialog.ShowDialog();
      if (string.IsNullOrEmpty(wDialog.FileName)) return;
      using (Stream wStream = File.Create(wDialog.FileName)) { wPng.Save(wStream); }
    }

    public void PerspectiveCameraSetup(double x, double y, double z, double dx, double dy, double dz)
    {
      if (PerspectiveCamera == null) PerspectiveCamera = new PerspectiveCamera();
      PerspectiveCamera.Position = new Point3D(x, y, z);
      PerspectiveCamera.LookDirection = new Vector3D(dx, dy, dz);
      PerspectiveCamera.UpDirection = new Vector3D(0, 0, 1);
      PerspectiveCamera.FieldOfView = 60;
    }
  }
}
