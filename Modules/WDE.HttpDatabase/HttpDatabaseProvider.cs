using Prism.Events;
using Prism.Ioc;
using WDE.Common.Services;
using WDE.Common.Services.MessageBox;
using WDE.Module.Attributes;
using WDE.MySqlDatabaseCommon.Database.World;

namespace WDE.HttpDatabase;

[AutoRegister]
[SingleInstance]
public class HttpDatabaseProvider : WorldDatabaseDecorator
{
    private HttpDatabaseProviderImpl? db;
    public HttpDatabaseProvider(HttpDatabaseProviderImpl databaseImplementation,
        NullWorldDatabaseProvider nullWorldDatabaseProvider,
        IMessageBoxService messageBoxService,
        ILoadingEventAggregator loadingEventAggregator,
        IEventAggregator eventAggregator,
        IContainerProvider containerProvider) : base(nullWorldDatabaseProvider)
    {
        try
        {
            var cachedDatabase = containerProvider.Resolve<CachedDatabaseProvider>((typeof(IAsyncDatabaseProvider), databaseImplementation));
            cachedDatabase.TryConnect();
            impl = cachedDatabase;
            db = databaseImplementation;
        }
        catch (Exception e)
        {
            impl = nullWorldDatabaseProvider;
            messageBoxService.ShowDialog(new MessageBoxFactory<bool>().SetTitle("数据库错误")
                .SetIcon(MessageBoxIcon.Error)
                .SetMainInstruction("不能连接到数据库！")
                .SetContent(e.Message)
                .WithOkButton(true)
                .Build());
        }
    }
}