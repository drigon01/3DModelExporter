using HelixToolkit.Wpf;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3DModelExporter
{
    class PropertyMenuViewModel : PropertyChangedAware
    {

        public PropertyMenuViewModel(Action export,
            Action<string> setPresetCameraPosition)
        {
            ExportAction = export;
            SetPresetCameraPosition = setPresetCameraPosition;
        }

        Action ExportAction;
        Action<string> SetPresetCameraPosition;

        private OpenFileDialog mDialog = new OpenFileDialog { Title = "Model Browser" };

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

        private RelayCommand mExport;
        public RelayCommand Export
        {
            get
            {
                return mExport ?? (mExport = new RelayCommand((param) =>
                {
                    ExportAction();
                }, true));
            }
        }

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

        public void OpenModel()
        {
            mDialog.ShowDialog();
            Model = new FileModelVisual3D { Source = mDialog.FileName };
        }

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
    }
}
