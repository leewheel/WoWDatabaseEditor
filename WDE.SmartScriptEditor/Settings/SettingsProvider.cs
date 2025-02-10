using System.Collections.Generic;
using WDE.Common.Settings;
using WDE.Module.Attributes;

namespace WDE.SmartScriptEditor.Settings;

[AutoRegisterToParentScope]
public class SettingsProvider : IGeneralSettingsGroup
{
    private readonly IGeneralSmartScriptSettingsProvider smartSettingsProvider;
    public string Name => "常规智能脚本";
    public IReadOnlyList<IGenericSetting> Settings { get; }
    
    private ListOptionGenericSetting viewType;
    private ListOptionGenericSetting addingBehaviour;
    private ListOptionGenericSetting actionEditViewOrder;
    private BoolGenericSetting insertActionOnEventInsert;
    private BoolGenericSetting automaticallyApplyNonRepeatableFlag;
    private FloatSliderGenericSetting defaultScale;
    
    public void Save()
    {
        smartSettingsProvider.AddingBehaviour = (AddingElementBehaviour)addingBehaviour.SelectedOption;
        smartSettingsProvider.ViewType = (SmartScriptViewType)viewType.SelectedOption;
        smartSettingsProvider.ActionEditViewOrder = (ActionEditViewOrder)actionEditViewOrder.SelectedOption;
        smartSettingsProvider.DefaultScale = defaultScale.Value;
        smartSettingsProvider.InsertActionOnEventInsert = insertActionOnEventInsert.Value;
        smartSettingsProvider.AutomaticallyApplyNonRepeatableFlag = automaticallyApplyNonRepeatableFlag.Value;
        smartSettingsProvider.Apply();
    }

    public SettingsProvider(IGeneralSmartScriptSettingsProvider smartSettingsProvider)
    {
        this.smartSettingsProvider = smartSettingsProvider;

        viewType = new ListOptionGenericSetting("脚本视图类型",
            new object[]
            {
                SmartScriptViewType.Normal, SmartScriptViewType.Compact
            },
            smartSettingsProvider.ViewType, "紧凑模式占用较少的空间，但要添加操作，您必须使用上下文菜单而不是按钮。");

        addingBehaviour = new ListOptionGenericSetting("添加行为",
            new object[]
            {
                AddingElementBehaviour.Wizard, AddingElementBehaviour.DirectlyOpenDialog, AddingElementBehaviour.JustAdd
            },
            smartSettingsProvider.AddingBehaviour, "控制添加新操作的方式。向导显示三个对话框，其中包含所有可能的操作、来源和类型，“直接打开对话框”会添加默认操作并立即打开操作编辑对话框。“仅添加”只会添加一个操作。");

        actionEditViewOrder = new ListOptionGenericSetting("操作编辑查看顺序",
            new object[]
            {
                ActionEditViewOrder.SourceActionTarget, ActionEditViewOrder.ActionSourceTarget
            },
            smartSettingsProvider.ActionEditViewOrder, "决定动作编辑对话框中的顺序以及向导添加行为模式中选择元素的顺序。");

        insertActionOnEventInsert = new BoolGenericSetting("在事件插入时插入新动作",
            smartSettingsProvider.InsertActionOnEventInsert, "启用后，插入新事件时将插入新动作。");
        
        automaticallyApplyNonRepeatableFlag = new BoolGenericSetting("自动应用不可重复标志",
            smartSettingsProvider.AutomaticallyApplyNonRepeatableFlag, "启用后，不可重复标志将根据事件的计时器自动应用并从事件中删除。");
        
        defaultScale = new FloatSliderGenericSetting("默认缩放比例",
            smartSettingsProvider.DefaultScale,
            0.5f,
            1f);
        
        Settings = new List<IGenericSetting>()
        {
            viewType,
            addingBehaviour,
            actionEditViewOrder,
            insertActionOnEventInsert,
            automaticallyApplyNonRepeatableFlag,
            defaultScale
        };
    }
}