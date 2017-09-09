using HelixToolkit.Wpf;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Media.Media3D;

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

    private Visual3D mModel;
    public Visual3D Model
    {
      get { return mModel; }
      set
      {
        mModel = value;
        NotifyPropertyChanged("Model");
      }
    }

    public void OpenModel()
    {
      mDialog.ShowDialog();
      mDialog.FileOk += MDialog_FileOk;
    }

    private void MDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
    {
      mDialog.FileOk -= MDialog_FileOk;

      Model = new FileModelVisual3D { Source = mDialog.FileName };
    }
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
