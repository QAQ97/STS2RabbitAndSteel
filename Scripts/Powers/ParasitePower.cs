using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Powers;

[RegisterPower]
public class ParasitePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}1.png",
        BigIconPath: $"res://RabbitAndSteel/images/icons/{GetType().Name}0.png"
    );

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        // 仅当宿主死亡且未被阻止时触发
        if (wasRemovalPrevented || creature != base.Owner)
            return;
            
        Creature? applier = base.Applier;
        if (applier == null || !applier.IsAlive)
            return;

        int healAmount = base.Amount;
        if (healAmount <= 0)
            return;

        Flash();
        SfxCmd.Play("heal.mp3");
        VfxCmd.PlayOnCreature(applier, "vfx/heal");

        await CreatureCmd.Heal(applier, healAmount);
    }
}