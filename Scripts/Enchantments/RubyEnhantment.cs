using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Enchantments;

[RegisterEnchantment]
public class RubyEnchantment : ModEnchantmentTemplate
{
    public override bool ShowAmount => true;

    public override bool HasExtraCardText => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(2, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain)];

    public override EnchantmentAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/icons/Ruby.png"
    );

    public override bool CanEnchantCardType(CardType cardType)
	{
		return cardType == CardType.Attack;
	}
    public override decimal EnchantDamageMultiplicative(decimal originalDamage, ValueProp props)
	{
		if (!props.IsPoweredAttack())
		{
			return 1m;
		}
		return 2m;
	}

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (cardPlay == null) return;
        var player = cardPlay.Card.Owner;

        PlayerCmd.EndTurn(player, canBackOut: false);
    }
}
