using PinGod.Core;
using PinGod.Game;
using System.Threading.Tasks;
using System.Threading;
using Godot;
using System;

/// <summary>This game is a GODOT PinGodGame from the pingod-addons with a PinGodProcGameController and MachinePROC<para/></summary>
public abstract partial class PinGodGameProc : PinGodGame
{
    /// <summary>A reference to the Modes (CanvasLayer) in the MainScene<para/>
    /// This is where a parent scene for a PROC goes if it has one<para/>
    /// eg: ModesLayer/Attract, ModesLayer/ScoreMode</summary>
    public CanvasLayer ModesCanvasLayer;

    /// <summary>P-ROC version of the <see cref="MachineNode"/>. Get to the database through this</summary>
    public MachinePROC MachinePROC;

    /// <summary>Procgame <see cref="IGameController"/></summary>
    public PinGodNetProcDataGameController NetProcGame;

    private Label _creditsLabel;

    private Task _procGameLoop;

    /// <summary>To cancel the PROC loop</summary>
    private CancellationTokenSource tokenSource;

	public bool GameReady { get; private set; }

	#region Godot Overrides

	public override void _EnterTree()
	{
        //create a config with extra options for P-ROC
        PinGodOverrideConfig = new PinGodProcGameConfigOverride();

        base._EnterTree();

        Logger.LogLevel = (PinGod.Base.LogLevel)((int)PinGodOverrideConfig.LogLevel);
    }

    /// <summary>Quit the P-ROC game loop and call ExitGame on the PGProcGame</summary>
    public override void _ExitTree()
    {
        //set display size + pos to database
        WindowSaveSettings();        

        NetProcGame?.ExitGame();

        if (!tokenSource?.IsCancellationRequested ?? true)
            tokenSource?.Cancel();

        base._ExitTree();
    }

    /// <summary>Runs until the resources are ready.<para/>
    /// For first run, load all scenes as this waits not blocking, then load the Attract mode</summary>
    /// <param name="_delta"></param>
    public override void _Process(double _delta)
    {
        base._Process(_delta);
        if (_resources != null)
        {
            bool result = _resources?.IsLoading() ?? true;
            if (!result)
            {
                //resources loaded
                SetProcess(false);
                OnResourcesLoaded();
            }
        }
    }

