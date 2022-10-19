# Comet: Account Server

The account server accepts clients and authenticates players from the client's login screen. If the player enters valid account credentials, then the server will send login details to the game server and disconnect the client. The client will reconnect to the game server with an access token from the account server.

## Project Structure

The following folders break down the project into actionable states:

* [Database](/src/Comet.Account/Database): Contains database table definitions as entities and access methods.
* [Database/Models](/src/Comet.Account/Database/Models): Contains database entities that are extended into account states.
* [Database/Repositories](/src/Comet.Account/Database/Repositories): Contains database methods for reading and writing persistent data.
* [Packets](/src/Comet.Account/Packets): Contains packet definitions and methods for encoding, decoding, and processing packets.
* [States](/src/Comet.Account/States): Contains account states from composed database models.

The following files are important, core building blocks for the server project:

* [Program.cs](/src/Comet.Account/Program.cs): The entry point that orchestrates server startup.
* [Server.cs](/src/Comet.Account/Server.cs): Outlines socket events for connecting, receiving packets, and disconnecting.
* [Kernel.cs](/src/Comet.Account/Kernel.cs): Contains global collections and constants to be accessed from multiple threads.
* [Database/Context.cs](/src/Comet.Account/Database/Context.cs): Composes all database table definitions.
* [States/Client.cs](/src/Comet.Account/States/Client.cs): Composes all account state structures for a player.
