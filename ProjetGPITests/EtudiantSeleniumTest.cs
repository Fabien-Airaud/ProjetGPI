using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ProjetGPI.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ProjetGPITests
{
    public class EtudiantSeleniumTest
    {
        private static readonly string baseUrl = "https://localhost:7212/";
        private static readonly string[] indexTableHeaders = ["Nom", "Prénom", "Email", "Sexe", "Date de Naissance", ""];


        private static void CheckHeaderBrand(IWebDriver driver)
        {
            IWebElement navbarBrand = driver.FindElement(By.CssSelector("header nav p.navbar-brand"));
            Assert.Equal("ProjetGPIApp", navbarBrand.Text);
        }

        private static void ClickButtonLink(IWebDriver driver, string linkUrl, string linkText)
        {
            IList<IWebElement> links = driver.FindElements(By.CssSelector("a[role='button']"));
            IWebElement link = links.First(l => l.GetAttribute("href") == linkUrl);
            Assert.Equal(linkText, link.Text);
            link.Click();
            Assert.StartsWith(linkUrl, driver.Url);
        }

        private static void CheckHasBackButton(IWebDriver driver, string backUrl)
        {
            IList<IWebElement> buttons = driver.FindElements(By.CssSelector("a[role='button']"));
            IWebElement backButton = buttons.First(b => b.GetAttribute("href") == backUrl);
            Assert.Equal("Retour", backButton.Text);
        }

        private static void IndexTableDataRowTest(IList<IWebElement> rowCells)
        {
            // Check Etudiant data
            Assert.Equal(indexTableHeaders.Length, rowCells.Count);
            for (int i = 0; i < rowCells.Count - 1; i++)
            {
                Assert.True(rowCells[i].Text.Length > 0);
            }

            // Check row action buttons
            IList<IWebElement> rowButtons = rowCells[^1].FindElements(By.CssSelector("a"));
            Assert.Equal(3, rowButtons.Count);
            Assert.Equal("Editer", rowButtons[0].Text);
            Assert.StartsWith(baseUrl + "Etudiants/Edit/", rowButtons[0].GetAttribute("href"));
            Assert.Equal("Détails", rowButtons[1].Text);
            Assert.StartsWith(baseUrl + "Etudiants/Details/", rowButtons[1].GetAttribute("href"));
            Assert.Equal("Supprimer", rowButtons[2].Text);
            Assert.StartsWith(baseUrl + "Etudiants/Delete/", rowButtons[2].GetAttribute("href"));
        }

        [Fact]
        public void IndexEtudiantTest()
        {
            // Arrange
            var chromeDriver = new ChromeDriver();
            chromeDriver.Navigate().GoToUrl(baseUrl);

            // Check the header brand
            CheckHeaderBrand(chromeDriver);

            // Get table rows
            IList<IWebElement> tableRows = chromeDriver.FindElements(By.CssSelector("table tr"));

            // Check table headers
            IWebElement tableHeaderRow = tableRows[0];
            IList<IWebElement> tableHeaderCells = tableHeaderRow.FindElements(By.CssSelector("th"));
            Assert.Equal(indexTableHeaders.Length, tableHeaderCells.Count);
            for (int i = 0; i < indexTableHeaders.Length; i++)
            {
                Assert.Equal(indexTableHeaders[i], tableHeaderCells[i].Text);
            }

            // Check table data
            for (int i = 1; i < tableRows.Count; i++)
            {
                IList<IWebElement> rowCells = tableRows[i].FindElements(By.CssSelector("td"));
                IndexTableDataRowTest(rowCells);
            }

            // Cleanup
            chromeDriver.Quit();
        }

        private static void FillInput(IWebDriver driver, string inputId, string labelText, string value, bool clearValue)
        {
            // Click on label to focus input
            IWebElement label = driver.FindElement(By.CssSelector($"label[for='{inputId}']"));
            Assert.Equal(labelText, label.Text);
            label.Click();

            // Check input is selected
            IWebElement active = driver.SwitchTo().ActiveElement();
            Assert.Equal("input", active.TagName);
            Assert.Equal(inputId, active.GetAttribute("id"));

            // Fill input
            if (clearValue) active.Clear();
            active.SendKeys(value);
        }

        private static void FillRadio(IWebDriver driver, string radioId, string labelText, string[] radioValues, string selectValue)
        {
            // Click on label to focus radio group
            IWebElement label = driver.FindElement(By.CssSelector($"label[for='{radioId}']"));
            Assert.Equal(labelText, label.Text);
            label.Click();

            // Check radios
            IList<IWebElement> radios = driver.FindElements(By.Id(radioId));
            Assert.Equal(radioValues.Length, radios.Count);
            Assert.Equal(radios.First(), driver.SwitchTo().ActiveElement()); // Check first radio is selected
            foreach (IWebElement radio in radios)
            {
                Assert.Equal("radio", radio.GetAttribute("type"));
                Assert.Equal(radioId, radio.GetAttribute("id"));
                Assert.Contains(radio.GetAttribute("value"), radioValues);
            }

            // Select radio
            IWebElement selectRadio = driver.FindElement(By.CssSelector($"input[type='radio'][id='{radioId}'][value='{selectValue}']"));
            selectRadio.Click();
        }

        private static void EtudiantForm(IWebDriver driver, Etudiant etudiant, bool clearValues = true)
        {
            // Fill form
            FillInput(driver, "Nom", "Nom", etudiant.Nom, clearValues);
            FillInput(driver, "Prenom", "Prénom", etudiant.Prenom, clearValues); ;
            FillInput(driver, "Email", "Email", etudiant.Email, clearValues);
            FillRadio(driver, "Sexe", "Sexe", ["Homme", "Femme", "Autre"], etudiant.Sexe);
            FillInput(driver, "DateNais", "Date de Naissance", etudiant.DateNais!.Value.ToString("MM/dd/yyyy"), clearValues); // Date format: MM/dd/yyyy

            // Submit form
            IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            Assert.Equal("Enregistrer", submitButton.GetAttribute("value"));
            submitButton.Click();
        }

        [Fact]
        public void CreateEtudiantTest()
        {
            // Arrange
            var chromeDriver = new ChromeDriver();
            chromeDriver.Navigate().GoToUrl(baseUrl);

            // Get number of rows before creating a new one
            IList<IWebElement> tableRows = chromeDriver.FindElements(By.CssSelector("table tr"));
            int rowCount = tableRows.Count;

            // Go to create page
            ClickButtonLink(chromeDriver, baseUrl + "Etudiants/Create", "Nouvel étudiant");

            // Check the header brand
            CheckHeaderBrand(chromeDriver);

            // Check title
            IWebElement title = chromeDriver.FindElement(By.CssSelector("h2"));
            Assert.Equal("Nouvel Etudiant", title.Text);

            // Check if back button is present
            CheckHasBackButton(chromeDriver, baseUrl);

            // Fill and submit form
            Etudiant etudiant = new()
            {
                Nom = "TestNom",
                Prenom = "TestPrénom",
                Email = "TestEmail@email.com",
                Sexe = "Femme",
                DateNais = DateTime.Parse("01/01/2000")
            };
            EtudiantForm(chromeDriver, etudiant, false);
            Assert.StartsWith(baseUrl, chromeDriver.Url);

            // Check if row was added
            tableRows = chromeDriver.FindElements(By.CssSelector("table tr"));
            Assert.Equal(rowCount + 1, tableRows.Count);

            // Cleanup
            chromeDriver.Quit();
        }
    }
}
