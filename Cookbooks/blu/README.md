![alt tag](logo.png)
=======

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
    <td>See below</td>
  </tr>
</table>

```ruby
    # Service account
    default['blu']['service_account'] = {
        user: 'blu',
        user_group: 'Administrators',
        create_local_user: true,
        data_bag_name: 'blu' # expecting a password for the mentioned user in the "credentials" sub data bag
    }
```

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
=====

Local user account:
-------------------

Make sure there is an encrypted data bag "blu" > "credentials" available with the following structure:
* environment_name
  * node
    * user:password
  * user:password

**Knife command:**
```
> knife data bag create blu credentials --secret-file path/to/encrypted_data_bag_secret  
```

Example data bag json:
```json
{
  "id": "credentials",
  "_default": {
    "blu": "password"
  },
  "dev": {
    "blu": "password"
  },
  "test": {
    "blu": "password"
  },
  "acc": {
    "srvdb03": {
      "blu": "password"
    }
  },
  "prd": {
    "srvdb03": {
      "blu": "password"
    }
  }
}
```
If a specific user is found for a node, that will be user, otherwise the default password for the environment will be used to create the user account.
The default attributes for user account is sufficient for local user (local user name is 'blu')

```ruby
    # Service account
    default['blu']['service_account'] = {
        user: 'blu',
        user_group: 'Administrators',
        create_local_user: true,
        data_bag_name: 'blu' # expecting a password for the mentioned user in the "credentials" sub data bag
    }
```

Then you can include `blu` in your node's `run_list`:
```json
{
  "name":"my_node",
  "run_list": [
    "recipe[blu]"
  ]
}
```

Domain user account:
--------------------

- Create a domain user (in our example it is svc_blu) with sufficient permissions.
- Using GPO add 'Logon as a service right' to the domain user account (svc_blu) [Computer Configuration -> Policies -> Windows Settings -> Security Settings -> Local Policies -> User Righta Assignment]
- Create a data bag for the user credentials:
**Knife command:**

```
> knife data bag create blu credentials --secret-file path/to/encrypted_data_bag_secret  
```

Example data bag json:

```json
{
  "id": "credentials",
    "dev_chef_environment": {
      "dev_domain\\svc_blu": "password"
    },
    "test_chef_environment": {
      "test_domain\\svc_blu": "password"
    },
    "acc_chef_environment": {
      "acc_domain\\svc_blu": "password"
    },
    "prod_chef_environment": {
      "prod_domain\\svc_blu": "password"
    }
}
```

- Create a role cookbook (e.g. customer_blu_role) and override the service account attributes:

```ruby
   # Service account
    default['blu']['service_account'] = {
    user: node.chef_environment + '\\svc_blu',
    user_group: 'Administrators',
    create_local_user: false,
    data_bag_name: 'blu' # expecting a password for the mentioned user in the "credentials" sub data bag
}
```

Note: 
- Service account name is overridden with: `node.chef_environment + '\\svc_blu'` assuming `node.chef_environment` is the same as Windows domain name. 
- If that is not the case, then you would need a switch statement to define servcie account based on the Windows domain name.
- Attribute `create_local_user` is set to `false`. 

Now you can include `customer_blu_role` in your node's `run_list`:

```json
{
  "name":"my_node",
  "run_list": [
    "recipe[customer_blu_role]"
  ]
}
```

Examples
==========


Run a script using blu
-----------
```ruby
blu_script 'Get current user blu is running as' do
  code '[System.Security.Principal.WindowsIdentity]::GetCurrent().Name'
end
```

Run a script with guard interpreter
-----------
```ruby
blu_script 'Test simple command' do
  code '[System.Security.Principal.WindowsIdentity]::GetCurrent().Name'
  only_if '$true -eq $false'
end
```
The above will never run, because $true will never equal $false

Run a script as another user
-----------
```ruby
blu_script 'Test simple command, with credentials' do
  code '[System.Security.Principal.WindowsIdentity]::GetCurrent().Name'
  credentials "#{user},#{node.chef_environment},#{pass}"
end
```
In this example you see how you can add credentials. The credentials 
string has to match either `<user>,<domain>,<pass>` or `<user>,<pass>`
always without spaces.

Contributing
============
1. Fork the repository on Github
2. Create a named feature branch (like `add_component_x`)
3. Write your change
4. Write tests for your change (if applicable)
5. Run the tests, ensuring they all pass
6. Submit a Pull Request using Github

License and Authors
-------------------
Anoosh Ahmadi (<aahmadi@schubergphilis.com>)  
Wouter Simons (<wsimons@schubergphilis.com>)  

Copyright 2015-2016, Schuberg Philis
