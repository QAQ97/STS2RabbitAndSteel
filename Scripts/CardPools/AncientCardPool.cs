using Godot;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace RabbitAndSteel.Scripts.CardPools;

public class AncientCardPool : TypeListCardPoolModel
{
    public override string Title => "ancient";
    public override string EnergyColorName => "ancient";
    public override string? TextEnergyIconPath => $"res://RabbitAndSteel/images/ancient/arts/energy_small.png";
    public override string? BigEnergyIconPath => $"res://RabbitAndSteel/images/ancient/arts/energy_big.png";

    public override Color DeckEntryCardColor => new(0.5f, 0.5f, 1f);
    public override Color EnergyOutlineColor => new(0.5f, 0.5f, 1f);
    private static readonly Material? _poolFrameMaterial = MaterialUtils.CreateRgbShaderMaterial(0.5f, 0.5f, 1f);
    public override Material? PoolFrameMaterial => _poolFrameMaterial;
    public override bool IsColorless => false;
}