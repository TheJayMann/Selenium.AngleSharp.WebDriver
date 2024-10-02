using AngleSharp.Html.Dom;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace Selenium.AngleSharp.WebDriver {
    internal class AngleSharpTargetLocator(AngleSharpDriver driver) : ITargetLocator {
        public IWebElement ActiveElement() {
            throw new NotImplementedException();
        }

        public IAlert Alert() {
            throw new NotImplementedException();
        }

        public IWebDriver DefaultContent() {
            driver.SetActiveContext(null);
            return driver;
        }

        public IWebDriver Frame(int frameIndex) {
            var webElement = driver.FindElements(By.TagName("iframe")).ElementAtOrDefault(frameIndex) ?? throw new NoSuchFrameException();
            if (AngleSharpWebElement.GetAngleSharpElement(webElement) is not IHtmlInlineFrameElement frame) throw new NoSuchFrameException();
            driver.SetActiveContext(frame.ContentDocument.Context);
            return driver;
        }

        public IWebDriver Frame(string frameName) {
            var webElement =
                driver.FindElements(By.Name(frameName)).FirstOrDefault()
                ??
                driver.FindElements(By.Id(frameName)).FirstOrDefault()
                ??
                throw new NoSuchFrameException($"No frame element found with name or id {frameName}")
            ;
            if (AngleSharpWebElement.GetAngleSharpElement(webElement) is not IHtmlInlineFrameElement frame) throw new NoSuchFrameException($"No frame element found with name or id {frameName}");
            driver.SetActiveContext(frame.ContentDocument.Context);
            return driver;
        }

        public IWebDriver Frame(IWebElement frameElement) {
            if (AngleSharpWebElement.GetAngleSharpElement(frameElement) is not IHtmlInlineFrameElement frame) throw new NoSuchFrameException();
            driver.SetActiveContext(frame.ContentDocument.Context);
            return driver;
        }

        public IWebDriver NewWindow(WindowType typeHint) {
            throw new NotImplementedException();
        }

        public IWebDriver ParentFrame() {
            driver.SetParentActive();
            return driver;
        }

        public IWebDriver Window(string windowName) {
            throw new NotImplementedException();
        }
    }
}
