using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using RabbitAndSteel.Scripts.CardPools;
using RabbitAndSteel.Scripts.Models;
using STS2RitsuLib;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Cards.Ancient;

[RegisterCard(typeof(AncientCardPool))]
public class ReminiscenceGrenadier : ModCardTemplate
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
    PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    ,VisualStyle : CardVisualStyle.Ancient
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] {
        new DamageVar(1, ValueProp.Move)
    };

    public ReminiscenceGrenadier() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int hitCount = IsUpgraded ? 6 : 5;
        var enemies = base.CombatState.Enemies.Where(e => e.IsAlive).ToList();
        if (!enemies.Any())
            return;

        // 使用 RitsuLib 托管 RNG 流，避免多人游戏 Niche RNG 状态不同步
        var runState = Owner.RunState as RunState;
        if (runState == null)
            return;
        var rng = RitsuLibFramework.GetModRunRng(runState, Entry.ModId, "reminiscence_grenadier");

        foreach (var enemy in enemies)
        {
            for (int i = 0; i < hitCount; i++)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this, cardPlay)
                    .Targeting(enemy)
                    .Execute(choiceContext);
                await rng.ApplyRandomDelayedDamageBuff(choiceContext, enemy, 2, Owner.Creature, this);
            }
        }
    }
}