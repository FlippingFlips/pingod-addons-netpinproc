# PinGod-AddOns-NetPinProc-Game
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white) ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![Godot Engine](https://img.shields.io/badge/GODOT-%23FFFFFF.svg?style=for-the-badge&logo=godot-engine) 

Demonstrates running the [NetPinProc Libraries](NetPinProc Libraries) with PinGod with fake or real controller board.

This uses the [NetPinProc.Game.Sqlite](NetPinProc.Game.Sqlite) package which includes NetPinProc.Game and NetPinProc.

* A Sqlite Database is generated when a game is launched. `netproc.db` = Edit .sql file and add machine items. The sql provided matches the same cabinet configuration as `pingod-addons`.

* A `proc.cfg` is generated when game is launched. = This makes it simpler to override settings like simulated and memory mapping.
