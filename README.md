<p align="center">
  <img src="Common\logo\blu.png"/>
</p>

Blu Powershell Service
----------------------

The quickes way to get started with Blu PowerShell service is to get a copy of BluService.exe and BluShell.exe and save it into C:\\Blu folder.
Then register the Windows service by::

    sc create "Blu Powershell Runspace Service" binpath= C:\Blu\BluService.exe
    
Then go to C:\Blu folder and execute a simple PowerShell command using BluShell.exe::

    cd C:\Blu
    BluShell.exe -Command 'Get-Command;'
    
This PowerShell command will show you a list of availiable commands, and is running in BluShell seamlessly as inside a PowerShell console.   
Now try::

    BluShell.exe -Command "$a = Get-Command;"
    
Then close the powershell session. 
Normally this is going to grabage collect all the objects and variables that are defined in a PowerShell runspace.  
To illustrate how BluService changes this behaviour, start a command prompt again (or a PowerShell session) and execute::

    cd C:\Blu
    BluShell.exe -Command "$a" 
    
As you can see, the variable $a still returns a valid list of all available commands.   
This new PowerShell behavior, which is extreamly useful in PowerShell automation and specially in Chef cookbooks.  
It happens becuase Blu PowerShell service does not dispose the vaiables in current the scope unless you dispose them manually.  
You can also look at Windows event viewer for event id 271, source BluService to monitor what is running under the hood. 

To know more about many other futures of Blu framework, please continue reading: 

[a link](http://backslashblu.readthedocs.io/)
