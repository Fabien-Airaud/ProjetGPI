using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetGPI.Controllers;
using ProjetGPI.Models;

namespace ProjetGPITests
{
    public class EtudiantUnitTest
    {
        [Fact]
        public async Task CreateEtudiantTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProjetGPIDbContext>()
                .UseInMemoryDatabase(databaseName: "ProjetGPIDB")
                .Options;

            using var context = new ProjetGPIDbContext(options);
            var controller = new EtudiantsController(context);

            // Initialisation d'un étudiant
            Etudiant etudiant = new()
            {
                Nom = "Doe",
                Prenom = "John",
                Email = "john@example.com",
                Sexe = "Homme",
                DateNais = DateTime.Now.AddYears(-25)
            };

            // Act
            var result = await controller.Create(etudiant) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            // Vérification de l'ajout de l'étudiant dans la base de données
            var etudiantInDatabase = await context.Etudiants.FirstOrDefaultAsync();
            Assert.NotNull(etudiantInDatabase);
            Assert.Equal("Doe", etudiantInDatabase.Nom);
            Assert.Equal("John", etudiantInDatabase.Prenom);
        }
    }
}