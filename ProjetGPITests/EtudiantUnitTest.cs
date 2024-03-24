using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetGPI.Controllers;
using ProjetGPI.Models;

namespace ProjetGPITests
{
    public class EtudiantUnitTest
    {
        public static IEnumerable<object[]> IndexData =>
        [
            [Array.Empty<Etudiant>()],
            [new Etudiant[] {
                new()
                {
                    Nom = "Doe",
                    Prenom = "John",
                    Email = "john@example.com",
                    Sexe = "Homme",
                    DateNais = DateTime.Now.AddYears(-25)
                }
            }],
            [new Etudiant[] {
                new()
                {
                    Nom = "Doe",
                    Prenom = "John",
                    Email = "john@example.com",
                    Sexe = "Homme",
                    DateNais = DateTime.Now.AddYears(-25)
                },
                new()
                {
                    Nom = "Johnson",
                    Prenom = "Alice",
                    Email = "alice@example.com",
                    Sexe = "Femme",
                    DateNais = DateTime.Now.AddYears(-23)
                }
            }]
        ];

        [Theory]
        [MemberData(nameof(IndexData))]
        public async Task IndexEtudiantTest(Etudiant[] etudiants)
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProjetGPIDbContext>()
                .UseInMemoryDatabase(databaseName: "ProjetGPIDBIndex")
                .Options;

            using var context = new ProjetGPIDbContext(options);
            var controller = new EtudiantsController(context);

            // Act
            foreach (Etudiant etudiant in etudiants) await controller.Create(etudiant) ; // Ajout des étudiants
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Etudiant>>(result.Model);
            Assert.Equal(etudiants.Length, (result.Model as List<Etudiant>).Count);

            // Vérification de la présence du bon nombre d'étudiants dans la base de données
            var etudiantsInDatabase = await context.Etudiants.ToListAsync();
            Assert.Equal(etudiants.Length, etudiantsInDatabase.Count);

            // Suppression des données de la base de données
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task CreateEtudiantTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProjetGPIDbContext>()
                .UseInMemoryDatabase(databaseName: "ProjetGPIDBCreate")
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

            // Suppression des données de la base de données
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task DetailsEtudiantTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProjetGPIDbContext>()
                .UseInMemoryDatabase(databaseName: "ProjetGPIDBDetails")
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
            await controller.Create(etudiant);
            var result = await controller.Details(etudiant.Id) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Etudiant>(result.Model);
            Assert.Equal(etudiant.Id, (result.Model as Etudiant).Id);

            // Suppression des données de la base de données
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task EditEtudiantTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProjetGPIDbContext>()
                .UseInMemoryDatabase(databaseName: "ProjetGPIDBEdit")
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

            // Act
            etudiant.Prenom = "Jane";
            var result = await controller.Edit(etudiant.Id, etudiant) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            // Vérification de la modification de l'étudiant dans la base de données
            var etudiantInDatabase = await context.Etudiants.FirstOrDefaultAsync();
            Assert.NotNull(etudiantInDatabase);
            Assert.Equal(etudiant.Id, etudiantInDatabase.Id);
            Assert.Equal("Doe", etudiantInDatabase.Nom);
            Assert.Equal("Jane", etudiantInDatabase.Prenom);

            // Suppression des données de la base de données
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async Task DeleteEtudiantTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ProjetGPIDbContext>()
                .UseInMemoryDatabase(databaseName: "ProjetGPIDBDelete")
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

            // Suppression des données de la base de données
            context.Database.EnsureDeleted();
        }
    }
}