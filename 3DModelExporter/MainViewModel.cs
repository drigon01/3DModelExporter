using HelixToolkit.Wpf;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace _3DModelExporter
{
    class MainViewModel : PropertyChangedAware
    {
        private OpenFileDialog mDialog = new OpenFileDialog { Title = "Model Browser" };


        private int mNumberOfDetails = 8;
        public int NumberOfDetails
        {
            get { return mNumberOfDetails; }
            set
            {
                mNumberOfDetails = value;
                NumberOfDetailsX = value;
                NumberOfDetailsY = value;
                NumberOfDetailsZ = value;
                NotifyPropertyChanged("NumberOfDetails");
            }
        }

        private int mNumberOfDetailsX = 8;
        public int NumberOfDetailsX
        {
            get { return mNumberOfDetailsX; }
            set
            {
                mNumberOfDetailsX = value;
                NotifyPropertyChanged("NumberOfDetailsX");
            }
        }

        private int mNumberOfDetailsY = 8;
        public int NumberOfDetailsY
        {
            get { return mNumberOfDetailsY; }
            set
            {
                mNumberOfDetailsY = value;
                NotifyPropertyChanged("NumberOfDetailsY");
            }
        }

        private int mNumberOfDetailsZ = 8;
        public int NumberOfDetailsZ
        {
            get { return mNumberOfDetailsZ; }
            set
            {
                mNumberOfDetailsZ = value;
                NotifyPropertyChanged("NumberOfDetailsZ");
            }
        }

        public bool IsModellAvaiable { get { return !(Model == null); } }

        private int mDotsPerInch = 110;
        public int DotsPerInch
        {
            get { return mDotsPerInch; }
            set
            {
                mDotsPerInch = value;
                NotifyPropertyChanged("DotsPerInch");
            }
        }

        private double mLineDiameter = 3;
        public double LineDiameter
        {
            get { return mLineDiameter; }
            set
            {
                mLineDiameter = value;
                NotifyPropertyChanged("LineDiameter");
            }
        }

        private Color mWireframeColor = Colors.Green;
        public Color WireframeColor
        {
            get { return mWireframeColor; }
            set
            {
                mWireframeColor = value;
                NotifyPropertyChanged("WireframeColor");
            }
        }

        private PerspectiveCamera mPerspectiveCamera = new PerspectiveCamera();
        public PerspectiveCamera PerspectiveCamera { get; set; }

        private Visual3D mModel;// = new new FileModelVisual3D { Source = mDialog.FileName };
        public Visual3D Model
        {
            get { return mModel; }
            set
            {
                mModel = value;
                NotifyPropertyChanged("Model");
                NotifyPropertyChanged("IsModellAvaiable");
            }
        }

        private RelayCommand mPerspectiveSelected;
        public RelayCommand PerspectiveSelected
        {
            get
            {
                return mPerspectiveSelected ?? (mPerspectiveSelected =
                    new RelayCommand((param) =>
                    { SetPresetCameraPosition(param.ToString()); }, true));
            }
        }

        public void SetPresetCameraPosition(string presetName)
        {
            ///PerspectiveCamera.UpDirection*3
            var wBounds = Visual3DHelper.FindBounds(Model, null);
            switch (presetName)
            {// Need to add proper calc to reposition over model's center
                case "Top": PerspectiveCameraSetup(0, -0.1, wBounds.SizeZ + 5, 0, -0.1, -(wBounds.SizeZ + 5)); break;

                //Both should scale better with the model to enclose everything in the scene
                case "Front": PerspectiveCameraSetup(0, -(wBounds.SizeY + 5), 0, 0, wBounds.SizeY + 5, 0); break;
                case "Side": PerspectiveCameraSetup(wBounds.SizeX + 15, 0, 0, -(wBounds.SizeX + 15), 0, 0); break;
            }
        }

        private RelayCommand mExport;
        public RelayCommand Export
        {
            get
            {
                return mExport ?? (mExport = new RelayCommand((param) =>
                {
                    ExportToPNG((FrameworkElement)param);
                }, true));
            }
        }

        private void ExportToPNG(FrameworkElement viewport)
        {
            //viewport.Background = ();

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)1920, (int)1080, DotsPerInch, DotsPerInch, PixelFormats.Pbgra32);
            bmp.Render(viewport);

            var wPng = new PngBitmapEncoder();
            wPng.Frames.Add(BitmapFrame.Create(bmp));

            var wDialog = new SaveFileDialog();
            wDialog.ShowDialog();
            if (string.IsNullOrEmpty(wDialog.FileName)) return;
            using (Stream wStream = File.Create(wDialog.FileName)) { wPng.Save(wStream); }
        }

        public void OpenModel()
        {
            mDialog.ShowDialog();
            Model = new FileModelVisual3D { Source = mDialog.FileName };
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

    public class RelayCommand : ICommand
    {
        private Action<object> mAction;
        private bool mCanExecute;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> action, bool canExecute)
        {
            mAction = action;
            mCanExecute = canExecute;
        }

        public bool CanExecute(object parameter) { return mCanExecute; }
        public void Execute(object parameter) { mAction(parameter); }
    }

    class PropertyChangedAware : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    static class Extensions
    {
        //public static string NameOf(object property)
        //{
        //  return (property.GetType()).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Where(p => p)
        //}
    }
}
