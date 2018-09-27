# Comet
Comet is a Conquer Online server project containing an account server and game server. The account server authenticates players, while the game server services players in the game world. This simple two-server architecture acts as a good introduction into server programming and networking. 

### Getting Started
To get started, download and install [.NET Core 2.1.4](https://www.microsoft.com/net/download/dotnet-core/2.1) or higher. This project uses [Visual Studio Code](https://code.visualstudio.com/) for lightweight development, and is highly recommended but optional. After installing both, open the top directory for Comet. You will be asked to install the C# extension for Visual Studio Code, and restore packages. When done, restart Visual Studio Code. Then, you'll be ready to build.

After building Comet, open and modify the configuration files in the Comet.Account and Comet.Game bin folders. Modify the MySQL connection details to point to your instance. If you don't have MySQL installed, consider [MySQL Community Server](https://dev.mysql.com/downloads/mysql/) (comes with MySQL Workbench). Import scripts are located in the repository's "sql" folder.

Once imported, open `realm` from the `comet.account` database. Enter your external IP address (port forwarding required) or an internal IP address for testing. Security between the realm and account server is optional, but highly recommended if not running on one host. You may generate 256-bit key from [RandomKeygen](https://randomkeygen.com/). Use the same key in the configuration file for Comet.Game.

### Common Questions & Answers

#### How do I create an account?

Accounts can be created by inserting a username and password into the `account` table. On insert, a trigger will automatically replace your plaintext password with a salted SHA-256 hash. The hashed password and salt will be saved in place of the plaintext password. Due to client limitations, passwords must be less than 16 characters long. Join with the `account_authority` and `account_status` tables to set account activation status and permissions.
