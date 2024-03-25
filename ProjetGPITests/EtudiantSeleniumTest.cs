using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ProjetGPITests
{
    public class EtudiantSeleniumTest
    {
        static void HeaderBrandTest(IWebDriver driver)
        {
            // Act
            var navbarBrand = driver.FindElement(By.CssSelector("header nav p.navbar-brand"));

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
            var createButton = chromeDriver.FindElement(By.CssSelector("a[href=\"Etudiants/Create\"]"));
            Assert.Equal("Nouvel étudiant", createButton.Text);

            // Cleanup
            chromeDriver.Quit();
        }
    }
}
