using System.Collections.Generic;
using WDE.Common.Settings;
using WDE.Module.Attributes;

namespace WoWDatabaseEditorCore.Settings;

[AutoRegister]
public class SettingsProvider : IGeneralSettingsGroup
{
    private readonly IGeneralEditorSettingsProvider editorSettingsProvider;
    public string Name => "����";
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

        restoreOpenTabsMode = new ListOptionGenericSetting("�ָ��򿪱�ǩҳģʽ",
            new object[]
            {
                RestoreOpenTabsMode.RestoreWhenCrashed, RestoreOpenTabsMode.AlwaysRestore, RestoreOpenTabsMode.NeverRestore
            },
            editorSettingsProvider.RestoreOpenTabsMode, "�༭�������ڱ���ʱ�ָ��򿪵�ѡ���ÿ�ζ����ԣ�������Զ���ᡣ");

        toolbarIconStyle = new ListOptionGenericSetting("��������ť��ʽ",
            new object[]
            {
                ToolBarButtonStyle.Icon, ToolBarButtonStyle.IconAndText, ToolBarButtonStyle.Text
            },
            editorSettingsProvider.ToolBarButtonStyle, "��������ť����ʽ�������������а�ť����");
        
        Settings = new List<IGenericSetting>()
        {
            restoreOpenTabsMode,
            toolbarIconStyle
        };
    }
}