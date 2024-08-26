using NetPinProc.Domain.PinProc;
using PinGod.Core;
using PinGodAddOns.netpinproc_game;

public partial class MyPinGodProcGame : PinGodGameProc
{
    /// <summary>Creates a MyPinGodProcGameController <para/>
    /// Use the <see cref="PinGodGameConfigOverride.Simulated"/> flag from the PROC.cfg in game directory for settings</summary>
    /// <param name="machineConfig"></param>
    public override void CreateProcGame()
    {
        if (NetProcGame == null)
        {
            var pinGodLogger = new PinGodProcLogger() { LogLevel = (LogLevel)PinGodProcConfig.LogLevel };
            NetProcGame = new MyPinGodProcGameController(
                MachineType.PDB, this,
                PinGodProcConfig.DeleteDbOnInit,
                pinGodLogger, PinGodProcConfig.Simulated);
        }
        else
            Logger.Warning("game has already been created...");
    }

    public override void RemoveAttractMode() =>
        NetProcGame.Modes
        .Remove((NetProcGame as MyPinGodProcGameController)._AttractMode);
}
