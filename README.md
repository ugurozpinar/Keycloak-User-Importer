
![](https://repository-images.githubusercontent.com/620187077/4758a744-2898-475b-9c30-055f7a579625)

# Keycloak User Importer
This project is a console application that allows you to bulk import users from an Excel file to Keycloak. Includes user attributes

# Getting Started
* Prerequisites
* .NET 4.7.2 SDK
* Keycloak Server

# Usage
* Clone this repository.
* Open the solution in Visual Studio.
* Build the solution.

# Configuration
You can configure the following settings in the Program.cs file:

## KeycloakSettings
**ServerUrl**: The URL of the Keycloak server.

**AdminUserName**: The admin username of the Keycloak server.

**AdminPassword**: The admin password of the Keycloak server.

**Realm**: The realm in Keycloak where the users will be imported.


## ExcelSettings
SheetName: The name of the sheet in the Excel file that contains the user data.

I used 4,5,6,7,8th columns for user attributes. You can change variable names and index numbers.

# License
This project is licensed under the MIT License. See the LICENSE file for details.
