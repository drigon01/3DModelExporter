using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace _3DModelExporter
{
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

  public static class Extensions
  {
    public static string NameOf<T, TProp>(this T o, Expression<Func<T, TProp>> propertySelector)
    {
      var wBody = (MemberExpression)propertySelector.Body;
      return wBody.Member.Name;
    }
    /// <summary>
    /// Finds a Child of a given item in the visual tree. 
    /// </summary>
    /// <param name="parent">A direct parent of the queried item.</param>
    /// <typeparam name="T">The type of the queried item.</typeparam>
    /// <param name="childName">x:Name or Name of child. </param>
    /// <returns>The first parent item that matches the submitted type parameter. 
    /// If not matching item can be found, 
    /// a null parent is being returned.</returns>
    public static T FindChild<T>(this DependencyObject parent, string childName)
           where T : DependencyObject
    {
      // Confirm parent and childName are valid. 
      if (parent == null) return null;

      T wFoundChild = null;

      int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
      for (int i = 0; i < childrenCount; i++)
      {
        var wChild = VisualTreeHelper.GetChild(parent, i);
        // If the child is not of the request child type child
        T wChildType = wChild as T;
        if (wChildType == null)
        {
          // recursively drill down the tree
          wFoundChild = FindChild<T>(wChild, childName);

          // If the child is found, break so we do not overwrite the found child. 
          if (wFoundChild != null) break;
        }
        else if (!string.IsNullOrEmpty(childName))
        {
          var frameworkElement = wChild as FrameworkElement;
          // If the child's name is set for search
          if (frameworkElement != null && frameworkElement.Name == childName)
          {
            // if the child's name is of the request name
            wFoundChild = (T)wChild;
            break;
          }
        }
        else
        {
          // child element found.
          wFoundChild = (T)wChild;
          break;
        }
      }
      return wFoundChild;
    }
  }
}