    /// <summary>Creates a P-ROC game then Starts the <see cref="IGameController.RunLoop"/></summary>
    public override void _Ready()
    {
        base._Ready();

        ModesCanvasLayer = GetNodeOrNull<CanvasLayer>(Paths.ROOT_MODES_CANVAS);
        if (ModesCanvasLayer == null) Logger.WarningRich("[color=red]", nameof(PinGodGameProc), $": No modes canvas found in Main Scene", "[/color]");

        MachinePROC = this.MachineNode as MachinePROC;
        if (MachinePROC != null)
        {
            //CREATE AND SETUP PROC MACHINE
            try
            {
                CreateProcGame();

                Logger.Info(nameof(PinGodGameProc), ": ProcGame created. Setting up MachineNode from ProcGame.");

                if (NetProcGame == null)
                {
                    throw new NullReferenceException("P-ROC game couldn't be created. Simulated: " + 
                        ((PinGodProcGameConfigOverride)PinGodOverrideConfig).Simulated);
                }

                //SET MACHINE ITEMS FROM PROC TO PINGOD            
                //SetupPinGodotFromProcGame();
                Logger.Info(nameof(MachinePROC), ": ProcGame and database setup complete.");

                WindowLoadSettings();
            }
            catch (System.DllNotFoundException dllEx)
            {
                Logger.Error(dllEx.Message + ". Game must contain `lib` folder with `libpinproc` libraries");
                this.GetTree().Quit();
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex.Message);
                this.GetTree().Quit();
            }
        }
    }
    #endregion

    public override void AddCredits(byte amt)
    {
        Credits += amt;

        //let any other scenes know we've updated the credits
        EmitSignal(nameof(CreditAdded), Credits);
    }

    /// <summary> Adds a mode to the PROC modes </summary>
    /// <param name="mode"></param>
    public virtual void AddMode(PinGodProcMode mode) => NetProcGame.Modes.Add(mode);

    /// <summary> Does nothing, override and use a switch statement to load one by name</summary>
    /// <param name="mode"></param>
    public virtual void AddMode(string mode) => NetProcGame.AddMode(mode);

    /// <summary>Use the <see cref="PinGodGameConfigOverride.Simulated"/> flag from the PROC.cfg in game directory for settings</summary>
    /// <param name="machineConfig"></param>
    public virtual void CreateProcGame() { }

    /// <summary>Finds a child node of the Modes Canvas.<para/>
    /// Use this from a PROC mode via pingodGame to get a scene instance from the tree</summary>
    /// <param name="name"></param>
    /// <returns>returns null if not found</returns>
    public virtual Node GetSceneFromModesCanvas(string name) =>
        ModesCanvasLayer.GetNodeOrNull(name);

    /// <summary>The Resources node has loaded all resources. Now we can get any packed scenes and use in modes <para/>
    /// The MainScene must exist<para/>
    /// This runs the P-ROC game loop and sets GameReady<para/></summary>
    public virtual void OnResourcesLoaded()
    {
        var ms = GetNodeOrNull<Node>(Paths.ROOT_MAINSCENE);
        if (ms != null)
        {
            try
            {
                StartProcGameLoop();

                NetProcGame.GodotResourcesReady();

                GameReady = true;
            }
            catch (System.Exception ex) { Logger.Error(nameof(MachinePROC), nameof(_Ready), $"{ex.Message} - {ex.InnerException?.Message}"); }
        }
        else { Logger.WarningRich(nameof(PinGodGameProc), $"[color=yellow] no {Paths.ROOT_MAINSCENE} found.[/color]"); }
    }

    /// <summary>Sets the /root to paused</summary>
    public virtual void PauseGodot(bool pause = true) => GetNode("/root").GetTree().Paused = pause;
    public virtual void Quit(int exitCode = 0) => GetTree().Quit(exitCode);

    public virtual void RemoveAttractMode() { }

    /// <summary> Removes a mode from the PROC modes </summary>
    /// <param name="mode"></param>
    public virtual void RemoveMode(PinGodProcMode mode) => NetProcGame.Modes.Remove(mode);

    /// <summary>Removes a PROC mode by name</summary>
    /// <param name="mode"></param>
    public virtual void RemoveMode(string mode) => NetProcGame.RemoveMode(mode);

    /// <summary>Override the default window setup to do nothing</summary>
    public override void SetupWindow() { }

    /// <summary>Calls NetProcDataGame methods to start a netprocgame<para/>
    /// The attract mode is removed</summary>
    public void StartProcGame()
    {
        NetProcGame.Logger.Log("start button, trough full");
        NetProcGame.StartGame();
        NetProcGame.AddPlayer();
        NetProcGame.StartBall();
        NetProcGame.IncrementAudit("CREDITS_TOTAL", 1);
        NetProcGame.IncrementAudit("CREDITS", -1);
        Credits--;

        NetProcGame.RemoveMode("attract");
    }

    /// <summary>Remove the mode scene from the ModesCanvasLayer and call QueueFree</summary>
    /// <param name="sceneName">This just requires the name of the mode to find in the canvas</param>
    public virtual void RemoveModeScene(string sceneName)
    {
        Logger.Verbose($"attempting to {nameof(RemoveModeScene)}: {sceneName}");
        var node = ModesCanvasLayer?.GetNodeOrNull(sceneName);
        if (node != null)
        {
            ModesCanvasLayer?.RemoveChild(node);
            node.QueueFree();
            Logger.Verbose($"{sceneName} removed.");
        }
        else { Logger.Warning($"{sceneName} wasn't found in the tree to remove"); }
    }

	/// <summary>Adds machine items from <see cref="NetProcGame"/> into this Machine node <para/>
	/// TODO: perhaps this machine class overrides the base and uses the <see cref="NetProcGame"/> instead <para/>
	/// </summary>
    /// 
	private void SetupPinGodotFromProcGame()
	{
		MachinePROC.ClearMachineItems();

		if (NetProcGame != null)
		{
			//add switches from p-roc
			if (NetProcGame.Switches?.Count > 0)
				NetProcGame.Switches.Values.ForEach(x => MachinePROC.AddSwitch(x.Name, (byte)x.Number));
			if (NetProcGame.Coils?.Count > 0)
				NetProcGame.Coils.Values.ForEach(x => MachinePROC.AddCoil(x.Name, (byte)x.Number));
			if (NetProcGame.Lamps?.Count > 0)
				NetProcGame.Lamps.Values.ForEach(x => MachinePROC.AddLamp(x.Name, (byte)x.Number));
			if (NetProcGame.LEDS?.Count > 0)
				NetProcGame.LEDS.Values.ForEach(x => MachinePROC.AddLed(x.Name, (byte)x.Number));
		}
	}

	/// <summary>Starts netproc <see cref="IGameController.RunLoop"/>, creates cancel token source to end the loop</summary>
	private void StartProcGameLoop()
	{
		if (_procGameLoop?.Status == TaskStatus.Running) { return; }

        //Setup simulated memory mapping?
		if (PinGodOverrideConfig.MemoryMapEnabled && ((PinGodProcGameConfigOverride)PinGodOverrideConfig).Simulated)
		{
			NetProcGame._memMap = GetNodeOrNull<MemoryMapPROCNode>("/root/MemoryMap");			

			if (NetProcGame._memMap == null)
				Logger.Warning(nameof(PinGodGameProc), $": WARN: no {nameof(MemoryMapPROCNode)} found in root/MemoryMap");

			NetProcGame.InitSimStates();

			NetProcGame._memMap?.WriteStates();
		}
        else
            Logger.Debug(nameof(PinGodGameProc), $": simulator memory mapping is disabled, enable for simulator memory mappings");

        //run game loop
        tokenSource = new CancellationTokenSource();
		_procGameLoop = Task.Run(() =>
		{
			Logger.Info(nameof(PinGodGameProc), ":running game loop");

			//run proc game loop delay 1 save CPU //TODO: maybe this run loop needs to re-throw exception if any caught
			NetProcGame.RunLoop(((PinGodProcGameConfigOverride)PinGodOverrideConfig).Delay, tokenSource);

			//means the proc loop threw exception
			if (tokenSource.IsCancellationRequested)
			{
				try
				{
					this?.GetTree()?.Quit(0);
				}
				catch { }
			}
			else
			{
				Logger.Info(nameof(PinGodGameProc), ":ending proc game loop");
				NetProcGame.EndRunLoop();
				Logger.Info(nameof(PinGodGameProc), ":proc game loop stopped");
			}            

		}, tokenSource.Token);
	}

	/// <summary>
	/// Set window from database settings. Return if <see cref="IGNORE_DB_DISPLAY_SETTINGS"/> is set to true
	/// </summary>
	private void WindowLoadSettings()
	{
		if (NetProcGame == null) return;
		
		if (((PinGodProcGameConfigOverride)PinGodOverrideConfig).IgnoreDbDisplay) return;        

		DisplayServer.WindowSetMode((DisplayServer.WindowMode)NetProcGame.GetAdjustment("DISP_MODE"));
		DisplayExtensions.SetContentScale(GetTree().Root, (Window.ContentScaleModeEnum)NetProcGame.GetAdjustment("DISP_CONT_SCALE_MODE"));
        DisplayExtensions.SetAspectOption(GetTree().Root, (Window.ContentScaleAspectEnum)NetProcGame.GetAdjustment("DISP_CONT_SCALE_ASPECT"));
        //get display size + pos from database values
        DisplayExtensions.SetSize(NetProcGame.GetAdjustment("DISP_W"), NetProcGame.GetAdjustment("DISP_H"));
        DisplayExtensions.SetPosition(NetProcGame.GetAdjustment("DISP_X"), NetProcGame.GetAdjustment("DISP_Y"));
        DisplayExtensions.SetAlwaysOnTop(NetProcGame.GetAdjustment("DISP_TOP") > 0 ? true : false);
	}

	/// <summary>
	/// Save window adjustments to database from the main window. Return if <see cref="IGNORE_DB_DISPLAY_SETTINGS"/> is set to true
	/// </summary>
	private void WindowSaveSettings()
	{
		if (NetProcGame == null) return;
		
		if (((PinGodProcGameConfigOverride)PinGodOverrideConfig).IgnoreDbDisplay) return;		

		var winSize = DisplayServer.WindowGetSize(0);
		var winPos = DisplayServer.WindowGetPosition(0);
		NetProcGame.SetAdjustment("DISP_W", winSize.X); NetProcGame.SetAdjustment("DISP_H", winSize.Y);
		NetProcGame.SetAdjustment("DISP_X", winPos.X); NetProcGame.SetAdjustment("DISP_Y", winPos.Y);
		NetProcGame.SetAdjustment("DISP_TOP", DisplayServer.WindowGetFlag(DisplayServer.WindowFlags.AlwaysOnTop) ? 1 : 0);
		NetProcGame.SetAdjustment("DISP_MODE", (int)DisplayServer.WindowGetMode());
		NetProcGame.SetAdjustment("DISP_CONT_SCALE_MODE", (int)DisplayExtensions.GetContentScale(GetTree().Root));
		NetProcGame.SetAdjustment("DISP_CONT_SCALE_ASPECT", (int)DisplayExtensions.GetAspectOption(GetTree().Root));
	}
}
