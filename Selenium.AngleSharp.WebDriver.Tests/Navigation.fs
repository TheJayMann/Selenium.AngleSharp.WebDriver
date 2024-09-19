module ``Navigation tests``

open System
open Xunit
open AngleSharp
open Selenium.AngleSharp.WebDriver


let currentDirectoryUri = Uri Environment.CurrentDirectory
let mainPageUri = Uri(currentDirectoryUri, "net8.0/pages/main.html")
let page1Uri = Uri(currentDirectoryUri, "net8.0/pages/page1.html")

let [<Literal>] mainPageTitle = "Main Page"
let [<Literal>] page1Title = "Page 1"
 
let ``Create driver``() =
  new AngleSharpDriver(
    Configuration.Default
      .WithDefaultLoader()
      .WithRequesters()
      .WithHistory()
  )
[<Fact>]
let ``Back and forward should work when no pages are loaded`` () =
  use driver = ``Create driver``()
  let navigation = driver.Navigate()
  navigation.Back()
  navigation.Forward()

[<Fact>]
let ``Back and forward should navigate through history`` () =
  use driver = ``Create driver``()
  let navigation = driver.Navigate();
  
  driver.Url <- mainPageUri.ToString()
  driver.Url <- page1Uri.ToString()

  navigation.Back()
  Assert.Equal(mainPageTitle, driver.Title)

  navigation.Forward()
  Assert.Equal(page1Title, driver.Title)

[<Fact>]
let ``GoToUrl should throw argument null exception when given null uri`` () =
  use driver = ``Create driver``()
  let navigation = driver.Navigate()

  Assert.Throws<ArgumentNullException>(fun () -> navigation.GoToUrl(null : Uri)) |> ignore
  Assert.Throws<ArgumentNullException>(fun () -> navigation.GoToUrl(null : string)) |> ignore

[<Fact>]
let ``GoToUrl should load page using string uri`` () =
  use driver = ``Create driver``()
  let navigation = driver.Navigate()

  navigation.GoToUrl(mainPageUri.ToString())
  Assert.Equal(mainPageTitle, driver.Title)

  navigation.GoToUrl(page1Uri.ToString())
  Assert.Equal(page1Title, driver.Title)

[<Fact>]
let ``GoToUrl should load page using uri`` () =
  use driver = ``Create driver``()
  let navigation = driver.Navigate()

  navigation.GoToUrl(mainPageUri)
  Assert.Equal(mainPageTitle, driver.Title)

  navigation.GoToUrl(page1Uri)
  Assert.Equal(page1Title, driver.Title)

[<Fact>]
let ``GoToUrl should maintain history`` () =
  use driver = ``Create driver``()
  let navigation = driver.Navigate()

  navigation.GoToUrl(mainPageUri)
  navigation.GoToUrl(page1Uri)
  Assert.Equal(page1Title, driver.Title)

  navigation.Back() 
  Assert.Equal(mainPageTitle, driver.Title)
