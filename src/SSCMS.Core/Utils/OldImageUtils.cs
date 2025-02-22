﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public static class OldImageUtils
    {
        public static Image GetImage(string imageFilePath)
        {
            var fs = new FileStream(imageFilePath, FileMode.Open);
            var br = new BinaryReader(fs);
            var bytes = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close();
            var ms = new MemoryStream(bytes);

            var image = Image.FromStream(ms, false);

            return image;
        }

        public static ImageFormat GetImageFormat(string imagePath)
        {
            var extName = StringUtils.ToLower(PathUtils.GetExtension(imagePath));
            switch (extName)
            {
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".emf":
                    return ImageFormat.Emf;
                case ".exif":
                    return ImageFormat.Exif;
                case ".gif":
                    return ImageFormat.Gif;
                case ".ico":
                    return ImageFormat.Icon;
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".png":
                    return ImageFormat.Png;
                case ".tiff":
                    return ImageFormat.Tiff;
                case ".wmf":
                    return ImageFormat.Wmf;
            }
            return ImageFormat.Png;
        }

        private static PointF GetWaterMarkPointF(Image image, int waterMarkPosition, float waterMarkWidth, float waterMarkHeight, bool textMark)
        {
            float x;
            float y;
            switch (waterMarkPosition)
            {
                case 1:
                    if (textMark)
                    {
                        x = waterMarkWidth / 2;
                    }
                    else
                    {
                        x = 0;
                    }
                    y = 0;
                    break;
                case 2:
                    if (textMark)
                    {
                        x = (image.Width / 2);
                    }
                    else
                    {
                        x = (image.Width / 2) - (waterMarkWidth / 2);
                    }
                    y = 0;
                    break;
                case 3:
                    if (textMark)
                    {
                        x = image.Width - waterMarkWidth / 2;
                    }
                    else
                    {
                        x = image.Width - waterMarkWidth;
                    }
                    y = 0;
                    break;
                case 4:
                    if (textMark)
                    {
                        x = waterMarkWidth / 2;
                    }
                    else
                    {
                        x = 0;
                    }
                    y = (image.Height / 2) - (waterMarkHeight / 2);
                    break;
                case 5:
                    if (textMark)
                    {
                        x = (image.Width / 2);
                    }
                    else
                    {
                        x = (image.Width / 2) - (waterMarkWidth / 2);
                    }
                    y = (image.Height / 2) - (waterMarkHeight / 2);
                    break;
                case 6:
                    if (textMark)
                    {
                        x = image.Width - waterMarkWidth / 2;
                    }
                    else
                    {
                        x = image.Width - waterMarkWidth;
                    }
                    y = (image.Height / 2) - (waterMarkHeight / 2);
                    break;
                case 7:
                    if (textMark)
                    {
                        x = waterMarkWidth / 2;
                    }
                    else
                    {
                        x = 0;
                    }
                    y = image.Height - waterMarkHeight;
                    break;
                case 8:
                    if (textMark)
                    {
                        x = (image.Width / 2);
                    }
                    else
                    {
                        x = (image.Width / 2) - (waterMarkWidth / 2);
                    }
                    y = image.Height - waterMarkHeight;
                    break;
                default:

                    if (textMark)
                    {
                        x = image.Width - waterMarkWidth / 2;
                    }
                    else
                    {
                        x = image.Width - waterMarkWidth;
                    }
                    y = image.Height - waterMarkHeight;
                    break;
            }
            return new PointF(x, y);
        }

        public static void AddTextWaterMark(string imagePath, string waterMarkText, string fontName, int fontSize, int waterMarkPosition, int waterMarkTransparency, int minWidth, int minHeight)
        {
            try
            {
                var image = GetImage(imagePath);

                if (minWidth > 0)
                {
                    if (image.Width < minWidth)
                    {
                        image.Dispose();
                        return;
                    }
                }
                if (minHeight > 0)
                {
                    if (image.Height < minHeight)
                    {
                        image.Dispose();
                        return;
                    }
                }

                var b = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
                var picture = Graphics.FromImage(b);
                picture.Clear(Color.White);
                picture.SmoothingMode = SmoothingMode.Default;
                picture.InterpolationMode = InterpolationMode.Default;

                picture.DrawImage(image, 0, 0, image.Width, image.Height);

                var sizes = new[] { fontSize, 16, 14, 12, 10, 8, 6, 4 };
                Font crFont = null;
                var crSize = new SizeF();
                for (var i = 0; i < 8; i++)
                {
                    crFont = new Font(fontName, sizes[i], FontStyle.Bold);
                    crSize = picture.MeasureString(waterMarkText, crFont);

                    if ((ushort)crSize.Width < (ushort)image.Width && (ushort)crSize.Height < (ushort)image.Height) break;
                }

                if (image.Width <= Convert.ToInt32(crSize.Width) || image.Height <= Convert.ToInt32(crSize.Height)) return;
                var pointF = GetWaterMarkPointF(image, waterMarkPosition, crSize.Width, crSize.Height, true);

                if (pointF.X < 0 || pointF.X >= image.Width || pointF.Y < 0 || pointF.Y >= image.Height) return;

                var strFormat = new StringFormat { Alignment = StringAlignment.Center };

                var alphaRate = (255 * waterMarkTransparency) / 10;
                if (alphaRate <= 0 || alphaRate > 255) alphaRate = 153;

                var semiTransBrush2 = new SolidBrush(Color.FromArgb(alphaRate, 0, 0, 0));
                picture.DrawString(waterMarkText, crFont, semiTransBrush2, pointF.X + 1, pointF.Y + 1, strFormat);

                var semiTransBrush = new SolidBrush(Color.FromArgb(alphaRate, 255, 255, 255));
                //
                picture.DrawString(waterMarkText, crFont, semiTransBrush, pointF.X, pointF.Y, strFormat);

                semiTransBrush2.Dispose();
                semiTransBrush.Dispose();

                var fileType = FileUtils.GetType(PathUtils.GetExtension(imagePath));
                var imageFormat = ImageFormat.Jpeg;
                if (fileType == FileType.Bmp)
                {
                    imageFormat = ImageFormat.Bmp;
                }
                else if (fileType == FileType.Gif)
                {
                    imageFormat = ImageFormat.Gif;
                }
                else if (fileType == FileType.Png)
                {
                    imageFormat = ImageFormat.Png;
                }

                b.Save(imagePath, imageFormat);
                b.Dispose();
                image.Dispose();

            }
            catch
            {
                // ignored
            }
        }

        public static void AddImageWaterMark(string imagePath, string waterMarkImagePath, int waterMarkPosition, int waterMarkTransparency, int minWidth, int minHeight)
        {
            try
            {
                var image = GetImage(imagePath);

                if (minWidth > 0)
                {
                    if (image.Width < minWidth)
                    {
                        image.Dispose();
                        return;
                    }
                }
                if (minHeight > 0)
                {
                    if (image.Height < minHeight)
                    {
                        image.Dispose();
                        return;
                    }
                }

                var b = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
                var picture = Graphics.FromImage(b);
                picture.Clear(Color.White);
                picture.SmoothingMode = SmoothingMode.Default;
                picture.InterpolationMode = InterpolationMode.Default;

                picture.DrawImage(image, 0, 0, image.Width, image.Height);

                var waterMark = GetImage(waterMarkImagePath);

                if (image.Width <= waterMark.Width || image.Height <= waterMark.Height) return;
                var pointF = GetWaterMarkPointF(image, waterMarkPosition, waterMark.Width, waterMark.Height, false);
                var xpos = Convert.ToInt32(pointF.X);
                var ypos = Convert.ToInt32(pointF.Y);

                if (xpos < 0 || xpos >= image.Width || ypos < 0 || ypos >= image.Height) return;

                var alphaRate = (255 * waterMarkTransparency) / 10;
                if (alphaRate <= 0 || alphaRate > 255) alphaRate = 153;

                var bmWaterMark = new Bitmap(waterMark);
                for (var ix = 0; ix < waterMark.Width; ix++)
                {
                    for (var iy = 0; iy < waterMark.Height; iy++)
                    {
                        int ir = bmWaterMark.GetPixel(ix, iy).R;
                        int ig = bmWaterMark.GetPixel(ix, iy).G;
                        int ib = bmWaterMark.GetPixel(ix, iy).B;

                        if (!(ir == 0 && ig == 0 && ib == 0))
                        {
                            picture.DrawEllipse(new Pen(new SolidBrush(Color.FromArgb(alphaRate, ir, ig, ib))), xpos + ix, ypos + iy, 1, 1);
                        }
                    }
                }

                waterMark.Dispose();

                b.Save(imagePath);
                b.Dispose();
                image.Dispose();

            }
            catch
            {
                // ignored
            }
        }

        public static bool MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, bool isLessSizeNotThumb, out Size originalSize)
        {
            originalSize = new Size();

            if (width == 0 && height == 0)
            {
                FileUtils.CopyFile(originalImagePath, thumbnailPath);
                return true;
            }
            DirectoryUtils.CreateDirectoryIfNotExists(thumbnailPath);
            if (!FileUtils.IsFileExists(originalImagePath)) return false;

            var originalImage = Image.FromFile(originalImagePath);
            originalSize = originalImage.Size;

            if (width == 0)
            {
                if (isLessSizeNotThumb && originalImage.Height < height)
                {
                    FileUtils.CopyFile(originalImagePath, thumbnailPath);
                    return true;
                }
                return MakeThumbnail(originalImage, originalImagePath, thumbnailPath, width, height, "H");
            }
            if (height == 0)
            {
                if (isLessSizeNotThumb && originalImage.Width < width)
                {
                    FileUtils.CopyFile(originalImagePath, thumbnailPath);
                    return true;
                }
                return MakeThumbnail(originalImage, originalImagePath, thumbnailPath, width, height, "W");
            }
            if (isLessSizeNotThumb && originalImage.Height < height && originalImage.Width < width)
            {
                FileUtils.CopyFile(originalImagePath, thumbnailPath);
                return true;
            }
            return MakeThumbnail(originalImage, originalImagePath, thumbnailPath, width, height, "HW");
        }

        public static bool MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, bool isLessSizeNotThumb)
        {
            return MakeThumbnail(originalImagePath, thumbnailPath, width, height, isLessSizeNotThumb, out _);
        }

        private static bool MakeThumbnail(Image originalImage, string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            var created = false;

            if (FileUtils.IsFileExists(originalImagePath))
            {
                var towidth = width;
                var toheight = height;
                var x = 0;
                var y = 0;
                var ow = originalImage.Width;
                var oh = originalImage.Height;
                switch (mode)
                {
                    case "HW":
                        break;
                    case "W":
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H":
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "Cut":
                        if ((double)originalImage.Width / originalImage.Height > towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        break;
                }
                Image bitmap = new Bitmap(towidth, toheight);
                var g = Graphics.FromImage(bitmap);
                g.InterpolationMode = InterpolationMode.Default;
                g.SmoothingMode = SmoothingMode.Default;
                g.Clear(Color.Transparent);
                g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);
                try
                {
                    bitmap.Save(thumbnailPath, GetImageFormat(originalImagePath));
                    created = true;
                }
                catch
                {
                    FileUtils.CopyFile(originalImagePath, thumbnailPath);
                    created = true;
                }
                finally
                {
                    originalImage.Dispose(); bitmap.Dispose(); g.Dispose();
                }
            }

            return created;
        }
    }
}
