using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using RabbitAndSteel.Scripts.Cards.Ancient;
using RabbitAndSteel.Scripts.Characters;
using STS2RitsuLib.Interactions.RightClick;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Ui.Toast;

namespace RabbitAndSteel.Scripts.Relics;

[RegisterRelic(typeof(AncientRelicPool))]
[RegisterCharacterStarterRelic(typeof(AncientCharacter))]
public class EternalPartner : ModRelicTemplate, IModRightClickableRelic
{
    // 所有可解锁的宠物类型（可根据需要扩展）
    private static readonly string[] AllPetTypes = ["Hero", "Stormbeast", "Dreadwyrm"];

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

    // 辅助属性：访问玩家运行状态中的解锁列表
    private HashSet<string>? UnlockedPets => Entry.Player.Get(Owner)?.UnlockedPets;

    public void AddStacks(int amount)
    {
        if (Owner == null) return;
        Entry.Player.Modify(Owner, data => data.CurrentStacks += amount);
        InvokeDisplayAmountChanged();
    }

    public void RemoveStacks(int amount)
    {
        if (Owner == null) return;
        Entry.Player.Modify(Owner, data => data.CurrentStacks -= amount);
        InvokeDisplayAmountChanged();
    }

    public int GetCurrentStacks()
    {
        if (Owner == null) return 0;
        return Entry.Player.Get(Owner)?.CurrentStacks ?? 0;
    }

    public void SetStacks(int value)
    {
        if (Owner == null) return;
        Entry.Player.Modify(Owner, data => data.CurrentStacks = value);
        InvokeDisplayAmountChanged();
    }

    // ----- 战斗开始添加一张 AbyssalCall -----
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || Owner.Creature.CombatState.RoundNumber != 1)
            return;
        Flash();

        var template = ModelDb.Card<AbyssalCall>();
        var card = Owner.Creature.CombatState.CreateCard(template, Owner);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }

    // ----- 右键点击逻辑（IModRightClickableRelic）-----
    public async Task OnRightClick(ModRightClickExecutionContext context)
    {
  
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

        // 获取尚未解锁的宠物列表
        var available = AllPetTypes.Where(pet => !unlocked.Contains(pet)).ToList();
        if (available.Count == 0)
        {
            RitsuToastService.ShowInfo("所有宠物已解锁！");
            return;
        }

        // 随机选择一种（使用玩家自己的 Rng，保证可复现）
        Rng rng = Owner.RunState.Rng.CombatTargets;
        string chosenPet = available[rng.NextInt(available.Count)];
        Entry.Player.Modify(Owner, data => data.UnlockedPets.Add(chosenPet));
        RemoveStacks(10);
        RitsuToastService.ShowInfo($"已解锁宠物：{chosenPet}！");
    }
}