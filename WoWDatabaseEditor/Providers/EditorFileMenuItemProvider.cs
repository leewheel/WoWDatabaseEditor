﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using WDE.Common;
using WDE.Common.Annotations;
using WDE.Common.Events;
using WDE.Common.Managers;
using WDE.Common.Menu;
using WDE.Common.Services;
using WDE.Common.Services.MessageBox;
using WDE.Common.Solution;
using WDE.Common.Utils;
using WDE.Common.Windows;
using WDE.Module.Attributes;
using WDE.MVVM;
using WDE.MVVM.Observable;
using WDE.Solutions;
using WoWDatabaseEditorCore.Managers;

namespace WoWDatabaseEditorCore.Providers
{
    [AutoRegister]
    public class EditorFileMenuItemProvider : IMainMenuItem, INotifyPropertyChanged
    {
        private readonly ISolutionManager solutionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly INewItemService newItemService;
        private readonly IConfigureService settings;
        private readonly Lazy<IMessageBoxService> messageBoxService;
        public IDocumentManager DocumentManager { get; }
        
        public string ItemName { get; } = "_文件";
        public List<IMenuItem> SubItems { get; }
        public MainMenuItemSortPriority SortPriority { get; } = MainMenuItemSortPriority.PriorityVeryHigh;

        public EditorFileMenuItemProvider(ISolutionManager solutionManager, IEventAggregator eventAggregator, INewItemService newItemService,  IDocumentManager documentManager, IConfigureService settings,
            IApplication application, ISolutionTasksService solutionTasksService, ISolutionSqlService solutionSqlService, Lazy<IMessageBoxService> messageBoxService)
        {
            this.solutionManager = solutionManager;
            this.eventAggregator = eventAggregator;
            this.newItemService = newItemService;
            DocumentManager = documentManager;
            this.settings = settings;
            this.messageBoxService = messageBoxService;
            SubItems = new List<IMenuItem>();
            if (solutionManager.CanContainAnyItem)
            {
                SubItems.Add(new ModuleMenuItem("_将物品添加到项目", new AsyncAutoCommand(AddNewItemWindow), new("Control+N")));
                SubItems.Add(new ModuleMenuItem("_新建/打开（无项目）", new AsyncAutoCommand(OpenNewItemWindow), new("Control+O")));
            }
            else
            {
                SubItems.Add(new ModuleMenuItem("_新建/打开", new AsyncAutoCommand(OpenNewItemWindow), new("Control+O")));
            }
            
            SubItems.Add(new ModuleMenuItem("_保存至数据库", 
                new DelegateCommand(
                        () =>
                        {
                            if (DocumentManager.SelectedTool != null && DocumentManager.SelectedTool is ISavableTool savable)
                                savable.Save.Execute(null);
                            else if (DocumentManager.ActiveSolutionItemDocument != null)
                                solutionTasksService.Save(DocumentManager.ActiveSolutionItemDocument!).ListenErrors(this.messageBoxService.Value);
                            else
                            {
                                async Task Func()
                                {
                                    if (documentManager.ActiveDocument is not IBeforeSaveConfirmDocument confirm || !await confirm.ShallSavePreventClosing()) 
                                        documentManager.ActiveDocument!.Save.Execute(null);
                                }

                                Func().ListenErrors();
                            }
                        },
                () => solutionTasksService.CanSaveToDatabase && (DocumentManager.SelectedTool is ISavableTool || (DocumentManager.ActiveDocument?.Save.CanExecute(null) ?? false)))
                    .ObservesProperty(() => DocumentManager.ActiveDocument)
                    .ObservesProperty(() => DocumentManager.SelectedTool)
                    .ObservesProperty(() => DocumentManager.ActiveDocument!.IsModified), new("Control+S")));
            
            SubItems.Add(new ModuleMenuItem("_全部保存", 
                new DelegateCommand(
                        () =>
                        {
                            foreach (var x in DocumentManager.AllTools)
                            {
                                if (x is ISavableTool savableTool)
                                    savableTool.Save.ExecuteAsync();
                            }

                            foreach (var x in DocumentManager.OpenedDocuments)
                            {
                                if (!x.IsModified)
                                    continue;
                                
                                if (x is ISolutionItemDocument sid)
                                    solutionTasksService.Save(sid).ListenErrors(this.messageBoxService.Value); // it adds 'save' to the queue, so it's ok not to wait here
                                else
                                {
                                    async Task Func()
                                    {
                                        if (x is not IBeforeSaveConfirmDocument confirm || !await confirm.ShallSavePreventClosing()) 
                                            await x.Save.ExecuteAsync();
                                    }

                                    Func().ListenErrors();
                                }   
                            }
                        },
                        () => solutionTasksService.CanSaveToDatabase), new("Control+Shift+S")));

            SubItems.Add(new ModuleMenuItem("_生成数据库查询", 
                new DelegateCommand(
                        () => solutionSqlService.OpenDocumentWithSqlFor(DocumentManager.ActiveSolutionItemDocument!.SolutionItem),
                    () => DocumentManager.ActiveSolutionItemDocument != null)
                .ObservesProperty(() => DocumentManager.ActiveSolutionItemDocument), new("F4")));

            var cmd = new DelegateCommand(
                    () => solutionTasksService.SaveAndReloadSolutionTask(DocumentManager.ActiveSolutionItemDocument!.SolutionItem),
                    () => DocumentManager.ActiveSolutionItemDocument != null && solutionTasksService.CanSaveAndReloadRemotely)
                .ObservesProperty(() => DocumentManager.ActiveSolutionItemDocument);
            SubItems.Add(new ModuleMenuItem("_保存到数据库并重新加载服务器", cmd, new("F5")));
            solutionTasksService.ToObservable(x => x.CanSaveAndReloadRemotely)
                .SubscribeAction(_ => cmd.RaiseCanExecuteChanged());
            
            SubItems.Add(new ModuleMenuItem("_生成所有已打开的查询", 
                new DelegateCommand(
                        () => solutionSqlService.OpenDocumentWithSqlFor(DocumentManager.OpenedDocuments.Select(d => (d as ISolutionItemDocument)?.SolutionItem!).Where(d => d != null).ToArray<ISolutionItem>()))
                    , new("F3")));

            SubItems.Add(new ModuleManuSeparatorItem());
            SubItems.Add(new ModuleMenuItem("_设置", new DelegateCommand(OpenSettings)));
            SubItems.Add(new ModuleManuSeparatorItem());
            SubItems.Add(new ModuleMenuItem("关闭当前标签", new AsyncAutoCommand(() => 
                documentManager.ActiveDocument?.CloseCommand?.ExecuteAsync() ?? Task.CompletedTask), new MenuShortcut("Control+W")));
            SubItems.Add(new ModuleMenuItem("关闭所有标签", new AsyncAutoCommand(async () => 
                await documentManager.TryCloseAllDocuments(false)), new MenuShortcut("Control+Shift+W")));
            SubItems.Add(new ModuleMenuItem("_退出", new DelegateCommand(() => application.TryClose())));
        }

        private async Task AddNewItemWindow()
        {
            try
            {
                ISolutionItem? item = await newItemService.GetNewSolutionItem();
                if (item != null)
                {
                    solutionManager.Add(item);
                    if (item is not SolutionFolderItem)
                        eventAggregator.GetEvent<EventRequestOpenItem>().Publish(item);
                }
            }
            catch (Exception e)
            {
                await messageBoxService.Value.SimpleDialog("错误", "生成新物品时报错", e.Message);
            }
        }
        
        private async Task OpenNewItemWindow()
        {
            try
            {
                ISolutionItem? item = await newItemService.GetNewSolutionItem(false);
                if (item != null)
                    eventAggregator.GetEvent<EventRequestOpenItem>().Publish(item);
            }
            catch (Exception e)
            {
                await messageBoxService.Value.SimpleDialog("错误", "生成新物品时报错", e.Message);
            }
        }
        private void OpenSettings() => settings.ShowSettings();

        public event PropertyChangedEventHandler? PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}