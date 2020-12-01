using System;
using NServiceBus;
namespace Publisher.Messages.Events
{
    public class SmartMeterRegistered : IEvent
    {
        public string RegistrationId { get; set; }
        public string Mpxn { get; set; }
        public DateTime SupplyStartDate { get; set; }
    }
}
