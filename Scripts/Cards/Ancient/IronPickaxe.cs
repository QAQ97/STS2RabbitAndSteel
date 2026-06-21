using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using RabbitAndSteel.Scripts.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Cards.Ancient;

[RegisterCard(typeof(AncientCardPool))]
public class IronPickaxe : ModCardTemplate
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(13, ValueProp.Move)
    ];

    public IronPickaxe() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    // ---------- 矿物掉落定义 ----------
    private class OreDrop
    {
        public string Name { get; set; }
        public int GoldValue { get; set; }
        public float Weight { get; set; }
        public string VfxPath { get; set; }
    }

    private static readonly List<OreDrop> OreDrops = new()
    {
        new OreDrop { Name = "Coal",    GoldValue = 3,  Weight = 40f, VfxPath = "res://RabbitAndSteel/images/Animate/Coal/coal.tscn" },
        new OreDrop { Name = "Iron",    GoldValue = 5,  Weight = 30f, VfxPath = "res://RabbitAndSteel/vfx/iron.tscn" },
        new OreDrop { Name = "Gold",    GoldValue = 10, Weight = 20f, VfxPath = "res://RabbitAndSteel/vfx/gold.tscn" },
        new OreDrop { Name = "Diamond", GoldValue = 20, Weight = 10f, VfxPath = "res://RabbitAndSteel/vfx/diamond.tscn" },
    };

    private static T WeightedRandom<T>(Rng rng, IEnumerable<T> items, Func<T, float> weightSelector)
    {
        float totalWeight = items.Sum(weightSelector);
        float roll = rng.NextFloat(totalWeight);
        foreach (var item in items)
        {
            roll -= weightSelector(item);
            if (roll <= 0f)
                return item;
        }
        return items.Last();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        Rng rng = Owner.PlayerRng.Rewards;
        var chosenOre = WeightedRandom(rng, OreDrops, o => o.Weight);
        VfxCmd.PlayOnCreature(cardPlay.Target, chosenOre.VfxPath);
        Owner.Gold += chosenOre.GoldValue;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}