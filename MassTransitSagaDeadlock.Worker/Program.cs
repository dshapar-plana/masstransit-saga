using System;
using System.Data;
using Dapper;
using GreenPipes;
using MassTransit;
using MassTransit.DapperIntegration;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.Saga;
using MassTransitSagaDeadlock.Worker.Auxiliary;
using MassTransitSagaDeadlock.Worker.Commands;
using MassTransitSagaDeadlock.Worker.Consumers;
using MassTransitSagaDeadlock.Worker.Events;
using MassTransitSagaDeadlock.Worker.Saga;
using MassTransitSagaDeadlock.Worker.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MassTransitSagaDeadlock.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransitHostedService();

                    //for Dapper
                    services.AddScoped<IDbConnection>(db =>
                        new SqlConnection(hostContext.Configuration.GetValue<string>("ConnectionString")));

                    services.Configure<MessagingSettings>(options =>
                        hostContext.Configuration.GetSection("MessagingSettings").Bind(options));

                    //DO NOT REMOVE
                    //services.AddScoped(typeof(SagaRepository<TransferSagaState>),
                    //    provider => EntityFrameworkSagaRepository<TransferSagaState>.CreatePessimistic(
                    //        () => provider.GetService<TransferSagaStateDbContext>()));

                    //services.AddSingleton(typeof(SagaRepository<TransferSagaState>),
                    //    provider => DapperSagaRepository<TransferSagaState>.Create(
                    //        hostContext.Configuration.GetValue<string>("ConnectionString")));

                    SqlMapper.AddTypeHandler(new MsSqlErrorsCollectionTypeHandler());
                    SqlMapper.AddTypeHandler(new MsSqlMetadataCollectionTypeHandler());

                    services.AddMassTransit(_ =>
                    {
                        _.AddConsumer<DepositFundsConsumer>();
                        _.AddConsumer<WithdrawFundsConsumer>();

                        _.AddSagaStateMachine<TransferSaga, TransferSagaState>()
                        // .DapperRepository(hostContext.Configuration.GetValue<string>("ConnectionString"));

                        //DO NOT REMOVE
                        .EntityFrameworkRepository(r =>
                        {
                            r.ConcurrencyMode =
                                ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion

                            r.AddDbContext<DbContext, TransferSagaStateDbContext>((provider, builder) =>
                            {
                                builder.UseSqlServer(hostContext.Configuration.GetValue<string>("ConnectionString")
                                    //,optionsBuilder => optionsBuilder.EnableRetryOnFailure()
                                    );
                            });
                        });

                        _.UsingRabbitMq((context, config) =>
                        {
                            var appSettings = context.GetService<IOptions<MessagingSettings>>().Value;
                            config.Host(appSettings.RabbitMq.Hostname, (ushort)appSettings.RabbitMq.Port, "/",
                                hostConfig =>
                                {
                                    hostConfig.Username(appSettings.RabbitMq.Username);
                                    hostConfig.Password(appSettings.RabbitMq.Password);
                                });

                            config.ReceiveEndpoint("TransferFunds", e =>
                            {
                                e.UseMessageRetry(r => r.Intervals(1000, 1000, 5000));
                                e.UseInMemoryOutbox();

                                e.ConfigureSaga<TransferSagaState>(context, s =>
                                {
                                    var partition = e.CreatePartitioner(1);

                                    s.Message<TransferFundsCommand>(x => x.UsePartitioner(partition, m => m.Message.TransferFundsId));
                                    s.Message<WithdrawFundsCompletedEvent>(x => x.UsePartitioner(partition, m => m.Message.TransferFundsId));
                                    s.Message<DepositFundsCompletedEvent>(x => x.UsePartitioner(partition, m => m.Message.TransferFundsId));
                                });
                            });

                            config.ReceiveEndpoint("DepositFunds", e =>
                            {
                                e.ConfigureConsumer<DepositFundsConsumer>(context);
                            });

                            config.ReceiveEndpoint("WithdrawFunds", e =>
                            {
                                e.ConfigureConsumer<WithdrawFundsConsumer>(context);
                            });
                        });
                    });
                });
    }
}
