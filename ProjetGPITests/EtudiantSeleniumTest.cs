using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ProjetGPITests
{
    public class EtudiantSeleniumTest
    {
        [Fact]
        public void IndexEtudiantTest()
        {
            // Arrange
            var chromeDriver = new ChromeDriver();
            chromeDriver.Navigate().GoToUrl("https://localhost:7212/");

            // Act
            var navbarBrand = chromeDriver.FindElement(By.CssSelector("header nav p.navbar-brand"));
            var etudiants = chromeDriver.FindElements(By.ClassName("etudiant"));

            // Assert
            Assert.Equal("ProjetGPIApp", navbarBrand.Text);
            Assert.NotEmpty(etudiants);

            // Cleanup
            chromeDriver.Quit();
        }
    }
}
