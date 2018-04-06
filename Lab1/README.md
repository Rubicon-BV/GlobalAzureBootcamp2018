## QnA Maker

**Microsoft QnA Maker** is a free, easy-to-use, REST API and web-based service that trains AI to respond to user's questions in a more natural, conversational way. Compatible across development platforms, hosting services, and channels, QnA Maker is the only question and answer service with a graphical user interface—meaning you don’t need to be a developer to train, manage, and use it for a wide range of solutions.
With optimized machine learning logic and the ability to integrate industry-leading language processing with ease, QnA Maker distills masses of information into distinct, helpful answers.

![QnA Maker Overview](https://github.com/Rubicon-BV/GlobalAzureBootcamp2018/blob/master/Lab1/Pics/botFrameworkArch.png)

_Our first action is to create a Q and A maker service_
1. Go to the website https://qnamaker.ai/
2. Click on **Create new service**;
3. Fill in the **Name** of your service
4. Choose for adding:
  * URL or a product manual page for a FAQ or
  * Upload a FAQ file in the form of a PDF
5. Click on **Create**
6. Click after creating on the **pencil icon** at the right side
7. Click on **Test** in the left side of the screen and test your application.
8. Click on **Save and retrain**
9. After that click on **Publish**
10. Click on **Publish** again
11. **Save (by copy/paste)** the generated information. A sample of this information is illustrated in the image below.

![QnA Maker2](https://github.com/Rubicon-BV/GlobalAzureBootcamp2018/blob/master/Lab1/Pics/samplehttprequest.png)

## Postman

**Postman** is a popular API client that makes it easy for developers to create, share, test and document APIs. This is done by allowing users to create and save simple and complex HTTP/s requests, as well as read their responses. 

Requirement: _You need to register an account to use Postman_

_Our second action is to run a request via Postman_
1. Go to [https://www.getpostman.com/apps](https://www.getpostman.com/apps)
2. Download the [Windows app](https://app.getpostman.com/app/download/win64?_ga=2.73860118.544879161.1520368971-700872036.1520368971) (…if running on Windows)
3. Install and run
4.	Click on **Request**, located under the tab _“Create new”_
5.	Fill in the **Name** under _“Request name”_
6. Click on **+Create collection”** and enter a name. If you have a collection, choose a collection
7. Click on **Save**
8. Change the _GET_ function into **POST**
9. Copy the **Host URL** from _step 11_ in the previous chapter and paste this HOST URL after the Post Function within Postman.
10. Copy the **POST URL** from _step 11_ in the previous chapter and paste this URL after the HOST URL in the Post Function within Postman.
11. Ensure **Inherith auth from parent** is selected as **Type** under the _Authorization_ tab.
12. Click on the **Headers** tab.
13. Enter the following key/value pairs (gathered from the generated input from step 11 in the previous chapter):

| Key | Value |
| --- | --- |
| Ocp-Apim-Subscription-Key | …from step 11 |
| Content-Type | …from step 11 |

 ![Postman1](https://github.com/Rubicon-BV/GlobalAzureBootcamp2018/blob/master/Lab1/Pics/Postman1.png)

14. Click on the **Body** tab.
15. Select **Raw**
16. On the body field add **{"question":"cw5"}** or **{"question":"hi"}**
17. Click on **Send**, the following illustration is a sample result

 ![Postman2](https://github.com/Rubicon-BV/GlobalAzureBootcamp2018/blob/master/Lab1/Pics/Postman2.png)
 
18.	Click on **Save**

**#
[ANNOTATION:

BY &#39;Tim Lemmob&#39;
ON &#39;2018-03-13T10:45:00&#39;TL
NOTE: &#39;Aangeven dat van een engelse Azure interface uitgegaan wordt. (of iets anders)&#39;] ****Azure Bot Service**

_Our third action is to create a bot via the Azure Bot Service and configure (and test) the channels Skype, Telegram and Facebook messenger._

1. Go to the [Azure Portal](https://portal.azure.com/)
2. Login with your Microsoft account, you need an Azure subscription
3. Click on &quot;Create a resource&quot;, located in the top left corner of the screen
4. Search for BOT
5. Select &quot;Web App Bot&quot;, and click on &quot;Create&quot;
6. Fill out the following fields
  1. Bot name (in this example GABC-Testbot)
  2. Subscription (choose your subscription)
  3. Resource group (Create New)
    1. Provide a name
  4. Location (Select &quot;West Europe&quot;)
  5.
e.Pricing Tier (
#
[ANNOTATION:

BY &#39;Tim Lemmob&#39;
ON &#39;2018-03-13T11:00:00&#39;TL
NOTE: &#39;Even afstemmen of we de Free hier willen opgeven.&#39;]
choose S1 Premium messages/unit)
  6. App name (in this example GABC-Testbot)
  7. Bot Template (choose Question and Answer template)
  8. App service plan (Create New)
    1. Provide a name
    2. Select &quot;West Europe&quot;
  9. Azure storage (Create New)
    1. Provide a name
  10. Application Insights (choose On)
  11. Location (Select &quot;West Europe&quot;)
  12.
l.
#
[ANNOTATION:

BY &#39;Tim Lemmob&#39;
ON &#39;2018-03-13T11:03:00&#39;TL
NOTE: &#39;Kan in principe weg, is de default instelling&#39;]
Microsoft App ID and password (choose Auto create App ID and Password)
7. Click on &quot;Create&quot;
8. When the bot is finished click on the right side Go to resource
9. On the left menu of the Bot click on Application settings
10. Under the section App settings, you find:
  1. QnAKnowledgebaseId (put your QandAmaker settings here, in our example 67c19131-d968-4a7e-9f16-feffb982d506)
  2. QnASubscriptionKey (put your QandAmaker settings here, in our example 8e132c2e5c3b43d98c66aa88ee1e9f73)
11. In the left menu of the Bot click on Test the Webchat
  1. Type for example hi or CW 4/CW 5/CW 3 and see if the webchat gives you an answer
12. In the left menu of the Bot click on Channels
