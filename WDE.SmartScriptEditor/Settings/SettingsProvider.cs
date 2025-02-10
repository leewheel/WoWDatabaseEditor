using System.Collections.Generic;
using WDE.Common.Settings;
using WDE.Module.Attributes;

namespace WDE.SmartScriptEditor.Settings;

[AutoRegisterToParentScope]
public class SettingsProvider : IGeneralSettingsGroup
{
    private readonly IGeneralSmartScriptSettingsProvider smartSettingsProvider;
    public string Name => "�������ܽű�";
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

        viewType = new ListOptionGenericSetting("�ű���ͼ����",
            new object[]
            {
                SmartScriptViewType.Normal, SmartScriptViewType.Compact
            },
            smartSettingsProvider.ViewType, "����ģʽռ�ý��ٵĿռ䣬��Ҫ��Ӳ�����������ʹ�������Ĳ˵������ǰ�ť��");

        addingBehaviour = new ListOptionGenericSetting("�����Ϊ",
            new object[]
            {
                AddingElementBehaviour.Wizard, AddingElementBehaviour.DirectlyOpenDialog, AddingElementBehaviour.JustAdd
            },
            smartSettingsProvider.AddingBehaviour, "��������²����ķ�ʽ������ʾ�����Ի������а������п��ܵĲ�������Դ�����ͣ���ֱ�Ӵ򿪶Ի��򡱻����Ĭ�ϲ����������򿪲����༭�Ի��򡣡�����ӡ�ֻ�����һ��������");

        actionEditViewOrder = new ListOptionGenericSetting("�����༭�鿴˳��",
            new object[]
            {
                ActionEditViewOrder.SourceActionTarget, ActionEditViewOrder.ActionSourceTarget
            },
            smartSettingsProvider.ActionEditViewOrder, "���������༭�Ի����е�˳���Լ��������Ϊģʽ��ѡ��Ԫ�ص�˳��");

        insertActionOnEventInsert = new BoolGenericSetting("���¼�����ʱ�����¶���",
            smartSettingsProvider.InsertActionOnEventInsert, "���ú󣬲������¼�ʱ�������¶�����");
        
        automaticallyApplyNonRepeatableFlag = new BoolGenericSetting("�Զ�Ӧ�ò����ظ���־",
            smartSettingsProvider.AutomaticallyApplyNonRepeatableFlag, "���ú󣬲����ظ���־�������¼��ļ�ʱ���Զ�Ӧ�ò����¼���ɾ����");
        
        defaultScale = new FloatSliderGenericSetting("Ĭ�����ű���",
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