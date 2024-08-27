using Godot;
using NetPinProc.Domain;
using PinGod.Core;
using System.Linq;

/// <summary>This is a P-ROC Mode that runs the godot service mode scenes. Service mode scene files are in `netpinproc-servicemode`<para/>
/// This handles the door switches (switches tagged with door) and pushes this onto the scenes script with "OnServiceButtonPressed" <para/>
/// Each time a door handle switch is open/closed the the `_serviceModePinGod.OnSwitchPressed` is called.<para/></summary>
public class ServiceMode : PinGodProcMode
{
    /// <summary>created from switches in the machine cofig tagged with door</summary>
    private readonly string[] _doorSwitches;

	private PinGodGameProc _pinGodProc;
	private PackedScene _serviceModeScene;
	private Node _serviceModeInstance;

	/// <summary>Can user exit from the service mode</summary>
    private bool _canExit = true;

	/// <summary>The script attatched to the service mode scene  <para/>
	/// React to PROC events here then shift onto the UI through this</summary>
    private ServiceModePinGod _serviceModePinGod;

    public ServiceMode(IGameController game, IPinGodGame pinGod, string name = nameof(ServiceMode),
		int priority = 80, string defaultScene = null, bool loadDefaultScene = true) :
		base(game, name, priority, pinGod, defaultScene, loadDefaultScene)
	{
		//get all switches tagged as 'door' and add a AddSwitchHandler to invoke HandleDoorSwitch
		_doorSwitches = Game.Config.GetNamesFromTag("door", MachineItemType.Switch);		

		if (_doorSwitches?.Length > 0)
		{
			//handle each switch when closed
			for (int i = 0; i < _doorSwitches.Length; i++)
			{
				AddSwitchHandler(_doorSwitches[i], SwitchHandleType.closed, 0, new SwitchAcceptedHandler(HandleDoorSwitch));
			}                
		}
		else { Game.Logger.Log("WARN: no door switches found.", NetPinProc.Domain.PinProc.LogLevel.Warning); }


		var allSwitches = Game.Switches.Values.Where(x => !x.Name.Contains("not_used"));
		foreach (var switches in allSwitches)
		{
            AddSwitchHandler(switches.Name, SwitchHandleType.open, 0, new SwitchAcceptedHandler(HandleSwitches));
            AddSwitchHandler(switches.Name, SwitchHandleType.closed, 0, new SwitchAcceptedHandler(HandleSwitches));
		}

		//PingodGame p-roc, use to get hold of the machine so we can add credits.
		_pinGodProc = pinGod as PinGodGameProc;
	}

	public override void LoadDefaultSceneToCanvas(string scenePath)
	{
		if(_modesCanvas != null)
		{
			var res = GD.Load<PackedScene>(scenePath);
			var inst = res.Instantiate();
			CanvasLayer?.AddChild(inst);
			_modesCanvas.AddChild(CanvasLayer);
		}
	}

	public override void ModeStarted()
	{
		if (_resources != null)
		{
			//get the pre loaded resource, create instance and add to base mode canvas
			_serviceModeScene = _resources?.GetResource(defaultScene) as PackedScene;
			_serviceModeInstance = _serviceModeScene.Instantiate();
            _serviceModePinGod = _serviceModeInstance as ServiceModePinGod;
            AddChildSceneToCanvasLayer(_serviceModeInstance);
		}
		else { Logger.WarningRich(nameof(AttractMode), nameof(ModeStarted), ": [color=yellow]no resources found, can't create attract scene[/color]"); }
	}

    /// <summary>Invokes OnSwitchPressed on the Godot user interface through _serviceModePinGod</summary>
    /// <param name="sw"></param>
    /// <returns></returns>
    public virtual bool HandleSwitches(NetPinProc.Domain.Switch sw)
	{
        _serviceModePinGod?.CallDeferred("OnSwitchPressed", sw.Name, sw.Number, sw.IsClosed());
        return SWITCH_CONTINUE;
    }

    bool HandleDoorSwitch(NetPinProc.Domain.Switch sw)
	{
		switch (sw.Name)
		{
			case "exit":
    //            //Exiting service menu?
    //            if (Game.Switches["coinDoor"].IsOpen() && _canExit)
    //            {
    //                //Game.Modes.Modes.RemoveAll(x => x.GetType() == typeof(ServiceMode));
    //                _pinGodProc.CallDeferredThreadGroup("AddMode", "attract");

    //                Game.Modes.ToString();
    //                //_pinGodProc.PinGodProcGame.Modes.Modes.Remove(_pinGodProc.PinGodProcGame._AttractMode);
    //            }
				//else { _serviceModePinGod?.CallDeferred("OnServiceButtonPressed", sw.Name); }
    //            break;
			case "up":
            case "down":
            case "enter":
                _serviceModePinGod?.CallDeferred("OnServiceButtonPressed", sw.Name);
                break;
			default:
				break;
		}
		return SWITCH_CONTINUE;
	}
}
