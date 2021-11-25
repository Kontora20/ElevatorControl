# ElevatorControl

After running project, a swagger page should load with available APIs. The idea is that after you issue a PATCH request to an elevator, it only triggers elevator
to move to a desired floor in a separate thread and you should be able to see elevator status and logs with separate GET APIs.
Configuration changes can be done in appsettings.json file under "ElevatorConfig" section.
