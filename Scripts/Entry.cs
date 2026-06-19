using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;
using Godot.Bridge;
using RabbitAndSteel.Scripts.Cards.Ancient;
using STS2RitsuLib.Patching.Core;
using RabbitAndSteel.Scripts.Patches;
using STS2RitsuLib.RunData;
using RabbitAndSteel.Scripts.Models;


namespace RabbitAndSteel.Scripts;

[ModInitializer(nameof(Init))]
public class Entry
{
    public const string ModId = "RabbitAndSteel";
        // 全局数据句柄
    public static RunSavedData<ChallengeRunState> Challenge = null!;
    // 玩家数据句柄
    public static PlayerRunSavedData<PlayerRunState> Player = null!;
    public static readonly MegaCrit.Sts2.Core.Logging.Logger Logger = RitsuLibFramework.CreateLogger(ModId);

    public static void Init()
    {
        var harmony = new Harmony("sts2.reme.RabbitAndSteel");
        harmony.PatchAll();

        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        Log.Info("Mod initialized!");

        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);

        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

        RitsuLibFramework.RegisterArchaicToothTranscendenceMapping<AbyssalCall, Unleashed>();
        var patcher = RitsuLibFramework.CreatePatcher(ModId, "core-patches");
        patcher.RegisterPatch<MonsterModel_VisualsPatch>();
        patcher.RegisterPatch<CombatWeightCounterPatch>();

        if (!patcher.PatchAll())
            throw new InvalidOperationException("Critical patches failed.");

        using (RitsuLibFramework.BeginModDataRegistration(ModId))
        {
            var store = RitsuLibFramework.GetRunSavedDataStore(ModId);

            // 注册全局共享的配置
            Challenge = store.Register(
                key: "challenge",
                defaultFactory: () => new ChallengeRunState(),
                options: new RunSavedDataOptions
                {
                    WritePolicy = RunSavedDataWritePolicy.WhenNonDefault,
                    SyncLobbyOnChange = true, // 允许在大厅修改时同步给队友
                });

            // 注册按玩家独立的配置
            Player = store.RegisterPerPlayer(
                key: "player",
                defaultFactory: () => new PlayerRunState(),
                options: new RunSavedDataOptions
                {
                    WritePolicy = RunSavedDataWritePolicy.WhenSet,
                    SyncLobbyOnChange = true, // 允许在大厅修改时同步给队友
                });
        }
        
    }
}

