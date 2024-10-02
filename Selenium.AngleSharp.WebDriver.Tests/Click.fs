module Click

open System
open System.IO
open Xunit
open AngleSharp
open AngleSharp.Css
open OpenQA.Selenium
open Selenium.AngleSharp.WebDriver

let currentDirectoryUri = Path.Combine(Environment.CurrentDirectory, ".") |> Uri
let pageDirectoryUri = Uri(currentDirectoryUri, "pages/")
let simpleTestPageUri = Uri(pageDirectoryUri, "simpleTest.html")
let clickPageUri = Uri(pageDirectoryUri, "clicks.html")
let styledPageUri = Uri(pageDirectoryUri, "styledPage.html")
let javascriptPageUri = Uri(pageDirectoryUri, "javascriptPage.html")
let formsPageUri = Uri(pageDirectoryUri,"formPage.html")
let clickOobPageUri = Uri(pageDirectoryUri, "click_out_of_bounds.html")
let clickSurroundingStrongTagPageUri = Uri(pageDirectoryUri, "ClickTest_testClicksASurroundingStrongTag.html")
let googleMapPageUri = Uri(pageDirectoryUri, "click_tests/google_map.html")
let clickTooBigPageUri = Uri(pageDirectoryUri, "click_too_big.html")
let clickTooBigInFramePageUri = Uri(pageDirectoryUri, "click_too_big_in_frame.html")
let clickRtlPageUri = Uri(pageDirectoryUri, "click_rtl.html")
let fixedFooterNoScrollPageUri = Uri(pageDirectoryUri, "fixedFooterNoScroll.html")
let fixedFooterNoScrollQuirksModePageUri = Uri(pageDirectoryUri, "fixedFooterNoScrollQuirksMode.html")
let linkThatWrapsPageUri = Uri(pageDirectoryUri, "click_tests/link_that_wraps.html")
let spanThatWrapsPageUri = Uri(pageDirectoryUri, "click_tests/span_that_wraps.html")
let disabledElementPageUri = Uri(pageDirectoryUri, "click_tests/disabled_element.html")
 
let ``Create driver``() =
  new AngleSharpDriver(
    Configuration.Default
      .WithDefaultLoader(Io.LoaderOptions(IsResourceLoadingEnabled=true))
      .WithRequesters()
      .WithHistory()
      .WithCss()
      .WithRenderDevice(DefaultRenderDevice(ViewPortWidth=1280, ViewPortHeight=720))
  )

let waitFor timeout message func =
  let endTime = DateTime.Now.Add timeout
  let rec waitFor() = 
    try
      if not <| func() then
        if DateTime.Now > endTime
        then Assert.Fail $"Connection time out: {message}"
        else System.Threading.Thread.Sleep 100; waitFor()
    with
    |  e  ->
      if DateTime.Now > endTime
      then raise (WebDriverException("Operation timed out", e))
      else System.Threading.Thread.Sleep 100; waitFor()

  waitFor()



[<Fact>]
let ``Follow link on click``() =
  let driver = ``Create driver``()
  driver.Url <- clickPageUri.ToString()
  driver.FindElement(By.Id "normal").Click()

  fun () -> driver.Title = "XHTML Test Page"
  |> waitFor  (TimeSpan.FromSeconds 5)  "Browser title was not 'XHTML Test Page'"

  Assert.Equal("XHTML Test Page", driver.Title)


[<Fact>]
let ``Follow link that is overflowed``() =
  let driver = ``Create driver``()
  driver.Url <- clickPageUri.ToString()
  driver.FindElement(By.Id "overflowLink").Click()

  fun () -> driver.Title = "XHTML Test Page"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'XHTML Test Page'"


