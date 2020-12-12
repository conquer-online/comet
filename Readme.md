# Comet

Comet is a simple, educational game networking project targeting the Conquer Online game client.

The project is split between two servers: an account server and game server. The account server authenticates players, while the game server services players in the game world. This simple two-server architecture acts as a good introduction into server programming and concurrency patterns. The server is interoperable with the Conquer Online game client, but a modified client will not be provided.

![Example Picture](/doc/Images/Example.jpg)

## Getting Started

Get started by cloning the repository. Select a branch based on the patch you wish to target. You can view a sorted list of supported patches [here](https://gitlab.com/spirited/comet/-/branches/all?sort=name_asc).

Before setting up the project, download and install the following:

* [.NET Core 3.1 or higher](https://dotnet.microsoft.com/download) - Primary language compiler
* [MariaDB](https://mariadb.org/) - Recommended flavor of MySQL for project databases 
* [Visual Studio Code](https://code.visualstudio.com/) - Recommended editor for modifying and building project files

In a terminal, run the following commands to build the project (or build with Shift+Ctrl+B):

```
dotnet restore
dotnet build
```

After building Comet, open and modify `Comet.Account.config` and `Comet.Game.config` in the compiled bin folder with your MySQL login credentials. In MySQL, import scripts located in the repository's sql folder. Once imported, open the `realm` table from the `comet.account` database. Enter your external IP address (port forwarding required) or an internal IP address for testing (the client does not support 127.0.0.1). Make sure your realm name matches the server name selected in the client.

## Common Questions & Answers

### How do I create an account?

Accounts can be created by inserting a username and password into the `account` table. On insert, a trigger will automatically replace your plaintext password with a salted SHA-1 hash. The hashed password and salt will be saved in place of the plaintext password. Due to client limitations, passwords must be less than 16 characters long. Join with the `account_authority` and `account_status` tables to set account activation status and permissions.


### Why can't I connect to the server?

There are a few reasons why you might not be able to connect. First, check that you can connect locally using a loopback adapter. If you can connect locally, but cannot connect externally, then check your firewall settings and port forwarding settings. If you can connect to the Account server but not the Game server, then check your IP address and port in the `realm` table. Confirm that your firewall allows the port, and that port forwarding is also set up for the Game server (and not just the Account server).

## Legality

Algorithms and packet structuring used by this project for interoperability with the Conquer Online game client is a result of reverse engineering. By Sec. 103(f) of the DMCA (17 U.S.C. § 1201 (f)), legal possession of the Conquer Online client is permitted for this purpose, including circumvention of client protection necessary for archiving interoperability (though the client will not be provided for this purpose). Comet is a non-profit, academic project and not associated with TQ Digital Entertainment. All rights over Comet are reserved by Gareth Jensen "Spirited". All rights over the game client are reserved by TQ Digital Entertainment.
