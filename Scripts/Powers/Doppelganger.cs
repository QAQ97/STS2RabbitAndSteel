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
public sealed class Doppelganger : ModPowerTemplate, IPetKeywordHandler
{
    private static readonly Random _random = new();
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


        var enemies = player.Creature.CombatState.HittableEnemies;
        if (enemies == null || !enemies.Any())
            return false;
        var target = player.RunState.Rng.CombatTargets.NextItem(enemies);
        if (target == null)
            return false;
        decimal baseDamage = cardPlay.Card.DynamicVars.Damage.EnchantedValue;
        if (baseDamage <= 0)
            baseDamage = 1m;
        if (pet.Monster is not ZangHuaMonster zangPet)
            return false;


        await CreatureCmd.TriggerAnim(pet, "Cast", 0.25f);
        VfxCmd.PlayOnCreature(target, "vfx/vfx_attack_slash");
        await CreatureCmd.Damage(choiceContext, target,
            new DamageVar(baseDamage, ValueProp.Move),
            pet,
            cardPlay.Card);

        return true;
    }
}