#
# Author:: Gitlocker (<aahmadi@schubergphilis.com>)
# Cookbook Name:: blu
# Provider::script
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
require 'win32/service'
use_inline_resources

unless ::Win32::Service.exists?('BluService')
  Chef::Log.warn('--------------------------------------------------------------')
  Chef::Log.warn(' Blu Powershell Service is not installed on this machine yet.')
  Chef::Log.warn(' All blu_script resources are ignored in the current chef run.')
  Chef::Log.warn('--------------------------------------------------------------')
end

action :run do
  new_resource.updated_by_last_action(true)
  execute 'blu_script' do
    cwd node['blu']['root']
    command "#{node['blu']['root']}\\BluShell.exe -Command \"#{new_resource.code}\""
    only_if { ::Win32::Service.exists?('BluService') }
  end
end

action :define do
  new_resource.updated_by_last_action(true)
  execute 'blu_script' do
    cwd node['blu']['root']
    command "#{node['blu']['root']}\\BluShell.exe -Define \"#{new_resource.code}\""
    only_if { ::Win32::Service.exists?('BluService') }
  end
end
