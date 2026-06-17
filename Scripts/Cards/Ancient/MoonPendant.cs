using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RabbitAndSteel.Scripts.CardPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
namespace RabbitAndSteel.Scripts.Cards.Ancient;


[RegisterCard(typeof(AncientCardPool))]

public class MoonPendant : ModCardTemplate
{
	private const int energyCost = 1;
	private const CardType type = CardType.Attack;
	private const CardRarity rarity = CardRarity.Common;
	private const TargetType targetType = TargetType.AllEnemies;
    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] {CardKeyword.Retain};
	private const bool shouldShowInCardLibrary = true;
	public override CardAssetProfile AssetProfile => new(
	PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
	);
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(4, ValueProp.Move)
	];
	public MoonPendant() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
	{
	}
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState)
        .Execute(choiceContext);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}