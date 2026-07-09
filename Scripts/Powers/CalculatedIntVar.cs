using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace RabbitAndSteel.Scripts.Powers;

/// <summary>
/// 一个可计算的 DynamicVar，用于在 power 描述中动态显示计算后的整数值。
/// 构造时立即计算，SetOwner 时重新计算，GetBaseValueForIConvertible 时刷新。
/// 用于 {Name} 格式（非 diff()），SmartFormat 通过 IConvertible 接口读取值。
/// </summary>
public class CalculatedIntVar : DynamicVar
{
    private readonly Func<int> _calculator;

    public CalculatedIntVar(Func<int> calculator, string name) : base(name, calculator())
    {
        _calculator = calculator;
    }

    public override void SetOwner(AbstractModel owner)
    {
        base.SetOwner(owner);
        Refresh();
    }

    public void Refresh()
    {
        int value = _calculator();
        BaseValue = value;
    }

    protected override decimal GetBaseValueForIConvertible()
    {
        Refresh();
        return BaseValue;
    }
}
