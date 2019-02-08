using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AngleSharp.Dom;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;

namespace Selenium.AngleSharp.WebDriver {
    partial class AngleSharpDriver : IFindsById, IFindsByLinkText, IFindsByName, IFindsByClassName, IFindsByPartialLinkText, IFindsByTagName, IFindsByCssSelector {
        private static IWebElement WrapElement(IElement element) => throw new NotImplementedException();

        private static IWebElement GetElement(IEnumerable<IElement> elements) => WrapElement(elements?.FirstOrDefault());

        private static ReadOnlyCollection<IWebElement> GetElements(IEnumerable<IElement> elements) =>
            new ReadOnlyCollectionBuilder<IWebElement>(elements?.Select(WrapElement)?? new IWebElement[0])
            .ToReadOnlyCollection()
        ;

        #region By ID

        public IWebElement FindElementById(string id) => WrapElement(_RootContext.Active?.GetElementById(id));

        public ReadOnlyCollection<IWebElement> FindElementsById(string id) =>
            new ReadOnlyCollectionBuilder<IWebElement>(1) {
                FindElementById(id)
            }
            .ToReadOnlyCollection()
        ;

        #endregion

        #region By link text

        private IEnumerable<IElement> ByLinkText(string linkText) =>
            _RootContext.Active
            ?.GetElementsByTagName("a")
            ?.Where(e => e.InnerHtml == linkText)
        ;

        public IWebElement FindElementByLinkText(string linkText) => GetElement(ByLinkText(linkText));
        public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText) => GetElements(ByLinkText(linkText);

        #endregion

        #region By name

        private IEnumerable<IElement> ByName(string name) => _RootContext.Active?.GetElementsByName(name);
        public IWebElement FindElementByName(string name) => GetElement(ByName(name));
        public ReadOnlyCollection<IWebElement> FindElementsByName(string name) => GetElements(ByName(name));

        #endregion

        #region By class name

        private IEnumerable<IElement> ByClassName(string className) => _RootContext.Active?.GetElementsByClassName(className);
        public IWebElement FindElementByClassName(string className) => GetElement(ByClassName(className));
        public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className) => GetElements(ByClassName(className));

        #endregion

        #region By partial link text

        private IEnumerable<IElement> ByPartialLinkText(string partialLinkText) =>
            _RootContext.Active
            ?.GetElementsByTagName("a")
            ?.Where(e => e.InnerHtml.Contains(partialLinkText))
        ;

        public IWebElement FindElementByPartialLinkText(string partialLinkText) => GetElement(ByPartialLinkText(partialLinkText));
        public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText) => GetElements(ByPartialLinkText(partialLinkText));

        #endregion

        #region By tag name

        private IEnumerable<IElement> ByTagName(string tagName) => _RootContext.Active?.GetElementsByTagName(tagName);
        public IWebElement FindElementByTagName(string tagName) => GetElement(ByTagName(tagName));
        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName) => GetElements(ByTagName(tagName));

        #endregion

        #region By CSS selector

        public IWebElement FindElementByCssSelector(string cssSelector) => WrapElement(_RootContext.Active?.QuerySelector(cssSelector));
        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector) => GetElements(_RootContext.Active?.QuerySelectorAll(cssSelector));

        #endregion

    }
}
