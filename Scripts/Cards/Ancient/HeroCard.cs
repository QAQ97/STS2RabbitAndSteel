using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using RabbitAndSteel.Scripts.CardPools;
using RabbitAndSteel.Scripts.Models;
using RabbitAndSteel.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Cards.Ancient;
[RegisterCard(typeof(AncientCardPool))]
public class HeroCard : PetCardBase
{
    private const int energyCost = 0;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Ancient;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
    PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );

    public HeroCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override string PetTypeName => "Hero";

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<DodgeRollPower>()
        ];
}