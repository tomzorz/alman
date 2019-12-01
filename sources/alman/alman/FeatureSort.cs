using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alman
{
    public static class FeatureSort
    {
        public static async Task Sort()
        {
            var currentFolder = Environment.CurrentDirectory;

            var files = Directory.GetFiles(currentFolder).Select(x => new FileInfo(x)).ToList();

            SortHelpers.EnsureFolders();

            await SortHelpers.EnsureFfmpeg();

            // raws

            var raws = files.Where(x => x.Extension.ToUpper() == Constants.EXT_DNG).ToList(); // final

            var rawNames = raws.Select(x => x.JustTheName()).ToList();

            foreach (var fileInfo in raws) SortHelpers.TryMove(fileInfo, Constants.FOLDER_RAW);

            // saved

            var saved = files.Where(x => x.Extension.ToUpper() == Constants.EXT_JPG && !x.Name.Contains(Constants.EDIT_INDICATOR) && !rawNames.Contains(x.JustTheName())).ToList(); // final

            foreach (var fileInfo in saved) SortHelpers.TryMove(fileInfo, Constants.FOLDER_SAVED);

            // edits

            var editResultImages = files.Where(x => x.Extension.ToUpper() == Constants.EXT_JPG && x.Name.Contains(Constants.IMAGE_EDIT)).ToList(); // final
            var editResultVideos = files.Where(x => x.Extension.ToUpper() == Constants.EXT_MOV && x.Name.Contains(Constants.IMAGE_EDIT)).ToList(); // final

            var erioNames = editResultImages.Select(x => x.JustTheName().RemoveE()).ToList();
            var ervoNames = editResultVideos.Select(x => x.JustTheName().RemoveE()).ToList();
            var originalImages = files.Where(x => x.Extension.ToUpper() == Constants.EXT_HEIC && erioNames.Any(y => x.Name.Contains(y))).ToList(); // final
            var originalImageEdits = files.Where(x => x.Extension.ToUpper() == Constants.EXT_AAE && erioNames.Any(y => x.Name.Contains(y))).ToList(); // final
            var originalVideos = files.Where(x => x.Extension.ToUpper() == Constants.EXT_MOV && ervoNames.Any(y => x.Name.Contains(y))).ToList();
            var (originalNormalVideos, originalSlomoVideos) = SortHelpers.SplitSlomo(originalVideos); // final
            var originalVideoEdits = files.Where(x => x.Extension.ToUpper() == Constants.EXT_AAE && ervoNames.Any(y => x.Name.Contains(y))).ToList(); // final

            foreach (var fileInfo in originalImages) SortHelpers.TryMove(fileInfo, Constants.FOLDER_ORIGINALS);
            foreach (var fileInfo in originalImageEdits) SortHelpers.TryMove(fileInfo, Constants.FOLDER_ORIGINALS);
            foreach (var fileInfo in originalNormalVideos) SortHelpers.TryMove(fileInfo, Constants.FOLDER_ORIGINALS);
            foreach (var fileInfo in originalSlomoVideos) SortHelpers.TryMove(fileInfo, Constants.FOLDER_SLOMO);
            foreach (var fileInfo in originalVideoEdits) SortHelpers.TryMove(fileInfo, Constants.FOLDER_ORIGINALS);

            foreach (var editResultImage in editResultImages)
            {
                File.Move(editResultImage.Name, editResultImage.Name.RemoveE());
            }
            foreach (var editResultVideo in editResultVideos)
            {
                File.Move(editResultVideo.Name, editResultVideo.Name.RemoveE());
            }

            // screenshots

            var screenshots = files.Where(x => x.Extension.ToUpper() == Constants.EXT_PNG && !x.Name.Contains(Constants.EDIT_INDICATOR)).ToList(); // final
            var screencaps = files.Where(x => x.Extension.ToUpper() == Constants.EXT_MP4 && !x.Name.Contains(Constants.IMAGE_NOEDIT)).ToList(); // final

            foreach (var fileInfo in screenshots) SortHelpers.TryMove(fileInfo, Constants.FOLDER_SCREENSHOTS);
            foreach (var fileInfo in screencaps) SortHelpers.TryMove(fileInfo, Constants.FOLDER_SCREENSHOTS);

            // random

            var randomVideos = files.Where(x => x.Extension.ToUpper() == Constants.EXT_MOV && !x.Name.Contains(Constants.IMAGE_NOEDIT)).ToList(); // final
            var randomImages = files.Where(x => x.Extension.ToUpper() == Constants.EXT_JPG && !x.Name.Contains(Constants.IMAGE_NOEDIT)).ToList(); // final

            foreach (var fileInfo in randomImages) SortHelpers.TryMove(fileInfo, Constants.FOLDER_RANDOMIMAGES);
            foreach (var fileInfo in randomVideos) SortHelpers.TryMove(fileInfo, Constants.FOLDER_RANDOMVIDEOS);

            // non-rendered edits

            var remainingFiles1 = Directory.GetFiles(currentFolder).Select(x => new FileInfo(x)).ToList();

            var nrEdits = remainingFiles1.Where(x => x.Extension.ToUpper() == Constants.EXT_AAE && x.Name.Contains(Constants.IMAGE_NOEDIT)).ToList(); // final
            var nrOriginals = nrEdits.Select(x => new FileInfo(x.JustTheName() + Constants.EXT_MOV)).ToList();
            var (nrNormalOriginals, nrSlomoOriginals) = SortHelpers.SplitSlomo(nrOriginals); // final

            foreach (var fileInfo in nrEdits) SortHelpers.TryMove(fileInfo, Constants.FOLDER_ORIGINALS);
            foreach (var fileInfo in nrNormalOriginals) SortHelpers.TryMove(fileInfo, Constants.FOLDER_ORIGINALS);
            foreach (var fileInfo in nrSlomoOriginals) SortHelpers.TryMove(fileInfo, Constants.FOLDER_SLOMO);

            // leftovers

            var remainingFiles2 = Directory.GetFiles(currentFolder).Select(x => new FileInfo(x)).ToList();

            var sFiles = remainingFiles2.Where(x => x.Name.Contains(Constants.IMAGE_S)).ToList(); // final
            var aaeFiles = remainingFiles2.Where(x => x.Extension == Constants.EXT_AAE).ToList(); // final

            foreach (var fileInfo in sFiles) SortHelpers.TryMove(fileInfo, Constants.FOLDER_OTHER);
            foreach (var fileInfo in aaeFiles) SortHelpers.TryMove(fileInfo, Constants.FOLDER_OTHER);

            // cleanup

            Directory.Delete("ffmpeg", true);
        }
    }
}
