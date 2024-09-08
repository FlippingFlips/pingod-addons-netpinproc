using NetPinProc.Domain;
using NetPinProc.Domain.Mode;
using NetPinProc.Domain.PinProc;
using NetPinProc.Game.Sqlite;
using NetPinProc.Game.Sqlite.Model;
using System.Linq;
using System;
using System.Threading;
using System.Collections.Generic;

/// <summary>This doesn't use Godot, it is a PROC Game controller<para/>
/// <see cref="NetProcDataGameController"/></summary>
public abstract class PinGodNetProcDataGameController : NetProcDataGameController
{
    private CancellationTokenSource _gameLoopCancelToken;
    private bool _isSimulated;

    #region Constructors
    public PinGodNetProcDataGameController(
        MachineType machineType,
        IPinGodGame pinGodGame = null,
        bool deleteOnInit = false,
        ILoggerPROC logger = null,
        bool simulated = false,
        MachineConfiguration configuration = null) :
        base(machineType, deleteOnInit, logger, simulated, configuration)
    {
        _isSimulated = simulated;
        PinGodProcGame = pinGodGame as PinGodGameProc;
        PinGodProcGame.Credits = GetAudit("CREDITS");
    }
    #endregion

    #region Simulated Properties
    public byte[] _lastCoilStates;
    public byte[] _lastLampStates;
    public int[] _lastLedStates;
    public MemoryMapPROCNode _memMap;
    #endregion

    #region Modes
    public BallSave _ballSave;
    public BallSearch _ballSearch;
    #endregion

    public readonly PinGodGameProc PinGodProcGame;

    /// <summary>Use the Godot call deffered to use this method</summary>
    /// <param name="modeName"></param>
    public virtual void AddMode(string modeName) { }

    /// <summary>Increments total balls played, disables flippers <para/>
    /// Adds player stats</summary>
    public override void BallEnded()
    {
        base.BallEnded();

        EnableFlippers(false);
        if (_isSimulated) Coils["flippersRelay"].Disable();

        //add ball stats for the player
        var player = CurrentPlayer() as PlayerDb;
        player.BallsPlayed.Add(new BallPlayed { Ball = (byte)this.Ball, Points = player.Score, Time = this.GetBallTime() });
    }

    /// <summary>Runs perform search on the PROC <see cref="_ballSearch"/> mode</summary>
    public virtual void BallSearch() => _ballSearch?.Enable();

    public override void OnBallDrainedTrough()
    {
        this.Logger.Log(nameof(OnBallDrainedTrough), LogLevel.Debug);
        base.OnBallDrainedTrough();
    }

    public override void BallStarting()
    {
        base.BallStarting();
        //TODO: add modes on ball starting, these modes start when a new ball does

        EnableFlippers(true);

        if (_isSimulated) Coils["flippersRelay"].Enable();
    }

    /// <summary>Saves the database when calling <see cref="NetProcDataGameController.Quit"/></summary>
    public virtual void ExitGame()
    {
        try
        {
            Quit();
        }
        catch (System.Exception ex)
        {
            Logger.Log(nameof(PinGodNetProcDataGameController) + $"--ERROR: {ex.Message} {ex.InnerException?.Message}", LogLevel.Error);
        }
    }

    /// <summary>Used by simulators.<para/>
    /// Returns the led colors / states as int array. Used with memory mapping in loop</summary>
    /// <param name="stateCount"></param>
    /// <returns></returns>
    public int[] GetLedStatesArray(IEnumerable<LED> leds)
    {
        if (_memMap == null) return null;
        int[] arr = new int[_memMap.LedCount()];
        foreach (var item in leds)
        {
            if (item.Number * 3 <= arr.Length)
            {
                var c = System.Drawing.Color.FromArgb((int)item.CurrentColor[0], (int)item.CurrentColor[1], (int)item.CurrentColor[2]);
                arr[item.Number * 3] = item.Number;
                arr[item.Number * 3 + 1] = 1;
                arr[item.Number * 3 + 2] = System.Drawing.ColorTranslator.ToOle(c);
            }
        }

        return arr;
    }

