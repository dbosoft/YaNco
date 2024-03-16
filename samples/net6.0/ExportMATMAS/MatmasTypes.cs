using LanguageExt;
// ReSharper disable StringLiteralTypo

namespace ExportMATMAS;

public static class MatmasTypes
{
    public static readonly HashMap<string, string> Segment2Type = new(new[]
    {
        ("E2MARAM009", "E1MARAM"), // client data, MATMAS05
        ("E2MARCM008", "E1MARCM"), // plant data, MATMAS05
        ("E2MAKTM001", "E1MAKTM") // descriptions, MATMAS05
    });

    public static readonly HashMap<string, string> Type2Segment = new(new[]
    {
        ("E1MARAM", "E2MARAM009" ),
        ("E1MARCM", "E2MARCM008"),
        ("E1MAKTM", "E2MAKTM001")
    });
}