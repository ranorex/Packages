using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Applitools.Images;

namespace Applitools.ImageTester.TestObjects
{
    public class ImageStep : TestUnit
    {
        private Bitmap img_;

        public static string[] SupportedExtensionsWithSeparator { get; } = new[] { ".jpg", ".png",".gif", ".bmp" };

        public ImageStep(FileStream file) : base(file)
        {
        }

        public override void Run(Eyes eyes)
        {
            try
            {
                eyes.CheckImage(GetImage(), Name());
            }
            catch (IOException e)
            {
                Console.WriteLine("Failed to process image file: {0} \\n Reason: {1} \\n", file.Name, e.Message);
                throw;
            }
        }

        public virtual Bitmap GetImage()
        {
            if (img_ == null)
            {
                img_ = new Bitmap(file);
            }

            return img_;
        }

        public override string Name()
        {
            return Path.GetFileNameWithoutExtension(base.Name());
        }

        public static bool Supports(FileStream file)
        {
            return SupportedExtensionsWithSeparator
                .AsParallel()
                .Any(e => file.Name.EndsWith(e));
        }

        public override void Dispose()
        {
            img_?.Dispose();
        }
    }
}