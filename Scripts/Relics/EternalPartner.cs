using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using RabbitAndSteel.Scripts.Cards.Ancient;
using RabbitAndSteel.Scripts.Characters;
using STS2RitsuLib;
using STS2RitsuLib.Interactions.RightClick;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Ui.Toast;

namespace RabbitAndSteel.Scripts.Relics;

[RegisterRelic(typeof(AncientRelicPool))]
[RegisterCharacterStarterRelic(typeof(AncientCharacter))]
public class EternalPartner : ModRelicTemplate, IModRightClickableRelic
{
    // 所有可解锁的宠物类型
    private static readonly string[] AllPetTypes = ["Hero", "Stormbeast", "Dreadwyrm","LichOfFlowers"];

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        IconOutlinePath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png",
        BigIconPath: $"res://RabbitAndSteel/images/ancient/relics/{GetType().Name}.png"
    );

    public override bool ShowCounter => !base.IsCanonical;
    public override int DisplayAmount
    {
        get
        {
            if (base.IsCanonical || Owner == null)
                return -1;

            var data = Entry.Player.Get(Owner);
            return data?.CurrentStacks ?? -1;
        }
    }

    // ----- 战斗开始添加一张 AbyssalCall -----
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || Owner.Creature.CombatState.RoundNumber != 1)
            return;
        var template = ModelDb.Card<AbyssalCall>();
        var card = Owner.Creature.CombatState.CreateCard(template, Owner);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }

    // 辅助属性：访问玩家运行状态中的解锁列表
    private RunState? CurrentRunState => Owner?.RunState as RunState;

    private HashSet<string>? UnlockedPets
    {
        get
        {
            var rs = CurrentRunState;
            if (rs == null || Owner == null) return null;
            return Entry.Player.Get(rs, Owner.NetId)?.UnlockedPets;
        }
    }

    public void AddStacks(int amount)
    {
        var rs = CurrentRunState;
        if (rs == null || Owner == null) return;
        Entry.Player.Modify(rs, Owner.NetId, data => data.CurrentStacks += amount);
        InvokeDisplayAmountChanged();
    }

    public void RemoveStacks(int amount)
    {
        var rs = CurrentRunState;
        if (rs == null || Owner == null) return;
        Entry.Player.Modify(rs, Owner.NetId, data => data.CurrentStacks -= amount);
        InvokeDisplayAmountChanged();
    }

    public int GetCurrentStacks()
    {
        var rs = CurrentRunState;
        if (rs == null || Owner == null) return 0;
        return Entry.Player.Get(rs, Owner.NetId)?.CurrentStacks ?? 0;
    }

    public void SetStacks(int value)
    {
        var rs = CurrentRunState;
        if (rs == null || Owner == null) return;
        Entry.Player.Modify(rs, Owner.NetId, data => data.CurrentStacks = value);
        InvokeDisplayAmountChanged();
    }

    public async Task OnRightClick(ModRightClickExecutionContext context)
    {
        var rs = CurrentRunState;
        if (rs == null || Owner == null)
        {
            RitsuToastService.ShowError("无法获取运行状态。");
            return;
        }

        if (GetCurrentStacks() < 10)
        {
            RitsuToastService.ShowWarning("需要至少 10 层才能解锁新宠物。");
            return;
        }

        var unlocked = UnlockedPets;
        if (unlocked == null)
        {
            RitsuToastService.ShowError("无法获取玩家数据。");
            return;
        }

        var available = AllPetTypes.Where(pet => !unlocked.Contains(pet)).ToList();
        if (available.Count == 0)
        {
            RitsuToastService.ShowInfo("所有宠物已解锁！");
            return;
        }

        // 使用 RitsuLib 托管 RNG 流，避免多人游戏 Niche RNG 状态不同步
        var rng = RitsuLibFramework.GetModPlayerRng(Owner, Entry.ModId, "eternal_partner_unlock");
        string chosenPet = available[rng.NextInt(available.Count)];
        Entry.Player.Modify(rs, Owner.NetId, data => data.UnlockedPets.Add(chosenPet));
        RemoveStacks(10);
        RitsuToastService.ShowInfo($"已解锁宠物：{chosenPet}！");
    }
}