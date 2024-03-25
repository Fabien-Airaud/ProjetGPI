using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;

namespace ProjetGPITests
{
    public class EtudiantSeleniumTest
    {
        static void HeaderBrandTest(IWebDriver driver)
        {
            // Act
            IWebElement navbarBrand = driver.FindElement(By.CssSelector("header nav p.navbar-brand"));

            // Assert
            Assert.Equal("ProjetGPIApp", navbarBrand.Text);
        }

        [Fact]
        public void IndexEtudiantTest()
        {
            // Arrange
            var chromeDriver = new ChromeDriver();
            chromeDriver.Navigate().GoToUrl("https://localhost:7212/");

            // Check the header brand
            HeaderBrandTest(chromeDriver);

            // Check create button
            IWebElement createButton = chromeDriver.FindElement(By.CssSelector("a[href=\"Etudiants/Create\"]"));
            Assert.Equal("Nouvel étudiant", createButton.Text);

            // Get table rows
            ReadOnlyCollection<IWebElement> tableRows = chromeDriver.FindElements(By.CssSelector("table tr"));

            // Check table headers
            string[] tableHeaders = ["Nom", "Prénom", "Email", "Sexe", "Date de Naissance", ""];
            IWebElement tableHeaderRow = tableRows[0];
            ReadOnlyCollection<IWebElement> tableHeaderCells = tableHeaderRow.FindElements(By.CssSelector("th"));
            Assert.Equal(tableHeaders.Length, tableHeaderCells.Count);
            for (int i = 0; i < tableHeaders.Length; i++)
            {
                Assert.Equal(tableHeaders[i], tableHeaderCells[i].Text);
            }

            // Cleanup
            chromeDriver.Quit();
        }
    }
}
