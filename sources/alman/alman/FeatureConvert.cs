using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace alman
{
    public static class FeatureConvert
    {
        public static async Task Convert()
        {
            var currentFolder = Environment.CurrentDirectory;

            if (!Directory.Exists(Constants.FOLDER_CONVERTED)) Directory.CreateDirectory(Constants.FOLDER_CONVERTED);

            var files = Directory.GetFiles(currentFolder).Where(y => y.EndsWith("heic", true, CultureInfo.InvariantCulture)).Select(x => new FileInfo(x)).ToList();

            foreach (var file in files)
            {
                using var image = new MagickImage(file) {Format = MagickFormat.Jpg, Quality = 100};
                var fp = $"{Constants.FOLDER_CONVERTED}\\{file.JustTheName()}.jpg";
                image.Write(new FileInfo(fp));
            }
        }
    }
}
