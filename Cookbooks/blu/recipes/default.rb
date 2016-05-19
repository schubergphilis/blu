#
# Author:: Gitlocker (<aahmadi@schubergphilis.com>)
# Cookbook Name:: blu
# Recipe:: default
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

# Create runtime directory
# ------------------------
directory node['blu']['root'] do
  recursive true 
  action :create
end

if node['blu']['install_blu_service'] == true 
  include_recipe "blu::service"
end
