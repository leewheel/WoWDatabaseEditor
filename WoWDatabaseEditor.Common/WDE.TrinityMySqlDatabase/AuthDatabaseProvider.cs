using System;
using System.Data;
using WDE.Common.Services.MessageBox;
using WDE.Module.Attributes;
using WDE.MySqlDatabaseCommon.Database.Auth;
using WDE.MySqlDatabaseCommon.Providers;
using WDE.TrinityMySqlDatabase.Database;
using WDE.TrinityMySqlDatabase.Models;

namespace WDE.TrinityMySqlDatabase
{
    [AutoRegister]
    [SingleInstance]
    public class AuthDatabaseProvider : AuthDatabaseDecorator
    {
        public AuthDatabaseProvider(DatabaseResolver databaseResolver,
            NullAuthDatabaseProvider nullAuthDatabaseProvider,
            IAuthDatabaseSettingsProvider settingsProvider,
            IMessageBoxService messageBoxService
        ) : base(nullAuthDatabaseProvider)
        {
            if (settingsProvider.Settings.IsEmpty)
                return;

            try
            {
                using var db = new TrinityAuthDatabase();
                if (db.Connection.State != ConnectionState.Open)
                {
                    db.Connection.Open();
                    db.Connection.Close();   
                }
                impl = databaseResolver.ResolveAuth();
            }
            catch (Exception e)
            {
                impl = nullAuthDatabaseProvider;
                messageBoxService.ShowDialog(new MessageBoxFactory<bool>().SetTitle("���ݿ����")
                    .SetIcon(MessageBoxIcon.Error)
                    .SetMainInstruction("�������ӵ�auth���ݿ�")
                    .SetContent(e.Message)
                    .WithOkButton(true)
                    .Build());
            }
        }
    }
}