using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AbroadConcepts.MVVM;

public interface IDialogService
{
    void Register(Type viewType, Type viewModelType);

    bool? ShowDialog<TViewModel>(bool modal = false) where TViewModel : ViewModelBase, IDialogViewModel;
}
