using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;

namespace ProjetGPITests
{
    public class EtudiantSeleniumTest
    {
        private static readonly string indexUrl = "https://localhost:7212/";
        private static readonly string[] indexTableHeaders = ["Nom", "Prénom", "Email", "Sexe", "Date de Naissance", ""];


        private static void HeaderBrandTest(IWebDriver driver)
        {
            // Act
            IWebElement navbarBrand = driver.FindElement(By.CssSelector("header nav p.navbar-brand"));

            // Assert
            Assert.Equal("ProjetGPIApp", navbarBrand.Text);
        }

        private static void IndexTableDataRowTest(ReadOnlyCollection<IWebElement> rowCells)
        {
            // Check Etudiant data
            Assert.Equal(indexTableHeaders.Length, rowCells.Count);
            for (int i = 0; i < rowCells.Count - 1; i++)
            {
                Assert.True(rowCells[i].Text.Length > 0);
            }

            // Check row action buttons
            ReadOnlyCollection<IWebElement> rowButtons = rowCells[^1].FindElements(By.CssSelector("a"));
            Assert.Equal(3, rowButtons.Count);
            Assert.Equal("Editer", rowButtons[0].Text);
            Assert.StartsWith(indexUrl + "Etudiants/Edit/", rowButtons[0].GetAttribute("href"));
            Assert.Equal("Détails", rowButtons[1].Text);
            Assert.StartsWith(indexUrl + "Etudiants/Details/", rowButtons[1].GetAttribute("href"));
            Assert.Equal("Supprimer", rowButtons[2].Text);
            Assert.StartsWith(indexUrl + "Etudiants/Delete/", rowButtons[2].GetAttribute("href"));
        }

        [Fact]
        public void IndexEtudiantTest()
        {
            // Arrange
            var chromeDriver = new ChromeDriver();
            chromeDriver.Navigate().GoToUrl(indexUrl);

            // Check the header brand
            HeaderBrandTest(chromeDriver);

            // Get table rows
            ReadOnlyCollection<IWebElement> tableRows = chromeDriver.FindElements(By.CssSelector("table tr"));

            // Check table headers
            IWebElement tableHeaderRow = tableRows[0];
            ReadOnlyCollection<IWebElement> tableHeaderCells = tableHeaderRow.FindElements(By.CssSelector("th"));
            Assert.Equal(indexTableHeaders.Length, tableHeaderCells.Count);
            for (int i = 0; i < indexTableHeaders.Length; i++)
            {
                Assert.Equal(indexTableHeaders[i], tableHeaderCells[i].Text);
            }

            // Check table data
            for (int i = 1; i < tableRows.Count; i++)
            {
                ReadOnlyCollection<IWebElement> rowCells = tableRows[i].FindElements(By.CssSelector("td"));
                IndexTableDataRowTest(rowCells);
            }

            // Cleanup
            chromeDriver.Quit();
        }

        [Fact]
        public void CreateEtudiantTest()
        {
            // Arrange
            var chromeDriver = new ChromeDriver();
            chromeDriver.Navigate().GoToUrl(indexUrl);

            // Get table rows
            ReadOnlyCollection<IWebElement> tableRows = chromeDriver.FindElements(By.CssSelector("table tr"));

            // Get number of rows before creating a new one
            int rowCount = tableRows.Count;

            // Check create button
            IWebElement createButton = chromeDriver.FindElement(By.CssSelector("a[href='Etudiants/Create']"));
            Assert.Equal("Nouvel étudiant", createButton.Text);

            // Cleanup
            chromeDriver.Quit();
        }
    }
}
