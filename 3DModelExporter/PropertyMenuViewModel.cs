using HelixToolkit.Wpf;
using Microsoft.Win32;
using System;
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

    public Action RedrawAction { get; set; }
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

    private RelayCommand mRedraw;
    public RelayCommand Redraw
    {
      get
      {
        return mRedraw ?? (mRedraw = new RelayCommand((param) =>
        {
          RedrawAction();
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
        NotifyPropertyChanged(this.NameOf(p => p.Model));
        NotifyPropertyChanged(this.NameOf(p => p.IsModellAvaiable));
      }
    }

    private ViewMode mViewMode = ViewMode.HelixToolkit;
    public ViewMode ViewMode
    {
      get { return mViewMode; }
      set
      {
        mViewMode = value;
        NotifyPropertyChanged(this.NameOf(p => p.ViewMode));
        NotifyPropertyChanged(this.NameOf(p => p.ViewModeName));
      }
    }
    public string ViewModeName { get { return ViewMode.ToString("G"); } }

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
        NotifyPropertyChanged(this.NameOf(p => p.NumberOfDetails));
      }
    }

    private int mNumberOfDetailsX = 8;
    public int NumberOfDetailsX
    {
      get { return mNumberOfDetailsX; }
      set
      {
        mNumberOfDetailsX = value;
        NotifyPropertyChanged(this.NameOf(p => p.NumberOfDetailsX));
      }
    }

    private int mNumberOfDetailsY = 8;
    public int NumberOfDetailsY
    {
      get { return mNumberOfDetailsY; }
      set
      {
        mNumberOfDetailsY = value;
        NotifyPropertyChanged(this.NameOf(p => p.NumberOfDetailsY));
      }
    }

    private int mNumberOfDetailsZ = 8;
    public int NumberOfDetailsZ
    {
      get { return mNumberOfDetailsZ; }
      set
      {
        mNumberOfDetailsZ = value;
        NotifyPropertyChanged(this.NameOf(p => p.NumberOfDetailsZ));
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
        NotifyPropertyChanged(this.NameOf(p => p.DotsPerInch));
      }
    }

    private double mLineDiameter = 3;
    public double LineDiameter
    {
      get { return mLineDiameter; }
      set
      {
        mLineDiameter = value;
        NotifyPropertyChanged(this.NameOf(p => p.LineDiameter));
      }
    }

    private Color mWireframeColor = Colors.Green;
    public Color WireframeColor
    {
      get { return mWireframeColor; }
      set
      {
        mWireframeColor = value;
        NotifyPropertyChanged(this.NameOf(p => p.WireframeColor));
      }
    }
  }
  public enum ViewMode
  {
    HelixToolkit,
    SharpGL
  }
}
