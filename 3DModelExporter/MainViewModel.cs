using HelixToolkit.Wpf;
using Microsoft.Win32;
using System.Windows.Media.Media3D;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace _3DModelExporter
{
    class MainViewModel : PropertyChangedAware
    {
        public MainViewModel() { Properties = new PropertyMenuViewModel(ExportToPNG, SetPresetCameraPosition); }

        private PropertyMenuViewModel mPropertyVM;
        public PropertyMenuViewModel Properties { get { return mPropertyVM; } internal set { mPropertyVM = value; } }// NotifyPropertyChanged("Properties"); } }

        private PerspectiveCamera mPerspectiveCamera = new PerspectiveCamera();
        public PerspectiveCamera PerspectiveCamera { get; set; }

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
            //viewport.Background = ();

            var wViewPort = Extensions.FindChild<HelixViewport3D>(Application.Current.MainWindow, "mViewPort");

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
