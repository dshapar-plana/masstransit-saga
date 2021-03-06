﻿using System;
using System.Data;
using System.IO;
using Dapper;
using GreenPipes;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

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
                    var connectionString = hostContext.Configuration.GetValue<string>("ConnectionString");
                    //for Dapper
                    services.AddScoped<IDbConnection>(db =>
                        new SqlConnection(connectionString));

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

                    var dbCreationCommand = File.ReadAllText("CreateDB.sql");
                    var tablesCreationCommand = File.ReadAllText("TransferSagaStates.sql");
                    // ReSharper disable once UseObjectOrCollectionInitializer
                    var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
                    var dbName = sqlConnectionStringBuilder.InitialCatalog;
                    sqlConnectionStringBuilder.InitialCatalog = "master";


                    Policy retryPolicy = Policy.Handle<SqlException>().WaitAndRetry(
                        retryCount: 7,
                        sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(1000));

                    retryPolicy.Execute(() =>
                    {
                        using var sqlConnection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
                        sqlConnection.Open();
                        sqlConnection.Execute(dbCreationCommand);
                    });

                    retryPolicy.Execute(() =>
                    {
                        sqlConnectionStringBuilder.InitialCatalog = dbName;
                        using var sqlConnection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
                        sqlConnection.Open();
                        sqlConnection.Execute(tablesCreationCommand);
 
                    });
                    
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
                                builder.UseSqlServer(connectionString
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
                                    var partition = e.CreatePartitioner(5);

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

                    services.AddMassTransitHostedService();

                });
    }
}
