# Comet
Comet is a Conquer Online server project containing an account server and game server. The account server authenticates players, while the game server services players in the game world. This simple two-server architecture acts as a good introduction into server programming and networking. The server is interoperable with the Conquer Online game client, patch 5017 (not provided by this project). 

![Example Picture](/img/example.jpg)

### Getting Started
To get started, download and install [.NET Core 3.1.x](https://dotnet.microsoft.com/download/dotnet-core/3.1) or higher. This project uses [Visual Studio Code](https://code.visualstudio.com/) for lightweight development, and is highly recommended but optional. After installing both, open the top directory for Comet in Visual Studio Code. You will be asked to install the C# extension for Visual Studio Code, and restore packages. When done, restart Visual Studio Code. Then, you'll be ready to build.

After building Comet, open and modify the `Comet.Account.config` and `Comet.Game.config` configuration files in the Comet.Account and Comet.Game bin folders. Modify the MySQL connection details to point to your instance. If you don't have MySQL installed, consider [MySQL Community Server](https://dev.mysql.com/downloads/mysql/) (comes with MySQL Workbench). Import scripts are located in the repository's "sql" folder. Run all SQL import scripts using MySQL Workbench's Data Import wizard. Refresh the schema list to see newly imported schemas. 

Once imported, open the `realm` table from the `comet.account` database. Enter your external IP address (port forwarding required) or an internal IP address for testing (the client does not support 127.0.0.1). Security between the realm 's game server and the account server is optional, but highly recommended if not running on one host. Check with your host provider for options on security groups or virtual networks between VMs.

### Feature Requests

Please do not file feature requests in the issue tracker. Requests for new features will be closed. To request a feature, please do so via discussion forums provided by an accompanying Conquer Online game community. You may also track ongoing progress of features and enhancements through Comet's [Trello storyboard](https://trello.com/b/tb8ChBlF/comet). 

### Common Questions & Answers

##### How do I create an account?

Accounts can be created by inserting a username and password into the `account` table. On insert, a trigger will automatically replace your plaintext password with a salted SHA-1 hash. The hashed password and salt will be saved in place of the plaintext password. Due to client limitations, passwords must be less than 16 characters long. Join with the `account_authority` and `account_status` tables to set account activation status and permissions.

##### How do I configure the client?

You may connect to your server instance without modifying client code. Create a shortcut to Conquer.exe and add "blacknull" as a target command-line argument. Open server.dat and enter the following (adding your IP address). Be sure not to specify an internal IP address (must be external or a loopback adapter). If you specify an internal IP address, the client will throw an error: "Server.dat is damaged".

```
[Header]
GroupAmount=1
Group1=GroupPic4

[Group1]
ServerAmount=1

Server1=Comet
Ip1=
Port1=9958
ServerName1=Comet
HintWord1= 
Pic1=servericon33
```

##### Why can't I connect to my server instance?

There are a few reasons why you might not be able to connect. First, check that you can connect locally using a loopback adapter. If you can connect locally, but cannot connect externally, then check your firewall settings and port forwarding settings. If you can connect to the Account server but not the Game server, then check your IP address and port in the `realm` table. Confirm that your firewall allows the port, and that port forwarding is also set up for the Game server (and not just the Account server).

### Legality

Algorithms and packet structuring used by this project for interoperability with the Conquer Online game client is a result of reverse engineering. By Sec. 103(f) of the DMCA (17 U.S.C. § 1201 (f)), legal possession of the Conquer Online client is permitted for this purpose, including circumvention of client protection necessary for archiving interoperability. Comet is a non-profit, academic project and not associated with TQ Digital Entertainment. All rights over Comet are reserved by Gareth Jensen "Spirited". All rights over the game client are reserved by TQ Digital Entertainment.