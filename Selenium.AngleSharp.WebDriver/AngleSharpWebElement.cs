using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Selenium.AngleSharp.WebDriver {
    partial class AngleSharpWebElement : IWebElement {

        public static IWebElement Create(IElement element) => throw new NotImplementedException();

        public static IWebElement GetElement(IEnumerable<IElement> elements) => Create(elements?.FirstOrDefault());

        public static ReadOnlyCollection<IWebElement> GetElements(IEnumerable<IElement> elements) =>
            new ReadOnlyCollectionBuilder<IWebElement>(elements?.Select(Create) ?? new IWebElement[0])
            .ToReadOnlyCollection()
        ;




        private readonly IElement _Element;

        public AngleSharpWebElement(IElement element) {
            _Element = element;
        }
        public string TagName => _Element.TagName;

        public string Text => _Element.TextContent;

        public bool Enabled => _Element.IsEnabled();

        public bool Selected => _Element.IsFocused;

        public Point Location => throw new NotImplementedException("Rendering not supported");

        public Size Size => throw new NotImplementedException("Rendering not supported");

        public bool Displayed =>_Element is IHtmlElement htmlElement && !htmlElement.IsHidden;

        public void Clear() {
            switch (_Element) {
                case IHtmlInputElement inputElement: inputElement.Value = null; break;
                case IHtmlTextAreaElement textAreaElement: textAreaElement.Value = null; break;
            }
        }

        public void Click() {
            // TODO: Either this method needs to perform navigation if an anchor or button is clicked,
            // an event handler needs to be added to specific element types which will perform navigation
            // on a click event, or AngleSharp needs to include a navigation service which will allow
            // clicking on specific elements to cause navigation if enabled.
            if (_Element is IHtmlElement htmlElement) htmlElement.DoClick();
        }

        public IWebElement FindElement(By by) => by.FindElement(this);

        public ReadOnlyCollection<IWebElement> FindElements(By by) => by.FindElements(this);

        public string GetAttribute(string attributeName) => _Element.GetAttribute(attributeName);

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
            // Otherwise, if the element is a form element, that form should be submitted.  Otherwise, check ancestory
            // until a form is found, and submit that form. Otherwise, do nothing.
            throw new NotImplementedException();
        }
    }
}
