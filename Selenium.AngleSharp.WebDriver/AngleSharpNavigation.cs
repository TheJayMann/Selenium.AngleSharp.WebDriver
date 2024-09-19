using AngleSharp;
using OpenQA.Selenium;
using System;

namespace Selenium.AngleSharp.WebDriver {
    public class AngleSharpNavigation : INavigation {

        private readonly IBrowsingContext _Context;

        public AngleSharpNavigation(IBrowsingContext context) {
            _Context = context;
        }

        // SessionHistory can be null
        public void Back() => _Context.SessionHistory?.Back();

        public void Forward() => _Context.SessionHistory.Forward();

        public void GoToUrl(string url) => _Context.OpenAsync(url).Wait();

        public void GoToUrl(Uri url) => GoToUrl(url?.ToString());

        // AngleSharp does not appear to support refresh.  Leave as a no-op.
        public void Refresh() { }
    }
}
