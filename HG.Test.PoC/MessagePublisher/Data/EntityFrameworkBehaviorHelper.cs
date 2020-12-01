using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NServiceBus.Persistence;
using NServiceBus.Pipeline;

namespace MessagePublisher.Data
{
    public class EntityFrameworkBehaviorHelper<T>
        : Behavior<IIncomingLogicalMessageContext> where T : DbContext
    {
        Func<SynchronizedStorageSession, T> contextFactory;

        public EntityFrameworkBehaviorHelper(Func<SynchronizedStorageSession, T> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public override async Task Invoke(
            IIncomingLogicalMessageContext context, Func<Task> next)
        {
            var ef = new EntityFrameworkHelper<T>(contextFactory);
            context.Extensions.Set(ef);
            await next().ConfigureAwait(false);
            context.Extensions.Remove<EntityFrameworkHelper<T>>();
        }
    }
}
