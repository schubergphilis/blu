.. |br| raw:: html

   <br />

######################
Blu PowerShell Service
######################

************************
About PowerShell Service
************************

Powershell_script resource in a normal Chef run is limited to a single PowerShell runspace. |br| 
When the resource execution is completed, all variables and object are garbage collected and are not accesible anymore. |br|  
This has major affects on the way we write cookbooks using PowerShell: a PowerShell configuration script cannot be structured in multiple resources and/or files. |br| 
For extensive PowerShell scripting, this ends up to an unreadable and long scripts in a single powershell_script resource and/or using template for config scripts. |br|
Also Ruby runtime is 32-bit which limits powershell_script to only 32-bit binaries and CmdLets, e.g. Microsoft Exchange Server and SharePoint. |br|

.. note::
	In Windows, It is not possible to run a 64-bit process as a child of a 32-bit process. The only method to communicate between 32-bit and 64-bit processes is IPC (interprocess communicatioin) This method is used in Blu PowerShell service to escape the 32-bit limitation of Ruby.exe  


Blu PowerShell service (a part of the blu cookbook) is meant to address both issues. |br| 
This Windows service which is installed automatically when you add blu cookbook to the node run_list, invokes a PowerShell runspace which is valid during the lifecycle of the Windows service. |br| 
The objects are only garbage collected when it is specifically requested in the recipe. |br| |br|
The following code disposes Blu PowerShell runspace: |br|
   
.. code-block:: ruby

    # Dispose Blu PowerShell Runspace
    blu_script 'DisposeRunspace' do
        code 'DisposeRunspace'
    action :run
    end


.. warning::
	If a cookbook doesn't dispose the runspace in the default recipe, all PowerShell vairables and objects from the previous chef run are still valid. This might cause unexpected results. Therefore it is a good practice to dispose PowerShell runspace at the begin and end of a cookbook code.    


The following screenshot illustrates how Blu PowerShell Service escapes the 32-bit Ruby/Chef boundary during Microsoft Exchange 2016 setup and runs ExSetup.exe in a 64-bit address space:

.. image:: 64bit.png
   :alt: Escaping to 64bit by IPC
   :align: center


******************************
Using blu_script Chef resource
******************************

Chef defines a resource as follows: |br|

A resource is a statement of configuration policy that:

* Describes the desired state for a configuration item
* Declares the steps needed to bring that item to the desired state
* Specifies a resource type—such as package, template, or service
* Lists additional details (also known as resource properties), as necessary
* Are grouped into recipes, which describe working configurations
* Where a resource represents a piece of the system (and its desired state), a provider defines the steps that are needed to bring that piece of the system from its current state into the desired state.


The blu_script resource inherits all behavior of a Chef resource and is defines in the blu cookbook in the script.rb file:

.. code-block:: ruby

    actions :run
    actions :define
    default_action :run
    attribute :code, kind_of: String, required: true


The provider that supports this resource is also defined in the blu cookbook as follow:

.. code-block:: ruby

    action :run do
    new_resource.updated_by_last_action(true)
    execute "blu_script" do
        cwd node['blu']['root']
        command "#{node['blu']['root']}\\BluShell.exe -Command \"#{new_resource.code}\""
        only_if { ::Win32::Service.exists?('BluService') }
    end
    end

    action :define do
    new_resource.updated_by_last_action(true)
    execute "blu_script" do
        cwd node['blu']['root']
        command "#{node['blu']['root']}\\BluShell.exe -Define \"#{new_resource.code}\""
        only_if { ::Win32::Service.exists?('BluService') }
    end
    end




You can use **blu_script** resource in recipe like the **powershell_script** resource. In the following example we load Active Directory management snap-in by the **define** action:

.. code-block:: ruby

    # Load AD module
    blu_script 'Load AD module' do
    code <<-EOF
        If (!(Get-module ActiveDirectory)) { Import-Module ActiveDirectory }
        If (!(Get-module ServerManager)) { Import-Module ServerManager }
    EOF
    action :define
    end


.. note::
	From this point on, your blu_script resources has access to ServerManager and ActiveDirectory snap-ins everywhere in recepies. These snapins remain valid until you dispose the runspace as specified above.    

A good practice is to define variables and snapin by **action :define** and run PowerShell converge scripts by **action :run** so that PowerShell code is more readable and also you can take advantage of other mechanisms of **define** action like type conversion. |br|
**Marshalling** between Ruby and PowerShell is covered later in this document. 

