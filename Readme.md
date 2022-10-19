# Comet

Comet is a simple, educational game networking project targeting the Conquer Online game client.

The project is split between two servers: an account server and game server. The account server authenticates players, while the game server services players in the game world. This simple two-server architecture acts as a good introduction into server programming and concurrency patterns. The server is interoperable with the Conquer Online game client, but a modified client will not be provided.

__Please note: This is a skeleton / base project, meaning no game features have been implemented beyond login.__

![Example Picture](/doc/Images/Example.jpg)

## Getting Started

Get started by cloning the repository. Select a branch based on the patch you wish to target. You can view a sorted list of supported patches [here](https://gitlab.com/spirited/comet/-/branches/all?sort=name_asc).

| Patch | Pipeline Status | Description |
| ----- | --------------- | ----------- |
| __4274__ | [![pipeline status](https://gitlab.com/spirited/comet/badges/4274/pipeline.svg)](https://gitlab.com/spirited/comet/-/commits/4274) | One of the last stable patches for Conquer 1.0 with the legacy brown wood interface. |
| __4294__ | [![pipeline status](https://gitlab.com/spirited/comet/badges/4294/pipeline.svg)](https://gitlab.com/spirited/comet/-/commits/4294) | One of the first stable patches for Conquer 2.0 with the blue fabric and stone interface. |
| __4343__ | [![pipeline status](https://gitlab.com/spirited/comet/badges/4343/pipeline.svg)](https://gitlab.com/spirited/comet/-/commits/4343) | Adds potency and new currency, but does not include the pay-to-win shopping mall. |
| __5017__ | [![pipeline status](https://gitlab.com/spirited/comet/badges/5017/pipeline.svg)](https://gitlab.com/spirited/comet/-/commits/5017) | Adds pay-to-win shopping mall, +12 items, WuXing Oven, new fonts, and more. |
| __5065__ | [![pipeline status](https://gitlab.com/spirited/comet/badges/5065/pipeline.svg)](https://gitlab.com/spirited/comet/-/commits/5065) | Adds new watercolor client, new hairstyles, new equipment, and more. |
| __5187__ | [![pipeline status](https://gitlab.com/spirited/comet/badges/5187/pipeline.svg)](https://gitlab.com/spirited/comet/-/commits/5187) | Adds talismans, ninjas, enlightenment, quiz show, mounts, clans, arena, and more. |

```
git clone https://gitlab.com/spirited/comet.git
git switch <patch_number> 
```

After selecting a branch to work with, you have two options for building and running Comet.

### Option 1: Running in Docker Containers

If you have [docker](https://www.docker.com/products/docker-desktop/) installed, then you may build and run Comet using containers. 

In [Visual Studio Code](https://code.visualstudio.com/): right click `compose.debug.yml`, select "Compose Up - Select Services", and then select `db` and `db-admin`. 

Once built and launched, open http://localhost:8081 for phpMyAdmin. In MySQL, import scripts located in the repository's sql folder. Once imported, open the `realm` table from the `comet.account` database. Enter your external IP address (port forwarding required) or an internal IP address for testing (the client does not support 127.0.0.1). Change the RpcIpAddress to `comet-game-1` and make sure your realm name matches the server name selected in the client.

Finally, go back to Visual Studio code, right click `compose.debug.yml`, and select "Compose Up".

### Option 2: Building Locally

Before setting up the project, download and install the following:

* [.NET 6](https://dotnet.microsoft.com/download) - Primary language compiler
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
