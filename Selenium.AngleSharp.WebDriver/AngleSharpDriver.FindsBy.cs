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
        #region By ID

        public IWebElement FindElementById(string id) => AngleSharpWebElement.Create(_RootContext.Active?.GetElementById(id));

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
            ?.Where(e => e.TextContent == linkText)
        ;

        public IWebElement FindElementByLinkText(string linkText) => AngleSharpWebElement.GetElement(ByLinkText(linkText));
        public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText) => AngleSharpWebElement.GetElements(ByLinkText(linkText));

        #endregion

        #region By name

        private IEnumerable<IElement> ByName(string name) => _RootContext.Active?.GetElementsByName(name);
        public IWebElement FindElementByName(string name) => AngleSharpWebElement.GetElement(ByName(name));
        public ReadOnlyCollection<IWebElement> FindElementsByName(string name) => AngleSharpWebElement.GetElements(ByName(name));

        #endregion

        #region By class name

        private IEnumerable<IElement> ByClassName(string className) => _RootContext.Active?.GetElementsByClassName(className);
        public IWebElement FindElementByClassName(string className) => AngleSharpWebElement.GetElement(ByClassName(className));
        public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className) => AngleSharpWebElement.GetElements(ByClassName(className));

        #endregion

        #region By partial link text

        private IEnumerable<IElement> ByPartialLinkText(string partialLinkText) =>
            _RootContext.Active
            ?.GetElementsByTagName("a")
            ?.Where(e => e.TextContent.Contains(partialLinkText))
        ;

        public IWebElement FindElementByPartialLinkText(string partialLinkText) => AngleSharpWebElement.GetElement(ByPartialLinkText(partialLinkText));
        public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText) => AngleSharpWebElement.GetElements(ByPartialLinkText(partialLinkText));

        #endregion

        #region By tag name

        private IEnumerable<IElement> ByTagName(string tagName) => _RootContext.Active?.GetElementsByTagName(tagName);
        public IWebElement FindElementByTagName(string tagName) => AngleSharpWebElement.GetElement(ByTagName(tagName));
        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName) => AngleSharpWebElement.GetElements(ByTagName(tagName));

        #endregion

        #region By CSS selector

        public IWebElement FindElementByCssSelector(string cssSelector) => AngleSharpWebElement.Create(_RootContext.Active?.QuerySelector(cssSelector));
        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector) => AngleSharpWebElement.GetElements(_RootContext.Active?.QuerySelectorAll(cssSelector));

        #endregion

    }
}
