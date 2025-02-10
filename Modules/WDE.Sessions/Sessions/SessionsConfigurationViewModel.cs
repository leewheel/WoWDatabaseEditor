using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using WDE.Common;
using WDE.Common.Sessions;
using WDE.Module.Attributes;
using WDE.MVVM;

namespace WDE.Sessions.Sessions
{
    [AutoRegister]
    public class SessionsConfigurationViewModel : ObservableBase, IConfigurable
    {
        private IEditorSessionStub? selectedItem;
        private bool? deleteOnSave;
        private bool isModified;
        public ICommand Save { get; }
        public string Name => "会话";
        public string? ShortDescription =>
            "会话允许您轻松生成 SQL，其中包含自上次开始会话以来所做的更改";
        public bool IsRestartRequired => false;
        public ConfigurableGroup Group => ConfigurableGroup.Basic;

        public SessionsConfigurationViewModel(ISessionService sessionService)
        {
            deleteOnSave = sessionService.DeleteOnSave;
            DeletedSessions = sessionService.DeletedSessions;
            RestoreSelectedSession = new DelegateCommand(() =>
            {
                sessionService.RestoreSession(SelectedItem!);
            }, () => SelectedItem != null).ObservesProperty(() => SelectedItem);
            Save = new DelegateCommand(() =>
            {
                sessionService.DeleteOnSave = deleteOnSave;
                IsModified = false;
            });
        }
        
        public ICommand RestoreSelectedSession { get; }

        public ObservableCollection<IEditorSessionStub> DeletedSessions { get; } = new();

        public bool? DeleteOnSave
        {
            get => deleteOnSave;
            set
            {
                SetProperty(ref deleteOnSave, value);
                IsModified = true;
            }
        }

        public bool IsModified
        {
            get => isModified;
            set => SetProperty(ref isModified, value);
        }
        
        public IEditorSessionStub? SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
    }
}