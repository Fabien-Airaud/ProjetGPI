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
        ensure                  => 'present',
        state                   => 'started',
        managed_runtime_version => 'v4.0',
        managed_pipeline_mode   => 'Integrated',
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
        require         => Iis_application_pool[$site], # On attend que le pool d'application soit créé
        # require         => Exec["create-${site}-pool"], # On attend que le pool d'application soit créé
    }
    # Redémarrage du site IIS si des modifications ont été effectuées
    exec { "restart-${site}-site":
        command     => "C:\\Windows\\System32\\inetsrv\\appcmd.exe recycle apppool /apppool.name:${config['application_pool']}",
        refreshonly => true,
        subscribe   => Iis_site[$site],
    }
}
