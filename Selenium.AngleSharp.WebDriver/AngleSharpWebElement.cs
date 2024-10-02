using AngleSharp.Attributes;
using AngleSharp.Browser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Dom.Events;
using AngleSharp.XPath;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using Selenium.AngleSharp.WebDriver.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Selenium.AngleSharp.WebDriver {
    partial class AngleSharpWebElement(IElement element) : IWebElement, IFindsElement {
        private readonly IElement element = element;

        internal static IElement GetAngleSharpElement(IWebElement element) =>
            element is AngleSharpWebElement angleSharpElement
            ? angleSharpElement.element
            : throw new InvalidOperationException("Provided IWebElement is not an AngleSharp web element.")
        ;

        private static Dictionary<string, Func<object, object>> GetDomProperties(IElement element) {
            var props = new Dictionary<string, Func<object, object>>();
            foreach (var @interface in element.GetType().GetInterfaces()) {
                foreach (var prop in @interface.GetProperties()) {
                    foreach (var propName in prop.GetCustomAttributes<DomNameAttribute>().Select(n => n.OfficialName)) {
                        props[propName] = prop.GetValue;
                    }
                }
            }
            return props;
        }

        private readonly Dictionary<string, Func<object, object>> _DomProperties = GetDomProperties(element);

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

        public string GetDomProperty(string propertyName) =>
            _DomProperties.TryGetValue(propertyName, out var prop)
            ? prop(element)?.ToString()
            : null
        ;

        public ISearchContext GetShadowRoot() => new AngleSharpShadowRoot(element.ShadowRoot);

        public string TagName => element.TagName;

        public string Text => element.GetInnerText().Trim();

        public bool Enabled => element.IsEnabled();

        public bool Selected => element.IsChecked();

        public Point Location => throw new NotImplementedException("Rendering not supported");

        public Size Size => throw new NotImplementedException("Rendering not supported");

        public bool Displayed => element is IHtmlElement htmlElement && !htmlElement.IsHidden;

        public void Clear() {
            if (element.IsDisabled() || element.IsReadOnly()) throw new InvalidElementStateException();
            if (element is IHtmlInputElement) {
                element.RemoveAttribute(AttributeNames.Value);
            }
            else if (element is IHtmlTextAreaElement or IHtmlElement { IsContentEditable: true }) {
                element.SetInnerText(string.Empty);
            }
        }

        private static bool IsClickedCancelled<TElement>(TElement target) where TElement : IEventTarget, INode =>
            target.Owner is Document document
            &&
            AsyncHelper.RunSync(() => document.Loop.EnqueueAsync(_ => target.Fire<MouseEvent>(m =>
                m.Init(EventNames.Click, bubbles: true, cancelable: true, document.DefaultView, 0, 0, 0, 0, 0, ctrlKey: false, altKey: false, shiftKey: false, metaKey: false, MouseButton.Primary, target)
            )))
        ;

        public void Click() {
            if (element is IHtmlLabelElement label && label.Control is not null and var control) {
                if (IsClickedCancelled(element)) return;
                new AngleSharpWebElement(control).Click();
            }
            else if (element is IHtmlInputElement { Type: "checkbox" } checkbox) {
                if (IsClickedCancelled(element)) return;
                checkbox.IsChecked = !checkbox.IsChecked;
            }
            else if (element is IHtmlMenuItemElement { Type: "checkbox" } menuItemCheckBox) {
                if (IsClickedCancelled(element)) return;
                menuItemCheckBox.IsChecked = !menuItemCheckBox.IsChecked;
            }
            else if (element is IHtmlInputElement { Type: "radio" } radio) {
                if (IsClickedCancelled(element)) return;
                radio.IsChecked = true;
            }
            else if (element is IHtmlInputElement { Type: "radio" } menuItemRadio) {
                if (IsClickedCancelled(element)) return;
                menuItemRadio.IsChecked = true;
            }
            else if (element is IHtmlOptionElement option) {
                if (IsClickedCancelled(element)) return;
                option.IsSelected = true;
            }
            else for (var htmlElement = element as IHtmlElement; htmlElement is not null; htmlElement = htmlElement.ParentElement as IHtmlElement) {
                if (htmlElement is IHtmlButtonElement or IUrlUtilities) {
                    htmlElement.DoClick();
                    break;
                }
            }
        }

        public IWebElement FindElement(By by) => by.FindElement(this);

        public ReadOnlyCollection<IWebElement> FindElements(By by) => by.FindElements(this);

        public string GetAttribute(string attributeName) => GetDomProperty(attributeName) ?? GetDomAttribute(attributeName);

        public string GetCssValue(string propertyName) {
            // TODO: Find out how AngleSharp applies CSS properties to elements
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
