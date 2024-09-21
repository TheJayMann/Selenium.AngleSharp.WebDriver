module ``Tests for finding child elements``

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
let ``Find element using XPath`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Name("form2"))
  let child = element.FindElement(By.XPath("select"))
  Assert.Equal("2", child.GetAttribute("id"))

[<Fact>]
let ``Test absolute XPath selector on child element`` () =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let element = driver.FindElement(By.Id("multiline"))
  let rootSelectedElements = driver.FindElements(By.XPath("//p"))
  let selectedElements = element.FindElements(By.XPath("//p"))
  Assert.Equal(rootSelectedElements.Count, selectedElements.Count)
  
[<Fact>]
let ``Test relative XPath selector only finds child elements``() =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let element = driver.FindElement(By.Id("multiline"))
  let selectedElements = element.FindElements(By.XPath("./p"))
  Assert.Equal(1, selectedElements.Count)
  Assert.Equal("A div containing", selectedElements[0].Text)
  
[<Fact>]
let ``Find single element by XPath should throw if not found`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Name("form2"))
  Assert.Throws<NoSuchElementException>(fun () -> element.FindElement(By.XPath("select/x")) : obj)

[<Fact>]
let ``Find multiple elements by XPath`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Name("form2"))
  let children = element.FindElements(By.XPath("select/option"))
  Assert.Equal(8, children.Count)
  Assert.Equal("One", children[0].Text)
  Assert.Equal("Two", children[1].Text)

[<Fact>]
let ``Find multiple elements by XPath should be empty of not found`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Name("form2"))
  let children = element.FindElements(By.XPath("select/x"))
  Assert.Equal(0, children.Count)

[<Fact>]
let ``Find element by name`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()
  
  let element = driver.FindElement(By.Name("form2"))
  let child = element.FindElement(By.Name("selectomatic"))
  Assert.Equal("2", child.GetAttribute("id"))

[<Fact>]
let ``Find elements by name`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()
  
  let element = driver.FindElement(By.Name("form2"))
  let child = element.FindElements(By.Name("selectomatic"))
  Assert.Equal(2, child.Count)
  
[<Fact>]
let ``Find element by id`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()
  
  let element = driver.FindElement(By.Name("form2"))
  let child = element.FindElement(By.Id("2"))
  Assert.Equal("selectomatic", child.GetAttribute("name"))

[<Fact>]
let ``Find child element by id when id exist for other element`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()
  
  let element = driver.FindElement(By.Id("test_id_div"))
  let child = element.FindElement(By.Id("test_id"))
  Assert.Equal("inside", child.Text)

[<Fact>]
let ``Find child element by id when id contains special characters`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()
  
  let element = driver.FindElement(By.Id("test_special_chars"))
  let child = element.FindElement(By.Id("white space"))
  Assert.Contains("space", child.Text)
  let child = element.FindElement(By.Id("css#.chars"))
  Assert.Equal("css escapes", child.Text)

[<Fact>]
let ``Find child element by id should throw if not found`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()
  
  let element = driver.FindElement(By.Id("test_id_div"))
  Assert.Throws<NoSuchElementException>(fun () -> element.FindElement(By.Id("test_id_out")) : obj)

[<Fact>]
let ``Find elements by id`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()
  
  let element = driver.FindElement(By.Name("form2"))
  let children = element.FindElements(By.Id("2"))
  Assert.Equal(2, children.Count)

[<Fact>]
let ``Find child element by link text`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Name("div1"))
  let child = element.FindElement(By.LinkText("hello world"))
  Assert.Equal("link1", child.GetAttribute("name"))

[<Fact>]
let ``Find child elements by link text`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Name("div1"))
  let children = element.FindElements(By.LinkText("hello world"))
  Assert.Equal(2, children.Count)
  Assert.Equal("link1", children[0].GetAttribute("name"))
  Assert.Equal("link2", children[1].GetAttribute("name"))

[<Fact>]
let ``Find child elements by id should not return root elements`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Id("test_id"))
  Assert.Equal(0, element.FindElements(By.Id("test_id")).Count)
  Assert.Throws<NoSuchElementException>(fun () -> element.FindElement(By.Id("test_id")) : obj)

[<Fact>]
let ``Find child element by class name`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Name("classes"))
  let child = element.FindElement(By.ClassName("one"))
  Assert.Equal("Find me", child.Text)

[<Fact>]
let ``Find child elements by class name`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Name("classes"))
  let children = element.FindElements(By.ClassName("one"))
  Assert.Equal(2, children.Count)

[<Fact>]
let ``Find child element by tag name`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Name("div1"))
  let child = element.FindElement(By.TagName("a"))
  Assert.Equal("link1", child.GetAttribute("name"))

[<Fact>]
let ``Find child elements by tag name`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Name("div1"))
  let children = element.FindElements(By.TagName("a"))
  Assert.Equal(2, children.Count)

[<Fact>]
let ``Find child element by CSS selector`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Name("form2"))
  let child = element.FindElement(By.CssSelector("*[name=\"selectomatic\"]"))
  Assert.Equal("2", child.GetAttribute("id"))

[<Fact>]
let ``Find child element by CSS3 selector`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Name("form2"))
  let child = element.FindElement(By.CssSelector("*[name^=\"selecto\"]"))
  Assert.Equal("2", child.GetAttribute("id"))

[<Fact>]
let ``Find child elemenst by CSS selector`` () =
  use driver = ``Create driver``()
  driver.Url <- nestedPageUri.ToString()

  let element = driver.FindElement(By.Name("form2"))
  let children = element.FindElements(By.CssSelector("*[name=\"selectomatic\"]"))
  Assert.Equal(2, children.Count)

[<Fact>]
let `` Find child elements by partial link text`` () =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let element = driver.FindElement(By.Id("links"))
  let children = element.FindElements(By.PartialLinkText("link"))
  Assert.Equal(6, children.Count)

[<Fact>]
let ``Find child element by partial link text when link has leading spaces matches innerText`` () =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let element = driver.FindElement(By.Id("links"))
  let child = element.FindElement(By.PartialLinkText("link with leading space"))
  Assert.Equal("link with leading space", child.Text)

[<Fact>]
let ``Find child element by partial link text when link has trailing spaces matches innerText`` () =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let element = driver.FindElement(By.Id("links"))
  let child = element.FindElement(By.PartialLinkText("link with trailing space"))
  Assert.Equal("link with trailing space", child.Text)

[<Fact>]
let ``Find child element by partial link text when link has trailing spaces`` () =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let element = driver.FindElement(By.Id("links"))
  let child = element.FindElement(By.PartialLinkText("link with trailing space"))
  Assert.Equal("linkWithTrailingSpace", child.GetAttribute("id"))
