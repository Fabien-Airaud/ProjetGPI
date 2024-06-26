pipeline {
    agent any
    environment {
        testAppSite = 'C:\\inetpub\\wwwroot\\ProjetGPI\\TestApp'
        completeAppSite = 'C:\\inetpub\\wwwroot\\ProjetGPI\\CompleteApp'
    }
    
    stages {
        stage('Clean') {
            steps {
                // Nettoyage de la solution
                dotnetClean configuration: 'Release', project: 'ProjetGPI.sln'
                // bat 'dotnet clean ProjetGPI.sln --configuration Release'
            }
        }
        
        // Restauration des dépendances implicite lors de la compilation
        stage('Build') {
            steps {
                // Compilation de la solution
                dotnetBuild configuration: 'Release', project: 'ProjetGPI.sln'
                // bat 'dotnet build ProjetGPI.sln --configuration Release'
            }
        }

        stage('Deploy with Puppet on IIS') {
            steps {
                // Utilisation de Puppet pour créer les sites sur IIS
                // puppetApply manifest: 'sites.pp'
                bat 'puppet apply sites.pp'
            }
        }

        // Même erreur qu'avec dotnetPublish lors des tests
        // stage('Release tests') {
        //     steps {
        //         bat 'dotnet build ProjetGPI.sln /p:PublishProfile="ProjetGPI\\Properties\\PublishProfiles\\ProjetGPIProfile.pubxml" /p:Platform="Any CPU" /p:DeployOnBuild=true /m'
        //     }
        // }

        // stage('Deploy tests') {
        //     steps {
        //         // Arrêt de IIS
        //         bat 'net stop "w3svc"'

        //         // Deploiement des paquets sur le site de test IIS
        //         bat '"C:\\Program Files (x86)\\IIS\\Microsoft Web Deploy V3\\msdeploy.exe" -verb:sync -source:package="ProjetGPI\\bin\\Debug\\net8.0\\ProjetGPI.zip" -dest:auto -setParam:"IIS Web Application Name"="TestAppSite" -skip:objectName=filePath,absolutePath=".\\\\PackageTmp\\\\Web.config$" -enableRule:DoNotDelete -allowUntrusted=true'

        //         // Redémarrage de IIS
        //         bat 'net start "w3svc"'
        //     }
        // }
        
        stage('Publish to Test IIS Site') {
            steps {
                // Arrêter le site de test sur IIS
                bat 'iisreset /stop'
                
                // Publier l'application sur le site de test
                dotnetPublish configuration: 'Release', project: 'ProjetGPI\\ProjetGPI.csproj', outputDirectory: env.testAppSite, noBuild: true
                
                // Redémarrer le site de test sur IIS
                bat 'iisreset /start'
            }
        }
        
        stage('Test') {
            steps {
                // Exécution des tests unitaires et fonctionnels
                dotnetTest configuration: 'Release', project: 'ProjetGPITests\\ProjetGPITests.csproj', logger: 'trx', noRestore: true
                // bat 'dotnet test ProjetGPITests\\ProjetGPITests.csproj --configuration Release --logger:trx --no-restore'
            }
        }

        stage('Publish to Complete IIS Site') {
            when {
                expression {
                    currentBuild.result == null || currentBuild.result == 'SUCCESS'
                }
            }
            steps {
                // Arrêter le site complet sur IIS
                bat 'iisreset /stop'
                
                // Publier l'application sur le site complet
                dotnetPublish configuration: 'Release', project: 'ProjetGPI\\ProjetGPI.csproj', outputDirectory: env.completeAppSite, noBuild: true
                
                // Redémarrer le site complet sur IIS
                bat 'iisreset /start'
            }
        }
    }
    post {
        always {
            script {
                def allStagesSuccessful = currentBuild.result == 'SUCCESS'
                if (allStagesSuccessful) {
                    echo "Toutes les étapes ont réussi et l'appliaction est déployée."
                } else {
                    echo "Le déploiement ou les tests ont échoué."
                }
            }
        }
    }
}
