using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Models;
public abstract class PetCardBase : ModCardTemplate
{
    public abstract string PetTypeName { get; }

    protected PetCardBase(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
}