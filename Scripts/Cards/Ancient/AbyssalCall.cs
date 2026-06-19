using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Random;
using RabbitAndSteel.Scripts.Commands;
using RabbitAndSteel.Scripts.Monsters.Pets;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace RabbitAndSteel.Scripts.Cards.Ancient;
[RegisterCard(typeof(EventCardPool))]
public class AbyssalCall : ModCardTemplate
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Event;
    private const TargetType targetType = TargetType.Self;
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain
    ];
    private const bool shouldShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );


    public AbyssalCall() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        Rng rng = Owner.PlayerRng.Rewards;

        // 1. 找到当前存活的宠物（任意类型）
        var currentPet = Owner.Creature.CombatState.Allies
            .FirstOrDefault(c => c.PetOwner == Owner && c.IsAlive);

        Type? currentType = currentPet?.Monster?.GetType();

        // 2. 随机一个不同于当前类型的宠物名称
        string? petName = RollPet(rng, currentType);
        if (petName == null)
        {
            // 没有可切换的类型，卡牌无效果（或播放失败特效）
            return;
        }

        // 3. 安全移除旧宠物（数据 + 节点）
        if (currentPet != null && currentPet.Monster?.GetType()?.Name != petName)
        {
            PetManager.RemovePetSafely(currentPet);
        }

        // 4. 召唤新宠物（此时不存在存活宠物，会进入全新召唤分支）
        switch (petName)
        {
            case "Hero":
                await PetManager.SummonPet<Hero>(choiceContext, Owner, this);
                break;
            case "Dreadwyrm":
                await PetManager.SummonPet<Dreadwyrm>(choiceContext, Owner, this);
                break;
            case "Stormbeast":
                await PetManager.SummonPet<Stormbeast>(choiceContext, Owner, this);
                break;
        }
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    
    protected override PileType GetResultPileTypeForCardPlay()
    {
        PileType resultPileTypeForCardPlay = base.GetResultPileTypeForCardPlay();
        if (resultPileTypeForCardPlay != PileType.Discard)
        {
            return resultPileTypeForCardPlay;
        }
        return PileType.Hand;
    }

    public static string? RollPet(Rng rng, Type? currentPetType = null)
    {
        var choices = new WeightedList<string>();

        if (currentPetType != typeof(Hero))
            choices.Add("Hero", 1);
        if (currentPetType != typeof(Stormbeast))
            choices.Add("Stormbeast", 1);

        // 如果所有宠物都被排除（比如只有一种宠物且恰好正在使用），返回 null
        if (choices.Count == 0)
            return null;

        return choices.GetRandom(rng, remove: true);
    }
    
}