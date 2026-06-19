using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using RabbitAndSteel.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Cards.Ancient;
[RegisterCard(typeof(TokenCardPool))]
public class Stalemate : ModCardTemplate
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Token;
    private const TargetType targetType = TargetType.Self;
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
       	CardKeyword.Ethereal,
		CardKeyword.Exhaust
    ];
    private const bool shouldShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://RabbitAndSteel/images/ancient/cards/{GetType().Name}.png"
    );

    public Stalemate() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ICombatState? combatState = Owner.Creature.CombatState;
        if (combatState == null) return;

        foreach (Creature playerCreature in combatState.PlayerCreatures)
        {
            Player player = playerCreature.Player;
            if (player == null) continue;

            decimal preservedEnergy = player.PlayerCombatState?.Energy ?? 0m;
            await PowerCmd.Apply<RetainHandPower>(choiceContext, playerCreature, 1m, Owner.Creature, this);
            await PowerCmd.Apply<BlurPower>(choiceContext, playerCreature, 1m, Owner.Creature, this);
            await PowerCmd.Apply<StalematePower>(choiceContext, playerCreature, preservedEnergy + 1m, Owner.Creature, this);
        }
        foreach (Creature enemy in combatState.HittableEnemies)
        {
            await CreatureCmd.Stun(enemy);
        }

        foreach (Creature playerCreature in combatState.PlayerCreatures)
        {
            Player player = playerCreature.Player;
            if (player != null)
            {
                PlayerCmd.EndTurn(player, canBackOut: false);
            }
        }
    }
        protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}