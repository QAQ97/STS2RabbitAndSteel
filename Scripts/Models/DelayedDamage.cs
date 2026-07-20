using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Random;
using RabbitAndSteel.Scripts.Powers;

namespace RabbitAndSteel.Scripts.Models;

public static class DelayedDamage
{
    public static int GetTotalDelayedDamageStacks(this Creature creature)
    {
        if (creature == null) return 0;
        
        return creature.GetPowerAmount<PoisonPower>() +
            creature.GetPowerAmount<BurnPower>() +
            creature.GetPowerAmount<CursePower>() +
            creature.GetPowerAmount<FreezePower>() +
            creature.GetPowerAmount<DeepFreezePower>();
    }
    public static async Task ApplyRandomDelayedDamageBuff(
            this Rng rng,
            PlayerChoiceContext choiceContext,
            Creature target,
            int amount,
            Creature applier,
            CardModel cardSource)
    {
        Type[] buffTypes = new Type[]
        {
            typeof(PoisonPower),
            typeof(BurnPower),
            typeof(CursePower),
            typeof(FreezePower),
            typeof(DeepFreezePower)
        };

        int index = rng.NextInt(buffTypes.Length);
        Type selectedType = buffTypes[index];

        // 通过反射调用 PowerCmd.Apply<T>
        var applyMethod = typeof(PowerCmd).GetMethod("Apply", new Type[] {
            typeof(PlayerChoiceContext),
            typeof(Creature),
            typeof(decimal),
            typeof(Creature),
            typeof(CardModel),
            typeof(bool)
        }) ?? throw new InvalidOperationException("PowerCmd.Apply method not found");

        var genericApply = applyMethod.MakeGenericMethod(selectedType);
        var task = (Task)genericApply.Invoke(null, new object[] {
            choiceContext,
            target,
            (decimal)amount,
            applier,
            cardSource,
            false
        });
        await task;
    }
}