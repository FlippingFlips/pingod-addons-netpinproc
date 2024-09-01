using Godot;
using NetPinProc.Domain;
using PinGod.Core;
using System.Linq;

/// <summary>A P-ROC (PinGodProcMode) but reusing the default PinGod Attract.tscn. <para/>
/// When the mode starts, the <see cref="ATTRACT_SCENE"/> is loaded into the tree. <para/> 
/// This mode handles start button to remove this mode, and then start the game if the P-ROC trough is full.
/// </summary>
public class AttractMode : PinGodProcMode
{
    /// <summary>attract scene to load when mode starts.<para/>
    /// The scene is already loaded in the resources, it just gets instantiated and added to the tree.</summary>
    const string ATTRACT_SCENE = "res://netpinproc-game/scenes/AttractMode/AttractProc.tscn";
    const string DISPLAY_MSG_SCENE = "res://netpinproc-game/scenes/!Shared/Messages/DisplayMessageControl.tscn";

    /// <summary>Just a small message to overlay over the top of the attract as and when</summary>
    private PackedScene _messageScene;

    public AttractMode(IGameController game,
        int priority,
        IPinGodGame pinGod,
        string name = nameof(AttractMode),
        string defaultScene = ATTRACT_SCENE,
        bool loadDefaultScene = true) :
        base(game, name, priority, pinGod, defaultScene, loadDefaultScene) 
    {
        _messageScene = PinGodGameProc._resources.GetPackedSceneFromResource(DISPLAY_MSG_SCENE);
    }            

    /// <summary>adds the attract scene to this modes canvas layer</summary>
    public override void ModeStarted()
    {
        //use this to create the scene
        base.ModeStarted();
    }

    /// <summary>removes the attract layer from modes canvas and frees it</summary>
    public override void ModeStopped()
    {
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
        if (sw.IsState(true))
        {
            Game.Logger?.Log(NetPinProc.Domain.PinProc.LogLevel.Info, "Attract: start button active credits: " + PinGodGameProc.Credits);

            //no credits
            if (PinGodGameProc.Credits <= 0) return SWITCH_CONTINUE;

            if (NetProcDataGame.Trough?.IsFull() ?? false)
                ///_pingod is on the Godot UI game thread
                PinGodGameProc.CallDeferred(nameof(PinGodGameProc.StartProcGame));
            else
            {                
                var tBalls = NetProcDataGame.Trough.NumBalls();
                Game.Logger?.Log(
                    NetPinProc.Domain.PinProc.LogLevel.Debug,
                        "attract start. trough balls=" + tBalls +
                        ", running ball search.", NetPinProc.Domain.PinProc.LogLevel.Debug);

                //create a message scene instance from the packed scene
                //set the text to display for 2 seconds
                var instance = _messageScene.Instantiate() as DisplayMessageControl;
                instance.SetText($"ball search active\nballs int trough\n{tBalls}");
                instance.SetTime(2f);

                //this will call deferred on the game thread
                LoadChildSceneIntoCanvas(instance);

                //trigger ball search, trough isn't full                
                
                NetProcDataGame._ballSearch.Disable();
                NetProcDataGame._ballSearch.Enable();
            }
        }
        else
        {

        }
        
        return SWITCH_CONTINUE;
    }
}
