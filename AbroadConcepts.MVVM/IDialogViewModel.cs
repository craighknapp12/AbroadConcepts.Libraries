using System.Windows;

namespace AbroadConcepts.MVVM;

public interface IDialogViewModel
{
    public string Title { get; }
    public WindowStartupLocation StartupLocation { get; }

}