[<Fact(Skip="Javascript not yet implemented")>]
let ``Click on fragment link without reloading page``() =
  let driver = ``Create driver``()
  driver.Url <- clickPageUri.ToString()
  (driver :> IJavaScriptExecutor).ExecuteScript("document.latch = true") |> ignore

  driver.FindElement(By.Id "anchor").Click()

  let samePage = (driver :> IJavaScriptExecutor).ExecuteScript("return document.latch") :?> bool

  Assert.True(samePage, "Latch was reset")


[<Fact(Skip="AngleSharp does not support frames as target")>]
let ``Load link in another frame``() =
  let driver = ``Create driver``()
  driver.Url <- clickPageUri.ToString()
  driver.SwitchTo().Frame("source") |> ignore

  driver.FindElement(By.Id "otherframe").Click()
  driver.SwitchTo().DefaultContent().SwitchTo().Frame("target") |> ignore

  Assert.Contains("Hello WebDriver", driver.PageSource)


[<Fact(Skip="Javascript not yet implemented")>]
let ``Click link from JS and load in another frame``() =
  let driver = ``Create driver``()
  driver.Url <- clickPageUri.ToString()
  driver.SwitchTo().Frame("source") |> ignore

  let toClick = (driver :> IJavaScriptExecutor).ExecuteScript("return document.getElementById('otherframe');") :?> IWebElement
  toClick.Click()
  driver.SwitchTo().DefaultContent().SwitchTo().Frame("target") |> ignore

  Assert.Contains("Hello WebDriver", driver.PageSource)


[<Fact(Skip="Javascript not yet implemented")>]
let ``Click link from JS which has been loaded previously in another frame``() =
  let driver = ``Create driver``()
  driver.Url <- clickPageUri.ToString()
  driver.SwitchTo().Frame("source") |> ignore

  // Prime the cache of elements
  driver.FindElement(By.Id "otherframe") |> ignore

  // This _should_ return the same element
  let toClick = (driver :> IJavaScriptExecutor).ExecuteScript("return document.getElementById('otherframe');") :?> IWebElement
  toClick.Click()
  driver.SwitchTo().DefaultContent().SwitchTo().Frame("target") |> ignore

  Assert.Contains("Hello WebDriver", driver.PageSource)


[<Fact(Skip="SendKeys not implemented")>]
let ``Click element which has top set to a negative number``() =
  let driver = ``Create driver``()
  driver.Url <- styledPageUri.ToString()
  let searchBox = driver.FindElement(By.Name "searchBox")
  searchBox.SendKeys("Cheese")
  driver.FindElement(By.Name "btn").Click()

  let log = driver.FindElement(By.Id "log").Text
  Assert.Equal("click", log)


[<Fact(Skip="Javascript not yet implemented")>]
let ``Mouse over sets related target``() =
  let driver = ``Create driver``()
  driver.Url <- javascriptPageUri.ToString()

  driver.FindElement(By.Id "movable").Click()

  let log = driver.FindElement(By.Id "result").Text

  Assert.Equal("parent matches? true", log)


[<Fact>]
let ``Clicking link uses first client with nonzero size``() =
  let driver = ``Create driver``()
  driver.Url <- clickPageUri.ToString()
  driver.FindElement(By.Id "twoClientRects").Click()

  fun () -> driver.Title = "XHTML Test Page"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'XHTML Test Page'"

  Assert.Equal("XHTML Test Page", driver.Title)


[<Fact(Skip="AngleSharp does not support tracking windows.")>]
let ``Clicking a link opening a window only opens one new window``() =
  let driver = ``Create driver``()
  driver.Url <- clickPageUri.ToString()
  let windowHandlesBefore = driver.WindowHandles.Count

  driver.FindElement(By.Id("new-window")).Click()

  fun () -> driver.WindowHandles.Count >= windowHandlesBefore + 1
  |> waitFor  (TimeSpan.FromSeconds 5) $"Window handles was not {windowHandlesBefore + 1}"

  Assert.Equal(windowHandlesBefore + 1, driver.WindowHandles.Count)


