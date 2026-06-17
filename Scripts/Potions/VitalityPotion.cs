using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Godot;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace RabbitAndSteel.Scripts.Potions;

[RegisterPotion(typeof(SharedPotionPool))]
public class VitalityPotion : ModPotionTemplate
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyPlayer;

    public override PotionAssetProfile AssetProfile => new(
        ImagePath: "res://RabbitAndSteel/images/potions/VitalityPotion.png",
        OutlinePath: "res://RabbitAndSteel/images/potions/VitalityPotion.png"
    );

    // 缓存反射方法，提升性能
    private static MethodInfo? _setMaxHpInternal;
    private static MethodInfo? _setCurrentHpInternal;

    static VitalityPotion()
    {
        var creatureType = typeof(Creature);
        _setMaxHpInternal = creatureType.GetMethod("SetMaxHpInternal", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        _setCurrentHpInternal = creatureType.GetMethod("SetCurrentHpInternal", 
            BindingFlags.NonPublic | BindingFlags.Instance);
    }

protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
{
    PotionModel.AssertValidForTargetedPotion(target);
    NCombatRoom.Instance?.PlaySplashVfx(target, Colors.Green);

    // 记录原始最大生命值
    int originalMaxHp = target.MaxHp;
    int hpIncrease = 10;

    await CreatureCmd.GainMaxHp(target, hpIncrease);

    // 战斗结束时还原
    void OnCombatEnded(CombatRoom room)
    {
        if (target == null) return;
        
        // 确保只减少我们增加的部分
        int currentMaxHp = target.MaxHp;
        int newMaxHp = currentMaxHp - hpIncrease;
        
        // 保底逻辑：确保不会低于原始值
        if (newMaxHp < originalMaxHp)
        {
            newMaxHp = originalMaxHp;
        }

        // 设置新最大生命值
        _setMaxHpInternal?.Invoke(target, new object[] { (decimal)newMaxHp });
        
        // 如果当前生命值超出新上限，调整之
        if (target.CurrentHp > newMaxHp)
        {
            _setCurrentHpInternal?.Invoke(target, new object[] { (decimal)newMaxHp });
        }
        
        CombatManager.Instance.CombatEnded -= OnCombatEnded;
    }

    CombatManager.Instance.CombatEnded += OnCombatEnded;
}
}

