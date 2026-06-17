using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RabbitAndSteel.Scripts.CardPools;
using RabbitAndSteel.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
namespace RabbitAndSteel.Scripts.Cards.Ancient;


[RegisterCard(typeof(AncientCardPool))]

public class FlameBow : ModCardTemplate
{
	private const int energyCost = 1;
	private const CardType type = CardType.Attack;
	private const CardRarity rarity = CardRarity.Common;
	private const TargetType targetType = TargetType.AnyEnemy;
	private const bool shouldShowInCardLibrary = true;
	public override CardAssetProfile AssetProfile => new(
	PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
	);
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(6, ValueProp.Move)
	];
	
	public FlameBow() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
	{
	}
	protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<BurnPower>()];


	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		
		AttackCommand attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
			.FromCard(this)
			.Targeting(cardPlay.Target)
			.Execute(choiceContext);
		int totalDamage = 0;
		foreach (var damageResults in attackCommand.Results)
		{
			foreach (var result in damageResults)
			{
				totalDamage += result.TotalDamage + result.OverkillDamage;
			}
		}
		
		await PowerCmd.Apply<BurnPower>(choiceContext, cardPlay.Target, totalDamage, base.Owner.Creature, this);
	}
	protected override void OnUpgrade()
	{
		base.OnUpgrade();
		DynamicVars.Damage.UpgradeValueBy(3);
	}
}
