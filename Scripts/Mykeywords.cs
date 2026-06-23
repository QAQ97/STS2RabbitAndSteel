using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace RabbitAndSteel.Scripts;


[RegisterOwnedCardKeyword(nameof(pet), CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(sustain))]
public class MyKeywords
{
    public static readonly CardKeyword pet = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(pet)).GetModCardKeyword();
    public static readonly CardKeyword sustain = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(sustain)).GetModCardKeyword();
}