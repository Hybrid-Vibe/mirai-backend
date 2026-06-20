namespace Mirai.Infastructure.Clients;

internal static class ReplicateAspectRatioHelper
{
    private static readonly (string Ratio, double Value)[] SupportedRatios =
    [
        ("1:1", 1.0),
        ("16:9", 16.0 / 9.0),
        ("21:9", 21.0 / 9.0),
        ("2:3", 2.0 / 3.0),
        ("3:2", 3.0 / 2.0),
        ("4:5", 4.0 / 5.0),
        ("5:4", 5.0 / 4.0),
        ("3:4", 3.0 / 4.0),
        ("4:3", 4.0 / 3.0),
        ("9:16", 9.0 / 16.0),
        ("9:21", 9.0 / 21.0)
    ];

    public static string FromDimensions(int width, int height)
    {
        if (width <= 0 || height <= 0)
            return "16:9";

        var target = (double)width / height;

        return SupportedRatios
            .OrderBy(r => Math.Abs(r.Value - target))
            .First()
            .Ratio;
    }
}
