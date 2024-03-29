using Microsoft.IdentityModel.Tokens;
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

        private static void FillInput(IWebDriver driver, string inputId, string labelText, string value)
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
            string inputValue = active.GetAttribute("value");
            if (!inputValue.IsNullOrEmpty() && active.GetAttribute("type") == "date") inputValue = DateTime.Parse(inputValue).ToString("MM/dd/yyyy");
            if (!inputValue.Equals(value))
            {
                if (!inputValue.IsNullOrEmpty()) active.Clear();
                active.SendKeys(value);
            }
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

        private static void EtudiantCreateForm(IWebDriver driver, Etudiant etudiant)
        {
            // Fill form
            FillInput(driver, "Nom", "Nom", etudiant.Nom);
            FillInput(driver, "Prenom", "Prénom", etudiant.Prenom);
            FillInput(driver, "Email", "Email", etudiant.Email);
            FillRadio(driver, "Sexe", "Sexe", ["Homme", "Femme", "Autre"], etudiant.Sexe);
            FillInput(driver, "DateNais", "Date de Naissance", etudiant.DateNais!.Value.ToString("MM/dd/yyyy")); // Date format: MM/dd/yyyy

            // Submit form
            IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            Assert.Equal("Enregistrer", submitButton.GetAttribute("value"));
            submitButton.Click();
        }

        private static void CheckEtudiantRow(IWebDriver driver, Etudiant etudiant, int rowNumber = -1)
        {
            // Check if row was added
            IList<IWebElement> tableRows = driver.FindElements(By.CssSelector("table tr"));
            IWebElement lastRow = rowNumber == -1 ? tableRows.Last() : tableRows[rowNumber];
            IList<IWebElement> rowCells = lastRow.FindElements(By.CssSelector("td"));

            // Check row data
            Assert.Equal(etudiant.Nom, rowCells[0].Text);
            Assert.Equal(etudiant.Prenom, rowCells[1].Text);
            Assert.Equal(etudiant.Email, rowCells[2].Text);
            Assert.Equal(etudiant.Sexe, rowCells[3].Text);
            Assert.Equal(etudiant.DateNais!.Value.ToString("M/d/yyyy"), rowCells[4].Text);
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
            EtudiantCreateForm(chromeDriver, etudiant);
            Assert.StartsWith(baseUrl, chromeDriver.Url);

            // Check if row was added
            tableRows = chromeDriver.FindElements(By.CssSelector("table tr"));
            Assert.Equal(rowCount + 1, tableRows.Count);
            CheckEtudiantRow(chromeDriver, etudiant);

            // Cleanup
            chromeDriver.Quit();
        }

        private static void EtudiantEditForm(IWebDriver driver, Etudiant etudiant)
        {
            // Fill form
            FillInput(driver, "Nom", "Nom", etudiant.Nom);
            FillInput(driver, "Prenom", "Prénom", etudiant.Prenom);
            FillInput(driver, "Email", "Email", etudiant.Email);
            FillInput(driver, "Sexe", "Sexe", etudiant.Sexe);
            FillInput(driver, "DateNais", "Date de Naissance", etudiant.DateNais!.Value.ToString("MM/dd/yyyy")); // Date format: MM/dd/yyyy

            // Submit form
            IWebElement submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            Assert.Equal("Enregistrer", submitButton.GetAttribute("value"));
            submitButton.Click();
        }

        [Fact]
        public void EditEtudiantTest()
        {
            // Arrange
            var chromeDriver = new ChromeDriver();
            chromeDriver.Navigate().GoToUrl(baseUrl);

            // Get the first Etudiant row data before editing and get the number of rows
            IList<IWebElement> tableRows = chromeDriver.FindElements(By.CssSelector("table tr"));
            int rowCount = tableRows.Count;
            IList<IWebElement> rowCells = tableRows[1].FindElements(By.CssSelector("td"));
            Etudiant oldEtudiant = new()
            {
                Nom = rowCells[0].Text,
                Prenom = rowCells[1].Text,
                Email = rowCells[2].Text,
                Sexe = rowCells[3].Text,
                DateNais = DateTime.Parse(rowCells[4].Text)
            };

            // Go to edit page
            IWebElement editButton = rowCells[^1].FindElements(By.CssSelector("a"))[0];
            editButton.Click();
            Assert.StartsWith(baseUrl + "Etudiants/Edit/", chromeDriver.Url);

            // Check the header brand
            CheckHeaderBrand(chromeDriver);

            // Check titles
            IWebElement title = chromeDriver.FindElement(By.CssSelector("h2"));
            Assert.Equal("Editer", title.Text);
            title = chromeDriver.FindElement(By.CssSelector("h4"));
            Assert.Equal("Etudiant", title.Text);

            // Check if back button is present
            CheckHasBackButton(chromeDriver, baseUrl);

            // Fill and submit form
            Etudiant newEtudiant = new()
            {
                Nom = "TestNom",
                Prenom = "TestPrénom",
                Email = oldEtudiant.Email,
                Sexe = "Autre",
                DateNais = oldEtudiant.DateNais
            };
            EtudiantEditForm(chromeDriver, newEtudiant);
            Assert.StartsWith(baseUrl, chromeDriver.Url);

            // Check if row was added
            tableRows = chromeDriver.FindElements(By.CssSelector("table tr"));
            Assert.Equal(rowCount, tableRows.Count); // No new row was added after editing
            CheckEtudiantRow(chromeDriver, newEtudiant, 1);

            // Cleanup
            chromeDriver.Quit();
        }

        private static void DetailsEtudiantList(IWebDriver driver, Etudiant etudiant)
        {
            // Check Etudiant details by getting all dt and dd elements in dl
            IList<IWebElement> names = driver.FindElements(By.CssSelector("dl > *"));
            Assert.Equal(5 * 2, names.Count);

            Assert.Equal("dt", names[0].TagName);
            Assert.Equal("Nom", names[0].Text);
            Assert.Equal("dd", names[1].TagName);
            Assert.Equal(etudiant.Nom, names[1].Text);

            Assert.Equal("dt", names[2].TagName);
            Assert.Equal("Prénom", names[2].Text);
            Assert.Equal("dd", names[3].TagName);
            Assert.Equal(etudiant.Prenom, names[3].Text);

            Assert.Equal("dt", names[4].TagName);
            Assert.Equal("Email", names[4].Text);
            Assert.Equal("dd", names[5].TagName);
            Assert.Equal(etudiant.Email, names[5].Text);

            Assert.Equal("dt", names[6].TagName);
            Assert.Equal("Sexe", names[6].Text);
            Assert.Equal("dd", names[7].TagName);
            Assert.Equal(etudiant.Sexe, names[7].Text);

            Assert.Equal("dt", names[8].TagName);
            Assert.Equal("Date de Naissance", names[8].Text);
            Assert.Equal("dd", names[9].TagName);
            Assert.Equal(etudiant.DateNais!.Value.ToString("M/d/yyyy"), names[9].Text);
        }

        [Fact]
        public void DetailsEtudiantTest()
        {
            // Arrange
            var chromeDriver = new ChromeDriver();
            chromeDriver.Navigate().GoToUrl(baseUrl);

            // Get the first Etudiant row data before viewing details
            IList<IWebElement> tableRows = chromeDriver.FindElements(By.CssSelector("table tr"));
            IList<IWebElement> rowCells = tableRows[1].FindElements(By.CssSelector("td"));
            Etudiant etudiant = new()
            {
                Nom = rowCells[0].Text,
                Prenom = rowCells[1].Text,
                Email = rowCells[2].Text,
                Sexe = rowCells[3].Text,
                DateNais = DateTime.Parse(rowCells[4].Text)
            };

            // Go to details page
            IWebElement detailsButton = rowCells[^1].FindElements(By.CssSelector("a"))[1];
            detailsButton.Click();
            Assert.StartsWith(baseUrl + "Etudiants/Details/", chromeDriver.Url);

            // Check the header brand
            CheckHeaderBrand(chromeDriver);

            // Check titles
            IWebElement title = chromeDriver.FindElement(By.CssSelector("h2"));
            Assert.Equal("Details", title.Text);
            title = chromeDriver.FindElement(By.CssSelector("h4"));
            Assert.Equal("Etudiant", title.Text);

            // Check Etudiant data
            DetailsEtudiantList(chromeDriver, etudiant);

            // Go back to index
            ClickButtonLink(chromeDriver, baseUrl, "Retour");
            Assert.StartsWith(baseUrl, chromeDriver.Url);

            // Cleanup
            chromeDriver.Quit();
        }

        [Fact]
        public void DeleteEtudiantTest()
        {
            // Arrange
            var chromeDriver = new ChromeDriver();
            chromeDriver.Navigate().GoToUrl(baseUrl);

            // Get the last Etudiant row data before deleting it and get the number of rows
            IList<IWebElement> tableRows = chromeDriver.FindElements(By.CssSelector("table tr"));
            int rowCount = tableRows.Count;
            IList<IWebElement> rowCells = tableRows[^1].FindElements(By.CssSelector("td"));
            Etudiant etudiant = new()
            {
                Nom = rowCells[0].Text,
                Prenom = rowCells[1].Text,
                Email = rowCells[2].Text,
                Sexe = rowCells[3].Text,
                DateNais = DateTime.Parse(rowCells[4].Text)
            };

            // Go to delete page
            IWebElement deleteButton = rowCells[^1].FindElements(By.CssSelector("a"))[2];
            deleteButton.Click();
            Assert.StartsWith(baseUrl + "Etudiants/Delete/", chromeDriver.Url);

            // Check the header brand
            CheckHeaderBrand(chromeDriver);

            // Check titles
            IWebElement title = chromeDriver.FindElement(By.CssSelector("h2"));
            Assert.Equal("Supprimer", title.Text);
            title = chromeDriver.FindElement(By.CssSelector("h3"));
            Assert.Equal("Voulez-vous vraiment supprimer cet étudiant ?", title.Text);
            title = chromeDriver.FindElement(By.CssSelector("h4"));
            Assert.Equal("Etudiant", title.Text);

            // Check if back button is present
            CheckHasBackButton(chromeDriver, baseUrl);

            // Check Etudiant data
            DetailsEtudiantList(chromeDriver, etudiant);

            // Delete Etudiant
            IWebElement submitButton = chromeDriver.FindElement(By.CssSelector("input[type='submit']"));
            Assert.Equal("Supprimer", submitButton.GetAttribute("value"));
            submitButton.Click();
            Assert.StartsWith(baseUrl, chromeDriver.Url);

            // Cleanup
            chromeDriver.Quit();
        }
    }
}