[<Fact>]
let ``Clicking label updates associated checkbox``() =
  let driver = ``Create driver``()
  driver.Url <- formsPageUri.ToString()

  driver.FindElement(By.Id("label-for-checkbox-with-label")).Click()

  Assert.True(driver.FindElement(By.Id("checkbox-with-label")).Selected, "Checkbox should be selected")


[<Fact>]
let ``Follow link containing image when clicked``() =
  let driver = ``Create driver``()
  driver.Url <- clickPageUri.ToString()
  driver.FindElement(By.Id("link-with-enclosed-image")).Click()

  fun () -> driver.Title = "XHTML Test Page"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'XHTML Test Page'"

  Assert.Equal("XHTML Test Page", driver.Title)


[<Fact>]
let ``Follow link containing image when image clicked``() =
  let driver = ``Create driver``()
  driver.Url <- clickPageUri.ToString()
  driver.FindElement(By.Id("link-with-enclosed-image")).FindElement(By.TagName "img").Click()

  fun () -> driver.Title = "XHTML Test Page"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'XHTML Test Page'"

  Assert.Equal("XHTML Test Page", driver.Title)


[<Fact>]
let ``Follow link containing embedded elements on click``() =
  let driver = ``Create driver``()
  driver.Url <- clickPageUri.ToString()
  driver.FindElement(By.Id("link-with-enclosed-span")).Click()

  fun () -> driver.Title = "XHTML Test Page"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'XHTML Test Page'"

  Assert.Equal("XHTML Test Page", driver.Title)


[<Fact>]
let ``Follow link containing embedded block elements on click``() =
  let driver = ``Create driver``()
  driver.Url <- clickPageUri.ToString()
  driver.FindElement(By.Id "embeddedBlock").Click()

  fun () -> driver.Title = "XHTML Test Page"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'XHTML Test Page'"

  Assert.Equal("XHTML Test Page", driver.Title)


[<Fact>]
let ``Follow link containing embedded elements when embedded element clicked``() =
  let driver = ``Create driver``()
  driver.Url <- clickPageUri.ToString()
  driver.FindElement(By.Id("link-with-enclosed-span")).FindElement(By.TagName "span").Click()

  fun () -> driver.Title = "XHTML Test Page"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'XHTML Test Page'"

  Assert.Equal("XHTML Test Page", driver.Title)


[<Fact(Skip="Javascript not yet implemented")>]
let ``Click on element in viewport succeeeds``() =
  let driver = ``Create driver``()
  driver.Url <- clickOobPageUri.ToString()
  let button = driver.FindElement(By.Id "button")
  button.Click()


[<Fact>]
let ``Clicking link formatted strong succeeds``() =
  let driver = ``Create driver``()
  driver.Url <- clickSurroundingStrongTagPageUri.ToString()
  driver.FindElement(By.TagName "a").Click()

  fun () -> driver.Title = "XHTML Test Page"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'XHTML Test Page'"



[<Fact>]
let ``Clicking image map area succeeds``() =
  let driver = ``Create driver``()
  driver.Url <- googleMapPageUri.ToString()
  driver.FindElement(By.Id "rectG").Click()

  fun () -> driver.Title = "Target Page 1"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'Target Page 1'"


  driver.Url <- googleMapPageUri.ToString()
  driver.FindElement(By.Id "circleO").Click()

  fun () -> driver.Title = "Target Page 2"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'Target Page 2'"


  driver.Url <- googleMapPageUri.ToString()
  driver.FindElement(By.Id "polyLE").Click()

  fun () -> driver.Title = "Target Page 3"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'Target Page 3'"



[<Fact>]
let ``Clicking element larger than two viewports succeeds``() =
  let driver = ``Create driver``()
  driver.Url <- clickTooBigPageUri.ToString()

  let element = driver.FindElement(By.Id "click")

  element.Click()


  fun () -> driver.Title = "clicks"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'clicks'"



