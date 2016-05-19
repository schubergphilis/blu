
![alt tag](https://github.schubergphilis.com/aahmadi/BackslashBlu/blob/master/Common/logo/blu.png)

Blu cookbook contains files and code to install Blu PowerShell Service and BluStation

Requirements
------------
Blu is compiled with **.Net 4.0** so you need either add .Net cookbook to your run_list or add the framework to the windows template. 


Attributes
----------
#### blu::default
<table>
  <tr>
    <th>Key</th>
    <th>Type</th>
    <th>Description</th>
    <th>Default</th>
  </tr>
  <tr>
    <td><tt>['blu']['root']</tt></td>
    <td>Path(string)</td>
    <td>Where to install Blu</td>
    <td><tt>c:\blu</tt></td>
  </tr>
  <tr>
    <td><tt>['blu']['install_blu_service']</tt></td>
    <td>Boolean</td>
    <td>Whether to install Blu Powershell Service</td>
    <td><tt>true</tt></td>
  </tr>
  <tr>
    <td><tt>['blu']['serviceaccount']</tt></td>
    <td>string</td>
    <td>Blu PowerShell service account (domain\user)</td>
    <td><tt>none (use svc_blu)</tt></td>
  </tr>
  <tr>
    <td><tt>['blu']['serviceaccount']</tt></td>
    <td>string</td>
    <td>Blu PowerShell service account password</td>
    <td><tt>none</tt></td>
  </tr>
</table>


Reserved Attributes
-------------------
**These attributes are corresponding to functionalities that are not production ready yet.**   
Please keep them false in Production environments
<table>
  <tr>
    <th>Key</th>
    <th>Type</th>
    <th>Description</th>
    <th>Default</th>
  </tr>
  <tr>
    <td><tt>['blu']['override_powershell']</tt></td>
    <td>Boolean</td>
    <td>Whether to override powershell.exe for Ruby namespace</td>
    <td><tt>false</tt></td>
  </tr>
  <tr>
    <td><tt>['blu']['install_blu_station']</tt></td>
    <td>Path(string)</td>
    <td>Whether to BluStation.dll (Chef API and DSL traspiler)</td>
    <td><tt>false</tt></td>
  </tr>
  <tr>
    <td><tt>['blu']['sprint']</tt></td>
    <td>Path(string)</td>
    <td>Whether to start Blu sprint (independent Blu chef run)</td>
    <td><tt>false</tt></td>
  </tr>
</table>


Usage
-----
Include `blu` in your node's `run_list`:

```json
{
  "name":"my_node",
  "run_list": [
    "recipe[blu]"
  ]
}
```

Contributing
------------
1. Fork the repository on Github
2. Create a named feature branch (like `add_component_x`)
3. Write your change
4. Write tests for your change (if applicable)
5. Run the tests, ensuring they all pass
6. Submit a Pull Request using Github

License and Authors
-------------------
Anoosh Ahmadi (<aahmadi@schubergphilis.com>)  
Copyright 2015-2016, Schuberg Philis
