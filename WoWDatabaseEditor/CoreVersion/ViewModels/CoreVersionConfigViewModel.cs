﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using WDE.Common;
using WDE.Common.CoreVersion;
using WDE.Module.Attributes;
using WDE.MVVM;

namespace WoWDatabaseEditorCore.CoreVersion.ViewModels
{
    [AutoRegister]
    public class CoreVersionConfigViewModel : ObservableBase, ICoreVersionConfigurable
    {
        private readonly ICurrentCoreVersion currentCoreVersion;
        public ObservableCollection<ICoreVersion> CoreVersions { get; }

        private ICoreVersion selectedVersion;
        public ICoreVersion SelectedVersion
        {
            get => selectedVersion;
            set
            {
                SetProperty(ref selectedVersion, value);
                IsModified = true;
            }
        }
        
        private bool isModified;
        public bool IsModified
        {
            get => isModified;
            set => SetProperty(ref isModified, value);
        }

        public CoreVersionConfigViewModel(ICoreVersionsProvider versionsProvider,
            ICoreVersionSettings settings,
            ICurrentCoreVersion currentCoreVersion)
        {
            this.currentCoreVersion = currentCoreVersion;
            Watch(this, t => t.SelectedVersion, nameof(IsModified));
            selectedVersion = currentCoreVersion.Current;
            
            CoreVersions = new ObservableCollection<ICoreVersion>(versionsProvider.AllVersions);
            
            Save = new DelegateCommand(() =>
            {
                settings.UpdateCurrentVersion(SelectedVersion);
                IsModified = false;
            });
        }
        
        public ICommand Save { get; }
        public string ShortDescription =>
            "选择您要使用的核心版本。特定模块（如 SmartScripts 模块）可以遵循该版本。";
        public string Name => "核心板本";
        public bool IsRestartRequired => true;
        public ConfigurableGroup Group => ConfigurableGroup.Basic;
    }
}