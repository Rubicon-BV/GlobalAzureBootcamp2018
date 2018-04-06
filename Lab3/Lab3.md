# Lab 3

In Lab 3 we will connect the Bot to a Microsoft Dynamics CRM backend.   

## Step 1
Open the Bot solution from Lab 2 in Visual Studio  
Go to Tools - NuGet Package Manager - Manage NuGet Packages for Solution...  
Go to Browse and search for package Microsoft.IdentityModel.Clients.ActiveDirectory  
Install the latest version of the package to the SimpleEchoBot project

## Step 2
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

## Step 3
Log in to the Azure portal and navigate to your web bot's App Service  
Go to the Application settings blade under Settings  
Scroll down to Application settings, and use Add new setting to add the following settings:  

- ResourceUrl
- WebAPIUrl
- ApplicationID
- SecretId

The values will be provide to you by Rubicon

## Step 4
Add a new dialog to your bot to identify the customer. Name it IdentifyCustomerDialog

Have the dialog ask for the users e-mail address  
Create a new CRM request uri string with the email address returned by LUIS $"contacts?$select=fullname,contactid,firstname&$top=5&$filter=(emailaddress2 eq '{emailAddress}')";
And use the Dynamics helper to execute the request to the Dynamics CRM backend:
	var response = await DynamicsHelper<Contact>.HttpClient.GetAsync(requestContactsUri, HttpCompletionOption.ResponseHeadersRead);
	JObject repsonseAccounts = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());

You may need to add a reference to Newtonsoft.Json and using Newtonsoft.Json.Linq

## Step 5
Add another dialog that will query CRM orders for boilers sold to the user
If multiple boilers are found, prompt the user to make a selection