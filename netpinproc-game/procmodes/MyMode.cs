using NetPinProc.Domain;
using NetPinProc.Domain.PinProc;
using PinGod.Game;
using Switch = NetPinProc.Domain.Switch;

/// <summary>A base PROC mode to handle switches and add scores<para/>
/// - This mode mostly demonstrates switch handling and adding scores
/// - Game related uses like adding values to the database, using the trough<para/>
/// - This mode doesn't have a scene but can be added</summary>
public partial class MyMode : PinGodProcMode
{
    /// <summary>This constructor adds switch handlers for bumpers and targets</summary>
    /// <param name="game"></param>
    /// <param name="priority"></param>
    /// <param name="pinGod"></param>
    /// <param name="name"></param>
    public MyMode(IGameController game,
        int priority,
        IPinGodGame pinGod,
        string name = nameof(MyMode),
        bool loadDefaultScene = false) : base(game, name, priority, pinGod, null, loadDefaultScene)
    {
        //add switch handler for all bumpers
        for (int i = 0; i < 3; i++) { AddSwitchHandler($"bumper{i}", SwitchHandleType.active, 0, new SwitchAcceptedHandler(OnBumperHit)); }

        var targBankSwitches = Game.Config.GetNamesFromTag("targetBank", MachineItemType.Switch);
        foreach (var item in targBankSwitches) { AddSwitchHandler(item, SwitchHandleType.open, 0, new SwitchAcceptedHandler(OnTargetHit)); }
    }

    /// <summary>Launches a ball from the trough</summary>
    public override void ModeStarted()
    {
        Game.Logger.Log(GetType().Name+":"+nameof(ModeStarted), LogLevel.Debug);
        NetProcDataGame.Trough.LaunchBalls(1, null, false);
    }

    public override void ModeStopped()
    {
        Game.Logger.Log(GetType().Name + ":" + nameof(ModeStopped), LogLevel.Debug);

        base.ModeStopped(); //only need to call ModeStopped if need to clear scenes
    }

    /// <summary>Switch handler for flipper. Default to T</summary>
    /// <param name="sw"></param>
    /// <returns></returns>
    public bool sw_flipperLwL_active(Switch sw = null)
    {
        //Game.Logger.Log(GetType().Name + ":" + nameof(sw_flipperLwL_active)+ $": {sw.TimeSinceChange()}", LogLevel.Debug);
        return SWITCH_CONTINUE;
    }

    /// <summary>
    /// HOld delay Switch handler for flipper. Default to T
    /// </summary>
    /// <param name="sw"></param>
    /// <returns></returns>
    public bool sw_flipperLwL_active_for_1s(Switch sw = null)
    {
        //Game.Logger.Log(GetType().Name + ":" + nameof(sw_flipperLwL_active_for_1s) + $": {sw.TimeSinceChange()}", LogLevel.Debug);
        return SWITCH_CONTINUE;
    }

    /// <summary>Adds 100 points</summary>
    /// <param name="sw"></param>
    /// <returns><see cref="SWITCH_CONTINUE = true"/></returns>
    public bool sw_inlaneL_active(Switch sw = null)
    {
        AddPoints(100);
        return SWITCH_CONTINUE;
    }

    /// <summary>Adds 100 points</summary>
    /// <param name="sw"></param>
    /// <returns><see cref="SWITCH_CONTINUE = true"/></returns>
    public bool sw_inlaneR_active(Switch sw = null)
    {
        AddPoints(100);
        return SWITCH_CONTINUE;
    }

    /// <summary>Adds 100 points</summary>
    /// <param name="sw"></param>
    /// <returns><see cref="SWITCH_CONTINUE = true"/></returns>
    public bool sw_outlaneL_active(Switch sw = null)
    {
        AddPoints(100);
        return SWITCH_CONTINUE;
    }

    /// <summary>Adds 100 points</summary>
    /// <param name="sw"></param>
    /// <returns><see cref="SWITCH_CONTINUE = true"/></returns>
    public bool sw_outlaneR_active(Switch sw = null)
    {
        AddPoints(100);
        return SWITCH_CONTINUE;
    }

    /// <summary>Adds 250 points and ejects ball from a saucer</summary>
    /// <param name="sw"></param>
    /// <returns><see cref="SWITCH_CONTINUE = true"/></returns>
    public bool sw_saucer_active_for_1s(Switch sw = null)
    {
        AddPoints(250);
        Game.Coils["saucerEject"].Pulse(40);

        //TODO: start multiball
        return SWITCH_CONTINUE;
    }

    /// <summary>Adds 100 points</summary>
    /// <param name="sw"></param>
    /// <returns><see cref="SWITCH_CONTINUE = true"/></returns>
    public bool sw_slingL_active(Switch sw = null)
    {
        AddPoints(100);
        return SWITCH_CONTINUE;
    }

    /// <summary>Adds 100 points</summary>
    /// <param name="sw"></param>
    /// <returns><see cref="SWITCH_CONTINUE = true"/></returns>
    public bool sw_slingR_active(Switch sw = null)
    {
        AddPoints(100);
        return SWITCH_CONTINUE;
    }

    /// <summary>
    /// Start button, starts game and adds a player if the trough is full.
    /// </summary>
    /// <param name="sw"></param>
    /// <returns></returns>
    public bool sw_start_active(Switch sw)
    {
        //no credits
        if (PinGodGameProc.Credits <= 0) return SWITCH_CONTINUE;

        //TODO: change max players to database
        if (Game.Ball == 1 && Game.Players.Count < 4)
        {
            NetProcDataGame.IncrementAudit("CREDITS_TOTAL", 1);
            NetProcDataGame.IncrementAudit("CREDITS", 1);
            Game.AddPlayer();
            Game.Logger?.Log(nameof(MyMode) + ": player added");
        }

        return SWITCH_CONTINUE;
    }

    public override void UpdateLamps() => Game.Logger.Log(GetType().Name + ":" + nameof(UpdateLamps), LogLevel.Debug);

    void AddPoints(int amt) => NetProcDataGame.AddPoints(amt);

    /// <summary>Any bumper adds 150 points</summary>
    /// <param name="sw"></param>
    /// <returns><see cref="SWITCH_CONTINUE = true"/></returns>
    bool OnBumperHit(Switch sw)
    {
        AddPoints(150);
        return SWITCH_CONTINUE;
    }

    /// <summary>Any bumper adds 200 points</summary>
    /// <param name="sw"></param>
    /// <returns><see cref="SWITCH_CONTINUE = true"/></returns>
    bool OnTargetHit(Switch sw)
    {
        AddPoints(200);
        return SWITCH_CONTINUE;
    }
}
