using System;
using Microsoft.EntityFrameworkCore;
using NServiceBus.Persistence;


namespace MessagePublisher.Data
{
    public class EntityFrameworkHelper<T> where T : DbContext
    {
        Func<SynchronizedStorageSession, T> contextFactory;
        T context;

        public EntityFrameworkHelper(Func<SynchronizedStorageSession, T> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public T GetDataContext(SynchronizedStorageSession storageSession)
        {
            return context ??= contextFactory(storageSession);
        }
    }
}
