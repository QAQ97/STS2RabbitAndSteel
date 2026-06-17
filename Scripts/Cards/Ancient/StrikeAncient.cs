using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using RabbitAndSteel.Scripts.CardPools;
using RabbitAndSteel.Scripts.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Cards.Ancient;

[RegisterCard(typeof(AncientCardPool))]

[RegisterCharacterStarterCard(typeof(AncientCharacter), 4)]
public class StrikeAncient : ModCardTemplate
{
	private const int energyCost = 1;
	private const CardType type = CardType.Attack;
	private const CardRarity rarity = CardRarity.Basic;
	private const TargetType targetType = TargetType.AnyEnemy;
	protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Strike };
	private const bool shouldShowInCardLibrary = true;
	public override CardAssetProfile AssetProfile => new(
	PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
	);
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(6, ValueProp.Move)
	];

	public StrikeAncient() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
			.FromCard(this)
			.Targeting(cardPlay.Target!)
			.Execute(choiceContext);
	}
	protected override void OnUpgrade()
	{
		base.OnUpgrade();
		DynamicVars.Damage.UpgradeValueBy(3);
	}
}
