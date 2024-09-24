using AngleSharp;
using OpenQA.Selenium;
using System;
using System.Threading.Tasks;

namespace Selenium.AngleSharp.WebDriver {
    public class AngleSharpNavigation(IBrowsingContext context) : INavigation {

        // SessionHistory can be null
        public void Back() => context.SessionHistory?.Back();

        public Task BackAsync() {
            Back();
            return Task.CompletedTask;
        }

        public void Forward() => context.SessionHistory.Forward();

        public Task ForwardAsync() {
            Forward();
            return Task.CompletedTask;
        }

        public void GoToUrl(string url) => context.OpenAsync(url).Wait();

        public void GoToUrl(Uri url) => GoToUrl(url?.ToString());

        public Task GoToUrlAsync(string url) {
            GoToUrl(url);
            return Task.CompletedTask;
        }

        public Task GoToUrlAsync(Uri url) {
            GoToUrl(url);
            return Task.CompletedTask;
        }

        // AngleSharp does not appear to support refresh.  Leave as a no-op.
        public void Refresh() { }

        public Task RefreshAsync() {
            return Task.CompletedTask;
        }
    }
}
