using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using RabbitAndSteel.Scripts.CardPools;
using RabbitAndSteel.Scripts.PotionPools;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Scaffolding.Godot;
using STS2RitsuLib.Scaffolding.Visuals;
using STS2RitsuLib.Scaffolding.Visuals.StateMachine;

namespace RabbitAndSteel.Scripts.Characters;

[RegisterCharacter]
public class AncientCharacter : ModCharacterTemplate<AncientCardPool, AncientRelicPool, AncientPotionPool>
{
	
	public override Color NameColor => new(0.5f, 0.5f, 1f);
	public override Color EnergyLabelOutlineColor => new(0.5f, 0.5f, 1f);
	public override Color MapDrawingColor => new(0.5f, 0.5f, 1f);
	public override CharacterGender Gender => CharacterGender.Feminine;
	public override int StartingHp => 78;
	public override int StartingGold => 15;

	protected override ModAnimStateMachine? SetupCustomCombatAnimationStateMachine(
	Node visualsRoot,
	CharacterModel character)
    {
        return ModAnimStateMachines.StandardCue(
            visualsRoot,
            character,
            "idle",
            "die",false,
            "hit",false,
            "attack",false,
            "cast",false,
            "relaxed",false,
            VisualCues);
    }
	public override CharacterAssetProfile AssetProfile => CharacterAssetProfiles.Merge(
		CharacterAssetProfiles.Ironclad(),
		new(
			Scenes: new(
				// 人物模型tscn路径。
				VisualsPath: $"res://RabbitAndSteel/images/ancient/arts/ancient.tscn",
				// 能量表盘tscn路径。
				EnergyCounterPath: "res://RabbitAndSteel/images/ancient/arts/ancient_energy_counter.tscn"
				// 商店人物场景。
				//MerchantAnimPath: "res://Test/scenes/test_character_merchant.tscn"
				// 篝火休息场景。
				// RestSiteAnimPath: "res://Test/scenes/test_character_rest_site.tscn"
			),
				VisualCues: ModVisualCues.CueSet()
			.Sequence("idle", seq => seq
				.Frame("res://RabbitAndSteel/images/ancient/arts/idle_loop1.png",0.4f)
				.Frame("res://RabbitAndSteel/images/ancient/arts/idle_loop2.png",0.4f).Loop()
				)
			.Sequence("hurt", seq => seq
				.Frame("res://RabbitAndSteel/images/ancient/arts/hurt1.png",0.3f)
				.Frame("res://RabbitAndSteel/images/ancient/arts/hurt2.png",0.3f)
				)
            .Sequence("attack", seq => seq
                .Frame("res://RabbitAndSteel/images/ancient/arts/attack1.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/arts/attack2.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/arts/attack3.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/arts/attack4.png", 0.2f)
                .Frame("res://RabbitAndSteel/images/ancient/arts/attack5.png", 0.2f)
				)
			.Single("die", "res://RabbitAndSteel/images/ancient/arts/die.png",0.5f)
			.Sequence("cast", seq => seq
                .Frame("res://RabbitAndSteel/images/ancient/arts/cast3.png", 0.3f)
                .Frame("res://RabbitAndSteel/images/ancient/arts/cast2.png", 0.3f)
                .Frame("res://RabbitAndSteel/images/ancient/arts/cast1.png", 0.3f)
                .Frame("res://RabbitAndSteel/images/ancient/arts/cast2.png", 0.3f)
                .Frame("res://RabbitAndSteel/images/ancient/arts/cast3.png", 0.3f)
				)
			.Sequence("relaxed", seq => seq
				.Frame("res://RabbitAndSteel/images/ancient/arts/idle_loop1.png",0.4f)
				.Frame("res://RabbitAndSteel/images/ancient/arts/idle_loop2.png",0.4f)
				)
            .Build(),
			Ui: new(
				// 人物头像路径。
				IconTexturePath: $"res://RabbitAndSteel/images/ancient/arts/Avatar.png",
				// 人物头像2号。
				// IconPath: "res://scenes/ui/character_icons/ironclad_icon.tscn",
				// 人物选择背景。
				CharacterSelectBgPath: $"res://RabbitAndSteel/images/ancient/arts/char_select_bg_random_character.tscn",
				// 人物选择图标。
				CharacterSelectIconPath: $"res://RabbitAndSteel/images/ancient/arts/char_select_ancient.png"
				// 人物选择图标-锁定状态。
				//CharacterSelectLockedIconPath: "res://Test/images/char_select_test_locked.png"
				// 人物选择过渡动画。
				// CharacterSelectTransitionPath: "res://materials/transitions/ironclad_transition_mat.tres",
				// 地图上的角色标记图标、表情轮盘上的角色头像
				// MapMarkerPath: null
			),
			Vfx: new(
				// 卡牌拖尾场景。
				// TrailPath: "res://scenes/vfx/card_trail_ironclad.tscn"
			),
			Audio: new(
				// 攻击音效
				// AttackSfx: null,
				// 施法音效
				// CastSfx: null,
				// 死亡音效
				// DeathSfx: null,
				// 角色选择音效
				// CharacterSelectSfx: null,
				// 过渡音效
				// CharacterTransitionSfx: "event:/sfx/ui/wipe_ironclad"
			),
			Multiplayer: new(
				// 多人模式-手指。
				// ArmPointingTexturePath: null,
				// 多人模式剪刀石头布-石头。
				// ArmRockTexturePath: null,
				// 多人模式剪刀石头布-布。
				// ArmPaperTexturePath: null,
				// 多人模式剪刀石头布-剪刀。
				// ArmScissorsTexturePath: null
			)));

	// 攻击和施法动画延迟，以对齐动画
	public override float AttackAnimDelay => 0f;
	public override float CastAnimDelay => 0f;

	// 如果你的人物不需要时间线小故事，加上这句。
	public override bool RequiresEpochAndTimeline => false;

	// 自动转换人物场景，让你不需要手动挂脚本。复制即可。
	protected override NCreatureVisuals? TryCreateCreatureVisuals() => RitsuGodotNodeFactories.CreateFromScenePath<NCreatureVisuals>(AssetProfile.Scenes!.VisualsPath!);

	// 攻击建筑师的攻击特效列表
	public override List<string> GetArchitectAttackVfx() => [
		// "vfx/vfx_attack_blunt",
		// "vfx/vfx_heavy_blunt",
		// "vfx/vfx_attack_slash",
		// "vfx/vfx_bloody_impact",
		// "vfx/vfx_rock_shatter"
	];
}
