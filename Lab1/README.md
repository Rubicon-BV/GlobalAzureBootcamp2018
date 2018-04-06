## 1. QnA Maker

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

## 2. Postman

**Postman** is a popular API client that makes it easy for developers to create, share, test and document APIs. This is done by allowing users to create and save simple and complex HTTP/s requests, as well as read their responses. 

**Requirement:** _You need to register an account to use Postman_

_Our second action is to run a request via Postman_
1. Go to [https://www.getpostman.com/apps](https://www.getpostman.com/apps)
2. Download the [Windows app](https://www.getpostman.com/apps) (…if running on Windows)
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
| Ocp-Apim-Subscription-Key | …from step 1.11 |
| Content-Type | …from step 1.11 |

 ![Postman1](https://github.com/Rubicon-BV/GlobalAzureBootcamp2018/blob/master/Lab1/Pics/Postman1.png)

14. Click on the **Body** tab.
15. Select **Raw**
16. On the body field add **{"question":"cw5"}** or **{"question":"hi"}*17. Click on **Send**, the following illustration is a sample result

 ![Postman2](https://github.com/Rubicon-BV/GlobalAzureBootcamp2018/blob/master/Lab1/Pics/Postman2.png)
 
18.	Click on **Save**

**Requirement:** _Voorkeur voor Azure Engelstalig_ 

## 3. Azure Bot Service

_Our third action is to create a bot via the Azure Bot Service and configure (and test) the channels Skype, Telegram and Facebook messenger._

1. Go to the [Azure Portal](https://portal.azure.com/)
2. Login with your Microsoft account, you need an **Azure subscription**
3. Click on **Create a resource**, located in the top left corner of the screen
4. Search for **BOT**
5. Select _Web App Bot_, and click on **Create**
6. Fill out the following fields
* Bot name (in this example GABC-Testbot)
* Subscription (choose your **subscription**)
* Resource group (**Create New**)
* Provide a **name**
* Location (Select **West Europe”**)
* Pricing Tier (choose **S1 Premium messages/unit**)
* App name (in this example GABC-Testbot)
* Bot Template (choose **Question and Answer template**)
* App service plan **(Create New**)
* Provide a **name**
* Select **West Europe**
7. Azure storage (**Create New**)
* Provide a **name**
8. Application Insights (choose **On**)
9. Location (Select **West Europe**)
10. Microsoft App ID and password (choose **Auto create App ID and Password**)
11. Click on **Create**
12. When the bot is finished click on the right side **Go to resource**
13. On the left menu of the Bot click on **Application settings**
14. Under the section **App settings**, you find:
* QnAKnowledgebaseId (put your QandAmaker settings here, in our example _67c19131-d968-4a7e-9f16-feffb982d506_)
* QnASubscriptionKey (put your QandAmaker settings here, in our example _8e132c2e5c3b43d98c66aa88ee1e9f73_)
15. In the left menu of the Bot click on **Test the Webchat**
  1. Type for example _hi_ or _CW 4/CW 5/CW 3_ and see if the webchat gives you an answer
12. In the left menu of the Bot click on **Channels**
