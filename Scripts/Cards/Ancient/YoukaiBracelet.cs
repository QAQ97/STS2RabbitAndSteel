using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using RabbitAndSteel.Scripts.CardPools;
using RabbitAndSteel.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Cards.Ancient;
[RegisterCard(typeof(AncientCardPool))]
public class YoukaiBracelet : ModCardTemplate
{
    private const int energyCost = 1;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
    PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );

    public YoukaiBracelet() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("StrengthSpell", 3),
        new DynamicVar("DexteritySpell", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		await PowerCmd.Apply<StrengthSpellPower>(choiceContext, base.Owner.Creature, base.DynamicVars["StrengthSpell"].BaseValue, base.Owner.Creature, this);
		await PowerCmd.Apply<DexteritySpellPower>(choiceContext, base.Owner.Creature, base.DynamicVars["DexteritySpell"].BaseValue, base.Owner.Creature, this);
	}

	protected override void OnUpgrade()
	{
		base.DynamicVars["StrengthSpell"].UpgradeValueBy(2m);
		base.DynamicVars["DexteritySpell"].UpgradeValueBy(2m);
	}
}