#
# Author:: Gitlocker (<aahmadi@schubergphilis.com>)
# Cookbook Name:: blu
# Attribute:: default
#
# Copyright 2015-2016, Schuberg Philis
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
#

default['blu']['root'] = 'c:\blu'

# BluService and BluShell are Windows service and IPC client for Blu PowerShell runspace service
default['blu']['install_blu_service'] = true

# Determines if powershell.exe should be placed in embedded directory (overriding powershell.exe for Chef client)
default['blu']['override_powershell'] = false

# Service account
default['blu']['service_account'] = {
    user: 'blu',
    user_group: 'Administrators',
    create_local_user: true,
    data_bag_name: 'blu' # expecting a password for the mentioned user in the "credentials" sub data bag
}
