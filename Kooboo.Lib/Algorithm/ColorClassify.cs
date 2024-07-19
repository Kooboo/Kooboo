using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp.PixelFormats;

namespace Kooboo.Lib.Algorithm
{
    public class ColorClassify
    {
        public List<ColorCluster> TopColors(byte[] imagesBytes, int TopN)
        {
            var imageSize = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetImageSize(imagesBytes);

            int width = 500;
            int height = 360;

            if (imageSize != null && imageSize.Height > 0 && imageSize.Width > 0)
            {
                if (width > imageSize.Width)
                {
                    width = imageSize.Width;
                    height = imageSize.Height;
                }
                else
                {
                    width = 500;
                    height = (int)width * imageSize.Height / imageSize.Width;
                }
            }

            var rightSizeImage = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetThumbnailImageStream(imagesBytes, width, height);
            rightSizeImage.Position = 0;
            var bitmap = SixLabors.ImageSharp.Image.Load<Rgb24>(rightSizeImage);
            List<Rgb24> colors = new();

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    colors.Add(bitmap[x, y]);
                }
            }



            List<Rgb24> RandomColor = new List<Rgb24>();

            int widthScale = (int)bitmap.Width / TopN;
            int heightScale = (int)bitmap.Height / TopN;


            for (int i = 0; i < TopN; i++)
            {
                int widthRandom = widthScale * i + 1;

                if (widthRandom >= bitmap.Width)
                {
                    widthRandom = bitmap.Width - 1;
                }

                int heightRandom = heightScale * i + 1;

                if (heightRandom >= bitmap.Height)
                {
                    heightRandom = bitmap.Height - 1;
                }

                var color = bitmap[widthRandom, heightRandom];

                RandomColor.Add(color);
            }

