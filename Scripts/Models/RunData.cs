namespace RabbitAndSteel.Scripts.Models;

// 全局共享
public sealed class ChallengeRunState
{
}

// 属于单个玩家数据
public sealed class PlayerRunState
{
    public int CurrentStacks { get; set; } = 0;
}