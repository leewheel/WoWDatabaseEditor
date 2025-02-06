using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using Prism.Commands;
using WDE.Common.CoreVersion;
using WDE.Common.Database;
using WDE.Common.DBC;
using WDE.Common.Factories.Http;
using WDE.Common.History;
using WDE.Common.Managers;
using WDE.Common.Services;
using WDE.Common.Types;
using WDE.Common.Utils;
using WDE.Module.Attributes;
using WoWDatabaseEditorCore.CoreVersion;

namespace WoWDatabaseEditorCore.ViewModels
{
    [AutoRegister]
    public class AboutViewModel : IDocument, IUserAgentPart
    {
        private readonly IApplicationVersion applicationVersion;
        private readonly IDatabaseProvider databaseProvider;
        private readonly IDbcStore dbcStore;
        private readonly IRemoteConnectorService remoteConnectorService;

        public AboutViewModel(IApplicationVersion applicationVersion,
            IDatabaseProvider databaseProvider, 
            IDbcStore dbcStore,
            Lazy<IConfigureService> settings,
            ICurrentCoreVersion coreVersion,
            IRemoteConnectorService remoteConnectorService,
            ISourceCodePathService sourceCodePathService)
        {
            this.applicationVersion = applicationVersion;
            this.databaseProvider = databaseProvider;
            this.dbcStore = dbcStore;
            this.remoteConnectorService = remoteConnectorService;

            ConfigurationChecks.Add(new ConfigurationCheckup(coreVersion.Current is not UnspecifiedCoreVersion,
                "核心版本兼容模式",
                "WoW 数据库编辑器支持多个 WoW 服务器核心。为了实现最大兼容性，请选择最匹配的版本。\n您正在使用: " + coreVersion.Current.FriendlyName + " 兼容模式。"));
            
            ConfigurationChecks.Add(new ConfigurationCheckup(dbcStore.IsConfigured, 
                "DBC 设定",
                "DBC 是随 WoW 客户端提供的 DataBaseClient 文件。其中包含很多有用的脚本内容，例如法术数据。要获得最大功能，您必须提供 DBC 文件路径。所有 WoW 服务器都需要这些文件才能运行，因此如果您有可运行的核心，则必须已经拥有 DBC 文件。"));
            
            ConfigurationChecks.Add(new ConfigurationCheckup(databaseProvider.IsConnected,
                "数据库连接",
                "WoW 数据库编辑器在设计上就是数据库编辑器。它存储所有数据并从 wow 数据库加载内容。因此，要激活所有功能，您必须提供与 wow 兼容的数据库连接设置。"));

            ConfigurationChecks.Add(new ConfigurationCheckup(remoteConnectorService.HasValidSettings,
                "远程连接",
                "WDE 可以为您调用重新加载命令以加快工作速度。要启用此功能，您必须在 worldserver 配置中启用远程连接并在设置中提供详细信息。"));

            ConfigurationChecks.Add(new ConfigurationCheckup(sourceCodePathService.SourceCodePaths.Count > 0,
                "源代码集成",
                "WDE 可以与你的服务器源代码集成。任何时候可以在源代码中进行搜索。"));

            AllConfigured = ConfigurationChecks.All(s => s.Fulfilled);

            OpenSettingsCommand = new DelegateCommand(() => settings.Value.ShowSettings());
        }

        public ICommand OpenSettingsCommand { get; }
        public bool AllConfigured { get; }
        public ObservableCollection<ConfigurationCheckup> ConfigurationChecks { get; } = new();
        public int BuildVersion => applicationVersion.BuildVersion;
        public string Branch => applicationVersion.Branch;
        public string CommitHash => applicationVersion.CommitHash;
        public bool VersionKnown => applicationVersion.VersionKnown;
        public string ReleaseData => $"WoWDatabaseEditor, branch: {Branch}, build: {BuildVersion}, commit: {CommitHash}";

        public ImageUri? Icon => new ImageUri("Icons/wde_icon.png");
        public string Title { get; } = "About";
        public ICommand Undo { get; } = new DisabledCommand();
        public ICommand Redo { get; } = new DisabledCommand();
        public ICommand Copy { get; } = new DisabledCommand();
        public ICommand Cut { get; } = new DisabledCommand();
        public ICommand Paste { get; } = new DisabledCommand();
        public IAsyncCommand Save { get; } = AlwaysDisabledAsyncCommand.Command;
        public AsyncAwaitBestPractices.MVVM.IAsyncCommand? CloseCommand { get; set; } = null;
        public bool CanClose { get; } = true;
        public bool IsModified { get; } = false;
        public IHistoryManager? History { get; } = null;
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Dispose()
        {
        }

        public class ConfigurationCheckup
        {
            public bool Fulfilled { get; }
            public string Title { get; }
            public string Description { get; }

            public ConfigurationCheckup(bool fulfilled, string title, string description)
            {
                Fulfilled = fulfilled;
                Title = title;
                Description = description;
            }
        }

        public string Part => $"(DBC: {BoolToString(dbcStore.IsConfigured)}, DB: {BoolToString(databaseProvider.IsConnected)}, SOAP: {BoolToString(remoteConnectorService.IsConnected)})";

        private string BoolToString(bool b)
        {
            return b ? "Yes" : "No";
        }
    }

    public class DisabledCommand : ICommand
    {
        public bool CanExecute(object? parameter)
        {
            return false;
        }

        public void Execute(object? parameter)
        {
            throw new NotImplementedException();
        }

        public event EventHandler? CanExecuteChanged;
    }
}