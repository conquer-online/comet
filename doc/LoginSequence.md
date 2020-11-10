# Login Sequence

Conquer Online's login sequence is split between two stages: account and game.

## Account Authentication Stage

First, the game client connects to the Account server and sends a MsgAccount message containing the player's username, encrypted password, and realm name. Note: sending any plaintext or encrypted password over a network is not a good practice. This was later modified in patch 5532+ to use the Secure Remote Password protocol (SRP6A).

After the account server verifies the password, it sends an single-use authentication token first to the game server and then to the client in a MsgConnectEx message, which also contains the game server's IP address and port information. The game client then disconnects and reconnects to that game server.

## Game Login Stage

The first message received by the game server on client connect is MsgConnect. This message contains the authentication token generated in the last stage. The game server uses this token with the player's IP address to verify the login attempt. If the login is valid, then the game server will attempt to load the player's character.

If the character doesn't exist, then a MsgTalk message will be sent containing the text "NEW_ROLE" on the Login channel. This indicates that the client should open the character creation screen. After the player selects a class, character name, and body size, the game client will send the server a MsgRegister message to register the new player. If the name of the character has been taken, then an error will be sent back using MsgTalk on the Register channel. If the character was created successfully, then the server will send MsgTalk with "ANSWER_OK" on the Register channel.

If the character already exists, then "ANSWER_OK" will be sent instead on the Login channel, followed by a MsgUserInfo message containing the player's character details. The client then responds by asking for the character's map and coordinates using a MsgAction message. Once the server responds to this packet, the client will send a chain of MsgAction messages responsible for loading friends, items, PK mode, offline whispers, etc.
