using NModbus;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;
using System.IO;
using IndigoVergeTask.DB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IndigoVergeTask
{
    class Program
    {
        private static IServiceProvider serviceProvider;

        public static AppConfig _config { get; set; }
        static async Task Main(string[] args)
        {
            ConfigureServices();

            var db = new SensorDataDB();
            await db.Database.EnsureCreatedAsync();

            var processors = serviceProvider.GetServices<ISensorDataProcessor>();

            foreach (var item in processors)
            {
                RepeatingTask(item.ProcessSensorRecord, item.IntervalMs, CancellationToken.None);

            }
           
            Console.ReadLine();
        }

        private static void ConfigureServices()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var services = new ServiceCollection();


            var appConfig = new AppConfig();

            config.GetSection(AppConfig.SectionName).Bind(appConfig);
            services.AddSingleton<IAppConfig>(appConfig);

            services.AddSingleton<IModbusMaster>((IServiceProvider) =>
            {
                TcpClient client = new TcpClient(appConfig.IpAddress, appConfig.Port);
                var factory = new ModbusFactory();
                return factory.CreateMaster(client);
            });

            services.AddTransient<SensorDataDB>();

            services.Scan(sc =>
                  sc.FromAssemblyOf<ISensorDataReader>()
                  .AddClasses(classes => classes.AssignableTo<ISensorDataReader>())
                  // We then specify what type we want to register these classes as.
                  // In this case, we want to register the types as all of its implemented interfaces.
                  // So if a type implements 3 interfaces; A, B, C, we'd end up with three separate registrations.
                  .AsImplementedInterfaces()
                  // And lastly, we specify the lifetime of these registrations.
                  .WithTransientLifetime()
              );


            // autoregister tax calculators
            // this assumes all tax calculators live under the same assembly
            services.Scan(sc =>
                sc.FromAssemblyOf<ISensorDataProcessor>()
                .AddClasses(classes => classes.AssignableTo<ISensorDataProcessor>())
                // We then specify what type we want to register these classes as.
                // In this case, we want to register the types as all of its implemented interfaces.
                // So if a type implements 3 interfaces; A, B, C, we'd end up with three separate registrations.
                .AsImplementedInterfaces()
                // And lastly, we specify the lifetime of these registrations.
                .WithTransientLifetime()
            );


            serviceProvider = services.BuildServiceProvider();
        }

        static void RepeatingTask(Func<Task> action, int miliseconds, CancellationToken token)
        {
            if (action == null)
                return;
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await action();
                    await Task.Delay(TimeSpan.FromMilliseconds(miliseconds), token);
                }
            }, token);
        }

    }
}
