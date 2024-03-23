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

            // Initialisation d'un �tudiant
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

            // V�rification de l'ajout de l'�tudiant dans la base de donn�es
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

            // Initialisation d'un �tudiant
            Etudiant etudiant = new()
            {
                Nom = "Doe",
                Prenom = "John",
                Email = "john@example.com",
                Sexe = "Homme",
                DateNais = DateTime.Now.AddYears(-25)
            };

            // Ajout de l'�tudiant dans la base de donn�es
            await controller.Create(etudiant);

            // R�cup�ration de l'identifiant de l'�tudiant
            int etudiantId = etudiant.Id;

            // Act part 1
            var result = await controller.Delete(etudiantId) as ViewResult;

            // Assert
            Assert.NotNull(result);

            // V�rification de la r�cup�ration de l'�tudiant pour la suppression
            var etudiantToDelete = result.Model as Etudiant;
            Assert.NotNull(etudiantToDelete);

            // Act part 2
            // Suppression de l'�tudiant
            var deleteResult = await controller.DeleteConfirmed(etudiantId) as RedirectToActionResult;
            Assert.NotNull(deleteResult);
            Assert.Equal("Index", deleteResult.ActionName);

            // V�rification de la suppression de l'�tudiant de la base de donn�es
            var etudiantInDatabase = await context.Etudiants.FirstOrDefaultAsync(e => e.Id == etudiantId);
            Assert.Null(etudiantInDatabase);
        }
    }
}