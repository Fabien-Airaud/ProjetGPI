pipeline {
    agent any

    environment {
        dotnet = 'C:\\Program Files\\dotnet\\dotnet.exe'
        puppet = 'C:\\Program Files\\Puppet Labs\\Puppet\\bin\\puppet.bat'
        testAppSite = 'C:\\inetpub\\wwwroot\\ProjetGPI\\TestApp'
        completeAppSite = 'C:\\inetpub\\wwwroot\\ProjetGPI\\CompleteApp'
    }
    
    stages {

        // stage('Checkout Git') {
        //     steps {
        //         git credentialsId: '561f9ea4-610b-4151-9862-9dfe385dcd87', url: 'https://github.com/Fabien-Airaud/ProjetGPI.git', branch: 'master'
        //     }
        // }

        stage('Clean') {
            steps {
                // Nettoyage de la solution
                dotnetClean configuration: 'Release', project: 'ProjetGPI.sln'
                // bat 'dotnet clean ProjetGPI.sln --configuration Release'
            }
        }
        
        // // Restauration des dépendances implicite lors de la compilation
        // stage('Build') {
        //     steps {
        //         // Compilation de la solution
        //         dotnetBuild configuration: 'Release', project: 'ProjetGPI.sln'
        //         // bat 'dotnet build ProjetGPI.sln --configuration Release'
        //     }
        // }

        // stage('Deploy with Puppet on IIS') {
        //     steps {
        //         // Utilisation de Puppet pour créer les sites sur IIS
        //         puppetApply manifest: 'sites.pp'
        //         // bat 'puppet apply sites.pp'
        //     }
        // }
        
        // stage('Publish to Test IIS Site') {
        //     steps {
        //         // Arrêter le site de test sur IIS
        //         bat 'iisreset /stop'
                
        //         // Publier l'application sur le site de test
        //         dotnetPublish configuration: 'Release', project: 'ProjetGPI\\ProjetGPI.csproj', outputPath: env.testAppSite, noBuild: true
                
        //         // Redémarrer le site de test sur IIS
        //         bat 'iisreset /start'
        //     }
        // }
        
        // stage('Test') {
        //     steps {
        //         // Exécution des tests unitaires et fonctionnels
        //         dotnetTest configuration: 'Release', project: 'ProjetGPITests\\ProjetGPITests.csproj', logger: 'trx', noBuild: true
        //         // bat 'dotnet test ProjetGPITests\\ProjetGPITests.csproj --configuration Release --logger:trx --no-build'
        //     }
        // }

        // stage('Publish to Complete IIS Site') {
        //     when {
        //         expression {
        //             currentBuild.result == null || currentBuild.result == 'SUCCESS'
        //         }
        //     }
        //     steps {
        //         // Arrêter le site complet sur IIS
        //         bat 'iisreset /stop'
                
        //         // Publier l'application sur le site complet
        //         dotnetPublish configuration: 'Release', project: 'ProjetGPI\\ProjetGPI.csproj', outputPath: env.completeAppSite, noBuild: true
                
        //         // Redémarrer le site complet sur IIS
        //         bat 'iisreset /start'
        //     }
        // }
    }
}
