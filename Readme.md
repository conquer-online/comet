# Comet

You can find setup instructions in the [main branch's readme](https://gitlab.com/spirited/comet/-/blob/main/Readme.md).

This document outlines key features of the project and gives an overview of the solution.

## Solution Overview

Comet is made up of multiple server, library, and test projects. 

Server projects:

* [Comet.Account](/src/Comet.Account/): Authenticates and redirects players to a game server.
* [Comet.Game](/src/Comet.Game/): Serves authenticated players in a game world (multiple can be set up).

Library projects:

* [Comet.Core](/src/Comet.Core/): Contains language or system additions that are not game centric.
* [Comet.Network](/src/Comet.Network/): Contains base networking classes for creating networked actors.
* [Comet.Shared](/src/Comet.Shared/): Contains shared message classes for RPC between the servers.

Test projects:

* [Comet.Account.Tests](/tests/Comet.Account.Tests/): Unit tests for validating basic account server functionality.
* [Comet.Core.Tests](/tests/Comet.Core.Tests/): Unit tests for validating core functionality.
* [Comet.Network.Tests](/tests/Comet.Network.Tests/): Unit tests for validating network and base server functionality.

## Docker Files

In the top directory, you can find docker specific files for building and running containers:

* [.dockerignore](.dockerignore): Specifies files to be ignored by Docker copies to containers.
* [compose.debug.yml](compose.debug.yml): Describes how servers run and their dependencies (debug config).
* [compose.release.yml](compose.release.yml): Describes how servers run and their dependencies (release config).
* [dockerfile](dockerfile): The build and server image the Account and Game server run with.
