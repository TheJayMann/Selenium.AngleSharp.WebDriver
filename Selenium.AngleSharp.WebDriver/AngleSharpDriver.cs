using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using AngleSharp;

namespace Selenium.AngleSharp.WebDriver {
    public class AngleSharpDriver : IWebDriver {

        IBrowsingContext _RootContext;

        public AngleSharpDriver(IConfiguration cfg = null) {
            _RootContext = BrowsingContext.New(cfg);
        }

        public string Url {
            get => _RootContext.Active?.Url;
            // URL set is defined to navigate to the page and
            // wait until the page has been loaded.
            set => _RootContext.OpenAsync(value).Wait();
        }

        public string Title => _RootContext.Active?.Title;

        public string PageSource => _RootContext.Active?.Source?.Text;

        public string CurrentWindowHandle => _RootContext.Current?.Name;

        // Existing BrowserContext implementation does not allow 
        // listing children, and also does not differentiate
        // between top level windows and embedded documents.
        // Thus, currently, only the current window name can
        // be retrieved
        public ReadOnlyCollection<string> WindowHandles => 
            _RootContext.Current?.Name is string name 
            ? new ReadOnlyCollection<string>(new [] { name })
            : new ReadOnlyCollection<string>(new string[0])
        ;

        public void Quit() => _RootContext.Current?.Close();

        public void Close() {
            _RootContext.Current?.Close();
        }

        // The following WebDriver interfaces have not yet been defined,
        // thus, the following methods cannot yet be implemented.
        public IWebElement FindElement(By by) {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by) {
            throw new NotImplementedException();
        }

        public IOptions Manage() {
            throw new NotImplementedException();
        }

        public INavigation Navigate() {
            throw new NotImplementedException();
        }


        public ITargetLocator SwitchTo() {
            throw new NotImplementedException();
        }

        #region IDisposable Support

        public void Dispose() { if (_RootContext is IDisposable disposable) disposable.Dispose(); }

        #endregion
    }
}

