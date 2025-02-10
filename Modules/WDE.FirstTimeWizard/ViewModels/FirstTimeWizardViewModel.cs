using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using PropertyChanged.SourceGenerator;
using WDE.Common;
using WDE.Common.Managers;
using WDE.Common.Services.MessageBox;
using WDE.Common.Utils;
using WDE.FirstTimeWizard.Services;
using WDE.MVVM;
using WDE.Updater.Services;

namespace WDE.FirstTimeWizard.ViewModels;

public partial class FirstTimeWizardViewModel : ObservableBase, IDialog
{
    private readonly IUpdateService updateService;

    public FirstTimeWizardViewModel(IEnumerable<IFirstTimeWizardConfigurable> configs,
        ICoreVersionConfigurable coreVersionConfig,
        IMessageBoxService messageBoxService,
        IFirstTimeWizardSettings settings,
        IUpdateService updateService)
    {
        this.updateService = updateService;
        CoreVersionViewModel = coreVersionConfig;
        Configurables = configs.ToList();
        selectedConfigurable = Configurables[0];
        HasCoreVersion = settings.State == FirstTimeWizardState.HasCoreVersion;
        Accept = new AsyncAutoCommand(async () =>
        {
            if (NoCoreVersion)
            {
                settings.State = FirstTimeWizardState.HasCoreVersion;
                coreVersionConfig.Save.Execute(null);
            }
            else
            {
                foreach (var config in Configurables)
                {
                    config.Save.Execute(null);
                }

                settings.State = FirstTimeWizardState.Completed;
            }

            await messageBoxService.ShowDialog(new MessageBoxFactory<bool>()
                .SetTitle("需要重新启动")
                .SetMainInstruction("需要重新启动")
                .SetContent(
                    "为了保存设置，您必须重新启动编辑器。")
                .WithOkButton(true)
                .Build());

            await updateService.CloseForUpdate();
            
            CloseOk?.Invoke();
        });
        Cancel = new AsyncAutoCommand(async () =>
        {
            settings.State = FirstTimeWizardState.Canceled;
            
            await messageBoxService.ShowDialog(new MessageBoxFactory<bool>()
                .SetTitle("首次向导")
                .SetMainInstruction("取消设置")
                .SetContent(
                    "您已取消首次向导，但不用担心，您可以稍后在文件 -> 设置菜单中更改设置。")
                .WithOkButton(true)
                .Build());
            
            CloseCancel?.Invoke();
        });
    }

    public object CoreVersionViewModel { get; }
    public List<IFirstTimeWizardConfigurable> Configurables { get; }
    [Notify] private IFirstTimeWizardConfigurable? selectedConfigurable;
    
    public bool NoCoreVersion => !HasCoreVersion;
    public bool HasCoreVersion { get; }
    
    public int DesiredWidth => 800;
    public int DesiredHeight => 600;
    public string Title => "首次向导";
    public bool Resizeable => true;
    public ICommand Accept { get; }
    public ICommand Cancel { get; }

    public event Action? CloseCancel;
    public event Action? CloseOk;
}