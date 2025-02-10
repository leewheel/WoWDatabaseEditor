using System.Collections.Generic;
using WDE.Common.Settings;
using WDE.Module.Attributes;

namespace WoWDatabaseEditorCore.Settings;

[AutoRegister]
public class SettingsProvider : IGeneralSettingsGroup
{
    private readonly IGeneralEditorSettingsProvider editorSettingsProvider;
    public string Name => "常规";
    public IReadOnlyList<IGenericSetting> Settings { get; }
    
    private ListOptionGenericSetting restoreOpenTabsMode;
    private ListOptionGenericSetting toolbarIconStyle;
    
    public void Save()
    {
        editorSettingsProvider.RestoreOpenTabsMode = (RestoreOpenTabsMode)restoreOpenTabsMode.SelectedOption;
        editorSettingsProvider.ToolBarButtonStyle = (ToolBarButtonStyle)toolbarIconStyle.SelectedOption;
        editorSettingsProvider.Apply();
    }

    public SettingsProvider(IGeneralEditorSettingsProvider editorSettingsProvider)
    {
        this.editorSettingsProvider = editorSettingsProvider;

        restoreOpenTabsMode = new ListOptionGenericSetting("恢复打开标签页模式",
            new object[]
            {
                RestoreOpenTabsMode.RestoreWhenCrashed, RestoreOpenTabsMode.AlwaysRestore, RestoreOpenTabsMode.NeverRestore
            },
            editorSettingsProvider.RestoreOpenTabsMode, "编辑器可以在崩溃时恢复打开的选项卡，每次都可以，或者永远不会。");

        toolbarIconStyle = new ListOptionGenericSetting("工具栏按钮样式",
            new object[]
            {
                ToolBarButtonStyle.Icon, ToolBarButtonStyle.IconAndText, ToolBarButtonStyle.Text
            },
            editorSettingsProvider.ToolBarButtonStyle, "工具栏按钮的样式（不适用于所有按钮）。");
        
        Settings = new List<IGenericSetting>()
        {
            restoreOpenTabsMode,
            toolbarIconStyle
        };
    }
}