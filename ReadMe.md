![https://autox.googlecode.com/git/dotnet/AutoX/Resources/Project.jpg](https://autox.googlecode.com/git/dotnet/AutoX/Resources/Project.jpg) _You can also find this file at the installation target folder._
# Installation Guide #
## Pre-requiste: ##
1. Microsoft .net framework 4.5
2. Database installation finished
  * This system accept Postgresql, MySQL, MongoDB as backend database.
  * For the limitation of installshield, installer cannot create database & tables for you.
## Install ##
  * Run setup.exe, the program will be installed to “c:\Program Files\AuotX\
## After Installation ##
  * For the limitation of installshield, installer cannot create database & tables for you. Please find the enclosed sql scripts(.sql) to create the database & tables.
  * And you need to manually update the config file([app](app.md).config or web.config) to point to the Database.()
## Known Issues ##
## Contact ##
Author: [Jien Huang](mailto:huangjien@gmail.com)
## Change Log ##
New With 0.45:
  * Local Supported Browser: Firefox, IE, Chrome
  * Supported Database: Postgresql, MySQL, MongoDB
  * Supported automation actions: Enter, Click, Check, Close, Command, Start, Wait, GetValue, Existed ,NotExisted, VerifyValue, VerifyTable