using AngleSharp;
using AngleSharp.Browser.Dom;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Selenium.AngleSharp.WebDriver {
    internal class AngleSharpHistory : IHistory {
        private readonly IBrowsingContext browsingContext;
        private readonly Stack<IDocument> backStack = new Stack<IDocument>();
        private IDocument current = null;
        private readonly Stack<IDocument> forwardStack = new Stack<IDocument>();


        public AngleSharpHistory(IBrowsingContext browsingContext) {
            this.browsingContext = browsingContext;
        }

        private void LoadCurrent() {
            browsingContext.Active = current;
        }

        // TODO: keeping the history simple for now, only supporting documents and not state
        public IDocument this[int index] =>
            index < 0 || index > backStack.Count + forwardStack.Count ? throw new ArgumentOutOfRangeException(nameof(index)) :
            index == backStack.Count ? current :
            index <= backStack.Count ? backStack.ElementAt(backStack.Count - index + 1) :
            forwardStack.ElementAt(index - backStack.Count - 1)

        ;

        public int Length => backStack.Count + forwardStack.Count + 1;

        public int Index => backStack.Count;

        public object State => current;

        public void Back() {
            if (backStack.Count > 0) {
                if (current is IDocument) forwardStack.Push(current);
                current = backStack.Pop();
                LoadCurrent();
            }

        }

        public void Forward() {
            if (forwardStack.Count > 0) {
                if (current is IDocument) backStack.Push(current);
                current = forwardStack.Pop();
                LoadCurrent();
            }
        }

        public void Go(int delta = 0) {
            if (delta == 0) return;
            if (delta < 0) {
                if (-delta > backStack.Count) return;
                while (delta < 0) {
                    Back();
                    delta++;
                }
            }
            else {
                if (delta > forwardStack.Count) return;
                while (delta > 0) {
                    Forward();
                    delta--;
                }
            }
            LoadCurrent();

        }

        public void PushState(object data, string title, string url = null) {
            if (data is IDocument document) {
                forwardStack.Clear();
                backStack.Push(current);
                current = document;
                LoadCurrent();
            }
        }

        public void ReplaceState(object data, string title, string url = null) {
            if (data is IDocument document) {
                current = document;
                LoadCurrent();
            }
        }
    }
}
