using PinGod.Core;
using PinGod.Game;
using System.Threading.Tasks;
using System.Threading;
using Godot;
using System;

/// <summary>This game is a GODOT PinGodGame from the pingod-addons with a PinGodProcGameController and MachinePROC<para/></summary>
public abstract partial class PinGodGameProc : PinGodGame
{
    public CanvasLayer _modesCanvas;

    /// <summary>P-ROC version of the <see cref="MachineNode"/>. Get to the database through this</summary>
    public MachinePROC MachinePROC;

    /// <summary>Procgame <see cref="IGameController"/></summary>
    public PinGodNetProcDataGameController NetProcGame;

    private Label _creditsLabel;

    private Task _procGameLoop;

    /// <summary>To cancel the PROC loop</summary>
    private CancellationTokenSource tokenSource;

	/// <summary>Developer config</summary>
	public static PinGodGameConfigOverride PinGodProcConfig { get; private set; } = new();

	public bool GameReady { get; private set; }

	#region Godot Overrides

	public override void _EnterTree()
	{
		LoadLocalProcConfig();

		Logger.LogLevel = (PinGod.Base.LogLevel)((int)PinGodProcConfig.LogLevel);

        //WindowActionsNode._switchWindowEnabled = PinGodProcConfig.SwitchWindowEnabled;

        base._EnterTree();
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

        _modesCanvas = GetNodeOrNull<CanvasLayer>(Paths.ROOT_MAINSCENE+"/Modes");
        if (_modesCanvas == null) Logger.WarningRich("[color=red]", nameof(PinGodGameProc), $": No modes canvas found in Main Scene", "[/color]");

        MachinePROC = this.MachineNode as MachinePROC;
        if (MachinePROC != null)
        {
            //CREATE AND SETUP PROC MACHINE
            try
            {
                CreateProcGame();

                Logger.Info(nameof(PinGodGameProc), ": ProcGame created. Setting up MachineNode from ProcGame.");

                //PinGodProcGame.Logger. = NetProc.Domain.PinProc.LogLevel.Verbose;
                var lvl = (int)LogLevel;
                Logger.LogLevel = (PinGod.Base.LogLevel)lvl;

                if (NetProcGame == null)
                {
                    throw new NullReferenceException("P-ROC game couldn't be created. Simulated: " + PinGodProcConfig.Simulated);
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

    /// <summary> Adds a mode by name </summary>
    /// <param name="mode"></param>
    public virtual void AddMode(string mode) => NetProcGame.AddMode(mode);

    public virtual Node AddModeScene(string scenePath)
    {
        if (_resources != null)
        {
            //get the pre loaded resource, create instance and add to base mode canvas
            try
            {
                var scene = _resources?.GetResource(scenePath.GetBaseName()) as PackedScene;
                var instance = scene.Instantiate();

                NetProcGame.Logger.Log($"instance null: " + instance == null);

                if(_modesCanvas == null)
                    NetProcGame.Logger.Log($" modes canvas not available for this mode: " + scenePath);
                else _modesCanvas.AddChild(instance);

                return instance;
            }
            catch (Exception ex)
            {
                Logger.WarningRich(nameof(PinGodGameProc), nameof(AddModeScene), $": [color=red] mode scene {scenePath} couldn't be created. {ex.Message} [/color]");
                return null;
            }
        }
        else
        {
            Logger.WarningRich(nameof(PinGodGameProc), nameof(AddModeScene), ": [color=yellow]no resources found, can't create attract scene[/color]");
            return null;
        }
    }

    /// <summary>Use the <see cref="PinGodGameConfigOverride.Simulated"/> flag from the PROC.cfg in game directory for settings</summary>
    /// <param name="machineConfig"></param>
    public virtual void CreateProcGame() { }

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

    public virtual void RemoveAttractMode() { }

    /// <summary> Removes a mode from the PROC modes </summary>
    /// <param name="mode"></param>
    public virtual void RemoveMode(PinGodProcMode mode) => NetProcGame.Modes.Remove(mode);

    public virtual void RemoveMode(string mode) => NetProcGame.RemoveMode(mode);

    /// <summary>Override the default window setup to do nothing</summary>
    public override void SetupWindow() { }

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

    /// <summary>Remove the mode scene from the _modesCanvas and QueueFreed</summary>
    /// <param name="sceneName"></param>
    internal void RemoveModeScene(string sceneName)
    {
        var node = _modesCanvas?.GetNodeOrNull(sceneName);
        if (node != null)
        {
            _modesCanvas?.RemoveChild(node);
            node.QueueFree();
        }
    }

    /// <summary>Create a P-ROC config file for config that cannot be in the database. <para/>
    /// This way we can easy make changes to it for simulated and delays in the directory while developing from Godot or filesystem</summary>
    private void LoadLocalProcConfig()
    {
        var cfgPath = $"res://{ProjectSettings.GetSetting("application/config/name")}.cfg";
        var config = new ConfigFile();
        Error err = config.Load(cfgPath);
        if (err != Error.Ok)
        {
            //create config
            config.SetValue("DEV", "simulated", PinGodProcConfig.Simulated);
            config.SetValue("DEV", "delete_db_on_init", PinGodProcConfig.DeleteDbOnInit);
            config.SetValue("DEV", "ignore_db_display", PinGodProcConfig.IgnoreDbDisplay);
            config.SetValue("PROC", "delay", PinGodProcConfig.Delay);
            config.SetValue("DEV", "log_level", (int)PinGodProcConfig.LogLevel);
            config.SetValue("DEV", "switch_window", PinGodProcConfig.SwitchWindowEnabled);
            config.SetValue("MEMORYMAP", "enabled", PinGodProcConfig.MemoryMapEnabled);
            config.SetValue("MEMORYMAP", "write", PinGodProcConfig.MemoryMapWriteDelay);
            config.SetValue("MEMORYMAP", "read", PinGodProcConfig.MemoryMapReadDelay);

            config.Save(cfgPath);
        }
        else
        {
            PinGodProcConfig.Delay = (byte)config.GetValue("PROC", "delay");
            PinGodProcConfig.IgnoreDbDisplay = (bool)config.GetValue("DEV", "ignore_db_display");
            PinGodProcConfig.DeleteDbOnInit = (bool)config.GetValue("DEV", "delete_db_on_init");
            PinGodProcConfig.Simulated = (bool)config.GetValue("DEV", "simulated");
            PinGodProcConfig.LogLevel = (PinGod.Base.LogLevel)((int)config.GetValue("DEV", "log_level"));
            PinGodProcConfig.SwitchWindowEnabled = (bool)config.GetValue("DEV", "switch_window");

            PinGodProcConfig.MemoryMapEnabled = (bool)config.GetValue("MEMORYMAP", "enabled");
            PinGodProcConfig.MemoryMapWriteDelay = (int)config.GetValue("MEMORYMAP", "write");
            PinGodProcConfig.MemoryMapReadDelay = (int)config.GetValue("MEMORYMAP", "read");
        }

        GD.Print("log level: " + PinGodProcConfig.LogLevel);
    }

	/// <summary>
	/// Adds machine items from <see cref="NetProcGame"/> into this Machine node <para/>
	/// TODO: perhaps this machine class overrides the base and uses the <see cref="NetProcGame"/> instead
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
		if (PinGodProcConfig.Simulated)
		{
			NetProcGame._memMap = GetNodeOrNull<MemoryMapPROCNode>("/root/MemoryMap");			

			if (NetProcGame._memMap == null)
				Logger.Warning(nameof(PinGodGameProc), $": WARN: no {nameof(MemoryMapPROCNode)} found in root/MemoryMap");

			NetProcGame.InitSimStates();

			NetProcGame._memMap?.WriteStates();
		}

        //run game loop
		tokenSource = new CancellationTokenSource();
		_procGameLoop = Task.Run(() =>
		{
			Logger.Info(nameof(PinGodGameProc), ":running game loop");

			//run proc game loop delay 1 save CPU //TODO: maybe this run loop needs to re-throw exception if any caught
			NetProcGame.RunLoop(PinGodProcConfig.Delay, tokenSource);

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
		
		if (PinGodProcConfig.IgnoreDbDisplay) return;        

		DisplayServer.WindowSetMode((DisplayServer.WindowMode)NetProcGame.GetAdjustment("DISP_MODE"));
		Display.SetContentScale(GetTree().Root, (Window.ContentScaleModeEnum)NetProcGame.GetAdjustment("DISP_CONT_SCALE_MODE"));
		Display.SetAspectOption(GetTree().Root, (Window.ContentScaleAspectEnum)NetProcGame.GetAdjustment("DISP_CONT_SCALE_ASPECT"));
		//get display size + pos from database values
		Display.SetSize(NetProcGame.GetAdjustment("DISP_W"), NetProcGame.GetAdjustment("DISP_H"));
		Display.SetPosition(NetProcGame.GetAdjustment("DISP_X"), NetProcGame.GetAdjustment("DISP_Y"));
		Display.SetAlwaysOnTop(NetProcGame.GetAdjustment("DISP_TOP") > 0 ? true : false);
	}

	/// <summary>
	/// Save window adjustments to database from the main window. Return if <see cref="IGNORE_DB_DISPLAY_SETTINGS"/> is set to true
	/// </summary>
	private void WindowSaveSettings()
	{
		if (NetProcGame == null) return;
		
		if (PinGodProcConfig.IgnoreDbDisplay) return;		

		var winSize = DisplayServer.WindowGetSize(0);
		var winPos = DisplayServer.WindowGetPosition(0);
		NetProcGame.SetAdjustment("DISP_W", winSize.X); NetProcGame.SetAdjustment("DISP_H", winSize.Y);
		NetProcGame.SetAdjustment("DISP_X", winPos.X); NetProcGame.SetAdjustment("DISP_Y", winPos.Y);
		NetProcGame.SetAdjustment("DISP_TOP", DisplayServer.WindowGetFlag(DisplayServer.WindowFlags.AlwaysOnTop) ? 1 : 0);
		NetProcGame.SetAdjustment("DISP_MODE", (int)DisplayServer.WindowGetMode());
		NetProcGame.SetAdjustment("DISP_CONT_SCALE_MODE", (int)Display.GetContentScale(GetTree().Root));
		NetProcGame.SetAdjustment("DISP_CONT_SCALE_ASPECT", (int)Display.GetAspectOption(GetTree().Root));
	}
}
