using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AbroadConcepts.MVVM
{
    public class DialogViewModel : ViewModelBase, IDialogViewModel
    {
        public string Title { get; } = "Title";
        public WindowStartupLocation StartupLocation { get;  } = WindowStartupLocation.CenterScreen;
    }
}
