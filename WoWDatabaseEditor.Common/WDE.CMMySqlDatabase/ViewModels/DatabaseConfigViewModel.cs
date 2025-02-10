using WDE.Module.Attributes;
using WDE.MySqlDatabaseCommon.Providers;
using WDE.MySqlDatabaseCommon.ViewModels;

namespace WDE.CMMySqlDatabase.ViewModels
{
    [AutoRegister(Platforms.Desktop)]
    public class CMDatabaseConfigViewModel : BaseDatabaseConfigViewModel
    {
        public CMDatabaseConfigViewModel(IWorldDatabaseSettingsProvider worldSettingsProvider,
            IAuthDatabaseSettingsProvider authDatabaseSettingsProvider,
            IHotfixDatabaseSettingsProvider hotfixDatabaseSettingsProvider) : base(worldSettingsProvider, authDatabaseSettingsProvider, hotfixDatabaseSettingsProvider)
        {
        }

        public override string Name => "CMaNGOS 数据库";
        public override string ShortDescription => "要获得所有编辑器功能，您必须连接到任何与 CMaNGOS 兼容的数据库。";
    }
}