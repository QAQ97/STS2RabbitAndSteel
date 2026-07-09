using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using RabbitAndSteel.Scripts.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Powers;

[RegisterPower]
public sealed class GarnetFlare : ModPowerTemplate, IPetKeywordHandler
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(4, ValueProp.Move)
    ];

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}1.png",
        BigIconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}0.png"
    );

    public async Task<bool> OnPetKeywordTriggered(PlayerChoiceContext choiceContext, CardPlay cardPlay, Creature pet)
{
    var playerCreature = pet.PetOwner?.Creature;
    if (playerCreature == null) return false;
    var player = cardPlay.Card.Owner;
    if (player != pet.PetOwner) return false;
    if (pet.Monster is not ZangHuaMonster zangPet) return false;

    var hand = player.PlayerCombatState.Hand;
    var statusCards = hand.Cards.Where(c => c.Type == CardType.Status).ToList();
    foreach (var statusCard in statusCards)
    {
        await CardCmd.Exhaust(choiceContext, statusCard);
    }

    decimal baseDamage = 2;
    decimal petStrengthBonus = pet.PetOwner?.Creature?.Powers.OfType<StrengthSpellPower>().Sum(p => p.Amount) ?? 0m;
    decimal attackDamage = baseDamage + petStrengthBonus;

    await CreatureCmd.TriggerAnim(pet, "Cast", 0.25f);
    await DamageCmd.Attack(attackDamage)
        .WithHitCount(3)
        .FromCard(cardPlay.Card, cardPlay)
        .TargetingAllOpponents(pet.CombatState)
        .Unpowered()
        .Execute(choiceContext);

    return true;
}
}