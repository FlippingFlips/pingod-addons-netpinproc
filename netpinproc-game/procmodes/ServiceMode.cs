using Godot;
using NetPinProc.Domain;
using PinGod.Core;
using System.Linq;

/// <summary>This is a P-ROC Mode that runs the godot service mode scenes found in the addons `netpinproc-servicemode`<para/>
/// - This handles the P-ROC door switches (switches tagged with door)<para/>
/// - All P-ROC switches are handled if not named `not_used`<para/>
/// - The reason all switches are mapped are for tests inside service modes</summary>
public class ServiceMode : PinGodProcMode
{
    /// <summary>created from switches in the machine config tagged with door</summary>
    private readonly string[] _doorSwitches;

    /// <summary>Can user exit from the service mode</summary>
    private bool _canExit = true;

    private PinGodGameProc _pinGodProc;

    /// <summary>The script attached to the service mode scene  <para/>
    /// React to PROC events here then shift onto the UI through this</summary>
    private ServiceModePinGod _serviceModePinGod;

    const string SERVICE_SCENE = "res://addons/netpinproc-servicemode/ServiceModePROC.tscn";

    /// <summary>Sets up handlers for all switches and door switches.</summary>
    /// <param name="game"></param>
    /// <param name="pinGod"></param>
    /// <param name="name"></param>
    /// <param name="priority"></param>
    /// <param name="defaultScene"></param>
    /// <param name="loadDefaultScene"></param>
    public ServiceMode(
        IGameController game,
        IPinGodGame pinGod,
        string name = nameof(ServiceMode),
		int priority = 80,
        string defaultScene = SERVICE_SCENE,
        bool loadDefaultScene = true) :
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
	}

    /// <summary>Handles switches coming from the PROC board or fake <para/>
    /// Posts switches onto the game thread<para/>
    /// Invokes the <see cref="ServiceModePinGod.OnSwitchPressed(string, ushort, bool)"/> on the Godot script: ServiceModePinGod</summary>
    /// <param name="sw"></param>
    /// <returns></returns>
    public virtual bool HandleSwitches(NetPinProc.Domain.Switch sw)
    {
        _serviceModePinGod?.CallDeferred(("OnSwitchPressed"),sw.Name, sw.Number, sw.IsClosed());

        return SWITCH_CONTINUE;
    }
        
    public override void ModeStarted()
    {
        //invoke this to create the scene and add it to the tree
        base.ModeStarted();

        //cast the instance loaded from the default scene
        //we can call deferred on this instance and onto the game thread if we need to interact with the scene directly
        _serviceModePinGod = ModeSceneInstance as ServiceModePinGod;
    }

    /// <summary>P-ROC Service mode buttons are handed off to <see cref="ServiceModePinGod.OnServiceButtonPressed(string)"/></summary>
    /// <param name="sw"></param>
    /// <returns></returns>
    bool HandleDoorSwitch(NetPinProc.Domain.Switch sw)
	{
		switch (sw.Name)
		{
			case "exit":
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
