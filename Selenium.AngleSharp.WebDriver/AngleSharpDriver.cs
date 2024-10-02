using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.XPath;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Selenium.AngleSharp.WebDriver {
    public sealed partial class AngleSharpDriver(IConfiguration cfg = null) : IWebDriver, IJavaScriptExecutor, IFindsElement {
        private readonly IBrowsingContext _RootContext = BrowsingContext.New(cfg);
        private IBrowsingContext _ActiveContext = null;

        private IBrowsingContext _CurrentContext => _ActiveContext ?? _RootContext;

        internal void SetActiveContext(IBrowsingContext context) {
            _ActiveContext = context;
        }

        internal void SetParentActive() {
            _ActiveContext = _ActiveContext?.Parent;
            if (ReferenceEquals(_RootContext, _ActiveContext)) _ActiveContext = null;
        }

        public string Url {
            get => _CurrentContext.Active?.Url;
            // URL set is defined to navigate to the page and
            // wait until the page has been loaded.
            set => _CurrentContext.OpenAsync(value).Wait();
        }

        public string Title => _RootContext.Active?.Title;

        public string PageSource => _CurrentContext.Active?.Source?.Text;

        public string CurrentWindowHandle => _CurrentContext.Current?.Name;

        // Existing BrowserContext implementation does not allow 
        // listing children, and also does not differentiate
        // between top level windows and embedded documents.
        // Thus, currently, only the current window name can
        // be retrieved
        public ReadOnlyCollection<string> WindowHandles =>
            _RootContext.Current?.Name is string name
            ? new ReadOnlyCollection<string>([name])
            : ReadOnlyCollection<string>.Empty
        ;

        public void Quit() => _CurrentContext.Current?.Close();

        public void Close() {
            _CurrentContext.Current?.Close();
        }

        // FindElement and FindElements are already implemented by the By class by assuming
        // an object is provided implementing various IFind* interfaces.
        public IWebElement FindElement(By by) => by.FindElement(this);

        public ReadOnlyCollection<IWebElement> FindElements(By by) => by.FindElements(this);

        public INavigation Navigate() => new AngleSharpNavigation(_CurrentContext);

        // The following WebDriver interfaces have not yet been defined,
        // thus, the following methods cannot yet be implemented.
        public IOptions Manage() => throw new NotImplementedException();


        public ITargetLocator SwitchTo() => new AngleSharpTargetLocator(this);

        public IWebElement FindElement(string mechanism, string value) => FindElements(mechanism, value) is [var element, ..] ? element : throw new NoSuchElementException();

        public ReadOnlyCollection<IWebElement> FindElements(string mechanism, string value) =>
            mechanism switch {
                "css selector" => AngleSharpWebElement.GetElements(_CurrentContext.Active?.QuerySelectorAll(value)),
                "link text" => AngleSharpWebElement.GetElements(_CurrentContext.Active?.GetElementsByTagName("a")?.Where(e => e.GetInnerText() == value)),
                "partial link text" => AngleSharpWebElement.GetElements(_CurrentContext.Active?.GetElementsByTagName("a")?.Where(e => e.GetInnerText().Contains(value))),
                "tag name" => AngleSharpWebElement.GetElements(_CurrentContext.Active?.GetElementsByTagName(value)),
                "xpath" => AngleSharpWebElement.GetElements(_CurrentContext.Active?.DocumentElement.SelectNodes(value)),
                _ => throw new NotImplementedException()
            }
        ;

        public void Dispose() { if (_CurrentContext is IDisposable disposable) disposable.Dispose(); }

        public object ExecuteScript(string script, params object[] args) {
            throw new NotImplementedException();
        }

        public object ExecuteScript(PinnedScript script, params object[] args) {
            throw new NotImplementedException();
        }

        public object ExecuteAsyncScript(string script, params object[] args) {
            throw new NotImplementedException();
        }
    }
}