[<Fact>]
let ``Clicking element in frame larger than two viewports succeeds``() =
  let driver = ``Create driver``()
  driver.Url <- clickTooBigInFramePageUri.ToString()

  let frame = driver.FindElement(By.Id "iframe1")
  driver.SwitchTo().Frame(frame) |> ignore

  let element = driver.FindElement(By.Id "click")

  element.Click()


  fun () -> driver.Title = "clicks"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'clicks'"



[<Fact>]
let ``Clicking on RTL link succeeds``() =
  let driver = ``Create driver``()
  driver.Url <- clickRtlPageUri.ToString()

  let element = driver.FindElement(By.Id "ar_link")
  element.Click()


  fun () -> driver.Title = "clicks"
  |> waitFor  (TimeSpan.FromSeconds 5) "Expected title to be 'clicks'"

  Assert.Equal("clicks", driver.Title)


[<Fact>]
let ``Clicking link in absolutely positioned footer succeeds``() =
  let driver = ``Create driver``()
  driver.Url <- fixedFooterNoScrollPageUri.ToString()

  driver.FindElement(By.Id "link").Click()

  fun () -> driver.Title = "XHTML Test Page"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'XHTML Test Page'"

  Assert.Equal("XHTML Test Page", driver.Title)


[<Fact>]
let ``Clicking link in absolutely positioned footer in quirks mode succeeds``() =
  let driver = ``Create driver``()
  driver.Url <- fixedFooterNoScrollQuirksModePageUri.ToString()

  driver.FindElement(By.Id "link").Click()

  fun () -> driver.Title = "XHTML Test Page"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'XHTML Test Page'"

  Assert.Equal("XHTML Test Page", driver.Title)


[<Fact(Skip="Javascript not yet implemented")>]
let ``Clicking link without href succeeds``() =
  let driver = ``Create driver``()
  driver.Url <- javascriptPageUri.ToString()

  let element = driver.FindElement(By.LinkText("No href"))
  element.Click()


  fun () -> driver.Title = "Changed"
  |> waitFor  (TimeSpan.FromSeconds 5) "Expected title to be 'Changed'"

  Assert.Equal("Changed", driver.Title)


[<Fact>]
let ``Clicking link that wraps to another line succeeds``() =
  let driver = ``Create driver``()
  driver.Url <- linkThatWrapsPageUri.ToString()

  driver.FindElement(By.Id "link").Click()


  fun () -> driver.Title = "Submitted Successfully!"
  |> waitFor  (TimeSpan.FromSeconds 5) "Expected title to be 'Submitted Successfully!'"

  Assert.Equal("Submitted Successfully!", driver.Title)


[<Fact(Skip="Javascript not yet implemented")>]
let ``Clicking element that wraps to another line succeeds``() =
  let driver = ``Create driver``()
  driver.Url <- spanThatWrapsPageUri.ToString()

  driver.FindElement(By.Id "span").Click()


  fun () -> driver.Title = "Submitted Successfully!"
  |> waitFor  (TimeSpan.FromSeconds 5) "Expected title to be 'Submitted Successfully!'"

  Assert.Equal("Submitted Successfully!", driver.Title)


[<Fact>]
let ``Clicking a disabled element does nothing``() =
  let driver = ``Create driver``()
  driver.Url <- disabledElementPageUri.ToString()

  let element = driver.FindElement(By.Name "disabled")
  element.Click()


//------------------------------------------------------------------
// Tests below here are not included in the Java test suite
//------------------------------------------------------------------
[<Fact>]
let ``Clicking link with line break succeeds``() =
  let driver = ``Create driver``()
  driver.Url <- simpleTestPageUri.ToString()
  driver.FindElement(By.Id "multilinelink").Click()

  fun () -> driver.Title = "We Arrive Here"
  |> waitFor  (TimeSpan.FromSeconds 5) "Browser title was not 'We Arrive Here'"

  Assert.Equal("We Arrive Here", driver.Title)

