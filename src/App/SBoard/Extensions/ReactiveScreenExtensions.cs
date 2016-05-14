using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using ReactiveUI;

namespace SBoard.Extensions
{
    public static class ReactiveScreenExtensions
    {
        public static ObservableAsPropertyHelper<TRet> ToLoadedProperty<TObj, TRet>(this IObservable<TRet> This, TObj source, Expression<Func<TObj, TRet>> property, out ObservableAsPropertyHelper<TRet> result, TRet initialValue = default(TRet), IScheduler scheduler = null) where TObj : ReactiveObject
        {
            var res = This.ToProperty(source, property, out result, initialValue, scheduler);
            source.WhenAnyValue(property)
                .Subscribe(_ => { });

            return res;
        }
    }
}