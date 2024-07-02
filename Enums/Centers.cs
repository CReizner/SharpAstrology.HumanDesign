namespace SharpAstrology.Enums;

public enum Centers
{
    Root,
    Sacral,
    Emotions,
    Spleen,
    Heart,
    Self,
    Throat,
    Mind,
    Crown
}

public static class CentersExtensionMethods
{
    public static Gates[] ToGates(this Centers center)
    {
        return center switch
        {
            Centers.Root => [Gates.Key58, Gates.Key38, Gates.Key54, Gates.Key53, Gates.Key60, Gates.Key52, Gates.Key19, Gates.Key39, Gates.Key41],
            Centers.Sacral => [Gates.Key27, Gates.Key34, Gates.Key5, Gates.Key14, Gates.Key29, Gates.Key59, Gates.Key9, Gates.Key3, Gates.Key42],
            Centers.Spleen => [Gates.Key18, Gates.Key28, Gates.Key32, Gates.Key50, Gates.Key44, Gates.Key57, Gates.Key48],
            Centers.Emotions => [Gates.Key6, Gates.Key37, Gates.Key22, Gates.Key36, Gates.Key30, Gates.Key55, Gates.Key49],
            Centers.Heart => [Gates.Key22, Gates.Key40, Gates.Key26, Gates.Key51],
            Centers.Self => [Gates.Key1, Gates.Key13, Gates.Key25, Gates.Key46, Gates.Key2, Gates.Key15, Gates.Key10, Gates.Key7],
            Centers.Throat => [Gates.Key20, Gates.Key16, Gates.Key62, Gates.Key23, Gates.Key56, Gates.Key35, Gates.Key12, Gates.Key45, Gates.Key33, Gates.Key8, Gates.Key31],
            Centers.Mind => [Gates.Key43, Gates.Key17, Gates.Key47, Gates.Key24, Gates.Key4, Gates.Key11],
            Centers.Crown => [Gates.Key64, Gates.Key61, Gates.Key63]
        };
    }

    public static Channels[] ConnectedChannels(this Centers center)
    {
        return center switch
        {
            Centers.Root =>
            [
                Channels.Key18Key58, Channels.Key28Key38, Channels.Key32Key54, Channels.Key42Key53, Channels.Key3Key60,
                Channels.Key9Key52, Channels.Key19Key49, Channels.Key39Key55, Channels.Key30Key41
            ],
            Centers.Sacral =>
            [
                Channels.Key5Key15, Channels.Key2Key14, Channels.Key29Key46, Channels.Key6Key59,
                Channels.Key9Key52, Channels.Key3Key60, Channels.Key42Key53, Channels.Key27Key50,
                Channels.Key34Key57
            ],
            Centers.Spleen =>
            [
                Channels.Key18Key58, Channels.Key28Key38, Channels.Key32Key54, Channels.Key27Key50, Channels.Key27Key50,
                Channels.Key26Key44, Channels.Key10Key57, Channels.Key20Key57, Channels.Key34Key57, Channels.Key16Key48
            ],
            Centers.Emotions =>
            [
                Channels.Key19Key49, Channels.Key39Key55, Channels.Key30Key41, Channels.Key6Key59, Channels.Key37Key40,
                Channels.Key12Key22, Channels.Key35Key36
            ],
            Centers.Heart =>
            [
                Channels.Key21Key45, Channels.Key25Key51, Channels.Key26Key44, Channels.Key37Key40
            ],
            Centers.Self =>
            [
                Channels.Key1Key8, Channels.Key13Key33, Channels.Key25Key51, Channels.Key29Key46, Channels.Key2Key14,
                Channels.Key5Key15, Channels.Key10Key20, Channels.Key7Key31
            ],
            Centers.Throat =>
            [
                Channels.Key13Key33, Channels.Key1Key8, Channels.Key7Key31, Channels.Key10Key20, Channels.Key20Key34,
                Channels.Key20Key57, Channels.Key16Key48, Channels.Key17Key62, Channels.Key23Key43, Channels.Key11Key56,
                Channels.Key35Key36, Channels.Key12Key22, Channels.Key21Key45
            ],
            Centers.Mind =>
            [
                Channels.Key23Key43, Channels.Key17Key62, Channels.Key47Key64, Channels.Key24Key61, Channels.Key4Key63,
                Channels.Key11Key56
            ],
            Centers.Crown =>
            [
                Channels.Key47Key64, Channels.Key24Key61, Channels.Key4Key63
            ]
        };
    }
}