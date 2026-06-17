using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using STS2RitsuLib.Interop.AutoRegistration;

namespace RabbitAndSteel.Scripts.Monsters.Pets
{
    [RegisterMonster]
    public class Dreadwyrm : ZangHuaMonster
    {
        public override int MinInitialHp => 1;
        public override int MaxInitialHp => 1;
        public override bool IsHealthBarVisible => false;

        protected override string GetCustomVisualPath() =>
            "res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/dreadwyrm.tscn";

        protected override MonsterMoveStateMachine GenerateMoveStateMachine()
        {
            var nothing = new MoveState("NOTHING", _ => Task.CompletedTask);
            nothing.FollowUpState = nothing;
            return new MonsterMoveStateMachine([nothing], nothing);
        }

        public override CreatureAnimator GenerateAnimator(MegaSprite controller)
        {
            var idle = new AnimState("idle_loop", true);
            var attack = new AnimState("attack");
            var hurt = new AnimState("hurt");
            var die = new AnimState("die");
            var cast = new AnimState("cast");
            var deadLoop = new AnimState("dead_loop", true);

            attack.NextState = idle;
            hurt.NextState = idle;
            die.NextState = deadLoop;

            var animator = new CreatureAnimator(idle, controller);
            animator.AddAnyState("Attack", attack);
            animator.AddAnyState("Hit", hurt);
            animator.AddAnyState("Dead", die);
            animator.AddAnyState("Cast", cast);
            return animator;
        }
    }
}