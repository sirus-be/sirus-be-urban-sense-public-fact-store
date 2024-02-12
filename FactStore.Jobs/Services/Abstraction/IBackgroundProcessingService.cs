using System;
using System.Linq.Expressions;

namespace FactStore.Jobs.Services
{
    public interface IBackgroundProcessingService
    {
        void RegisterRecurrentJob<T>(Expression<Action<T>> methodCall, int minuteInterval);
        void RegisterEnqueuedJob<T>(Expression<Action<T>> methodCall);
        void RegisterRecurrentJobDayly<T>(Expression<Action<T>> methodCall);
    }
}
