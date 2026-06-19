using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.Models;
public abstract class ZangHuaMonster : ModMonsterTemplate
{
    protected abstract string GetCustomVisualPath();

    public virtual NCreatureVisuals? CreateCustomVisuals()
    {
        var path = GetCustomVisualPath();
        if (string.IsNullOrEmpty(path) || !ResourceLoader.Exists(path))
            return null;

        var scene = ResourceLoader.Load<PackedScene>(path);
        if (scene == null) return null;

        var source = scene.Instantiate();
        if (source == null) return null;

        if (source is NCreatureVisuals visuals)
            return visuals;

        return MigrateToNCreatureVisuals(source);
    }

    private NCreatureVisuals MigrateToNCreatureVisuals(Node source)
    {
        var visuals = new NCreatureVisuals();
        visuals.Name = source.Name;
        var children = source.GetChildren().ToList();
        foreach (var child in children)
        {
            source.RemoveChild(child);
            visuals.AddChild(child);
            child.Owner = visuals;
        }
        source.QueueFree();
        return visuals;
    }
    public virtual Task<bool> OnPetKeywordTriggered(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return Task.FromResult(false);
    }

    public virtual CreatureAnimator? SetupCustomAnimationStates(MegaSprite controller)
    {
        return null;
    }

    protected static CreatureAnimator SetupAnimationState(MegaSprite controller, string idleName,
    string? deadName = null, bool deadLoop = false,
    string? hitName = null, bool hitLoop = false,
    string? attackName = null, bool attackLoop = false,
    string? castName = null, bool castLoop = false)
    {
        var idleAnim = new AnimState(idleName, true);
        var deadAnim = deadName == null ? idleAnim : new AnimState(deadName, deadLoop);
        var hitAnim = hitName == null ? idleAnim :
            new AnimState(hitName, hitLoop) { NextState = idleAnim };
        var attackAnim = attackName == null ? idleAnim :
            new AnimState(attackName, attackLoop) { NextState = idleAnim };
        var castAnim = castName == null ? idleAnim :
            new AnimState(castName, castLoop) { NextState = idleAnim };

        var animator = new CreatureAnimator(idleAnim, controller);
        animator.AddAnyState("Idle", idleAnim);
        animator.AddAnyState("Dead", deadAnim);
        animator.AddAnyState("Hit", hitAnim);
        animator.AddAnyState("Attack", attackAnim);
        animator.AddAnyState("Cast", castAnim);

        return animator;
    }
    public virtual Type DefaultPowerType => typeof(DieForYouPower);
}