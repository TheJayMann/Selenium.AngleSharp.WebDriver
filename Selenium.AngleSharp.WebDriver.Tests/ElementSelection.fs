module ``Tests for finding elements``

open System
open Xunit
open AngleSharp
open OpenQA.Selenium
open Selenium.AngleSharp.WebDriver

let currentDirectoryUri = Uri Environment.CurrentDirectory
let pageDirectoryUri = Uri(currentDirectoryUri, "net8.0/pages/")
let nestedPageUri = Uri(pageDirectoryUri, "nestedElements.html")
let simpleTestPageUri = Uri(pageDirectoryUri, "simpleTest.html")

let [<Literal>] mainPageTitle = "Main Page"
let [<Literal>] page1Title = "Page 1"
 
let ``Create driver``() =
  new AngleSharpDriver(
    Configuration.Default
      .WithDefaultLoader()
      .WithRequesters()
      .WithHistory()
      .WithCss()
  )


[<Fact>]
let ``Find element by id`` () =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let element = driver.FindElement(By.Id("oneline")) 
  Assert.Equal("A single line of text", element.Text)

[<Fact>]
let ``Find element by link text`` () =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let element = driver.FindElement(By.LinkText("link with leading space")) 
  Assert.Equal("link with leading space", element.Text)

[<Fact>]
let ``Find element by name`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Name("div1")) 
  Assert.Equal("hello world hello world", element.Text)

[<Fact>]
let ``Find element by XPath selector`` () =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let element = driver.FindElement(By.XPath("/body/p[1]")) 
  Assert.Equal("A single line of text", element.Text)

[<Fact>]
let ``Find element by class name`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.ClassName("one")) 
  Assert.Equal("Span with class of one", element.Text)

[<Fact>]
let ``Find element by partial link text`` () =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let element = driver.FindElement(By.PartialLinkText("leading space")) 
  Assert.Equal("link with leading space", element.Text)

[<Fact>]
let ``Find element by tag name`` () =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let element = driver.FindElement(By.TagName("H1")) 
  Assert.Equal("Heading", element.Text)

[<Fact>]
let ``Find elements by id`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let elements = driver.FindElements(By.Id("test_id")) 
  Assert.Equal(2, elements.Count)

[<Fact>]
let ``Find elements by link text`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let elements = driver.FindElements(By.LinkText("hello world")) 
  Assert.Equal(12, elements.Count)

[<Fact>]
let ``Find elements by name`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let elements = driver.FindElements(By.Name("form1")) 
  Assert.Equal(4, elements.Count)

[<Fact>]
let ``Find elements by XPath selector`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let elements = driver.FindElements(By.XPath("//a")) 
  Assert.Equal(12, elements.Count)

[<Fact>]
let ``Find elements by class name`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let elements = driver.FindElements(By.ClassName("one")) 
  Assert.Equal(3, elements.Count)

[<Fact>]
let ``Find elements by partial link text`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let elements = driver.FindElements(By.PartialLinkText("world")) 
  Assert.Equal(12, elements.Count)

[<Fact>]
let ``Find elements by tag name`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let elements = driver.FindElements(By.TagName("a")) 
  Assert.Equal(12, elements.Count)
