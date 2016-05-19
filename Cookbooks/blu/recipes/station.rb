#
# Author:: Gitlocker (<aahmadi@schubergphilis.com>)
# Cookbook Name:: blu
# Recipe:: station
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


# Create configuration in registry HKLM\Software\Blu\Config
# ---------------------------------------------------------
# ClientName
registry_key "HKEY_LOCAL_MACHINE\\Software\\Blu\\Config" do
  values [{
    :name => 'ClientName',
    :type => :string,
    :data => Chef::Config[:node_name]
  }]
  action :create
  recursive true
end

# ClientPath
registry_key "HKEY_LOCAL_MACHINE\\Software\\Blu\\Config" do
  values [{
    :name => 'ClientPath',
    :type => :string,
    :data => node['blu']['conf_dir']
  }]
  action :create
end

# ClientPem
registry_key "HKEY_LOCAL_MACHINE\\Software\\Blu\\Config" do
  values [{
    :name => 'ClientPem',
    :type => :string,
    :data => Chef::Config[:client_key]
  }]
  action :create
end

# ClientRb
registry_key "HKEY_LOCAL_MACHINE\\Software\\Blu\\Config" do
  values [{
    :name => 'ClientRb',
    :type => :string,
    :data => node['blu']['conf_dir'] + "\\client.rb"
  }]
  action :create
end

# NodeName
registry_key "HKEY_LOCAL_MACHINE\\Software\\Blu\\Config" do
  values [{
    :name => 'NodeName',
    :type => :string,
    :data => Chef::Config[:node_name]
  }]
  action :create
end


# Organization
registry_key "HKEY_LOCAL_MACHINE\\Software\\Blu\\Config" do
  values [{
    :name => 'Organization',
    :type => :string,
    :data => Chef::Config[:organization]
  }]
  action :create
end

# OrganizationUri
registry_key "HKEY_LOCAL_MACHINE\\Software\\Blu\\Config" do
  values [{
    :name => 'OrganizationUri',
    :type => :string,
    :data => Chef::Config[:chef_server_url]
  }]
  action :create
end

# Copy BluStation.dll to the script path
# --------------------------------------
cookbook_file node['blu']['root'] + '\BluStation.dll' do
    source 'BluStation.dll'
    action :create
end