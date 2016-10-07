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

# Copy BluShell.exe to the root path
cookbook_file node['blu']['root'] + '\BluShell.exe' do
    source 'BluShell.exe'
    action :create
end

# Copy BluService.exe to the root path as a temp file (to check if the file is updated by cookbook)
cookbook_file node['blu']['root'] + '\_BluService.exe' do
    source 'BluService.exe'
    action :create
    notifies :run, 'powershell_script[register_blu_service]', :immediately
end

# If bluservice.exe file is updated by cookbook: stop service > copy file > start service
# Register service if it is not registered yet
passwords = Chef::EncryptedDataBagItem.load("blu", "passwords")
powershell_script 'register_blu_service' do
  code <<-EOF
    $bluservice_exe_temp_file = "#{node['blu']['root']}\\_BluService.exe";
    $binaryPath = "#{node['blu']['root']}\\BluService.exe";
    $secpasswd = ConvertTo-SecureString "#{passwords[node.chef_environment]}" -AsPlainText -Force
    $credentials = New-Object System.Management.Automation.PSCredential ("#{node.chef_environment + '\svc_blu'}", $secpasswd)
    
    function RegisterBluService
    {
        Copy-Item $bluservice_exe_temp_file $binaryPath;
        if (!(Get-Service "BluService" -ErrorAction SilentlyContinue))
        {
            New-Service -Name BluService `
                -DisplayName "Blu Powershell Runspace Service" `
                -Description "Provides a Runspace to execute PowerShell commands in service mode." `
                -BinaryPathName $binaryPath `
                -StartupType Automatic `
                -Credential $credentials
        }
    }
    
    function UpdateBluService
    {
        Stop-Service BluService;
        Copy-Item $bluservice_exe_temp_file $binaryPath;
    }
    
    if (Test-Path $binaryPath) 
    {
        if ((Get-Item $bluservice_exe_temp_file).LastWriteTime -gt (Get-Item $binaryPath).LastWriteTime)
        {
            if (Get-Service "BluService" -ErrorAction SilentlyContinue)
            {
                UpdateBluService;
            }
            else
            {
                RegisterBluService;
            }
        }
    }
    else 
    { 
        RegisterBluService; 
    }  
  EOF
  action :nothing
  notifies :start, 'service[BluService]', :immediately
end

# Start Blu Service
service 'BluService' do
  startup_type :automatic
  action :nothing
end