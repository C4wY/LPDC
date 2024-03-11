using System.Linq;
using UnityEngine;

public static class Colors
{
    public static Color32 Hex(string hex, byte alpha = 255)
    {
        if (hex[0] == '#')
            hex = hex[1..];

        if (hex.Length == 3 || hex.Length == 4)
            hex = string.Concat(hex.Select(c => $"{c}{c}"));

        var r = byte.Parse(hex[0..2], System.Globalization.NumberStyles.HexNumber);
        var g = byte.Parse(hex[2..4], System.Globalization.NumberStyles.HexNumber);
        var b = byte.Parse(hex[4..6], System.Globalization.NumberStyles.HexNumber);

        if (hex.Length == 8)
            alpha = byte.Parse(hex[6..8], System.Globalization.NumberStyles.HexNumber);

        return new(r, g, b, alpha);
    }

    public static Color32 Hex(string hex, float alpha) =>
        Hex(hex, (byte)(alpha * 255));

    public static string ToHex(Color32 color) => $"{color.r:X2}{color.g:X2}{color.b:X2}";

    public static Color aliceblue = Hex("#f0f8ff");
    public static Color antiquewhite = Hex("#faebd7");
    public static Color aqua = Hex("#00ffff");
    public static Color aquamarine = Hex("#7fffd4");
    public static Color azure = Hex("#f0ffff");
    public static Color beige = Hex("#f5f5dc");
    public static Color bisque = Hex("#ffe4c4");
    public static Color black = Hex("#000000");
    public static Color blanchedalmond = Hex("#ffebcd");
    public static Color blue = Hex("#0000ff");
    public static Color blueviolet = Hex("#8a2be2");
    public static Color brown = Hex("#a52a2a");
    public static Color burlywood = Hex("#deb887");
    public static Color cadetblue = Hex("#5f9ea0");
    public static Color chartreuse = Hex("#7fff00");
    public static Color chocolate = Hex("#d2691e");
    public static Color coral = Hex("#ff7f50");
    public static Color cornflowerblue = Hex("#6495ed");
    public static Color cornsilk = Hex("#fff8dc");
    public static Color crimson = Hex("#dc143c");
    public static Color cyan = Hex("#00ffff");
    public static Color darkblue = Hex("#00008b");
    public static Color darkcyan = Hex("#008b8b");
    public static Color darkgoldenrod = Hex("#b8860b");
    public static Color darkgray = Hex("#a9a9a9");
    public static Color darkgreen = Hex("#006400");
    public static Color darkgrey = Hex("#a9a9a9");
    public static Color darkkhaki = Hex("#bdb76b");
    public static Color darkmagenta = Hex("#8b008b");
    public static Color darkolivegreen = Hex("#556b2f");
    public static Color darkorange = Hex("#ff8c00");
    public static Color darkorchid = Hex("#9932cc");
    public static Color darkred = Hex("#8b0000");
    public static Color darksalmon = Hex("#e9967a");
    public static Color darkseagreen = Hex("#8fbc8f");
    public static Color darkslateblue = Hex("#483d8b");
    public static Color darkslategray = Hex("#2f4f4f");
    public static Color darkslategrey = Hex("#2f4f4f");
    public static Color darkturquoise = Hex("#00ced1");
    public static Color darkviolet = Hex("#9400d3");
    public static Color deeppink = Hex("#ff1493");
    public static Color deepskyblue = Hex("#00bfff");
    public static Color dimgray = Hex("#696969");
    public static Color dimgrey = Hex("#696969");
    public static Color dodgerblue = Hex("#1e90ff");
    public static Color firebrick = Hex("#b22222");
    public static Color floralwhite = Hex("#fffaf0");
    public static Color forestgreen = Hex("#228b22");
    public static Color fuchsia = Hex("#ff00ff");
    public static Color gainsboro = Hex("#dcdcdc");
    public static Color ghostwhite = Hex("#f8f8ff");
    public static Color goldenrod = Hex("#daa520");
    public static Color gold = Hex("#ffd700");
    public static Color gray = Hex("#808080");
    public static Color green = Hex("#008000");
    public static Color greenyellow = Hex("#adff2f");
    public static Color grey = Hex("#808080");
    public static Color honeydew = Hex("#f0fff0");
    public static Color hotpink = Hex("#ff69b4");
    public static Color indianred = Hex("#cd5c5c");
    public static Color indigo = Hex("#4b0082");
    public static Color ivory = Hex("#fffff0");
    public static Color khaki = Hex("#f0e68c");
    public static Color lavenderblush = Hex("#fff0f5");
    public static Color lavender = Hex("#e6e6fa");
    public static Color lawngreen = Hex("#7cfc00");
    public static Color lemonchiffon = Hex("#fffacd");
    public static Color lightblue = Hex("#add8e6");
    public static Color lightcoral = Hex("#f08080");
    public static Color lightcyan = Hex("#e0ffff");
    public static Color lightgoldenrodyellow = Hex("#fafad2");
    public static Color lightgray = Hex("#d3d3d3");
    public static Color lightgreen = Hex("#90ee90");
    public static Color lightgrey = Hex("#d3d3d3");
    public static Color lightpink = Hex("#ffb6c1");
    public static Color lightsalmon = Hex("#ffa07a");
    public static Color lightseagreen = Hex("#20b2aa");
    public static Color lightskyblue = Hex("#87cefa");
    public static Color lightslategray = Hex("#778899");
    public static Color lightslategrey = Hex("#778899");
    public static Color lightsteelblue = Hex("#b0c4de");
    public static Color lightyellow = Hex("#ffffe0");
    public static Color lime = Hex("#00ff00");
    public static Color limegreen = Hex("#32cd32");
    public static Color linen = Hex("#faf0e6");
    public static Color magenta = Hex("#ff00ff");
    public static Color maroon = Hex("#800000");
    public static Color mediumaquamarine = Hex("#66cdaa");
    public static Color mediumblue = Hex("#0000cd");
    public static Color mediumorchid = Hex("#ba55d3");
    public static Color mediumpurple = Hex("#9370db");
    public static Color mediumseagreen = Hex("#3cb371");
    public static Color mediumslateblue = Hex("#7b68ee");
    public static Color mediumspringgreen = Hex("#00fa9a");
    public static Color mediumturquoise = Hex("#48d1cc");
    public static Color mediumvioletred = Hex("#c71585");
    public static Color midnightblue = Hex("#191970");
    public static Color mintcream = Hex("#f5fffa");
    public static Color mistyrose = Hex("#ffe4e1");
    public static Color moccasin = Hex("#ffe4b5");
    public static Color navajowhite = Hex("#ffdead");
    public static Color navy = Hex("#000080");
    public static Color oldlace = Hex("#fdf5e6");
    public static Color olive = Hex("#808000");
    public static Color olivedrab = Hex("#6b8e23");
    public static Color orange = Hex("#ffa500");
    public static Color orangered = Hex("#ff4500");
    public static Color orchid = Hex("#da70d6");
    public static Color palegoldenrod = Hex("#eee8aa");
    public static Color palegreen = Hex("#98fb98");
    public static Color paleturquoise = Hex("#afeeee");
    public static Color palevioletred = Hex("#db7093");
    public static Color papayawhip = Hex("#ffefd5");
    public static Color peachpuff = Hex("#ffdab9");
    public static Color peru = Hex("#cd853f");
    public static Color pink = Hex("#ffc0cb");
    public static Color plum = Hex("#dda0dd");
    public static Color powderblue = Hex("#b0e0e6");
    public static Color purple = Hex("#800080");
    public static Color rebeccapurple = Hex("#663399");
    public static Color red = Hex("#ff0000");
    public static Color rosybrown = Hex("#bc8f8f");
    public static Color royalblue = Hex("#4169e1");
    public static Color saddlebrown = Hex("#8b4513");
    public static Color salmon = Hex("#fa8072");
    public static Color sandybrown = Hex("#f4a460");
    public static Color seagreen = Hex("#2e8b57");
    public static Color seashell = Hex("#fff5ee");
    public static Color sienna = Hex("#a0522d");
    public static Color silver = Hex("#c0c0c0");
    public static Color skyblue = Hex("#87ceeb");
    public static Color slateblue = Hex("#6a5acd");
    public static Color slategray = Hex("#708090");
    public static Color slategrey = Hex("#708090");
    public static Color snow = Hex("#fffafa");
    public static Color springgreen = Hex("#00ff7f");
    public static Color steelblue = Hex("#4682b4");
    public static Color tan = Hex("#d2b48c");
    public static Color teal = Hex("#008080");
    public static Color thistle = Hex("#d8bfd8");
    public static Color tomato = Hex("#ff6347");
    public static Color turquoise = Hex("#40e0d0");
    public static Color violet = Hex("#ee82ee");
    public static Color wheat = Hex("#f5deb3");
    public static Color white = Hex("#ffffff");
    public static Color whitesmoke = Hex("#f5f5f5");
    public static Color yellow = Hex("#ffff00");
    public static Color yellowgreen = Hex("#9acd32");
}