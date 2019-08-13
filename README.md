# Acme

## Introduction
Acme is C# (.Net frameword 4.7.2) based solution. It relies on Umbraco 8.1.1 to content provision and driving '\acme' website. 
This document describes Umbraco and Project setup and their details to help reset or understand the solution setup.  

---
## Pre-requisites
1. Visual Studio 2017+
2. .Net Framework 4.2.7
3. Git Client (Tortoise, GitExtension, Source Tree etc. )

## Umbraco 
Umbraco 8.1.1 is used to setup the document types, templates and content for the '\acme\' website. Following are its setup and further 
details. 

### Local Setup

#### When IIS is not installed locally:
1. Download and install [Visual Studio Code](https://code.visualstudio.com/)
2. Download and install [IIS Express](https://www.microsoft.com/en-us/download/details.aspx?id=48264) _(Optional, as IIS Expres for VSCode will install it)_
3. Download and install [IIS Express Extension for VS Code](https://marketplace.visualstudio.com/items?itemName=warren-buckley.iis-express)
4. Download [Umbraco](https://our.umbraco.com/download) zip file.
5. Unzip umbraco to a folder as per your preference. We are assuming it is "C:\Umbraco_8_1_1" to be used as reference in later steps.
6. Start __Visual Studio Code__ and open the "C:\Umbraco_8_1_1" folder in it (where we unzipped downloaded Umbraco zip file).
7. Run **(Ctrl+F5)** the Umbraco website. It will implicitly be picked up and run by _IIS express Extension_.
8. With IIS Express the url of Umbraco website would be like *http://localhost:40293* where "40293" is the port number and can be different then you.
9. Browse the currently running Umbraco website _(It should implicitly be done as soon as you start the website from Visual Code)_
10. The first screen would say __"Install Umbraco 8"__. Just provide your Name, Email and a password. _This detail will be used later by you 
to login to Umbraco backend as an administrator_. Click **"Customize"**.
11. The next screen would be **Configure an ASP.Net Machine Key**, Just click **"Continue"**
12. The next screen would be **Configure your database**. Just choose *Microsoft SQL Server Compact (SQL CE)* and click **"Continue"**.
13. The next screen would be **Install a starter website** click on the image rather than *"No thanks, I do not..."* link. It will install the 
sample website within umbraco which will help you to use it as reference while creating a website. __*"No thanks, I do not..."* link should be used where working 
for a dedicated website and you want to start from scratch__ 
14. The installation will start to setup the umbraco locally.
15. Once done you can browse to http:\\localhost:<IIS Express assigned port>\umbraco to browse to the backend 
16. Any created website will be browsed by visiting http:\\localhost:<IIS Express assigned port>

#### When IIS is installed locally:
1. Download [Umbraco](https://our.umbraco.com/download) zip file.
2. Unzip umbraco to a folder as per your preference. We are assuming it is "C:\Umbraco_8_1_1" to be used as reference in later steps.
3. Run "IIS"
4. Create a new website e.g. 'Acme'. In the **Add website** dialog:
	* Make sure the physical path is pointing to the folder where the unzipped content of umbraco are placed e.g. "C:\Umbraco_8_1_1" in our case.
	* Provide a host name e.g. 'Acme'
	* Keep all other settings as they are for basic setup.
	* Click OK
5. **Note**: The web site will not run when you will browse "http://Acme" or "http://Acme/Umbraco". You will have to register this host name "Acme" with local IP i.e. 127.0.0.1
in the *(C:\Windows\System32\drivers\etc\host)* file. (Make sure you edit it in Admin mode)
6. Once registered to host file, browse to the Acme. You will be presented with same installation screens as described above under **"When IIS is not installed locally"** section on step 10 onwards.
7. Install Umbraco same as step 10 onward in section above or as per your needs.   

---
## Visual Studio Code Setup
* Clone the the code from [github](https://github.com/KamranIE/Acme.git)
* Create a feature branch from master. e.g. feature/NewAthletePage
* Switch to the new branch
* You will find following solution with projects 
	1. Acme.sln
	2. Acme.UI
	3. Acme.Umbraco.Models
	
The details of the 3 is as follows:
### Acme.sln
It is the solution that contains the two projects "Acme.UI" and "Acme.Umbraco.Models". Always open this solution file in visual studio whenever you want to modify any of 
the two projects.

### Acme.UI Project
It is an ASP.Net MVC project and contains all the custome controllers, views and out custom handlers to the umbraco request pipeline. 
Please setup following after opening this project:
1. In the Project properties "Build Events" copy following lines to "Pre-build event command line:" 
`tasklist /FI "IMAGENAME eq VBCSCompiler.exe" 2>NUL | find /I /N "VBCSCompiler.exe">NUL
if "%ERRORLEVEL%"=="0" (taskkill /IM VBCSCompiler.exe /F) else (verify >NUL)`
_**Note: Above lines are necessary to kill the VBCSCompiler before you start building. Otherwise you will encounter build/publish issues as this process locks the files every time you compile and for some reason doesn't release the lock most of the times**_
2. You need to setup a publish profile as follows to help you publish to the right folder and debug. *(It should already be there - just modify it as per your settings if it is)*
	1. Name e.g. 'LocalDev'
	2. Publish Method: File System
	3. Target location: <Umbraco web site e.g. C:\Projects\Acme\UmbracoCms.8.1.1>
	4. Configuration: Debug
### Acme.Umbraco.Models Project
* It contains all the files from Umbraco models folder (e.g. \Acme\UmbracoCms.8.1.1\App_Data\Models in our case). 
* __Important:__ The files from models folder are not automatically copied and included to the project. You will have to include any new changes to the project manually.
 
---
## Git setup
The git is setup to ignore following files: 
1. `UmbracoCms.8.1.1/*`
2. `/src/.vs`
3. `/src/*/obj/*`
4. `/src/*/bin/*`
5. `/src/packages`
6. `/src/Acme.UI/fonts`
7. `/NuCache*`

Also besides Acme solution and projects the \UmbracoCms.8.1.1\App_Data\Models folder is included to be checked in. What that means is in case of any changes in the document types (addition, deletion or modification of properties or document types themselves), you need to do following:
1. If there is/are new document type(s)
	1. Add it to git changeset
	2. Include it to Acme.Umbraco.Models as existing item and with same properties as other models in that project.
	3. Make sure the new file is part of the changeset from both locations (i.e. Umbraco Models and Acme.Umbraco.Models)
2. If there is deletion of document type:	
	1. make sure it is deleted in the changeset both from Umbraco Models folder and from Acme.Umbraco.Models folder.
3. If document type is modified (e.g. a property is modified, deleted or updated):
	1. remove and reinclude it to the project Acme.Umbraco.Models. 
	2. Make sure it included in the changeset from both the locations (i.e. Umbraco Models and Acme.Umbraco.Models)


## TODO Tasks

1. Configure Examine to auto re-index
1. Physiotherapist Registration Page
1. Dependency Injection
1. Investigate custom urls without content that backs it, This will look into pipelines
1. Capture form (Assessment Form)
1. Define a custgom index and/or custom index field.