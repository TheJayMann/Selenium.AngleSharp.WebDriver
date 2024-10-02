using System;
using System.Threading;
using System.Threading.Tasks;

namespace Selenium.AngleSharp.WebDriver.Extensions;
internal static class AsyncHelper {
    private static readonly TaskFactory _myTaskFactory = new(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

    public static TResult RunSync<TResult>(Func<Task<TResult>> func) => 
        _myTaskFactory.StartNew(func)
        .Unwrap()
        .GetAwaiter()
        .GetResult()
    ;

    public static void RunSync(Func<Task> func) =>
        _myTaskFactory.StartNew(func)
        .Unwrap()
        .GetAwaiter()
        .GetResult()
    ;
}
