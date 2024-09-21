module ``Tests for finding links by partial text``

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
let ``Find link which has formatting tags``() =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let elem = driver.FindElement(By.Id("links"))
  let res = elem.FindElement(By.PartialLinkText("link with formatting tags"))
  Assert.NotNull(res)
  Assert.Equal("link with formatting tags", res.Text)

[<Fact>]
let ``Find link which has leading space``() =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let elem = driver.FindElement(By.Id("links"))
  let res = elem.FindElement(By.PartialLinkText("link with leading space"))
  Assert.NotNull(res)
  Assert.Equal("link with leading space", res.Text)

[<Fact>]
let ``Find link with trailing space``() =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let elem = driver.FindElement(By.Id("links"))
  let res = elem.FindElement(By.PartialLinkText("link with trailing space"))
  Assert.NotNull(res)
  Assert.Equal("link with trailing space", res.Text)

[<Fact>]
let ``Find mutiple links``() =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let elem = driver.FindElement(By.Id("links"))
  let elements = elem.FindElements(By.PartialLinkText("link"))
  Assert.NotNull(elements)
  Assert.Equal(6, elements.Count)

[<Fact>]
let ``Find link ignoring trailing space``() =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let link = driver.FindElement(By.LinkText("link with trailing space"))
  Assert.Equal("linkWithTrailingSpace", link.GetAttribute("id"))

[<Fact>]
let ``Find child link ignoring trailing space``() =
  use driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()

  let elem = driver.FindElement(By.Id("links"))
  let link = elem.FindElement(By.LinkText("link with trailing space"))
  Assert.Equal("linkWithTrailingSpace", link.GetAttribute("id"))

