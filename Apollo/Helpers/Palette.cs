using System;
using System.IO;
using System.Linq;

using Apollo.Structures;

namespace Apollo.Helpers {
    public class Palette {
        public static Palette Monochrome = new Palette((i) => new Color(i));
        
        public static Palette NovationPalette = new Palette(new Color[128] {
            new Color(0, 0, 0),
            new Color(15, 15, 15),
            new Color(63, 63, 63),
            new Color(127, 127, 127),
            new Color(127, 38, 38),
            new Color(127, 0, 0),
            new Color(44, 0, 0),
            new Color(12, 0, 0),
            new Color(127, 94, 54),
            new Color(127, 42, 0),
            new Color(44, 14, 0),
            new Color(19, 13, 0),
            new Color(127, 127, 38),
            new Color(127, 127, 0),
            new Color(44, 44, 0),
            new Color(12, 12, 0),
            new Color(68, 127, 38),
            new Color(42, 127, 0),
            new Color(14, 44, 0),
            new Color(10, 21, 0),
            new Color(38, 127, 38),
            new Color(0, 127, 0),
            new Color(0, 44, 0),
            new Color(0, 12, 0),
            new Color(38, 127, 47),
            new Color(0, 127, 12),
            new Color(0, 44, 6),
            new Color(0, 12, 1),
            new Color(38, 127, 68),
            new Color(0, 127, 42),
            new Color(0, 44, 14),
            new Color(0, 15, 9),
            new Color(38, 127, 91),
            new Color(0, 127, 76),
            new Color(0, 44, 26),
            new Color(0, 12, 9),
            new Color(38, 97, 127),
            new Color(0, 84, 127),
            new Color(0, 32, 41),
            new Color(0, 8, 12),
            new Color(38, 68, 127),
            new Color(0, 42, 127),
            new Color(0, 14, 44),
            new Color(0, 4, 12),
            new Color(38, 38, 127),
            new Color(0, 0, 127),
            new Color(0, 0, 44),
            new Color(0, 0, 12),
            new Color(67, 38, 127),
            new Color(42, 0, 127),
            new Color(12, 0, 50),
            new Color(7, 0, 24),
            new Color(127, 38, 127),
            new Color(127, 0, 127),
            new Color(44, 0, 44),
            new Color(12, 0, 12),
            new Color(127, 38, 67),
            new Color(127, 0, 42),
            new Color(44, 0, 14),
            new Color(17, 0, 9),
            new Color(127, 10, 0),
            new Color(76, 26, 0),
            new Color(60, 40, 0),
            new Color(33, 50, 0),
            new Color(1, 28, 0),
            new Color(0, 43, 26),
            new Color(0, 42, 63),
            new Color(0, 0, 127),
            new Color(0, 34, 39),
            new Color(18, 0, 102),
            new Color(63, 63, 63),
            new Color(16, 16, 16),
            new Color(127, 0, 0),
            new Color(94, 127, 22),
            new Color(87, 118, 3),
            new Color(50, 127, 4),
            new Color(8, 69, 0),
            new Color(0, 127, 67),
            new Color(0, 84, 127),
            new Color(0, 21, 127),
            new Color(31, 0, 127),
            new Color(61, 0, 127),
            new Color(89, 13, 62),
            new Color(32, 16, 0),
            new Color(127, 37, 0),
            new Color(68, 112, 3),
            new Color(57, 127, 10),
            new Color(0, 127, 0),
            new Color(29, 127, 19),
            new Color(44, 127, 56),
            new Color(28, 127, 102),
            new Color(45, 69, 127),
            new Color(24, 40, 99),
            new Color(67, 63, 116),
            new Color(105, 14, 127),
            new Color(127, 0, 46),
            new Color(127, 63, 0),
            new Color(92, 88, 0),
            new Color(72, 127, 0),
            new Color(65, 46, 3),
            new Color(28, 21, 0),
            new Color(10, 38, 8),
            new Color(6, 40, 28),
            new Color(10, 10, 21),
            new Color(11, 16, 45),
            new Color(52, 30, 14),
            new Color(84, 0, 5),
            new Color(111, 40, 30),
            new Color(108, 53, 14),
            new Color(127, 112, 19),
            new Color(79, 112, 23),
            new Color(51, 90, 7),
            new Color(15, 15, 24),
            new Color(110, 127, 53),
            new Color(64, 127, 94),
            new Color(77, 76, 127),
            new Color(71, 51, 127),
            new Color(32, 32, 32),
            new Color(58, 58, 58),
            new Color(112, 127, 127),
            new Color(80, 0, 0),
            new Color(26, 0, 0),
            new Color(13, 104, 0),
            new Color(3, 33, 0),
            new Color(92, 88, 0),
            new Color(31, 24, 0),
            new Color(89, 47, 0),
            new Color(37, 10, 1)
        });

