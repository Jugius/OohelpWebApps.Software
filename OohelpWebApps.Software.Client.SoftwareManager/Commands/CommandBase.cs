using System;
using System.Windows.Input;
using SoftwareManager.Services;

namespace SoftwareManager.Commands;
public abstract class CommandBase : ICommand
{
    public event EventHandler CanExecuteChanged;

    protected readonly ApiClientService ApplicationsService;
    protected readonly Services.DialogsProvider DialogProvider;

    protected CommandBase(ApiClientService applicationsService, DialogsProvider dialogProvider)
    {
        ApplicationsService = applicationsService ?? throw new ArgumentNullException(nameof(applicationsService));
        DialogProvider = dialogProvider ?? throw new ArgumentNullException(nameof(dialogProvider));
    }
    public virtual bool CanExecute(object parameter)
    {
        return true;
    }

    public abstract void Execute(object parameter);
    protected virtual void OnCanExecutedChanged()
    {
        CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
