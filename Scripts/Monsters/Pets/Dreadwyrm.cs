using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using RabbitAndSteel.Scripts.Models;
using RabbitAndSteel.Scripts.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Visuals;
using STS2RitsuLib.Scaffolding.Visuals.Definition;
using STS2RitsuLib.Scaffolding.Visuals.StateMachine;

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

    	protected override ModAnimStateMachine? SetupCustomCombatAnimationStateMachine(
	Node visualsRoot,
	MonsterModel monster)
    {
        return ModAnimStateMachines.StandardCue(
            visualsRoot,
            null,
            "idle",
            "die",false,
            "hit",false,
            "attack",false,
            "cast",false,
            "relaxed",false,
            MonsterVisualCues);
    }
    private static readonly VisualCueSet MonsterVisualCues = ModVisualCues.CueSet()
            .Sequence("idle", seq => seq
				.Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/idle_loop1.png",0.4f)
				.Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/idle_loop2.png",0.4f).Loop()
				)
            .Single("hurt", "res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/hurt1.png",0.5f)
            .Sequence("attack", seq => seq
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/attack1.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/attack2.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/attack3.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/attack4.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/attack3.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/attack2.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm//attack1.png", 0.2f)
				)
			.Single("die", "res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/die.png",0.5f)
			.Sequence("cast", seq => seq
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/cast1.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/cast2.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/cast3.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/cast4.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/cast3.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/cast4.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/cast3.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/cast4.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/cast2.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/cast1.png", 0.2f)
				)
            .Sequence("relaxed", seq => seq
				.Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/idle_loop1.png",0.4f)
				.Frame("res://RabbitAndSteel/images/ancient/Pets/Dreadwyrm/idle_loop2.png",0.4f)
				)
            .Build();
        
        public override Type DefaultPowerType => typeof(GarnetFlare);
    }
}