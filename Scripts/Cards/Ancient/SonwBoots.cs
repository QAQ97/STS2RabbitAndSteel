using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
public class SonwBoots : ModCardTemplate
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    public override bool GainsBlock => true;
    private const bool shouldShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(4, ValueProp.Move)
        , new DynamicVar("Freeze", 1)
    ];

    public SonwBoots() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<FreezePower>()
        ,HoverTipFactory.FromPower<DeepFreezePower>()
    ];

    private int GetCardsPlayedBeforeThis()
    {
        return CombatManager.Instance.History.CardPlaysFinished
            .Count(entry => entry.HappenedThisTurn(base.CombatState) && entry.CardPlay.Card.Owner == base.Owner);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
		{
            await PowerCmd.Apply<FreezePower>(choiceContext, hittableEnemy,base.DynamicVars["Freeze"].IntValue,base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
    }
}