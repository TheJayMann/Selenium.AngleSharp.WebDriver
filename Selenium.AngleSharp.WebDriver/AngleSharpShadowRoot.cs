using AngleSharp.Dom;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.ObjectModel;
using static Selenium.AngleSharp.WebDriver.AngleSharpWebElement;

namespace Selenium.AngleSharp.WebDriver {
    public class AngleSharpShadowRoot(IShadowRoot shadowRoot) : ISearchContext, IFindsElement {
        public IWebElement FindElement(By by) => by.FindElement(this);
        public ReadOnlyCollection<IWebElement> FindElements(By by) => by.FindElements(this);

        public IWebElement FindElement(string mechanism, string value) =>
            FindElements(mechanism, value) is [var element, ..] ? element
            : throw new NoSuchElementException()
        ;

        public ReadOnlyCollection<IWebElement> FindElements(string mechanism, string value) =>
            mechanism switch {
                "css selector" => GetElements(shadowRoot.QuerySelectorAll(value)),
                // GetElementsByTagName is implemented by ShadowRoot, but is not exposed in any way.
                //"link text" => GetElements(shadowRoot.GetElementsByTagName("a")?.Where(e => e.GetInnerText() == value)),
                //"partial link text" => GetElements(shadowRoot.GetElementsByTagName("a")?.Where(e => e.GetInnerText().Contains(value))),
                //"tag name" => GetElements(shadowRoot.GetElementsByTagName(value)),
                // XPath SelectNodes method only supports IElement and not IShadowRoot. Maybe `Host` property works?
                //"xpath" => GetElements(shadowRoot.SelectNodes(value)),
                _ => throw new NotImplementedException()
            }
        ;
    }
}
