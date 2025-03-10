using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AbroadConcepts.MVVM;

namespace WpfApp1.Mvvm;


public class DialogService : IDialogService
{
    private  Dictionary<Type, Type> dialogTypes = new Dictionary<Type, Type>();

    public  void Register(Type viewType, Type viewModelType)
    {
        dialogTypes[viewModelType] = viewType;
    }

    public bool? ShowDialog<TViewModel>(bool modal = false) where TViewModel : ViewModelBase, IDialogViewModel
    {
        var dialog = new Dialog();

        var viewModel = Activator.CreateInstance<TViewModel>();
        dialog.DataContext = viewModel;
        if (modal)
        {
            dialog.WindowStyle = WindowStyle.None;
        }

        var type = dialogTypes[typeof(TViewModel)];
        var content = Activator.CreateInstance(type) as UserControl;
        
        dialog.Content = content;
        dialog.Width = content.Width;
        dialog.Height = content.Height;
        return dialog.ShowDialog();

    }

}
