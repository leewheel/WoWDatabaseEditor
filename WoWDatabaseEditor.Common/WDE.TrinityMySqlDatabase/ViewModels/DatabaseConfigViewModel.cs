using WDE.Module.Attributes;
using WDE.MySqlDatabaseCommon.Providers;
using WDE.MySqlDatabaseCommon.ViewModels;

namespace WDE.TrinityMySqlDatabase.ViewModels
{
    [AutoRegister]
    public class DatabaseConfigViewModel : BaseDatabaseConfigViewModel
    {
        public DatabaseConfigViewModel(IWorldDatabaseSettingsProvider worldSettingsProvider,
            IAuthDatabaseSettingsProvider authDatabaseSettingsProvider,
            IHotfixDatabaseSettingsProvider hotfixDatabaseSettingsProvider) : base(worldSettingsProvider, authDatabaseSettingsProvider, hotfixDatabaseSettingsProvider)
        {
        }

        public override string Name => "核心数据库";
        public override string ShortDescription => "要获得所有编辑器功能，您必须连接到任何与本编辑器兼容的数据库。";
    }
}