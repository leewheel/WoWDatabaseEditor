using System;
using System.Collections.Generic;
using WDE.Common.Services.FindAnywhere;
using WDE.Common.Settings;
using WDE.Module.Attributes;

namespace WoWDatabaseEditorCore.Services.OutlinerTool;

[AutoRegister]
public class OutlinerGeneralSettings : IGeneralSettingsGroup
{
    private readonly IOutlinerSettingsService settingsService;
    public string Name => "�����ͼ";
    public IReadOnlyList<IGenericSetting> Settings { get; set; }
    private List<BoolGenericSetting> settings;
    private List<int> values = new();
    
    public OutlinerGeneralSettings(IOutlinerSettingsService settingsService)
    {
        this.settingsService = settingsService;
        settings = new List<BoolGenericSetting>();
        var includeSources = ~settingsService.SkipSources;
        foreach (var val in Enum.GetValues<FindAnywhereSourceType>())
        {
            if (val != FindAnywhereSourceType.None && val != FindAnywhereSourceType.All)
            {
                string? help = val == FindAnywhereSourceType.SmartScripts ? "������ѡ��Ҫ�ڴ�ٹ����в��ҵ�Ԫ�ء�" : null;
                settings.Add(new BoolGenericSetting(val.ToString(), (includeSources & val) != 0, help));
                values.Add((int)val);
            }
        }
        Settings = settings;
    }
    
    public void Save()
    {
        FindAnywhereSourceType result = 0;
        for (int i = 0; i < settings.Count; ++i)
        {
            var include = settings[i].Value;
            if (include)
                result |= (FindAnywhereSourceType)values[i];
        }
        settingsService.SkipSources = FindAnywhereSourceType.All &~ result;
    }
}