using System;

namespace Rise.FontTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            App.OnInit += () =>
            {
                var font = new Font("Inconsolata.otf", null);
                var size = new FontSize(font, 24f);
                FontChar info;

                var packer = new RectanglePacker(4096, 4096, font.CharacterCount);

                foreach (char chr in font.Characters)
                {
                    if (!size.IsEmpty(chr))
                    {
                        size.GetCharInfo(chr, out info);
                        packer.Add(chr, info.Width + 1, info.Height + 1, true);
                    }
                }

                var time = Time.ClockMilliseconds;

                if (packer.Pack())
                {
                    Console.WriteLine("Pack time: " + (Time.ClockMilliseconds - time) + " ms");

                    int atlasW, atlasH;
                    packer.GetBounds(out atlasW, out atlasH);

                    var atlas = new Bitmap(atlasW.ToPowerOf2(), atlasH.ToPowerOf2());
                    var bitmap = new Bitmap(size.MaxCharW, size.MaxCharH);

                    int maxSize = Math.Max(size.MaxCharW, size.MaxCharH);
                    var rotated = new Bitmap(maxSize, maxSize);

                    int id;
                    RectangleI rect;
                    for (int i = 0; i < packer.PackedCount; ++i)
                    {
                        packer.GetPacked(i, out id, out rect);
                        rect.W--;
                        rect.H--;

                        size.GetPixels((char)id, bitmap);

                        if (bitmap.Width == rect.W)
                        {
                            atlas.CopyPixels(bitmap, rect.X, rect.Y);
                        }
                        else
                        {
                            bitmap.RotateRight(rotated);
                            atlas.CopyPixels(rotated, rect.X, rect.Y);
                        }
                    }

                    atlas.SavePng("font.png");
                }
                else
                    Console.WriteLine("Fail: " + packer.PackedCount);
            };

            App.Init<PlatformSDL.PlatformSDL2>();
            App.Run("Font Test", 640, 360, null);
        }
    }
}
