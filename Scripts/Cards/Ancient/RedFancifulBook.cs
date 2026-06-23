using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RabbitAndSteel.Scripts.CardPools;
using STS2RitsuLib.Combat.HandSize;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Cards.Ancient;

[RegisterCard(typeof(AncientCardPool))]
public class RedFancifulBook : ModCardTemplate, IMaxHandSizeModifier
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1)
    ];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );
    public RedFancifulBook() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
	{
		if (player != base.Owner && PileType.Hand.GetPile(Owner).Cards.Contains(this))
		{
			return amount;
		}
        int bonus = IsUpgraded ? 2 : 1;
		return amount + bonus;
	}

	public override decimal ModifyHandDraw(Player player, decimal count)
	{
		if (player != base.Owner && PileType.Hand.GetPile(Owner).Cards.Contains(this))
		{
			return count;
		}
		return Math.Max(0m, count - 1m);
	}

    protected override void OnUpgrade()
	{
        base.DynamicVars.Energy.UpgradeValueBy(1);
	}
}