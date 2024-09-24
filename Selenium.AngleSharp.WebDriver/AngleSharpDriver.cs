using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.XPath;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Selenium.AngleSharp.WebDriver {
    public sealed partial class AngleSharpDriver(IConfiguration cfg = null) : IWebDriver, IFindsElement {
        private readonly IBrowsingContext _RootContext = BrowsingContext.New(cfg);

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
            ? new ReadOnlyCollection<string>([name])
            : ReadOnlyCollection<string>.Empty
        ;

        public void Quit() => _RootContext.Current?.Close();

        public void Close() {
            _RootContext.Current?.Close();
        }

        // FindElement and FindElements are already implemented by the By class by assuming
        // an object is provided implementing various IFind* interfaces.
        public IWebElement FindElement(By by) => by.FindElement(this);

        public ReadOnlyCollection<IWebElement> FindElements(By by) => by.FindElements(this);

        public INavigation Navigate() => new AngleSharpNavigation(_RootContext);

        // The following WebDriver interfaces have not yet been defined,
        // thus, the following methods cannot yet be implemented.
        public IOptions Manage() => throw new NotImplementedException();


        public ITargetLocator SwitchTo() => throw new NotImplementedException();

        public IWebElement FindElement(string mechanism, string value) => FindElements(mechanism, value) is [var element, ..] ? element : throw new NoSuchElementException();

        public ReadOnlyCollection<IWebElement> FindElements(string mechanism, string value) =>
            mechanism switch {
                "css selector" => AngleSharpWebElement.GetElements(_RootContext.Active?.QuerySelectorAll(value)),
                "link text" => AngleSharpWebElement.GetElements(_RootContext.Active?.GetElementsByTagName("a")?.Where(e => e.GetInnerText() == value)),
                "partial link text" => AngleSharpWebElement.GetElements(_RootContext.Active?.GetElementsByTagName("a")?.Where(e => e.GetInnerText().Contains(value))),
                "tag name" => AngleSharpWebElement.GetElements(_RootContext.Active?.GetElementsByTagName(value)),
                "xpath" => AngleSharpWebElement.GetElements(_RootContext.Active?.DocumentElement.SelectNodes(value)),
                _ => throw new NotImplementedException()
            }
        ;

        public void Dispose() { if (_RootContext is IDisposable disposable) disposable.Dispose(); }
    }
}

