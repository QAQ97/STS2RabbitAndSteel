using Godot;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using RabbitAndSteel.Scripts.Models;
using RabbitAndSteel.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Scaffolding.Visuals.StateMachine;

namespace RabbitAndSteel.Scripts.Monsters.Pets
{
    [RegisterMonster]
    public class Stormbeast : ZangHuaMonster,IModCreatureVisualsFactory, IModCreatureCombatAnimationStateMachineFactory
    {
        public override int MinInitialHp => 1;
        public override int MaxInitialHp => 1;
        public override bool IsHealthBarVisible => false;

        protected override string GetCustomVisualPath() =>
            "res://RabbitAndSteel/images/ancient/Pets/Stormbeast/stormbeast.tscn";

        protected override MonsterMoveStateMachine GenerateMoveStateMachine()
        {
            var nothing = new MoveState("NOTHING", _ => Task.CompletedTask);
            nothing.FollowUpState = nothing;
            return new MonsterMoveStateMachine([nothing], nothing);
        }

    public ModAnimStateMachine? TryCreateCombatAnimationStateMachine(Node visualsRoot)
        {
            var builder = ModAnimStateMachineBuilder.Create();
            builder.AddState("idle", loop: true).AsInitial();
            builder.AddState("attack", loop: false).WithNext("idle");
            builder.AddState("hurt", loop: false).WithNext("idle");
            builder.AddState("cast", loop: false).WithNext("idle");
            builder.AddState("die", loop: false);
            builder
                .AddBranch("idle", "Attack", "attack")
                .AddBranch("idle", "Hit", "hurt")
                .AddBranch("idle", "Cast", "cast")
                .AddAnyState("Dead", "die")
                .AddAnyState("Idle", "idle");

            return builder.BuildForVisualsRoot(visualsRoot);
        }

        public override Type DefaultPowerType => typeof(Doppelganger);
    }
}