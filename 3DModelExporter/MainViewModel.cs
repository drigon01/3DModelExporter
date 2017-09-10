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
                NotifyPropertyChanged("NumberOfDetails");
            }
        }

        private int mDotsPerInch = 32;
        public int DotsPerInch
        {
            get { return mDotsPerInch; }
            set
            {
                mDotsPerInch = value;
                NotifyPropertyChanged("DotsPerInch");
            }
        }

        private int mLineDiameter = 3;
        public int LineDiameter
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
            }
        }

        private RelayCommand mPerspectiveSelected;
        public RelayCommand PerspectiveSelected
        {
            get
            {
                return mPerspectiveSelected ?? (mPerspectiveSelected =
                    new RelayCommand((param) =>
                    {
                        switch (param.ToString())
                        {// Need to add proper calc to reposition over model's center
                            case "Top": PerspectiveCameraSetup(10, -10, 10, -10, 10, -10); break;
                            //Both should scale with the model to enclose everything in the scene
                            case "Front": PerspectiveCameraSetup(0, 15, 0, 0, -15, 0); break;
                            case "Side": PerspectiveCameraSetup(15, 0, 0, -15, 0, 0); break;
                        }
                    }, true));
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

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)viewport.ActualWidth, (int)viewport.ActualHeight, DotsPerInch, DotsPerInch, PixelFormats.Pbgra32);
            bmp.Render(viewport);

            var wPng = new PngBitmapEncoder();
            wPng.Frames.Add(BitmapFrame.Create(bmp));

            using (Stream wStream = File.Create(@"D:\\" + DateTime.Now.Ticks + ".png")) { wPng.Save(wStream); }
        }

        public void OpenModel()
        {
            mDialog.FileOk += MDialog_FileOk;
            mDialog.ShowDialog();
        }

        private void MDialog_FileOk(object sender, CancelEventArgs e)
        {
            mDialog.FileOk -= MDialog_FileOk;

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
