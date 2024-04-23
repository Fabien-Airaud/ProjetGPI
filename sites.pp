# Variables du projet
$site_name = 'ProjetGPI'
$physical_path = "C:\\inetpub\\wwwroot\\${site_name}"

# Création du dossier physique du projet
file { $physical_path:
    ensure  => directory,
}

# Configurations pour les sites du projet
$configurations_iis = {
    # Configuration pour le site de tests (pré-production)
    'TestAppSite' => {
        'physical_path' => "${physical_path}\\TestApp",
        'port' => 5153,
        'application_pool' => 'TestAppPool',
    },
    # Configuration pour le site complet (production)
    'CompleteAppSite' => {
        'physical_path' => "${physical_path}\\CompleteApp",
        'port' => 5152,
        'application_pool' => 'CompleteAppPool',
    },
}

# Pour chaque site, on crée le dossier physique puis le pool d'application puis le site
$configurations_iis.each |$site, $config| {
    # Création du dossier physique
    file { $config['physical_path']:
        ensure  => directory,
    }
    # Création du pool d'application
    if $config['application_pool'] in iis_application_pool {
        exec { "restart-${config['application_pool']}-pool": # Redémarrage du pool d'application si déjà existant
            command     => "C:\\Windows\\System32\\inetsrv\\appcmd.exe recycle apppool /apppool.name:${config['application_pool']}",
            refreshonly => true,
            require     => File[$config['physical_path']],
        }
    } else {
        iis_application_pool {$config['application_pool']: # Création du pool d'application si inexistant
            ensure                  => 'present',
            state                   => 'started',
            managed_runtime_version => 'v4.0',
            managed_pipeline_mode   => 'Integrated',
            require                 => File[$config['physical_path']], # On attend que le dossier physique soit créé
        }
    }
    # # Cas utilisant un iis_application_pool pour la création du pool d'application
    # iis_application_pool {$config['application_pool']:
    #     ensure                  => 'present',
    #     state                   => 'started',
    #     managed_runtime_version => 'v4.0',
    #     managed_pipeline_mode   => 'Integrated',
    #       require                 => File[$config['physical_path']], # On attend que le dossier physique soit créé
    # }
    # # Cas utilisant un exec pour la création du pool d'application
    # exec { "create-${config['application_pool']}-pool":
    #     command => "C:\\Windows\\System32\\inetsrv\\appcmd.exe add apppool /name:${config['application_pool']} /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated",
    #     onlyif  => "C:\\Windows\\System32\\inetsrv\\appcmd.exe list apppool | findstr /i ${config['application_pool']}",
    #     unless  => "C:\\Windows\\System32\\inetsrv\\appcmd.exe list apppool | findstr /i ${config['application_pool']}",
    #     require => File[$config['physical_path']], # On attend que le dossier physique soit créé
    # }
    # Attente de la création du pool d'application
    exec { "wait-${config['application_pool']}-pool":
        command   => "C:\\Windows\\System32\\inetsrv\\appcmd.exe list apppool | findstr /i:${config['application_pool']}",
        path      => 'C:\\Windows\\System32\\inetsrv',
        tries     => 10,
        try_sleep => 5,
        unless    => "C:\\Windows\\System32\\inetsrv\\appcmd.exe list apppool | findstr /i:${config['application_pool']}",
        require   => File[$config['physical_path']], # On attend que le dossier physique soit créé
    }
    # Création du site
    iis_site { $site:
        ensure          => 'started',
        physicalpath    => $config['physical_path'],
        applicationpool => $config['application_pool'],
        bindings        => [
            {
                'protocol'           => 'http',
                'bindinginformation' => "*:${config['port']}:",
            },
        ],
    }
    # Redémarrage du site IIS si des modifications ont été effectuées
    exec { "restart-${site}-site":
        command     => "C:\\Windows\\System32\\inetsrv\\appcmd.exe recycle apppool /apppool.name:${config['application_pool']}",
        refreshonly => true,
        subscribe   => Iis_site[$site],
    }
}