            return KMeansCluster(colors, RandomColor, TopN);
        }

        public List<ColorCluster> KMeansCluster(List<Rgb24> colors, List<Rgb24> CenterColorIds, int k)
        {
            //Initialize centroids randomly
            List<Rgb24>[] clusters = new List<Rgb24>[k];

            bool centroidsChanged;

            do
            {
                centroidsChanged = false;
                for (int i = 0; i < k; i++)
                    clusters[i] = new List<Rgb24>();

                // Assign colors to the nearest centroid
                foreach (var color in colors)
                {
                    int nearestCentroid = FindNearestCentroid(color, CenterColorIds);
                    clusters[nearestCentroid].Add(color);
                }

                // Update centroids
                for (int i = 0; i < k; i++)
                {
                    var clusterI = clusters[i];

                    if (clusterI.Any())
                    {
                        var newCentroid = CalculateCentroid(clusters[i]);
                        if (!newCentroid.Equals(CenterColorIds[i]))
                        {
                            CenterColorIds[i] = newCentroid;
                            centroidsChanged = true;
                        }
                    }
                }

            } while (centroidsChanged);

            List<ColorCluster> result = new List<ColorCluster>();

            long totalCount = 0;

            for (int i = 0; i < k; i++)
            {
                var center = CenterColorIds[i];
                ColorCluster cluster = new ColorCluster();

                cluster.Color = RGBColor.FromSystemColor(center);

                var items = clusters[i];

                cluster.Count = items.Count();
                if (cluster.Count > 0)
                {
                    totalCount += cluster.Count;
                    result.Add(cluster);
                }
            }

            foreach (var item in result)
            {
                item.Percent = Math.Round((double)(item.Count) / (double)totalCount, 2);
            }


            return result.OrderByDescending(o => o.Count).ToList();
        }

        private int FindNearestCentroid(Rgb24 color, List<Rgb24> centroids)
        {
            double minDistance = double.MaxValue;
            int nearest = 0;

            for (int i = 0; i < centroids.Count; i++)
            {
                double distance = ColorDistance(color, centroids[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = i;
                }
            }

            return nearest;
        }

        private Rgb24 CalculateCentroid(List<Rgb24> cluster)
        {
            long r = 0, g = 0, b = 0;
            foreach (var color in cluster)
            {
                r += color.R;
                g += color.G;
                b += color.B;
            }

            long count = cluster.Count;
            var red = (byte)(r / count);
            var green = (byte)(g / count);
            var yellow = (byte)(b / count);

            return new Rgb24(red, green, yellow);
        }

        private double ColorDistance(Rgb24 c1, Rgb24 c2)
        {
            return Math.Sqrt(Math.Pow(c1.R - c2.R, 2) + Math.Pow(c1.G - c2.G, 2) + Math.Pow(c1.B - c2.B, 2));
        }
    }

    public class ColorCluster
    {
        public RGBColor Color { get; set; }

        public int Count { get; set; }

        public double Percent { get; set; }
    }

    public class RGBColor
    {
        public static RGBColor FromSystemColor(Rgb24 color)
        {
            RGBColor result = new RGBColor();
            result.R = color.R;
            result.G = color.G;
            result.B = color.B;

            result.NearByColorName = GetNearestKnownColorName(color.R, color.G, color.B);

            result.ColorCode = $"#{color.R:X2}{color.G:X2}{color.B:X2}";

            return result;
        }

        public byte R;
        public byte G;
        public byte B;

        private string _colorCode;
        public string ColorCode
        {
            get
            {
                if (_colorCode == null)
                {
                    if (R > 0 || G > 0 || B > 0)
                    {
                        _colorCode = $"#{R:X2}{G:X2}{B:X2}";
                    }
                }
                return _colorCode;
            }
            set
            {
                _colorCode = value;
            }
        }

        public string NearByColorName { get; set; }

        private static string GetNearestKnownColorName(byte R, byte G, byte B)
        {
            return GetNearestKnownColorName(R, G, B, ColorList);
        }

        public static string GetNearestKnownSimpleColorName(byte R, byte G, byte B)
        {
            return GetNearestKnownColorName(R, G, B, SimpleColorList);
        }

        private static string GetNearestKnownColorName(byte R, byte G, byte B, List<RGBColor> colorList)
        {
            double minDistance = double.MaxValue;
            string nearestColorName = string.Empty;
            foreach (var color in colorList)
            {

                double distance = Math.Sqrt(
                    Math.Pow(color.R - R, 2) +
                    Math.Pow(color.G - G, 2) +
                    Math.Pow(color.B - B, 2)
                );

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestColorName = color.NearByColorName;
                }
            }

            return nearestColorName;
        }


        private static List<RGBColor> _list;
        private static List<RGBColor> _simpleList;
        private static object _locker = new object();

        public static List<RGBColor> ColorList
        {
            get
            {
                if (_list == null)
                {
                    lock (_locker)
                    {
                        if (_list == null)
                        {
                            var newList = new List<RGBColor>();

                            newList.Add(GetColor("AliceBlue", 240, 248, 255));
                            newList.Add(GetColor("AntiqueWhite", 250, 235, 215));
                            newList.Add(GetColor("Aqua", 0, 255, 255));
                            newList.Add(GetColor("Aquamarine", 127, 255, 212));
                            newList.Add(GetColor("Azure", 240, 255, 255));
                            newList.Add(GetColor("Beige", 245, 245, 220));
                            newList.Add(GetColor("Bisque", 255, 228, 196));
                            newList.Add(GetColor("Black", 0, 0, 0));
                            newList.Add(GetColor("BlanchedAlmond", 255, 235, 205));
                            newList.Add(GetColor("Blue", 0, 0, 255));
                            newList.Add(GetColor("BlueViolet", 138, 43, 226));
                            newList.Add(GetColor("Brown", 165, 42, 42));
                            newList.Add(GetColor("BurlyWood", 222, 184, 135));
                            newList.Add(GetColor("CadetBlue", 95, 158, 160));
                            newList.Add(GetColor("Chartreuse", 127, 255, 0));
                            newList.Add(GetColor("Chocolate", 210, 105, 30));
                            newList.Add(GetColor("Coral", 255, 127, 80));
                            newList.Add(GetColor("CornflowerBlue", 100, 149, 237));
                            newList.Add(GetColor("Cornsilk", 255, 248, 220));
                            newList.Add(GetColor("Crimson", 220, 20, 60));
                            newList.Add(GetColor("Cyan", 0, 255, 255));
                            newList.Add(GetColor("DarkBlue", 0, 0, 139));
                            newList.Add(GetColor("DarkCyan", 0, 139, 139));
                            newList.Add(GetColor("DarkGoldenRod", 184, 134, 11));
                            newList.Add(GetColor("DarkGray", 169, 169, 169));
                            newList.Add(GetColor("DarkGreen", 0, 100, 0));
                            newList.Add(GetColor("DarkKhaki", 189, 183, 107));
                            newList.Add(GetColor("DarkMagenta", 139, 0, 139));
                            newList.Add(GetColor("DarkOliveGreen", 85, 107, 47));
                            newList.Add(GetColor("DarkOrange", 255, 140, 0));
                            newList.Add(GetColor("DarkOrchid", 153, 50, 204));
                            newList.Add(GetColor("DarkRed", 139, 0, 0));
                            newList.Add(GetColor("DarkSalmon", 233, 150, 122));
                            newList.Add(GetColor("DarkSeaGreen", 143, 188, 143));
                            newList.Add(GetColor("DarkSlateBlue", 72, 61, 139));
                            newList.Add(GetColor("DarkSlateGray", 47, 79, 79));
                            newList.Add(GetColor("DarkTurquoise", 0, 206, 209));
                            newList.Add(GetColor("DarkViolet", 148, 0, 211));
                            newList.Add(GetColor("DeepPink", 255, 20, 147));
                            newList.Add(GetColor("DeepSkyBlue", 0, 191, 255));
                            newList.Add(GetColor("DimGray", 105, 105, 105));
                            newList.Add(GetColor("DodgerBlue", 30, 144, 255));
                            newList.Add(GetColor("FireBrick", 178, 34, 34));
                            newList.Add(GetColor("FloralWhite", 255, 250, 240));
                            newList.Add(GetColor("ForestGreen", 34, 139, 34));
                            newList.Add(GetColor("Fuchsia", 255, 0, 255));
                            newList.Add(GetColor("Gainsboro", 220, 220, 220));
                            newList.Add(GetColor("GhostWhite", 248, 248, 255));
                            newList.Add(GetColor("Gold", 255, 215, 0));
                            newList.Add(GetColor("GoldenRod", 218, 165, 32));
                            newList.Add(GetColor("Gray", 128, 128, 128));
                            newList.Add(GetColor("Green", 0, 128, 0));
                            newList.Add(GetColor("GreenYellow", 173, 255, 47));
                            newList.Add(GetColor("HoneyDew", 240, 255, 240));
                            newList.Add(GetColor("HotPink", 255, 105, 180));
                            newList.Add(GetColor("IndianRed", 205, 92, 92));
                            newList.Add(GetColor("Indigo", 75, 0, 130));
                            newList.Add(GetColor("Ivory", 255, 255, 240));
                            newList.Add(GetColor("Khaki", 240, 230, 140));
                            newList.Add(GetColor("Lavender", 230, 230, 250));
                            newList.Add(GetColor("LavenderBlush", 255, 240, 245));
                            newList.Add(GetColor("LawnGreen", 124, 252, 0));
                            newList.Add(GetColor("LemonChiffon", 255, 250, 205));
                            newList.Add(GetColor("LightBlue", 173, 216, 230));
                            newList.Add(GetColor("LightCoral", 240, 128, 128));
                            newList.Add(GetColor("LightCyan", 224, 255, 255));
                            newList.Add(GetColor("LightGoldenRodYellow", 250, 250, 210));
                            newList.Add(GetColor("LightGray", 211, 211, 211));
                            newList.Add(GetColor("LightGreen", 144, 238, 144));
                            newList.Add(GetColor("LightPink", 255, 182, 193));
                            newList.Add(GetColor("LightSalmon", 255, 160, 122));
                            newList.Add(GetColor("LightSeaGreen", 32, 178, 170));
                            newList.Add(GetColor("LightSkyBlue", 135, 206, 250));
                            newList.Add(GetColor("LightSlateGray", 119, 136, 153));
                            newList.Add(GetColor("LightSteelBlue", 176, 196, 222));
                            newList.Add(GetColor("LightYellow", 255, 255, 224));
                            newList.Add(GetColor("Lime", 0, 255, 0));
                            newList.Add(GetColor("LimeGreen", 50, 205, 50));
                            newList.Add(GetColor("Linen", 250, 240, 230));
                            newList.Add(GetColor("Magenta", 255, 0, 255));
                            newList.Add(GetColor("Maroon", 128, 0, 0));
                            newList.Add(GetColor("MediumAquaMarine", 102, 205, 170));
                            newList.Add(GetColor("MediumBlue", 0, 0, 205));
                            newList.Add(GetColor("MediumOrchid", 186, 85, 211));
                            newList.Add(GetColor("MediumPurple", 147, 112, 219));
                            newList.Add(GetColor("MediumSeaGreen", 60, 179, 113));
                            newList.Add(GetColor("MediumSlateBlue", 123, 104, 238));
                            newList.Add(GetColor("MediumSpringGreen", 0, 250, 154));
                            newList.Add(GetColor("MediumTurquoise", 72, 209, 204));
                            newList.Add(GetColor("MediumVioletRed", 199, 21, 133));
                            newList.Add(GetColor("MidnightBlue", 25, 25, 112));
                            newList.Add(GetColor("MintCream", 245, 255, 250));
                            newList.Add(GetColor("MistyRose", 255, 228, 225));
                            newList.Add(GetColor("Moccasin", 255, 228, 181));
                            newList.Add(GetColor("NavajoWhite", 255, 222, 173));
                            newList.Add(GetColor("Navy", 0, 0, 128));
                            newList.Add(GetColor("OldLace", 253, 245, 230));
                            newList.Add(GetColor("Olive", 128, 128, 0));
                            newList.Add(GetColor("OliveDrab", 107, 142, 35));
                            newList.Add(GetColor("Orange", 255, 165, 0));
                            newList.Add(GetColor("OrangeRed", 255, 69, 0));
                            newList.Add(GetColor("Orchid", 218, 112, 214));
                            newList.Add(GetColor("PaleGoldenRod", 238, 232, 170));
                            newList.Add(GetColor("PaleGreen", 152, 251, 152));
                            newList.Add(GetColor("PaleTurquoise", 175, 238, 238));
                            newList.Add(GetColor("PaleVioletRed", 219, 112, 147));
                            newList.Add(GetColor("PapayaWhip", 255, 239, 213));
                            newList.Add(GetColor("PeachPuff", 255, 218, 185));
                            newList.Add(GetColor("Peru", 205, 133, 63));
                            newList.Add(GetColor("Pink", 255, 192, 203));
                            newList.Add(GetColor("Plum", 221, 160, 221));
                            newList.Add(GetColor("PowderBlue", 176, 224, 230));
                            newList.Add(GetColor("Purple", 128, 0, 128));
                            newList.Add(GetColor("Red", 255, 0, 0));
                            newList.Add(GetColor("RosyBrown", 188, 143, 143));
                            newList.Add(GetColor("RoyalBlue", 65, 105, 225));
                            newList.Add(GetColor("SaddleBrown", 139, 69, 19));
                            newList.Add(GetColor("Salmon", 250, 128, 114));
                            newList.Add(GetColor("SandyBrown", 244, 164, 96));
                            newList.Add(GetColor("SeaGreen", 46, 139, 87));
                            newList.Add(GetColor("SeaShell", 255, 245, 238));
                            newList.Add(GetColor("Sienna", 160, 82, 45));
                            newList.Add(GetColor("Silver", 192, 192, 192));
                            newList.Add(GetColor("SkyBlue", 135, 206, 235));
                            newList.Add(GetColor("SlateBlue", 106, 90, 205));
                            newList.Add(GetColor("SlateGray", 112, 128, 144));
                            newList.Add(GetColor("Snow", 255, 250, 250));
                            newList.Add(GetColor("SpringGreen", 0, 255, 127));
                            newList.Add(GetColor("SteelBlue", 70, 130, 180));
                            newList.Add(GetColor("Tan", 210, 180, 140));
                            newList.Add(GetColor("Teal", 0, 128, 128));
                            newList.Add(GetColor("Thistle", 216, 191, 216));
                            newList.Add(GetColor("Tomato", 255, 99, 71));
                            newList.Add(GetColor("Turquoise", 64, 224, 208));
                            newList.Add(GetColor("Violet", 238, 130, 238));
                            newList.Add(GetColor("Wheat", 245, 222, 179));
                            newList.Add(GetColor("White", 255, 255, 255));
                            newList.Add(GetColor("WhiteSmoke", 245, 245, 245));
                            newList.Add(GetColor("Yellow", 255, 255, 0));
                            newList.Add(GetColor("YellowGreen", 154, 205, 50));

                            _list = newList;
                        }
                    }
                }

                return _list;
            }

        }

        public static List<RGBColor> SimpleColorList
        {
            get
            {
                if (_simpleList == null)
                {
                    var newList = new List<RGBColor>
                    {
                        GetColor("White", 255, 255, 255),
                        GetColor("Silver", 192, 192, 192),
                        GetColor("Gray", 128, 128, 128),
                        GetColor("Black", 0, 0, 0),
                        GetColor("Red", 255, 0, 0),
                        GetColor("Maroon", 128, 0, 0),
                        GetColor("Yellow", 255, 255, 0),
                        GetColor("Olive", 128, 128, 0),
                        GetColor("Lime", 0, 255, 0),
                        GetColor("Green", 0, 128, 0),
                        GetColor("Aqua", 0, 255, 255),
                        GetColor("Teal", 0, 128, 128),
                        GetColor("Blue", 0, 0, 255),
                        GetColor("Navy", 0, 0, 128),
                        GetColor("Fuchsia", 255, 0, 255),
                        GetColor("Purple", 128, 0, 128)
                    };
                    _simpleList = newList;

                }
                return _simpleList;
            }
        }
        private static RGBColor GetColor(string name, byte R, byte G, byte B)
        {
            return new RGBColor() { R = R, G = G, B = B, NearByColorName = name };
        }
    }

}
