using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.XPath;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Selenium.AngleSharp.WebDriver {
    partial class AngleSharpWebElement(IElement element) : IWebElement, IFindsElement {

        public static IWebElement Create(INode node) =>
            node is null ? throw new NoSuchElementException() :
            node is IElement element ? new AngleSharpWebElement(element) :
            throw new InvalidSelectorException()
        ;

        public static IWebElement Create(IElement element) => 
            element is null ? throw new NoSuchElementException() :
            new AngleSharpWebElement(element)
        ;

        public static ReadOnlyCollection<IWebElement> GetElements(IEnumerable<IElement> elements) =>
            new ReadOnlyCollectionBuilder<IWebElement>(elements?.Select(Create) ?? [])
            .ToReadOnlyCollection()
        ;

        public static ReadOnlyCollection<IWebElement> GetElements(IEnumerable<INode> nodes) =>
            new ReadOnlyCollectionBuilder<IWebElement>(nodes?.Select(Create) ?? [])
            .ToReadOnlyCollection()
        ;

        public string GetDomAttribute(string attributeName) => element.GetAttribute(attributeName);

        public string GetDomProperty(string propertyName) => throw new NotImplementedException();

        public ISearchContext GetShadowRoot() => new AngleSharpShadowRoot(element.ShadowRoot);

        public string TagName => element.TagName;

        public string Text => element.GetInnerText().Trim();

        public bool Enabled => element.IsEnabled();

        public bool Selected => element.IsFocused;

        public Point Location => throw new NotImplementedException("Rendering not supported");

        public Size Size => throw new NotImplementedException("Rendering not supported");

        public bool Displayed =>element is IHtmlElement htmlElement && !htmlElement.IsHidden;

        public void Clear() {
            switch (element) {
                case IHtmlInputElement inputElement: inputElement.Value = null; break;
                case IHtmlTextAreaElement textAreaElement: textAreaElement.Value = null; break;
            }
        }

        public void Click() {
            // TODO: Either this method needs to perform navigation if an anchor or button is clicked,
            // an event handler needs to be added to specific element types which will perform navigation
            // on a click event, or AngleSharp needs to include a navigation service which will allow
            // clicking on specific elements to cause navigation if enabled.
            if (element is IHtmlElement htmlElement) htmlElement.DoClick();
        }

        public IWebElement FindElement(By by) => by.FindElement(this);

        public ReadOnlyCollection<IWebElement> FindElements(By by) => by.FindElements(this);

        public string GetAttribute(string attributeName) => element.GetAttribute(attributeName);

        public string GetCssValue(string propertyName) {
            // TODO: Find out how AngleSharp applies CSS properties to elements
            throw new NotImplementedException();
        }

        public string GetProperty(string propertyName) {
            // TODO: Find out how AngleSharp applies javascript properties to elements
            throw new NotImplementedException();
        }

        public void SendKeys(string text) {
            // Unlikely to support this, given the potentially complex input.
            throw new NotImplementedException();
        }

        public void Submit() {
            // TODO: If the element is a button with a name, the name and value should be added to the form data
            // to be submitted.  If the element is a button with the form attribute set, that form should be submitted.
            // Otherwise, if the element is a form element, that form should be submitted.  Otherwise, check ancestry
            // until a form is found, and submit that form. Otherwise, do nothing.
            throw new NotImplementedException();
        }

        public IWebElement FindElement(string mechanism, string value) =>
            FindElements(mechanism, value) is [var element, ..] ? element
            : throw new NoSuchElementException()
        ;

        public ReadOnlyCollection<IWebElement> FindElements(string mechanism, string value) =>
            mechanism switch {
                "css selector" => GetElements(element.QuerySelectorAll(value)),
                "link text" => GetElements(element.GetElementsByTagName("a")?.Where(e => e.GetInnerText() == value)),
                "partial link text" => GetElements(element.GetElementsByTagName("a")?.Where(e => e.GetInnerText().Contains(value))),
                "tag name" => GetElements(element.GetElementsByTagName(value)),
                "xpath" => GetElements(element.SelectNodes(value)),
                _ => throw new NotImplementedException()
            }
        ;
    }
}
