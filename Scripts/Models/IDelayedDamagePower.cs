using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using RabbitAndSteel.Scripts.Powers;

namespace RabbitAndSteel.Scripts.Models;

public static class CreatureDelayedDamageExtensions
{
    public static int GetTotalDelayedDamageStacks(this Creature creature)
    {
        if (creature == null) return 0;
        
        return creature.GetPowerAmount<PoisonPower>() +
            creature.GetPowerAmount<BurnPower>() +
            creature.GetPowerAmount<CursePower>() +
            creature.GetPowerAmount<DoomPower>();
    }
}