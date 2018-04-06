#Lab 3

Open the Bot solution from Lab 2 in Visual Studio  
Go to Tools - NuGet Package Manager - Manage NuGet Packages for Solution...  
Go to Browse and search for package Microsoft.IdentityModel.Clients.ActiveDirectory  
Install the latest version of the package to the SimpleEchoBot project

Create a new folder Models under the SimpleEchoBot project and add the 4 classes from Lab3/Code/Models:
- Contact.cs
- EntitiesWrapper.cs
- SalesOrderDetails.cs
- TODO (for creating an appointment)

Create a new folder Dynamics under the SimpleEchoBot project and add the 4 classes from Lab3/Code/Dynamics:
- Authentication.cs
- Configuration.cs
- DynamicsHelper.cs
- Exceptions.cs

Log in to the Azure portal and navigate to your web bot's App Service 
Go to the Application settings blade under Settings
Scroll down to Application settings, and use Add new setting to add the following settings:

- ResourceUrl
- WebAPIUrl
- ApplicationID
- SecretId

The values will be provide to you by Rubicon

