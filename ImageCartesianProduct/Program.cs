using SkiaSharp;
using System.Text.RegularExpressions;
using System.Xml;

public class Program
{
    public static string usage = @"Usage:

ImageCartesianProduct [rotation] [image1] [image2] [output-prefix]
ImageCartesianProduct delete [output-prefix]
ImageCartesianProduct verbose [rotation] [image1] [image2] [output-prefix]

Rotation should be 6 comma-separated numbers in degrees, to rotate the 6 4-dimensional planes.
[1st]: xy
[2nd]: xz
[3rd]: xw
[4th]: yz
[5th]: yw
[6th]: zw

Image1 and image2 should be file paths to PNG images. For best speed they should be small in width and height,
50x50 is good for example.

Output prefix should be a file path without '.png' at the end, output images will be named
[output-prefix]-[axis1-value]-[axis2-value].png

* delete will delete all .png files with the prefix specified
* verbose is like the standard command but it prints out all the trig results for debug purposes
";

    /*
    public static int GetAxisSize(SKBitmap image1, SKBitmap image2, char axis)
    {
        switch (axis)
        {
            case 'x':
                return image1.Width;
            case 'y':
                return image1.Height;
            case 'z':
                return image2.Width;
            case 'w':
                return image2.Height;
            default:
                throw new ArgumentException("Axis must be one of xyzw");
        }
    }

    public static string GetOtherAxes(char c1, char c2) =>
        "xyzw".Replace("" + c1, "").Replace("" + c2, "");
    */
    public static SKColor GetAverageColor(SKBitmap image1, SKBitmap image2, int x, int y, int z, int w)
    {
        if (x >= image1.Width  || x < 0 ||
            y >= image1.Height || y < 0 ||
            z >= image2.Width  || z < 0 ||
            w >= image2.Height || w < 0) return new SKColor(0, 0, 0, 0);
        SKColor color1 = image1.GetPixel(x, y);
        SKColor color2 = image2.GetPixel(z, w);
        SKColor result = new SKColor(
            (byte)Math.Sqrt((color1.Red * color1.Red + color2.Red * color2.Red) / 2),
            (byte)Math.Sqrt((color1.Green * color1.Green + color2.Green * color2.Green) / 2),
            (byte)Math.Sqrt((color1.Blue * color1.Blue + color2.Blue * color2.Blue) / 2),
            255
        );
        if (color1.Alpha == 0 || color2.Alpha == 0)
            result = result.WithAlpha(0);
        return result;
    }

    public static void Main(string[] args)
    {
        if (args.Length == 2 && args[0] == "delete")
        {
            foreach (string file in Directory.EnumerateFiles(new FileInfo(args[1]).Directory.FullName))
            {
                if (Path.GetFileName(file).StartsWith(args[1])
                    && Path.GetFileName(file).EndsWith(".png"))
                    File.Delete(file);
            }
            Environment.Exit(0);
        }
        else if (args.Length < 4)
        {
            Console.WriteLine(usage);
            Environment.Exit(1);
        }
        bool verbose = false;
        if (args[0] == "verbose")
        {
            verbose = true;
            args = args.Skip(1).ToArray();
        }

        double[] rotations = args[0].Split(',')
            .Select(x => int.Parse(x))
            .Select(x => x * (Math.PI / 180))
            .ToArray();
        /*
                char axis1 = args[0][0];
                char axis2 = args[0][1];
        */

        using SKBitmap image1 = SKBitmap.Decode(args[1]);
        using SKBitmap image2 = SKBitmap.Decode(args[2]);

/*
        string otherAxes = GetOtherAxes(axis1, axis2);

        Console.Error.WriteLine($@"Axis sizes:
    axis1: {GetAxisSize(image1, image2, axis1)}
    axis2: {GetAxisSize(image1, image2, axis2)}
    otherAxes[0]: {GetAxisSize(image1, image2, otherAxes[0])}
    otherAxes[1]: {GetAxisSize(image1, image2, otherAxes[1])}
");
*/


        for (int i = 0; i < image1.Width; i++)
        {
            for (int j = 0; j < image1.Height; j++)
            {
                using (SKBitmap slice = new SKBitmap(
                    Math.Max(image1.Width, image2.Width),
                    Math.Max(image1.Height, image2.Height)))
                {
                    for (int k = 0; k < image2.Width; k++)
                    {
                        for (int l = 0; l < image2.Height; l++)
                        {
                            double xb = i / (double)image1.Width - 0.5;
                            double yb = j / (double)image1.Height - 0.5;
                            double zb = k / (double)image2.Width - 0.5;
                            double wb = l / (double)image2.Height - 0.5;

                            double x1 = xb * Math.Cos(rotations[0]) - yb * Math.Sin(rotations[0]);
                            double y1 = xb * Math.Sin(rotations[0]) + yb * Math.Cos(rotations[0]);
                            double z1 = zb * Math.Cos(rotations[5]) - wb * Math.Sin(rotations[5]);
                            double w1 = zb * Math.Sin(rotations[5]) + wb * Math.Cos(rotations[5]);

                            double x2 = x1 * Math.Cos(rotations[1]) - z1 * Math.Sin(rotations[1]);
                            double z2 = x1 * Math.Sin(rotations[1]) + z1 * Math.Cos(rotations[1]);
                            double y2 = y1 * Math.Cos(rotations[4]) - w1 * Math.Sin(rotations[4]);
                            double w2 = y1 * Math.Sin(rotations[4]) + w1 * Math.Cos(rotations[4]);

                            double x3 = x2 * Math.Cos(rotations[2]) - w2 * Math.Sin(rotations[2]);
                            double w3 = x2 * Math.Sin(rotations[2]) + w2 * Math.Cos(rotations[2]);
                            double y3 = y2 * Math.Cos(rotations[3]) - z2 * Math.Sin(rotations[3]);
                            double z3 = y2 * Math.Sin(rotations[3]) + z2 * Math.Cos(rotations[3]);

                            int xr = (int)((x3 + 0.5) * image1.Width);
                            int yr = (int)((y3 + 0.5) * image1.Height);
                            int zr = (int)((z3 + 0.5) * image2.Width);
                            int wr = (int)((w3 + 0.5) * image2.Height);

                            if (verbose)
                            {
                                Console.Error.WriteLine($"xb, yb, zb, wb: {xb}, {yb}, {zb}, {wb}");
                                Console.Error.WriteLine($"x1, y1, z1, w1: {x1}, {y1}, {z1}, {w1}");
                                Console.Error.WriteLine($"x2, y2, z2, w2: {x2}, {y2}, {z2}, {w2}");
                                Console.Error.WriteLine($"x3, y3, z3, w3: {x3}, {y3}, {z3}, {w3}");
                                Console.Error.WriteLine($"xr, yr, zr, wr: {xr}, {yr}, {zr}, {wr}");
                            }

                            slice.SetPixel(k, l, GetAverageColor(image1, image2, xr, yr, zr, wr));
                        }
                    }
                    slice.Encode(new SKFileWStream($"{args[3]}-{i}-{j}.png"), SKEncodedImageFormat.Png, 100);
                }
                
            }
        }
    }
}