using Godot;
using NetPinProc.Domain;
using PinGod.Base;
using PinGod.Core.Service;

/// <summary>Only when running simulated. FakePROC.<para/>
/// This derives the MemoryMap from `pingod-addons`</summary>
public class MemoryMapPROC : MemoryMap
{
    private PinGodNetProcDataGameController _game;

    byte[] _coilBuffer;    
    byte[] _lampBuffer;
    int[] _ledBuffer;

    public MemoryMapPROC(IGameController game, string mutexName = "pingod_vp_mutex", string mapName = "pingod_vp",
                    int writeStates = -1, int readStates = -1,
            byte coilCount = 32, byte lampCount = 64, byte ledCount = 64, byte switchCount = 64,
            byte? vpCommandSwitch = null) : base (mutexName, mapName, writeStates, readStates, coilCount, lampCount, ledCount, switchCount, vpCommandSwitch)
    {
        _game = game as PinGodNetProcDataGameController;

        if (_coilBuffer == null) _coilBuffer = new byte[TOTAL_COIL];
        if (_lampBuffer == null) _lampBuffer = new byte[TOTAL_LAMP];
        if (_ledBuffer == null) _ledBuffer = new int[TOTAL_LED];
    }

    /// <summary> Writes states from the P-ROC </summary>
    public void WriteProcStates() 
    {
        ////CHECK IF COIL STATES ARE CHANGING
        //if (!Enumerable.SequenceEqual(_game._lastCoilStates, _coilBuffer))
        //{
        //    Logger.Debug("coil states changed");
        //}

        //CHECK IF LED STATES ARE CHANGING
        //if (!Enumerable.SequenceEqual(_game._lastLedStates, _ledBuffer))
        //{
        //    //Logger.Debug("led states changed");
        //}

        //get states saved from the game proc game loop. RunLoop
        _game._lastCoilStates.CopyTo(_coilBuffer, 0);
        _game._lastLampStates.CopyTo(_lampBuffer, 0);
        _game._lastLedStates.CopyTo(_ledBuffer, 0);

        //write the states to memory
        viewAccessor.WriteArray(1, _coilBuffer, 0, _coilBuffer.Length);
        viewAccessor.WriteArray(_offsetLamps, _lampBuffer, 0, _lampBuffer.Length);
        viewAccessor.WriteArray(_offsetLeds, _ledBuffer, 0, _ledBuffer.Length);        
    }

    /// <summary>
    /// Dummy to stop base doing nothing on write states. States written in game loop.
    /// </summary>
    public override void WriteStates()
    {
        //base.WriteStates();
        return;
    }

    protected override void ReadStates()
    {
        //read teh switches as normal
        base.ReadStates();

        //read the game state
        _gameStateAccess.Read(0, out byte gameState);
        switch ((GameSyncState)gameState)
        {
            //Stop the memory map tasks and quit the game window tree
            case GameSyncState.quit: 
                Stop();
                _game.PinGodProcGame.GetTree().Quit(0);
                return;
            case GameSyncState.pause: //pause / resume on a toggle, not held down
                _game.PinGodProcGame.GetNode("/root").GetTree().Paused = true;
                break;
            case GameSyncState.resume:
                _game.PinGodProcGame.GetNode("/root").GetTree().Paused = false;
                break;
            case GameSyncState.reset:
                var ev = new InputEventAction() { Action = "", Pressed = true };
                ev.Action = gameState.ToString();
                ev.Pressed = true;
                Input.ParseInputEvent(ev);
                break;
            default:
                break;
        }        
    }
}
