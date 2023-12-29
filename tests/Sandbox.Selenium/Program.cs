using System.Threading.Tasks;
using GhostCursor;
using GhostCursor.Selenium;
using OpenQA.Selenium.Chrome;

using var webDriver = new ChromeDriver();

webDriver.Navigate().GoToUrl("https://www.google.com/?hl=en");

// Create the cursor
var options = new CursorOptions
{
    Debug = true,
    DefaultSteps = 10
};

var cursor = webDriver.CreateCursor(options);

// Search for "Hello world" with the cursor

await using (await cursor.StartAsync())
{
    await cursor.ClickAsync(ElementSelector.FromXPath("//div[text() = 'Reject all']"));
    await cursor.ClickAsync("[title='Search']");
    await cursor.TypeAsync("Hello world");
    await cursor.ClickAsync("input[value='Google Search']");
}

// Wait for 2 seconds before closing the browser
await Task.Delay(2000);
