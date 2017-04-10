name             'blu'
maintainer       'Schuberg Philis'
maintainer_email 'wsimons@schubergphilis.com'
license          'All rights reserved'
description      'Installs/Configures blu'
long_description IO.read(File.join(File.dirname(__FILE__), 'README.md'))
version          '3.0.2'

chef_version     '>= 12.5'

depends 'sbp_win_user_management', '=1.0.0'