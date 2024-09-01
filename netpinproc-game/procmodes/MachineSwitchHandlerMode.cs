using NetPinProc.Domain;
using System.Linq;

/// <summary> P-ROC Mode that handles all the door switches and coins in the machine.<para/>
/// - This mode doesn't have a scene<para/>
/// - This mode adds a service mode and removes the attract if the coin door is open and enter is used on the door<para/>
/// - Handles the coin slots and sends event to Godot with AddCredits
/// - TODO: volume from coin door, send events door open</summary>
public class MachineSwitchHandlerMode : PinGodProcMode
{    
    private string[] _doorSwitches;

    private PinGodGameProc _pinGodProc;

    /// <summary>Mode to run if coin door opened and enter pushed for service</summary>
    private ServiceMode _serviceMode = null;

    public MachineSwitchHandlerMode(IGameController game,
        IPinGodGame pinGod,
        string name = nameof(MachineSwitchHandlerMode),
        int priority = 80,
        string defaultScene = null,
        bool loadDefaultScene = false) :
        base(game, name, priority, pinGod, defaultScene, loadDefaultScene)
    {
        //PingodGame p-roc, use to get hold of the machine so we can add credits.
        _pinGodProc = pinGod as PinGodGameProc;

        //get all switches tagged as 'door' and add a AddSwitchHandler to invoke HandleDoorSwitch
        _doorSwitches = Game.Config.GetNamesFromTag("door", MachineItemType.Switch);
        if (_doorSwitches?.Length > 0)
        {
            for (int i = 0; i < _doorSwitches.Length; i++)
            {
                if (_doorSwitches[i] == "coinDoor")
                    AddSwitchHandler(_doorSwitches[i], SwitchHandleType.open, 0, new SwitchAcceptedHandler(HandleDoorSwitch));
                else
                    AddSwitchHandler(_doorSwitches[i], SwitchHandleType.closed, 0, new SwitchAcceptedHandler(HandleDoorSwitch));
            }                
        }
        else { Game.Logger.Log("WARN: no door switches found.", NetPinProc.Domain.PinProc.LogLevel.Warning); }        
    }

    public bool HandleDoorSwitch(Switch sw)
    {
        switch (sw.Name)
        {
            case "down":
                if (Game.Switches["coinDoor"].IsOpen() && _serviceMode == null)
                {
                    Game.Logger.Log("todo: volume down", NetPinProc.Domain.PinProc.LogLevel.Info);
                }
                break;
            case "up":
                //todo: check if the service isn't running as well, this is our volume
                if (Game.Switches["coinDoor"].IsOpen() && _serviceMode == null)
                {
                    //TODO: volume up
                    //bool result = _pinGodProc.PinGodProcGame.IsModeRunning("ServiceMode");                    
                    Game.Logger.Log("todo: volume up ", NetPinProc.Domain.PinProc.LogLevel.Info);
                }
                break;
            case "enter":
                if (Game.Switches["coinDoor"].IsOpen() && !Game.Modes.Modes.Any(x => x.GetType() == typeof(ServiceMode)))
                {
                    //adds the service mode and remove attract
                    //_pinGodProc.CallDeferred("RemoveMode", "attract");

                    _pinGodProc.NetProcGame.RemoveMode("attract");
                    _pinGodProc.CallDeferred("AddMode", "service");

                    //Game.Modes.Remove(_pinGodProc.PinGodProcGame.);
                }
                break;
            case "coinDoor":
                if (sw.IsOpen())
                {
                    //todo: hardware: cut the power on the ground switch
                    Game.Logger.Log("coinDoor open");
                }
                else
                {
                    Game.Logger.Log("coinDoor closed");
                }
                break;
            case "coin1":
                UpdateCredits(1);
                break;
            case "coin2":
                UpdateCredits(2);
                break;
            case "coin3":
                UpdateCredits(3);
                break;
            default:
                break;
        }
        return SWITCH_CONTINUE;
    }

    public override void ModeStarted()
    {
        base.ModeStarted();
        Delay(nameof(UpdateCredits), NetPinProc.Domain.PinProc.EventType.None, 0.3, new AnonDelayedHandler(UpdateCredits));
    }

    /// <summary>Just a dummy trigger to update credits when this mode starts</summary>
    internal void UpdateCredits() => UpdateCredits(0);

    /// <summary>Increment the local <see cref="Credits"/>, update the database lookup, then update the view <see cref="CreditsLabel"/></summary>
    /// <param name="amt"></param>
    private void UpdateCredits(int amt = 0)
    {        
        _pinGodProc?.CallDeferred(nameof(_pinGodProc.AddCredits), ((byte)amt));

        _pinGodProc.NetProcGame.IncrementAudit("CREDITS", amt);
    }
}
