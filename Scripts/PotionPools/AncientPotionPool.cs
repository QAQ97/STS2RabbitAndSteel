using STS2RitsuLib.Scaffolding.Content;

namespace RabbitAndSteel.Scripts.PotionPools;

public class AncientPotionPool : TypeListPotionPoolModel
{
    // 描述中使用的能量图标。大小为24x24。
    public override string? TextEnergyIconPath => " ";
    // tooltip和卡牌左上角的能量图标。大小为74x74。
    public override string? BigEnergyIconPath => " ";

    public override string EnergyColorName => "ancient";
}