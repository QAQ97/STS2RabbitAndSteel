using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Powers;

[RegisterPower]
public sealed class GarnetFlare : ModPowerTemplate, IPetKeywordHandler, IPetDebuffHandler
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
        var player = cardPlay.Card.Owner;
        if (player != pet.PetOwner) return false;
        return await ExecuteAction(choiceContext, pet, cardPlay);
    }
    public async Task<bool> OnPetDebuffTriggered(PlayerChoiceContext choiceContext, PowerModel debuffPower, Creature debuffTarget, Creature pet)
    {
        var player = pet.PetOwner;
        if (player == null || player.Creature == null) return false;
        return await ExecuteAction(choiceContext, pet, null);
    }

    private async Task<bool> ExecuteAction(PlayerChoiceContext choiceContext, Creature pet, CardPlay? cardPlay)
    {
        var player = pet.PetOwner;
        if (player == null) return false;
        var playerCreature = player.Creature;
        if (playerCreature == null) return false;

        var hand = player.PlayerCombatState.Hand;
        var statusCards = hand.Cards.Where(c => c.Type == CardType.Status).ToList();
        foreach (var statusCard in statusCards)
        {
            await CardCmd.Exhaust(choiceContext, statusCard);
        }

        decimal baseDamage = 2;
        decimal strengthBonus = playerCreature.Powers.OfType<StrengthSpellPower>().Sum(p => p.Amount);
        decimal totalDamage = baseDamage + strengthBonus;


        var enemies = pet.CombatState.Enemies.Where(e => e.IsAlive).ToList();
        if (!enemies.Any())
            return false;

        await CreatureCmd.TriggerAnim(pet, "Cast", 0.25f);
        for (int i = 0; i < 3; i++)
        {
            await CreatureCmd.Damage(
                choiceContext,
                enemies,
                totalDamage,
                ValueProp.Unpowered,
                pet,
                cardPlay?.Card,
                cardPlay
            );
        }

        return true;
    }
}