# Comet: Game Server

The game server listens for authentication players with a valid access token from the account server, and hosts the game world. The game world in this project has been simplified into a single server executable.

## Project Structure

The following folders break down the project into actionable states:

* [Database](/src/Comet.Game/Database): Contains database table definitions as entities and access methods.
* [Database/Models](/src/Comet.Game/Database/Models): Contains database entities that are extended into game states.
* [Database/Repositories](/src/Comet.Game/Database/Repositories): Contains database methods for reading and writing persistent data.
* [Packets](/src/Comet.Game/Packets): Contains packet definitions and methods for encoding, decoding, and processing packets.
* [States](/src/Comet.Game/States): Contains game states from composed database models.

The following files are important, core building blocks for the server project:

* [Program.cs](/src/Comet.Game/Program.cs): The entry point that orchestrates server startup.
* [Server.cs](/src/Comet.Game/Server.cs): Outlines socket events for connecting, receiving packets, and disconnecting.
* [Remote.cs](/src/Comet.Game/Remote.cs): The RPC methods that can be called from the Account server.
* [Kernel.cs](/src/Comet.Game/Kernel.cs): Contains global collections and constants to be accessed from multiple threads.
* [Database/Context.cs](/src/Comet.Game/Database/Context.cs): Composes all database table definitions.
* [States/Client.cs](/src/Comet.Game/States/Client.cs): Composes all game state structures for a player.
