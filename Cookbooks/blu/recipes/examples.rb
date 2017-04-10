##############################################################
# Executed during compile-time   blu_script_out examples     #
##############################################################

# Start by making sure you can use the blu_script_out by telling Chef to go load it
::Chef::Recipe.send(:include, Chef::Mixin::BluScriptOut)

# Execute some command (in this case checking if powershell is installed)
powershell_installed = blu_script_out('(get-windowsfeature powershell).installed')

# This will raise an exception if there was a non-zero exit status
powershell_installed.error!

# print the results, you could use these in your cookbook logic of course!
puts "Powershell installation status: #{powershell_installed.stdout}"

##############################################################
# Some examples on how to use Blu                            #
##############################################################

blu_script 'Get current user blu is running as' do
  code '[System.Security.Principal.WindowsIdentity]::GetCurrent().Name'
end

blu_script 'Only execute if .NET 4.5 is installed' do
  code 'echo ".NET 4.5 is installed"'
  only_if '(get-windowsfeature NET-Framework-45-Core).installed'
end

# specific runspace usage
blu_script 'Create runspace' do
  runspace cookbook_name
  credentials node['blu_credentials']
  action :create_runspace
end

blu_script 'Get current user blu is running as' do
  code '[System.Security.Principal.WindowsIdentity]::GetCurrent().Name'
  runspace cookbook_name
  only_if '$true', :runspace => cookbook_name
end

blu_script 'Dispose runspace' do
  runspace cookbook_name
  action :dispose_runspace
end