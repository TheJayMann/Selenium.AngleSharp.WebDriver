module ``Test for clearing input``

open System
open System.IO
open Xunit
open AngleSharp
open AngleSharp.Css
open OpenQA.Selenium
open Selenium.AngleSharp.WebDriver

let currentDirectoryUri = Path.Combine(Environment.CurrentDirectory, ".") |> Uri
let pageDirectoryUri = Uri(currentDirectoryUri, "pages/")
let inputsPageUri = Uri(pageDirectoryUri, "inputs.html")
let readOnlyPageUri = Uri(pageDirectoryUri, "readOnlyPage.html")
 
let ``Create driver``() =
  new AngleSharpDriver(
    Configuration.Default
      .WithDefaultLoader()
      .WithRequesters()
      .WithHistory()
      .WithCss()
      .WithRenderDevice(DefaultRenderDevice(ViewPortWidth=1280, ViewPortHeight=720))
  )

let ShouldBeAbleToResetInput locator oldValue clearedValue =
  use driver = ``Create driver``()
  driver.Url <- inputsPageUri.ToString()
  let element = driver.FindElement locator
  Assert.Equal(oldValue, element.GetAttribute "value")
  element.Clear()
  Assert.Equal(clearedValue, element.GetAttribute "value")

let ShouldBeAbleToClearInput locator oldValue =
  ShouldBeAbleToResetInput locator oldValue String.Empty

[<Fact>]
let ``Writable text can be cleard``() =
  use driver = ``Create driver``()
  driver.Url <- readOnlyPageUri.ToString()
  let element = driver.FindElement(By.Id "writableTextInput")
  element.Clear()
  Assert.Equal(String.Empty, element.GetAttribute "value")

[<Fact>]
let ``Disabled text input should not be cleared``() =
  use driver = ``Create driver``()
  driver.Url <- readOnlyPageUri.ToString()
  let element = driver.FindElement(By.Id "textInputNotEnabled")
  Assert.False element.Enabled
  Assert.Throws<InvalidElementStateException>(fun () -> element.Clear())

[<Fact>]
let ``Read only text input should not be cleared``() =
  use driver = ``Create driver``()
  driver.Url <- readOnlyPageUri.ToString()
  let element = driver.FindElement(By.Id "readOnlyTextInput")
  Assert.Throws<InvalidElementStateException>(fun () -> element.Clear())

[<Fact>]
let ``Writeable text area can be cleared``() =
  use driver = ``Create driver``()
  driver.Url <- readOnlyPageUri.ToString()
  let element = driver.FindElement(By.Id "writableTextArea")
  element.Clear()
  Assert.Equal(String.Empty, element.GetAttribute "value")

[<Fact>]
let ``Disabled text area should not be cleared``() =
  use driver = ``Create driver``()
  driver.Url <- readOnlyPageUri.ToString()
  let element = driver.FindElement(By.Id "textAreaNotEnabled")
  Assert.Throws<InvalidElementStateException>(fun () -> element.Clear())

[<Fact>]
let ``Read only text area should not be cleared``() =
  use driver = ``Create driver``()
  driver.Url <- readOnlyPageUri.ToString()
  let element = driver.FindElement(By.Id "textAreaReadOnly")
  Assert.Throws<InvalidElementStateException>(fun () -> element.Clear())

[<Fact>]
let ``Content editable area can be cleared``() =
  use driver = ``Create driver``()
  driver.Url <- readOnlyPageUri.ToString()
  let element = driver.FindElement(By.Id "content-editable")
  element.Clear()
  Assert.Equal(String.Empty, element.Text)

[<Fact>]
let ``Can clear no type input``() =
  ShouldBeAbleToClearInput(By.Name "no_type") "input with no type"

[<Fact>]
let ``Can clear number input``() =
  ShouldBeAbleToClearInput(By.Name "number_input") "42"

[<Fact>]
let ``Can clear email input``() =
  ShouldBeAbleToClearInput(By.Name "email_input") "admin@localhost"

[<Fact>]
let ``Can clear password input``() =
  ShouldBeAbleToClearInput(By.Name "password_input") "qwerty"

[<Fact>]
let ``Can clear search input``() =
  ShouldBeAbleToClearInput(By.Name "search_input") "search"

[<Fact>]
let ``Can clear tel input``() =
  ShouldBeAbleToClearInput(By.Name "tel_input") "911"

[<Fact>]
let ``Can clear text input``() =
  ShouldBeAbleToClearInput(By.Name "text_input") "text input"

[<Fact>]
let ``Can clear url input``() =
  ShouldBeAbleToClearInput(By.Name "url_input") "https://selenium.dev/"

[<Fact(Skip="AngleSharp does not appear to have a way to set default values for range input types.  Default range value should be midpoint of min and max. Default min and max should be 1 and 100.")>]
let ``Can clear range input``() =
  ShouldBeAbleToResetInput(By.Name "range_input") "42"  "50"

[<Fact(Skip="AngleSharp does not appear to have a way to set default values for color input types. Default color value should be #000000")>]
let ``Can clear color input``() =
  ShouldBeAbleToResetInput(By.Name "color_input") "#00ffff"  "#000000"

[<Fact>]
let ``Can clear date input``() =
  ShouldBeAbleToClearInput(By.Name "date_input") "2017-11-22"

[<Fact>]
let ``Can clear datetime input``() =
  ShouldBeAbleToClearInput(By.Name "datetime_input") "2017-11-22T11:22"

[<Fact>]
let ``Can clear datetime-local input``() =
  ShouldBeAbleToClearInput(By.Name "datetime_local_input") "2017-11-22T11:22"

[<Fact>]
let ``Can clear time input``() =
  ShouldBeAbleToClearInput(By.Name "time_input") "11:22"

[<Fact>]
let ``Can clear month input``() =
  ShouldBeAbleToClearInput(By.Name "month_input") "2017-11"

[<Fact>]
let ``Can clear week input``() =
  ShouldBeAbleToClearInput(By.Name "week_input") "2017-W47"


