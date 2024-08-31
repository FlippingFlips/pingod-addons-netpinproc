using Godot;
using NetPinProc.Domain;
using PinGod.Core;

/// <summary>A P-ROC (PinGodProcMode) but reusing the default PinGod Attract.tscn. <para/>
/// When the mode starts, the <see cref="ATTRACT_SCENE"/> is loaded into the tree. <para/> 
/// This mode handles start button to remove this mode, and then start the game if the P-ROC trough is full.
/// </summary>
public class AttractMode : PinGodProcMode
{
    /// <summary>attract scene to load when mode starts.<para/>
    /// The scene is already loaded in the resources, it just gets instantiated and added to the tree.</summary>
    const string ATTRACT_SCENE = "res://netpinproc-game/scenes/AttractMode/AttractProc.tscn";

    private PinGodGameProc _pingod;

    public AttractMode(IGameController game,
        int priority,
        IPinGodGame pinGod,
        string name = nameof(AttractMode)) : base(game, name, priority, pinGod) =>
            _pingod = pinGod as PinGodGameProc;

    /// <summary>adds the attract scene to this modes canvas layer</summary>
    public override void ModeStarted()
    {
        //base.ModeStarted(); //base implementation does nothing

        //Log from NetProcGame
        _pingod.NetProcGame.Logger.Log(nameof(AttractMode), " " + nameof(ModeStarted));

        //tell Godot to load our scene
        _pingod.CallDeferred("AddModeScene", ATTRACT_SCENE);
    }

    /// <summary>removes the attract layer from modes canvas and frees it</summary>
    public override void ModeStopped()
    {
        //Log from Godot
        Logger.Debug(nameof(AttractMode), nameof(ModeStopped));

        //remove the scene from the modes canvas layer
        _pingod.CallDeferred("RemoveModeScene", "Attract");

        //removes this mode scene
        base.ModeStopped();
    }

    /// <summary>Start button closed event<para/>
    /// Checks for enough credits and if enough balls on the trough the game will be started.<para/>
    /// <see cref="PinGodGameProc.StartProcGame"/> will be used to start the game<para/>
    /// A PROC ball search will be activated if not enough balls are in the trough</summary>
    /// <param name="sw"></param>
    /// <returns>true if switch can continue</returns>
    public bool sw_start_active(NetPinProc.Domain.Switch sw)
    {
        Game.Logger?.Log("Attract: start button active credits: " + _pingod.Credits);

        //no credits
        if (_pingod.Credits <= 0) return SWITCH_CONTINUE;
        
        if (_game.Trough?.IsFull() ?? false)
            _pingod.CallDeferred("StartProcGame");
        else
        {
            Game.Logger?.Log("attract start. trough balls=" + _game.Trough.NumBalls() + ", running ball search.", NetPinProc.Domain.PinProc.LogLevel.Debug);
            _pingod.NetProcGame.BallSearch();

        }
        return SWITCH_CONTINUE;
    }
}
