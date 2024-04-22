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
    iis_application_pool {$site:
        ensure                    => 'present',
        enable32_bit_app_on_win64 => true,
        managed_runtime_version   => '',
        managed_pipeline_mode     => 'Classic',
    }
    # exec { "create-${site}-pool":
    #     # On ne recrée pas le pool si il existe déjà
    #     command  => "C:\\Windows\\System32\\inetsrv\\appcmd.exe add apppool /name:${config['application_pool']}",
    #     unless   => "C:\\Windows\\System32\\inetsrv\\appcmd.exe list apppool | findstr /i ${config['application_pool']}",
    #     provider => 'windows',
    #     require  => File[$config['physical_path']], # On attend que le dossier physique soit créé
    # }
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
        require         => Iis_application_pool[$site], # On attend que le pool d'application soit créé
        # require         => Exec["create-${site}-pool"], # On attend que le pool d'application soit créé
    }
    # Redémarrage du site pour prendre en compte les changements
    exec { "restart-${site}-site":
        command     => "C:\\Windows\\System32\\inetsrv\\appcmd.exe recycle apppool /apppool.name:${config['application_pool']}",
        refreshonly => true,
        provider    => 'windows',
        subscribe   => Iis_site[$site],
    }
}


