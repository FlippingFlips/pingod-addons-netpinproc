# PinGod-AddOns-NetPinProc-Game
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white) ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![Godot Engine](https://img.shields.io/badge/GODOT-%23FFFFFF.svg?style=for-the-badge&logo=godot-engine) 

Addons for extending the `pingod-addons` and adding `NetPinProc Libraries` for using P-ROC with PinGod.

This project uses the [NetPinProc.Game.Sqlite](NetPinProc.Game.Sqlite) package which includes `NetPinProc.Game` and `NetPinProc`.
This project also relies on the `pingod-addons` being in an `addons` folder for the Godot project.

- `addons-netpinproc` contains base implementations for PROC and PinGod with PROC with `pingod-addon` overrides.
- `autoload` contains scenes which can override the addons scenes.
- `netpinproc-game` contains custom game classes and files with scenes and example modes.
- `simulators` files for simulator like Visual Pinball and PinGod controller
- `sql` shipped with library, change to add your own machine configuration.
Set the `.cfg` file to delete on launch if you need to.



## Quick Start
1. Download the repo or clone
2. Download the addons folder from `pingod-addons`
3. Load project with Godot from this directory running `godot -e`
4. Build the project. Run to get errors if no PROC is connected.
5. A database has been generated and a `PinGod-AddOns-NetPinProcGame.cfg`
6. You can change the config for `Simulated` boards and how the database is generated / removed
  
  **A `{projectname}.cfg` is generated when the game is launched. This makes it simpler to override settings like simulated and memory mapping by editing this file in the root directory.**
6. Change simulated in the `cfg` in Godots editor to run a fake P-ROC.

  **A Sqlite Database is generated when a game is launched. `netproc.db` = Edit .sql file and add machine items. The sql provided matches the same cabinet configuration as `pingod-addons`.**
