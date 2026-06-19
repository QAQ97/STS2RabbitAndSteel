using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace RabbitAndSteel.Scripts;

[RegisterOwnedCardKeyword(nameof(pet), IconPath = "res://pet_icon.svg",CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
public class MyKeywords
{
    public static readonly CardKeyword pet = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(pet)).GetModCardKeyword();
}