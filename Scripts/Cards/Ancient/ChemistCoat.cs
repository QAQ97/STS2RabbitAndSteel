using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using RabbitAndSteel.Scripts.CardPools;
using RabbitAndSteel.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Cards.Ancient;

[RegisterCard(typeof(AncientCardPool))]
public class ChemistCoat : ModCardTemplate
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    public override bool GainsBlock => true;
    private const bool shouldShowInCardLibrary = true;
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );

    public ChemistCoat() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<PoisonPower>(3m),
        new DynamicVar("Decay", 2m),
        new BlockVar(4, ValueProp.Move)
    ];
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<PoisonPower>()
        ,HoverTipFactory.FromPower<DecayPower>()
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null) return;
        if (IsUpgraded)
        {
           await PowerCmd.Apply<DecayPower>(
                choiceContext, 
                target,
                base.DynamicVars["Decay"].IntValue,
                base.Owner.Creature, 
                this
            );
        }
        
        await PowerCmd.Apply<PoisonPower>(choiceContext, target, DynamicVars.Poison.BaseValue, Owner.Creature, this);

        int debuffTypeCount = target.Powers
            .Where(p => p.Type == PowerType.Debuff)
            .Select(p => p.GetType())
            .Distinct()
            .Count();
        for (int i = 0; i < debuffTypeCount; i++)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        }
    }
}