using Godot;
using NetPinProc.Domain;
using NetPinProc.Domain.Mode;
using NetPinProc.Domain.PinProc;
using System.Collections.Generic;
using System.Linq;

/// <summary>Custom PROC Game controller using the implementation from `addons/netpinproc-game` which uses sqlite PinProc IGameControlller <para/>
/// This controller can run real or fake proc with simulated flag. The base implementation runs the main PROC game loop<para/>
/// The main game loop handles events from a PROC and flushes the PROC data <para/>
/// The controller here is a good place to hold most modes and this demo game does that with all PROC modes<para/>
/// </summary>
public class MyPinGodProcGameController : PinGodNetProcDataGameController
{
    public MyPinGodProcGameController(
        MachineType machineType,
        IPinGodGame pinGodGame,
        bool deleteOnInit,
        ILoggerPROC logger = null,
        bool simulated = false,
        MachineConfiguration configuration = null) :
            base(machineType, pinGodGame, deleteOnInit, logger, simulated, configuration)
    {
    }

    #region PROC Game Modes
    public AttractMode _AttractMode;
    private MachineSwitchHandlerMode _machineSwitchHandlerMode;
    private MyMode _myMode;
    private ScoreDisplayProcMode _scoreDisplay;
    private ServiceMode _serviceMode;
    #endregion

    /// <summary>Overrides to add a <see cref="MyProcPlayer"/></summary>
    /// <returns></returns>
    public override IPlayer AddPlayer()
    {
        //todo: name the player when need to change from eg: Player 1
        var p = base.AddPlayer();
        p = new MyProcPlayer(p.Name);
        _scoreDisplay?.UpdateScores();
        return p;
    }

    /// <summary>Use the Godot call deferred to use this method</summary>
    /// <param name="modeName"></param>
    public override void AddMode(string modeName)
    {
        switch (modeName)
        {
            case "attract":
                GodotResourcesReady();
                break;
            case "service":
                Modes.Add(_serviceMode);
                break;            
            default:
                break;
        }
    }

    /// <summary>Add points to the CurrentPlayer and update the Godot display</summary>
    /// <param name="p">points</param>
    public override void AddPoints(long p)
    {
        base.AddPoints(p);
        _scoreDisplay?.UpdateScores();
    }

    /// <summary><see cref=""/></summary>
    public override void BallEnded()
    {
        base.BallEnded();

        //TODO: remove modes on ball starting,
        //these modes remove when a new ball ends
        Modes.Remove(_myMode);
        _scoreDisplay?.UpdateScores();
    }

    public override void BallStarting()
    {
        base.BallStarting();

        _myMode = new MyMode(this, 10, PinGodProcGame);
        Modes.Add(_myMode);
        _scoreDisplay?.UpdateScores();
        _ballSave.Start(now: false);
    }

    public override void ShootAgain()
    {
        base.ShootAgain();
    }

    /// <summary>Game ended, invoke Godot ready to reset game</summary>
    public override void GameEnded()
    {
        //this.EmitSignal(nameof(GameEnded));
        base.GameEnded();
        Modes.Remove(_scoreDisplay);

        this.GodotResourcesReady();
    }

    /// <summary>Should be called when Godot is ready or on a reset of the game.<para/>
    /// Modes and scenes are instantiated and calls a reset on the PROC game<para/>
    /// Modes created:
    ///  - Attract
    ///  - Service
    ///  - Score Display
    ///  - MachineSwitchHandler
    ///  - Ball Save</summary>
    public override void GodotResourcesReady()
    {
        Logger.Log(LogLevel.Debug, nameof(GodotResourcesReady));
        _AttractMode = new AttractMode(this, 12, PinGodProcGame);
        _serviceMode = new ServiceMode(this, PinGodProcGame, priority: 10, defaultScene: "res://addons/netpinproc-servicemode/ServiceModePROC.tscn".GetBaseName());
        _scoreDisplay = new ScoreDisplayProcMode(this, (PinGodGameProc)PinGodProcGame, priority: 2);
        _machineSwitchHandlerMode = new MachineSwitchHandlerMode(this, (PinGodGameProc)PinGodProcGame);
        _ballSave = new BallSave(this, "shootAgain", "plungerLane") { AllowMultipleSaves = false, Priority = 25 };
        
        SetupBallSearch();

        Reset();
    }

    public override void GameStarted()
    {
        base.GameStarted();

        Modes.Add(_scoreDisplay);
    }

    /// <summary>Use the godot call deferred to use this if invoking from Godot</summary>
    /// <param name="modeName"></param>
    public override void RemoveMode(string modeName)
    {
        switch (modeName)
        {
            case "service":
                Modes.Remove(_serviceMode);
                break;
            case "attract":
                Modes.Remove(_AttractMode);
                break;
            default:
                break;
        }
    }

    /// <summary>Resets ball saves and adds our base modes to the queue<para/>
    /// Calls the base implementation to create to a trough and to reset</summary>
    public override void Reset()
    {
        base.Reset();

        //_troughMode.EnableBallSave(true);        

        //link the trough to ball save
        Trough.BallSaveCallback = new AnonDelayedHandler(_ballSave.LaunchCallback);
        Trough.NumBallsToSaveCallback = new GetNumBallsToSaveHandler(_ballSave.GetNumBallsToSave);
        _ballSave.TroughEnableBallSave = new BallSaveEnable(Trough.EnableBallSave);

        Modes.Add(_AttractMode);
        Modes.Add(_machineSwitchHandlerMode);
        Modes.Add(_ballSave);
        Modes.Add(_ballSearch);

        _ballSearch.Enable();

        Logger.Log(LogLevel.Info, $"MODES RUNNING:" + Modes.Modes.Count);
    }

    /// <summary>TODO: This should be moved to base NetPinProc and not done here</summary>
    private void SetupBallSearch()
    {
        var coils = Config.PRCoils.Where(x => x.Search > 0)
                    .Select(n => n.Name)
                    .ToArray();

        var resetDict = new Dictionary<string, string>();
        var resetSw = Config.PRSwitches
            .Where(x => !string.IsNullOrWhiteSpace(x.SearchReset));
        foreach (var sw in resetSw)
        {
            resetDict.Add(sw.Name, sw.SearchReset);
        }

        var stopDict = new Dictionary<string, string>();
        var stopSw = Config.PRSwitches
            .Where(x => !string.IsNullOrWhiteSpace(x.SearchStop));
        foreach (var sw in stopSw)
        {
            stopDict.Add(sw.Name, sw.SearchStop);
        }

        _ballSearch = new BallSearch(this, 8, coils, resetDict, stopDict, null);
    }
}
