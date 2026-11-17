using System;
using System.Threading.Tasks;

namespace StackedDeck.WebAPI.Template.Integration.Tests.Utils;

/// <summary>
/// Polling helpers for asynchronous integration test assertions.
/// </summary>
internal static class AsyncPolling
{
    /// <summary>
    /// Polls <paramref name="condition"/> every second until it returns <c>true</c> or <paramref name="timeout"/> elapses.
    /// A final evaluation is attempted at the deadline boundary before throwing.
    /// </summary>
    /// <param name="condition">The async predicate to evaluate.</param>
    /// <param name="timeout">The maximum time to wait before throwing <see cref="TimeoutException"/>.</param>
    /// <exception cref="TimeoutException">Thrown when the condition is not satisfied within the given timeout.</exception>
    public static async Task WaitForAsync(Func<Task<bool>> condition, TimeSpan timeout)
    {
        var pollInterval = TimeSpan.FromSeconds(1);
        var deadline = DateTime.UtcNow + timeout;
        Exception lastException = null;

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                if (await condition())
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                lastException = ex;
            }

            await Task.Delay(pollInterval, TestContext.Current.CancellationToken);
        }

        try
        {
            if (!await condition())
            {
                throw new TimeoutException($"Condition did not become true within {timeout}.", lastException);
            }
        }
        catch (Exception ex) when (ex is not TimeoutException)
        {
            throw new TimeoutException($"Condition did not become true within {timeout}.", lastException ?? ex);
        }
    }
}
