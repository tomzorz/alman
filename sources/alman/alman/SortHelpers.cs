using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace alman
{
    public static class SortHelpers
    {
        public static async Task EnsureFfmpeg()
        {
            if (File.Exists("ffmpeg\\bin\\ffmpeg.exe") && File.Exists("ffmpeg\\bin\\ffprobe.exe")) return;

            var htc = new HttpClient();
            var stream = await htc.GetStreamAsync("https://ffmpeg.zeranoe.com/builds/win64/shared/ffmpeg-latest-win64-shared.zip");
            await using var fs = File.Create("ffmpeg.zip");
            await stream.CopyToAsync(fs);
            await fs.DisposeAsync();
            ZipFile.ExtractToDirectory("ffmpeg.zip", "ffunzip");
            File.Delete("ffmpeg.zip");
            Directory.Move(Directory.GetDirectories(Environment.CurrentDirectory + "\\ffunzip", "ffmpeg*")[0], "ffmpeg");
            Directory.Delete("ffunzip");
            if (!File.Exists("ffmpeg\\bin\\ffmpeg.exe")) throw new Exception();
            if (!File.Exists("ffmpeg\\bin\\ffprobe.exe")) throw new Exception();
        }

        public static void EnsureFolders()
        {
            Directory.CreateDirectory(Constants.FOLDER_SAVED);
            Directory.CreateDirectory(Constants.FOLDER_RANDOMIMAGES);
            Directory.CreateDirectory(Constants.FOLDER_RANDOMVIDEOS);
            Directory.CreateDirectory(Constants.FOLDER_SCREENSHOTS);
            Directory.CreateDirectory(Constants.FOLDER_RAW);
            Directory.CreateDirectory(Constants.FOLDER_ORIGINALS);
            Directory.CreateDirectory(Constants.FOLDER_SLOMO);
            Directory.CreateDirectory(Constants.FOLDER_OTHER);
        }

        public static void TryMove(FileInfo file, string toFilder)
        {
            file.Refresh();
            if (!file.Exists) return;
            File.Move(file.FullName, file.DirectoryName + "\\" + toFilder + "\\" + file.Name);
        }

        public static string JustTheName(this FileInfo fi) => fi.Name.Replace(fi.Extension, string.Empty);

        public static string RemoveE(this string fi) => fi.Replace(Constants.EDIT_INDICATOR, Constants.NORMAL_INDICATOR);

        public static (List<FileInfo> normal, List<FileInfo> slomo) SplitSlomo(List<FileInfo> incoming)
        {
            var slomo = new List<FileInfo>();
            var normal = new List<FileInfo>();

            foreach (var fileInfo in incoming)
            {
                var sb = new StringBuilder();
                var p = new ProcessStartInfo("ffmpeg\\bin\\ffprobe.exe", "-v 0 -of csv=p=0 -select_streams v:0 -show_entries stream=r_frame_rate " + fileInfo.Name)
                {
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = Environment.CurrentDirectory
                };
                var s = new Process
                {
                    StartInfo = p,
                    EnableRaisingEvents = true,
                };
                s.OutputDataReceived += (sender, args) => sb.Append(args.Data);
                s.ErrorDataReceived += (sender, args) => sb.Append(args.Data);
                s.Start();
                s.BeginErrorReadLine();
                s.BeginOutputReadLine();
                s.WaitForExit();
                var result = sb.ToString();
                if (result.StartsWith("240") || result.StartsWith("120"))
                {
                    slomo.Add(fileInfo);
                }
                else
                {
                    normal.Add(fileInfo);
                }
            }

            return (normal, slomo);
        }
    }
}
