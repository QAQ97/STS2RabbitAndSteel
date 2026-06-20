using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using RabbitAndSteel.Scripts.CardPools;
using RabbitAndSteel.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Cards.Ancient;
[RegisterCard(typeof(AncientCardPool))]
public class RabbitAndTank : ModCardTemplate
{
    private const int energyCost = 3;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
    PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );

    public RabbitAndTank() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<ArtifactPower>(),
        HoverTipFactory.FromPower<VigorPower>(),
        HoverTipFactory.FromPower<PlatingPower>()
        ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("RabbitAndTank", 1),
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<ArtifactPower>(choiceContext, base.Owner.Creature, 1m, Owner.Creature, this);
		await PowerCmd.Apply<RabbitAndTankPower>(choiceContext, base.Owner.Creature, base.DynamicVars["RabbitAndTank"].BaseValue, base.Owner.Creature, this);
	}

	protected override void OnUpgrade()
	{
		AddKeyword(CardKeyword.Innate);
	}
}