using Godot;

public class PinGodProcGameConfigOverride : PinGodGameConfigOverride
{
    public bool Simulated { get; set; } = true;
    public byte Delay { get; set; } = 10;
    public bool IgnoreDbDisplay { get; set; } = true;
    public bool DeleteDbOnInit { get; set; }

    /// <summary>Create a P-ROC config file for config that cannot be in the database. <para/>
    /// This way we can easy make changes to it for simulated and delays in the directory while developing from Godot or filesystem</summary>
    public override ConfigFile Load()
    {
        var config = base.Load();
        if (config.HasSection("PROC"))
        {
            Delay = (byte)config.GetValue("PROC", nameof(Delay));
            IgnoreDbDisplay = (bool)config.GetValue("PROC", nameof(IgnoreDbDisplay));
            DeleteDbOnInit = (bool)config.GetValue("PROC", nameof(DeleteDbOnInit));
            Simulated = (bool)config.GetValue("PROC", nameof(Simulated));

            //config.Save(cfgPath); //save now? don't need?
        }
        else
        {
            config.SetValue("PROC", nameof(Simulated), Simulated);
            config.SetValue("PROC", nameof(Delay), Delay);
            config.SetValue("PROC", nameof(DeleteDbOnInit), DeleteDbOnInit);
            config.SetValue("PROC", nameof(IgnoreDbDisplay), IgnoreDbDisplay);
        }

        return config;
    }
}
