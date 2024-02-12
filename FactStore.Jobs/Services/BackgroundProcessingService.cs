using Hangfire;
using System;
using System.Linq.Expressions;

namespace FactStore.Jobs.Services
{
    public class BackgroundProcessingService : IBackgroundProcessingService
    {
        public void RegisterRecurrentJob<T>(Expression<Action<T>> methodCall, int minuteInterval)
        {
            RecurringJob.AddOrUpdate(methodCall, Cron.MinuteInterval(minuteInterval));
        }

        public void RegisterEnqueuedJob<T>(Expression<Action<T>> methodCall)
        {
            BackgroundJob.Enqueue(methodCall);
        }

        public void RegisterRecurrentJobDayly<T>(Expression<Action<T>> methodCall)
        {
            RecurringJob.AddOrUpdate(methodCall, Cron.Daily);
        }
    }
}
