#region License
/* Copyright (c) 2018 W. Hampson
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
#endregion

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace WHampson.MapStitcher
{
    internal class IvMapStitcher
    {
        private const string PrograName = "IvMapStitcher";

        private const int TileWidth = 512;
        private const int TileHeight = 512;
        private const int TileCountX = 12;
        private const int TileCountY = 9;
        private const int TileCount = TileCountX * TileCountY;

        internal static void Main(string[] args)
        {
            if (args.Length < 2) {
                Usage();
                Environment.Exit(1);
            }

            string imageDir = args[0];
            string outFile = args[1];

            if (!Directory.Exists(imageDir)) {
                Console.WriteLine("Error: directory not found - '{0}'", imageDir);
                Environment.Exit(1);
            }

            string[] files = Directory.GetFiles(imageDir);
            if (files.Length < TileCount) {
                Console.Error.WriteLine("Error: not enough tile files found. Expecting {0}.", TileCount);
            }

            Image mapImage = new Bitmap(TileWidth * TileCountX, TileHeight * TileCountY);
            Graphics mapGraphics = Graphics.FromImage(mapImage);

            int currX = 0;
            int currY = 0;
            foreach (string imgFile in files) {
                Image tile = Image.FromFile(imgFile);
                Point loc = new Point(currX * TileWidth, currY * TileHeight);

                Console.WriteLine("Adding {0} at ({1},{2})...", imgFile, loc.X, loc.Y);
                mapGraphics.DrawImage(tile, loc);

                currX++;
                if (currX >= TileCountX) {
                    currX = 0;
                    currY++;
                }
            }

            mapImage.Save(outFile, ImageFormat.Png);
            Console.WriteLine("Created {0}", outFile);
        }

        private static void Usage()
        {
            Console.WriteLine("Usage: {0} map_files output_file", PrograName);
            Console.WriteLine("\nCreates a GTA IV map image by stitching together the radar tiles from the game data.");
            Console.WriteLine("Arguments:");
            Console.WriteLine("    map_files    directory containing the radar tiles");
            Console.WriteLine("    output_file  path to the new map file (PNG)");
            Console.WriteLine("\nCopyright (C) 2018 Wes Hampson");
            Console.WriteLine("Version: {0}", GetVersionString(Assembly.GetExecutingAssembly()));
        }

        private static string GetVersionString(Assembly asm)
        {
            if (asm == null) {
                return "";
            }

            return FileVersionInfo.GetVersionInfo(asm.Location).ProductVersion;
        }
    }
}
