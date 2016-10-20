#
# Author:: Gitlocker (<aahmadi@schubergphilis.com>) / Richard van der Brugge
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

directory node['blu']['root']

remote_directory node['blu']['root'] do
  source 'blu'
  notifies :run, 'execute[create BluService]', :immediately
end

execute 'create BluService' do
  command "sc create \"BluService\" binpath=\"#{node['blu']['root']}\\BluService.exe\" DisplayName=\"Provides a Runspace to execute PowerShell commands in service mode.\" "
  action :nothing
end

windows_service 'BluService' do
  run_as_user  node['blu']['serviceaccount']
  run_as_password  node['blu']['serviceaccount_pw']
  action [:enable, :start]
end
