using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.XPath;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;

namespace Selenium.AngleSharp.WebDriver {

    // IElement only has GetElementsByClassName and GetElementsByTagName, thus, searching will be limited to 
    // ByLinkText, ByClassName, ByPartialLinkText, and ByTagName
    partial class AngleSharpWebElement : IFindsByLinkText, IFindsByClassName, IFindsByPartialLinkText, IFindsByTagName, IFindsByName, IFindsById, IFindsByXPath, IFindsByCssSelector {

        #region By link text

        private IEnumerable<IElement> ByLinkText(string linkText) => _Element.GetElementsByTagName("a").Where(e => e.GetInnerText() == linkText);
        public IWebElement FindElementByLinkText(string linkText) => GetElement(ByLinkText(linkText));
        public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText) => GetElements(ByLinkText(linkText));

        #endregion

        #region By class name

        private IEnumerable<IElement> ByClassName(string className) => _Element.GetElementsByClassName(className);
        public IWebElement FindElementByClassName(string className) => GetElement(ByClassName(className));
        public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className) => GetElements(ByClassName(className));

        #endregion

        #region By partial link text

        private IEnumerable<IElement> ByPartialLinkText(string partialLinkText) => _Element.GetElementsByTagName("a").Where(e => e.GetInnerText().Contains(partialLinkText));
        public IWebElement FindElementByPartialLinkText(string partialLinkText) => GetElement(ByPartialLinkText(partialLinkText));
        public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText) => GetElements(ByPartialLinkText(partialLinkText));

        #endregion

        #region By tag name

        private IEnumerable<IElement> ByTagName(string tagName) => _Element.GetElementsByTagName(tagName);
        public IWebElement FindElementByTagName(string tagName) => GetElement(ByTagName(tagName));
        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName) => GetElements(ByTagName(tagName));

        #endregion

        #region By name


        public IWebElement FindElementByName(string name) => Create(_Element.QuerySelector($"[name={name}]"));
        public ReadOnlyCollection<IWebElement> FindElementsByName(string name) => GetElements(_Element.QuerySelectorAll($"[name={name}]"));

        #endregion

        #region By id

        public IWebElement FindElementById(string id) => Create(_Element.QuerySelector($"[id='{id}']"));
        public ReadOnlyCollection<IWebElement> FindElementsById(string id) => GetElements(_Element.QuerySelectorAll($"[id='{id}']"));

        #endregion

        #region By XPath selector

        public IWebElement FindElementByXPath(string xpath) => Create(_Element.SelectSingleNode(xpath));
        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath) => GetElements(_Element.SelectNodes(xpath));

        #endregion

        #region By Css selector

        public IWebElement FindElementByCssSelector(string cssSelector) => Create(_Element.QuerySelector(cssSelector));
        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector) => GetElements(_Element.QuerySelectorAll(cssSelector));

        #endregion

    }
}