        public static Palette mat1jaczyyyPalette = new Palette(new Color[128] {
            new Color(0, 0, 0),
            new Color(31, 0, 0),
            new Color(63, 0, 0),
            new Color(95, 0, 0),
            new Color(127, 31, 31),
            new Color(127, 0, 0),
            new Color(31, 7, 0),
            new Color(63, 15, 0),
            new Color(95, 23, 0),
            new Color(127, 55, 31),
            new Color(127, 31, 0),
            new Color(31, 15, 0),
            new Color(63, 31, 0),
            new Color(95, 47, 0),
            new Color(127, 79, 31),
            new Color(127, 63, 0),
            new Color(31, 23, 0),
            new Color(63, 47, 0),
            new Color(95, 71, 0),
            new Color(127, 103, 31),
            new Color(127, 95, 0),
            new Color(31, 31, 0),
            new Color(63, 63, 0),
            new Color(95, 95, 0),
            new Color(127, 127, 31),
            new Color(127, 127, 0),
            new Color(23, 31, 0),
            new Color(47, 63, 0),
            new Color(71, 95, 0),
            new Color(103, 127, 31),
            new Color(95, 127, 0),
            new Color(15, 31, 0),
            new Color(31, 63, 0),
            new Color(47, 95, 0),
            new Color(79, 127, 31),
            new Color(63, 127, 0),
            new Color(7, 31, 0),
            new Color(15, 63, 0),
            new Color(23, 95, 0),
            new Color(55, 127, 31),
            new Color(31, 127, 0),
            new Color(0, 31, 0),
            new Color(0, 63, 0),
            new Color(0, 95, 0),
            new Color(31, 127, 31),
            new Color(0, 127, 0),
            new Color(0, 31, 7),
            new Color(0, 63, 15),
            new Color(0, 95, 23),
            new Color(31, 127, 55),
            new Color(0, 127, 31),
            new Color(0, 31, 15),
            new Color(0, 63, 31),
            new Color(0, 95, 47),
            new Color(31, 127, 79),
            new Color(0, 127, 63),
            new Color(0, 31, 23),
            new Color(0, 63, 47),
            new Color(0, 95, 71),
            new Color(31, 127, 103),
            new Color(0, 127, 95),
            new Color(0, 31, 31),
            new Color(0, 63, 63),
            new Color(0, 95, 95),
            new Color(31, 125, 127),
            new Color(0, 127, 127),
            new Color(0, 23, 31),
            new Color(0, 47, 63),
            new Color(0, 71, 95),
            new Color(31, 103, 127),
            new Color(0, 95, 127),
            new Color(0, 15, 31),
            new Color(0, 31, 63),
            new Color(0, 47, 95),
            new Color(31, 79, 127),
            new Color(0, 63, 127),
            new Color(0, 7, 31),
            new Color(0, 15, 63),
            new Color(0, 23, 95),
            new Color(31, 55, 127),
            new Color(0, 31, 127),
            new Color(0, 0, 31),
            new Color(0, 0, 63),
            new Color(0, 0, 95),
            new Color(31, 31, 127),
            new Color(0, 0, 127),
            new Color(7, 0, 31),
            new Color(15, 0, 63),
            new Color(23, 0, 95),
            new Color(55, 31, 127),
            new Color(31, 0, 127),
            new Color(15, 0, 31),
            new Color(31, 0, 63),
            new Color(47, 0, 95),
            new Color(79, 31, 127),
            new Color(63, 0, 127),
            new Color(23, 0, 31),
            new Color(47, 0, 63),
            new Color(71, 0, 95),
            new Color(103, 31, 127),
            new Color(95, 0, 127),
            new Color(31, 0, 31),
            new Color(63, 0, 63),
            new Color(95, 0, 95),
            new Color(125, 31, 125),
            new Color(127, 0, 127),
            new Color(31, 0, 23),
            new Color(63, 0, 47),
            new Color(95, 0, 71),
            new Color(127, 31, 103),
            new Color(127, 0, 95),
            new Color(31, 0, 15),
            new Color(63, 0, 31),
            new Color(95, 0, 47),
            new Color(127, 31, 79),
            new Color(127, 0, 63),
            new Color(31, 0, 7),
            new Color(63, 0, 15),
            new Color(95, 0, 23),
            new Color(127, 31, 55),
            new Color(127, 0, 31),
            new Color(19, 19, 19),
            new Color(37, 37, 37),
            new Color(55, 55, 55),
            new Color(73, 73, 73),
            new Color(91, 91, 91),
            new Color(109, 109, 109),
            new Color(127, 127, 127)
        });

        public static Palette Decode(Stream file) {
            Color[] palette = new Color[128];

            using (StreamReader reader = new StreamReader(file)) {
                try {
                    for (int i = 0; i < 128; i++) {
                        byte[] split = reader.ReadLine().TrimEnd(';').Split(", ")[1].Split(' ').Select((x) => Convert.ToByte(x)).ToArray();
                        palette[i] = new Color(split[0], split[1], split[2]);
                    }
                } catch {
                    return null;
                }
            }

            return new Palette(palette);
        }

        public readonly Color[] BackingArray;

        Func<byte, Color> _converter;

        public Palette(Func<byte, Color> converter) => _converter = converter;

        public Palette(Color[] palette) {
            if (palette.Length == 128) {
                BackingArray = palette;
                _converter = (i) => BackingArray[i].Clone();
            }
        }
        
        public Color GetColor(byte color) => _converter.Invoke(color);
    }
}