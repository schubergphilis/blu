BluApi project files are added to BluStation project as linked items. 

Example:
The file ChefAPI.ChefRequest.cs is linked to BluStation.BluApi.ChefAPI.Request.cs
When adding new .cs to BluApi we need to link it to BluStation project.
This is done to avoid embedding BluApi.dll into BluStation.dll and be able to compile dynamically.

