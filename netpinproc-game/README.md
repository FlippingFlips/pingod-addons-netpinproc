# netpinproc-game demo
The `MainScene.tscn` scene is run when the game is launched.
All the files in this directory are for a starter game.

## Main PROC Game Files
- `MyPinGodProcGameController` - This is a custom P-ROC controller which builds from the `PinGodNetProcDataGameController`.
This `IGameController` uses sqlite for machine / game data store
- `MyPinGodProcGame` - This is a Godot PinGod Game using `PinGodGameProc` as a base.
`PinGodGameProc` is a custom `PinGodGame`.

## PROC MODES - procmodes
These are P-ROC modes which should handle your switches, do game logic and be able to control lamps / LEDS.
A `PinGodProcMode` is used as a base for these modes which inherits the `PROC.IMode` or `Mode`.
### Modes Included In Demo
These modes are not required for a game to run but you won't have much apart from logging from the P-ROC or Fake.
- MachineSwitchHandlerMode
- ServiceMode
- AttractMode
- ScoreDisplayMode
- MyMode
### Mode Scenes
Scenes are not required for P-ROC modes. These are Godot scenes with scripts.
The `AttractProc` and `ScoreModePROC` are created from using the `PinGod.Addons` `Attract` and `ScoreMode` and overriding it and doing PROC related stuff.
They can get access to the PROC game through `PinGodGameProc`.
This is served as a singleton from a running game at `/root/PinGodGame`. Cast the `PinGodGame` to `PinGodGameProc`.
The scene you send to the mode should be the full asset path.
For the attract it is placed at `"res://scenes/AttractMode/AttractProc.tscn"` with the script for the scene.
#### Adding / Removing Scenes in PROC Modes
To add a scene on UI thread:
`_pingod.CallDeferred("AddModeScene", ATTRACT_SCENE);`

To remove a scene on UI thread:
`_pingod.CallDeferred("RemoveModeScene", ATTRACT_SCENE);`
