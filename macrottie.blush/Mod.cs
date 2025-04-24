using GDWeave;
using macrottie.blush.Patches;

namespace macrottie.blush;

public class Mod : IMod {
    public Config Config;

    public Mod(IModInterface modInterface) {
        this.Config = modInterface.ReadConfig<Config>();
        
        modInterface.RegisterScriptMod(new PlayerPatcher());
    }

    public void Dispose() {
        // Cleanup anything you do here
    }
}
