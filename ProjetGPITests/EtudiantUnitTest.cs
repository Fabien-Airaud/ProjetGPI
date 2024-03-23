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

        [Fact]
        public async Task DeleteEtudiantTest()
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

            // Ajout de l'étudiant dans la base de données
            await controller.Create(etudiant);

            // Récupération de l'identifiant de l'étudiant
            int etudiantId = etudiant.Id;

            // Act part 1
            var result = await controller.Delete(etudiantId) as ViewResult;

            // Assert
            Assert.NotNull(result);

            // Vérification de la récupération de l'étudiant pour la suppression
            var etudiantToDelete = result.Model as Etudiant;
            Assert.NotNull(etudiantToDelete);

            // Act part 2
            // Suppression de l'étudiant
            var deleteResult = await controller.DeleteConfirmed(etudiantId) as RedirectToActionResult;
            Assert.NotNull(deleteResult);
            Assert.Equal("Index", deleteResult.ActionName);

            // Vérification de la suppression de l'étudiant de la base de données
            var etudiantInDatabase = await context.Etudiants.FirstOrDefaultAsync(e => e.Id == etudiantId);
            Assert.Null(etudiantInDatabase);
        }
    }
}