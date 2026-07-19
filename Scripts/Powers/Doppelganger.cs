using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using RabbitAndSteel.Scripts.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Powers;

[RegisterPower]
public sealed class Doppelganger : ModPowerTemplate, IPetKeywordHandler
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}1.png",
        BigIconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}0.png"
    );

    public async Task<bool> OnPetKeywordTriggered(PlayerChoiceContext choiceContext, CardPlay cardPlay, Creature pet)
    {
        if (cardPlay.Card.Type != CardType.Attack)
            return false;
        var player = cardPlay.Card.Owner;
        if (player != pet.PetOwner)
            return false;

        if (pet.Monster is not ZangHuaMonster)
            return false;

        if (player.Creature == null)
            return false;

        var enemies = player.Creature.CombatState.HittableEnemies;
        if (enemies == null || !enemies.Any())
            return false;
        var target = player.RunState.Rng.CombatTargets.NextItem(enemies);
        if (target == null)
            return false;

        decimal baseDamage;
        var dynVars = cardPlay.Card.DynamicVars;
        if (dynVars.ContainsKey("Damage"))
            baseDamage = dynVars.Damage.BaseValue;
        else if (dynVars.ContainsKey("CalculatedDamage"))
            baseDamage = dynVars.CalculatedDamage.Calculate(target);
        else
            baseDamage = 1m;

        if (baseDamage <= 0)
            baseDamage = 1m;

        decimal petStrengthBonus = pet.PetOwner?.Creature?.Powers.OfType<StrengthSpellPower>().Sum(p => p.Amount) ?? 0m;
        decimal attackDamage = baseDamage + petStrengthBonus;

        await CreatureCmd.TriggerAnim(pet, "Cast", 0.25f);
        VfxCmd.PlayOnCreature(target, "vfx/vfx_attack_slash");
        await CreatureCmd.Damage(choiceContext, target,
            new DamageVar(attackDamage, ValueProp.Move),
            pet,
            cardPlay.Card,
            cardPlay);

        return true;
    }
}