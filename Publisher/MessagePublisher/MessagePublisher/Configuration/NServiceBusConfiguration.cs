﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NServiceBus;
using Publisher.Messages.Events;
using System.Data.SqlClient;

namespace MessagePublisher.Configuration
{
    public class NServiceBusConfiguration
    {
        protected readonly string EndpointName;
        protected readonly EndpointConfiguration EndpointConfiguration;
        protected readonly IConfiguration _configuration;
        public NServiceBusConfiguration(string endpointName, IConfiguration configuration)
        {
            EndpointName = endpointName;
            _configuration = configuration;
            EndpointConfiguration = new EndpointConfiguration(endpointName);
        }
        public EndpointConfiguration Build()
        {
            //string nsbLicense = $"{ AppDomain.CurrentDomain.BaseDirectory }/license/license.xml";
            string nsbLicense = $"C://NServiceBus//license.xml";
            string messagePublisherConnection = _configuration.GetConnectionString("NServiceBus_Transport");

            var endpointConfiguration = new EndpointConfiguration(EndpointName);
            endpointConfiguration.LicensePath(nsbLicense);
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            endpointConfiguration.AddDeserializer<XmlSerializer>();
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            var serialization = endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            serialization.Settings(settings);

            string nServiceBusConnection = _configuration.GetConnectionString("NServiceBus_Transport");
            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.Transactions(TransportTransactionMode.TransactionScope);
            transport.ConnectionString(nServiceBusConnection);
            transport.SubscriptionSettings().DisableSubscriptionCache();

            var subscriptions = transport.SubscriptionSettings();
            subscriptions.SubscriptionTableName(
                tableName: "SubscriptionRouting",
                schemaName: "dbo",
                catalogName: "Message_NServiceBus");

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.ConnectionBuilder(() => new SqlConnection(messagePublisherConnection));

            var dialect = persistence.SqlDialect<SqlDialect.MsSqlServer>();

            transport.Routing().RouteToEndpoint(typeof(SmartMeterRegistered), "MessagePublisher");

            return endpointConfiguration;
        }

    }
}
