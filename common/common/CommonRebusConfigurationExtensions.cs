using Rebus.Config;
using Serilog;
using System;

namespace common
{
    public static class CommonRebusConfigurationExtensions
    {
        public static RebusConfigurer ConfigureEndpoint(this RebusConfigurer configurer, Role role)
        {
            configurer
                .Logging(l => l.Serilog(Log.Logger))
                .Transport(t =>
                {
                    if (role == Role.Client)
                    {
                        t.UseRabbitMqAsOneWayClient(Config.AppSetting("Rabbitmq")); //"amqp://quest:quest@192.168.99.100:5672"
                    }
                    else
                    {
                        t.UseRabbitMq(Config.AppSetting("Rabbitmq"), Config.AppSetting("QueueName"));
                    }
                })
                .Subscriptions(s =>
                {
                    var subscriptionsTableName = Config.AppSetting("SubscriptionsTableName");
                    s.StoreInSqlServer("RebusDatabase", subscriptionsTableName, isCentralized: true);
                })
                .Sagas(s =>
                {
                    if (role != Role.SagaHost) return;

                    var dataTableName = Config.AppSetting("SagaDataTableName");
                    var indexTableName = Config.AppSetting("SagaIndexTableName");

                    // store sagas in SQL Server to make them persistent and survive restarts
                    s.StoreInSqlServer("RebusDatabase", dataTableName, indexTableName);
                })
                .Timeouts(t =>
                {
                    if (role == Role.Client) return;

                    var tableName = Config.AppSetting("TimeoutsTableName");

                    // store timeouts in SQL Server to make them persistent and survive restarts
                    t.StoreInSqlServer("RebusDatabase", tableName);
                });

            return configurer;
        }
    }
}