    /// <summary>Should be invoked by Godot when initially booted and all resources have been loaded<para/>
    /// This implementation does nothing and should be used</summary>
    public abstract void GodotResourcesReady();

    /// <summary>Simulated use: Gets PROC driver coil states and creates array read to be sent to memory map</summary>
    /// <param name="drivers"></param>
    /// <returns></returns>
    public byte[] GetStates(IEnumerable<IDriver> drivers)
    {
        if (_memMap == null) return null;

        byte[] arr = new byte[_memMap.CoilCount()];
        foreach (var item in drivers)
        {
            //PDB index of 1st board/bank is 5. 5 * 8 + driver number = 40 == 0
            byte num = (byte)item.Number;
            if(this.Config.PRGame.MachineType == MachineType.PDB)
                num = (byte)(item.Number - 40);

            if (num * 2 <= arr.Length)
            {
                arr[num * 2] = num;
                var state = ((IVirtualDriver)item).GetCurrentState() ? (byte)1 : (byte)0;
                arr[num * 2 + 1] = state;
            }
        }
        return arr;
    }

    /// <summary>
    /// <inheritdoc/> <para/> Increments the POWERED_ON_TIMES.
    /// </summary>
    public override void InitializeDatabase()
    {
        var initTime = Godot.Time.GetTicksMsec();

        base.InitializeDatabase();

        var endTime = Godot.Time.GetTicksMsec();
        var total = (endTime - initTime) / 1000;

        Logger.Log(LogLevel.Info, nameof(PinGodNetProcDataGameController) + $": database initialized in {total} secs.");
    }

    /// <summary>Simulated use: gets states, coils, leds, lamps</summary>
    public virtual void InitSimStates()
    {
        _lastCoilStates = GetStates(Coils.Values);
        _lastLampStates = GetStates(Lamps.Values);
        _lastLedStates = GetLedStatesArray(LEDS.Values);
    }

    /// <summary>Remove from PROC modes. Does nothing, just placeholder</summary>
    /// <param name="modeName"></param>
    public virtual void RemoveMode(string modeName) { }

    public override void RunLoop(byte delay = 0,
        CancellationTokenSource cancellationToken = null)
    {
        //base.RunLoop(delay, cancellationToken);
        if (cancellationToken == null)
            _gameLoopCancelToken = new CancellationTokenSource();
        else
            _gameLoopCancelToken = cancellationToken;

        Event[] events;
        try
        {
            while (!_gameLoopCancelToken.IsCancellationRequested)
            {
                events = GetEvents(false);
                if (events?.Any() ?? false)
                {
                    foreach (Event evt in events)
                    {
                        ProcessEvent(evt);
                    }
                }

                this.Tick();

                //PinGod changed states. Used by memory mapping
                if (_isSimulated)
                {
                    _lastCoilStates = GetStates(Coils.Values);
                    _lastLampStates = GetStates(Lamps.Values);
                    _lastLedStates = GetLedStatesArray(LEDS.Values);
                    _memMap?.WriteStates();
                }

                TickVirtualDrivers();

                //MODES: Tick
                _modes?.Tick();

                _proc?.WatchDogTickle();
                if (delay > 0)
                    Thread.Sleep(delay);
            }
        }
        catch (Exception ex)
        {
            Logger.Log("RUN LOOP EXCEPTION: " + ex.ToString(), LogLevel.Error);
        }
        finally
        {
            Logger?.Log("Run loop ended", LogLevel.Info);
            double dt = NetPinProc.Domain.Time.GetTime() - BootTime;
            _proc.Close();
        }
    }

    public override void ShootAgain()
    {
        base.ShootAgain();
        Logger.Log($"extra balls: {this.CurrentPlayer().ExtraBalls}", LogLevel.Debug);
    }
}