.. warning::
    If you don't check the loaded snapin before loading them, by **If (!(Get-module <name>))** and also do not dispose PowerShell runspace, in the next Chef run you get an error that the required snapin is already loaded. 

 
*******************************
Marshalling and Type conversion
*******************************
Currently there are 4 new convertable data types are added to the blu namespace, namely blu_true, blu_false, blu_nil and blu_array:

Booleans (blu_true / blu_false):
--------------------------------   

You can define blu_true and blu_false in a node attribute, example:

.. code-block:: ruby

    default['myapp']['attribute1'] = 'blu_true'
    default['myapp']['attribute2'] = 'blu_false'

Define them in the blu_script resource:

.. code-block:: ruby

    # Boolean attributes in PowerShell variables
    blu_script 'boolean attributes in powershell variables' do
    code <<-EOF
        $Attribute1  = '#{node['myapp']['attribute1']}'
        $Attribute2  = '#{node['myapp']['attribute2']}'
    EOF
    action :define
    end  

And use them in the blu_script: 
 
.. code-block:: ruby
   
    # Boolean attributes in PowerShell
    blu_script 'boolean attributes in powershell' do
    code <<-EOF
        if ($Attribute1)
        {
            # Do some work
        }
        
        if (!$Attribute2)
        {
            # Do some other works
        }
    EOF
    action :run
    end  
    
    

When the resource action is **define**, Blu PowerShell service marshalls these attributes from string to PowerShell specific boolean types of **$True** and **$False**.

.. note::
	Such a type conversion does not happen when the resource action is ":run". We assume that all variables that need to be converted are defined in the blu_scripts with ":define" action.      


Null (blu_nil):
---------------
When the resource action **:define**; an attribute of string 'blu_nil' is converted to PowerShell **$Null**, example:

.. code-block:: ruby

    default['myapp']['attribute3'] = 'blu_nil'
    
Define $Null in blu_script:

.. code-block:: ruby

    # Null in PowerShell
    blu_script 'null in powershell variable' do
    code <<-EOF
        $Attribute3  = '#{node['myapp']['attribute3']}'
    EOF
    action :define
    end   

Use the variable in blu_script:

.. code-block:: ruby

    blu_script 'null in powershell' do
    code <<-EOF
        if ($Attribute3 -eq $Null)
        {
            # Do some work
        }
    EOF
    action :run
    end   


Array (blu_array@):
-------------------
When the resource action **:define**; an attribute of string 'blu_array' is converted to PowerShell array, the syntax of a blu_array definition is:

.. code-block:: ruby

    my_array = "blu_array@('<string1>', '<string2>', '<string3>')"
    
Example:

.. code-block:: ruby

    default['myapp']['attribute4'] = "blu_array@('pizza', 'ravioli', 'macaroni')"
    
Define $array in blu_script:

.. code-block:: ruby

    # Define array in PowerShell
    blu_script 'array in powershell variables' do
    code <<-EOF
        $Foods  = '#{node['myapp']['attribute4']}'
    EOF
    action :run
    end   

Do something with it:

.. code-block:: ruby

    # Array in PowerShell
    blu_script 'array in powershell' do
    code <<-EOF
        foreach ($ItalianFood in $Foods) {
            # buon appetito
        }
    EOF
    action :run
    end   

**********************
Guards and interpeters
**********************

Because blu_script is a Chef LWRP, all the syntax and rules of a resource gurard and interpreters are valid, example:

.. code-block:: ruby

    default['myapp']['guard'] = 'down'


.. code-block:: ruby

    # Guard in blu_script
    blu_script 'guard example' do
    code <<-EOF  
        if ($Attribute3 -eq $Null)
        {
            # Do some work
        }
    EOF
    action :run 
    only_if { node['myapp']['guard'] == 'down' }
    end

    
*********
Notifiers
*********

You can use blu_script notifiers like other resources in Chef, for example:

.. code-block:: ruby

    # Reboot handler
    reboot 'if_pending' do
    action :nothing
    only_if { reboot_pending? }
    end

    # Notifier in blu_script
    blu_script 'notifier example' do
    code <<-EOF  
        if ($Attribute3 -eq $Null)
        {
            # Do some work
        }
    EOF
    action :run 
    notifies :reboot_now, 'reboot[if_pending]'
    end
    
    
    








