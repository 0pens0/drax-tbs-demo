using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Publisher.Messages.Events;

namespace MessageSubscriber.Handlers
{
    public class SmartMeterRegisteredHandler : IHandleMessages<SmartMeterRegistered>
    {
        static ILog log = LogManager.GetLogger<SmartMeterRegistered>();
        public Task Handle(SmartMeterRegistered message, IMessageHandlerContext context)
        {
            // Call Solace API here

            Console.ForegroundColor = ConsoleColor.Green;
            log.Info($"Subscriber has received SmartMeterRegistered event with RegistrationId {message.RegistrationId}.");

            return Task.CompletedTask;
        }
    }
}
