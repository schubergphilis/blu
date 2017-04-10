#
# Author:: Gitlocker (<aahmadi@schubergphilis.com>)
# Cookbook Name:: blu
# Recipe::service
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

begin
  credentials = Chef::EncryptedDataBagItem.load(node['blu']['service_account'][:data_bag_name], 'credentials')[node.chef_environment]
rescue Net::HTTPServerException => ex
  err_message = "ERROR: Unable to load the data bag credentials using data bag: #{node['blu']['service_account'][:data_bag_name]}->credentials, inner message #{ex.message}"
  puts err_message
  raise
end

user = node['blu']['service_account'][:user]

# Make sure to set local domain for the user only if not a domain account
if user.include? '\\'
  ntusername = user
else
  ntusername = '.\\' + user
end

# Credentials can be host specific or generic for the entire environment
if credentials.key?(node.name) && credentials[node.name].key?(user)
  password = credentials[node.name][user]
elsif credentials.key?(user)
  password = credentials[user]
else
  raise "Unable to determine credentials for node #{node.name} and user #{user}, please check your data bag"
end


# Create local user (only if the flag is set to true for the service account)
if node['blu']['service_account'][:create_local_user]
  user user do
    password password
  end

  group node['blu']['service_account'][:user_group] do
    action :modify
    members user
    append true
  end

  sbp_win_user_management_lsa "Add #{user} to logon as a service" do
    account user
    action :logon_as_service
  end
end

# Copy BluShell.exe to the root path
cookbook_file node['blu']['root'] + '\BluShell.exe' do
    source 'BluShell.exe'
    action :create
end

# Copy BluRunspace.exe to the root path
cookbook_file node['blu']['root'] + '\BluRunspace.exe' do
  source 'BluRunspace.exe'
  action :create
end

# Copy Automation.dll
cookbook_file node['blu']['root'] + '\System.Management.Automation.dll' do
  source 'System.Management.Automation.dll'
  action :create
end

# Copy CommandLine.dll
cookbook_file node['blu']['root'] + '\CommandLine.dll' do
  source 'CommandLine.dll'
  action :create
end

# Copy BluService.exe to the root path as a temp file (to check if the file is updated by cookbook)
cookbook_file node['blu']['root'] + '\_BluService.exe' do
    source 'BluService.exe'
    action :create
    notifies :run, 'powershell_script[register_blu_service]', :immediately
end


install_service_script = <<-EOF
$bluservice_exe_temp_file = "#{node['blu']['root']}\\_BluService.exe";
$binaryPath = "#{node['blu']['root']}\\BluService.exe";
$secpasswd = ConvertTo-SecureString "#{password}" -AsPlainText -Force
$credentials = New-Object System.Management.Automation.PSCredential ("#{ntusername}", $secpasswd)

function RegisterOrUpdateBluService
{
    $serviceInstalled = Get-Service "BluService" -ErrorAction SilentlyContinue;

    if ($serviceInstalled) {
      Stop-Service BluService;
    }

    Copy-Item $bluservice_exe_temp_file $binaryPath;

    if (!$serviceInstalled)
    {
        New-Service -Name BluService `
            -DisplayName "Blu Powershell Runspace Service" `
            -Description "Provides a Runspace to execute PowerShell commands in service mode." `
            -BinaryPathName $binaryPath `
            -StartupType Automatic `
            -Credential $credentials
    }
}

if (Test-Path $binaryPath) 
{
    if ((Get-Item $bluservice_exe_temp_file).LastWriteTime -gt (Get-Item $binaryPath).LastWriteTime)
    {
        RegisterOrUpdateBluService;
    }
}
else 
{ 
    RegisterOrUpdateBluService; 
}
EOF

powershell_script 'register_blu_service' do
  code install_service_script
  action :nothing
  notifies :start, 'service[BluService]', :immediately
end

# Start Blu Service
service 'BluService' do
  startup_type :automatic
  action :nothing
end
