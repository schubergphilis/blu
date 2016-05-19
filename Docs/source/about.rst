About
--------

From the ground up **Blu** is designed to be **Chef** and  **Terraform** compatible. In Terraform it leverages :class:`file` and :class:`remote_exec` provisioners only. It communicates to Chef server as the configuration management system. 

.. note::
    If you are not using Chef as the configuration management system for your infrastructure, then this toolset is probably not useful for you.
    

Chef, which is extensively supported by DevOps community, is one the most methodological ways of configuration management of today's infrastructures. The value of those methods and concepts like resources, providers, data bags, organizations, etc. are not limited to any specific OS. They are as valuable in Windows OS as they are in any other operating system.

**Blu** is a new way to leverage Chef methods in Windows using PowerShell. To use Blu you need a Chef server installed and properly configured in your infrastructure. Blu is a set of Chef client-side tools, following the methods and concepts, accepted by the Chef community.

**BluService.exe** and **BluShell.exe** are server and client exetuables of Blu PowerShell service. BluService which is running as a Windows service is a PowerShell runspace and pipeline which contains PowerShell objects and pipe data during a chef run. One of the major problems of the current powershell_script and dsc resources in chef client is that each PowerShell block is being executed as isolated script lines of code and one piece of code has no access to the previous objects of powershell block. It means each instance of powershell_script is "blind" to what happened before. This is not the way that PowerShell is designed for and such a limitation causes many problems specially when switching (zigzaggin) between PowerShell and Ruby in the course of a cookbook. 
PowerShell is aware of Windows object model and CmdLet data output is a System.Object. If we execute isolated powershell_scripts without system state awareness and then try to interpret the restul (as Object) in the form of ShellOut, we simply cause a hell of trouble shooting when something goes wrong with the script block and also lose state awareness each time we scape to Ruby.
PowerShell service is meant to address this issue: by creating a constant runspace and a single pipeline for each PS block in the main runspace. By this way the powershell_script block is aware of what happened before him (unless we dispose the runspace and garbage collect the objects), so a PowerShell cookbook can be written very structurally where all variables and objects just needs to be defined one time at the first block or the beginning of the cookbook code. You can find more details of this concept in the PowerShell service documnetation.


**BluStation.dll**, the PowerShell CmdLets library of Blu and can be used in conjunction with Ruby chef-client, or as an additional tool-set for the existing cookbooks, or as an Ruby-less chef-client by itself. It is not meant to "replace" chef-client nor to limit your choices by forcing you to choose one syntax to another. It is written to extend your choices, to query the Chef server and execute Chef methods and/or run cookbooks natively on Windows.
**The key concept is 100% compatibility:** Any conflicting behavior of BluStation with the latest version of Chef Client or Chef Server is considered as a problem. 


**Minimum Complexity:** No installation, configuration, or even updating path variables are required. To use Blu, you just add the blu_toolkit cookbook to the runlist of the node and the installation is no more than file copies. The reason behind this portable design is an educated guess: When your are provisioning a machine, the least amount of processing should be required to enable the machine to communicate with the configuration management server. When the file is copied, the rest are just the most familiar PowerShell methods and CmdLets, supporting all PowerShell technologies including DSC.

**DevOps:** Blu is a young project and is under heavy development. The component shared in GitHub are meant to be production ready, but it doens't mean the solutions and ideas that are chosen to achive specific goals are on their ideal average. 
You need to be a real DevOps when using Blu: Don't wait for someone else to save your day; **share your ideas, join the club, change the code as you desire, request a pull and make it work!**
