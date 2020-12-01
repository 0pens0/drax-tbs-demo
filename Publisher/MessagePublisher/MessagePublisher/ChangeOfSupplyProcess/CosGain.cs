using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publisher.Messages.Events;

namespace MessagePublisher.ChangeOfSupplyProcess
{
    public class CosGain : BackgroundService
    {
        private readonly IServiceProvider provider;
        protected readonly IConfiguration configuration;
        static ILog log = LogManager.GetLogger<CosGain>();
        internal static IMessageSession session;

        public CosGain(IServiceProvider serviceProvider, IConfiguration config)
        {
            this.provider = serviceProvider;
            this.configuration = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;

                    session = provider.GetService<IMessageSession>();
                    Console.WriteLine("Press 1 to publish the event, any other key to exit");
                    var key = Console.ReadKey();
                    Console.WriteLine();

                    var regId = Guid.NewGuid();
                    if (key.Key != ConsoleKey.D1)
                    {

                        var registerSmartMeter = new SmartMeterRegistered()
                        {
                            RegistrationId = regId.ToString(),
                            Mpxn = "1234567890123",
                            SupplyStartDate = DateTime.Now.AddDays(5)

                        };
                        await session.Publish(registerSmartMeter).ConfigureAwait(false);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Published SmartMeterRegistered Event with Registration Id {registerSmartMeter.RegistrationId}.");
                    }
                }
                await Task.Delay(100, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                log.Warn($"{e.InnerException.ToString() ?? e.StackTrace}");
            }
        }
    }
}
