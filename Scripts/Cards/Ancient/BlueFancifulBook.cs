    // using MegaCrit.Sts2.Core.Entities.Cards;
    // using MegaCrit.Sts2.Core.Entities.Players;
    // using MegaCrit.Sts2.Core.Localization.DynamicVars;
    // using RabbitAndSteel.Scripts.CardPools;
    // using STS2RitsuLib.Combat.HandSize;
    // using STS2RitsuLib.Interop.AutoRegistration;
    // using STS2RitsuLib.Scaffolding.Content;

    // namespace RabbitAndSteel.Scripts.Cards.Ancient;

    // [RegisterCard(typeof(AncientCardPool))]
    // public class BlueFancifulBook : ModCardTemplate, IMaxHandSizeModifier
    // {
    //     private const int energyCost = 1;
    //     private const CardType type = CardType.Skill;
    //     private const CardRarity rarity = CardRarity.Common;
    //     private const TargetType targetType = TargetType.Self;
    //     private const bool shouldShowInCardLibrary = true;
    //     public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    //     protected override IEnumerable<DynamicVar> CanonicalVars => [
    //         new EnergyVar(1)
    //     ];

    //     public override CardAssetProfile AssetProfile => new(
    //         PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    //     );
    //     public RedFancifulBook() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    //     public override decimal ModifyMaxEnergy(Player player, decimal amount)
    //     {
    //         if (player != base.Owner)
    //         {
    //             return amount;
    //         }
    //         return amount + 1m;
    //     }

    //     public int ModifyMaxHandSize(Player player, int currentMaxHandSize)
    //     {
    //         if (player == Owner && PileType.Hand.GetPile(Owner).Cards.Contains(this))
    //             if (IsUpgraded)
    //                 return currentMaxHandSize - 1;
    //             else
    //                 return currentMaxHandSize - 3;
    //         return currentMaxHandSize;
    //     }


    // }