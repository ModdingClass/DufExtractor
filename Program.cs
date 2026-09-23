using System;
using System.IO;
using System.IO.Compression;

internal static class Program
{
    private static readonly byte[] GzipMagic = { 0x1F, 0x8B };

    [STAThread]
    private static int Main(string[] args)
    {
        System.Windows.Forms.Application.SetHighDpiMode(System.Windows.Forms.HighDpiMode.SystemAware);
        System.Windows.Forms.Application.EnableVisualStyles();

        if (args.Length == 0)
        {
            Toast.Show("DUF Extractor", "Usage: DufExtractor.exe <file.duf> [...]", ToastKind.Warning);
            return 1;
        }

        int failures = 0;
        foreach (var path in args)
        {
            try
            {
                ProcessFile(path);
            }
            catch (Exception ex)
            {
                failures++;
                Toast.Show("DUF Extractor - Failed", $"{Path.GetFileName(path)}\n{ex.Message}", ToastKind.Error);
            }
        }

        return failures == 0 ? 0 : 1;
    }

    private static void ProcessFile(string path)
    {
        if (!File.Exists(path))
        {
            Toast.Show("DUF Extractor - Not found", Path.GetFileName(path), ToastKind.Error);
            return;
        }

        byte[] header = ReadHeader(path, 2);

        if (header.Length >= 2 && header[0] == GzipMagic[0] && header[1] == GzipMagic[1])
        {
            DecompressInPlace(path);
            return;
        }

        if (LooksLikeJson(path))
        {
            Toast.Show("DUF Extractor", $"Already plain JSON:\n{Path.GetFileName(path)}", ToastKind.Info);
            return;
        }

        Toast.Show("DUF Extractor - Unrecognized", $"Neither gzip nor JSON:\n{Path.GetFileName(path)}", ToastKind.Warning);
    }

    private static byte[] ReadHeader(string path, int count)
    {
        using var fs = File.OpenRead(path);
        var buffer = new byte[count];
        int read = fs.Read(buffer, 0, count);
        if (read < count) Array.Resize(ref buffer, read);
        return buffer;
    }

    private static bool LooksLikeJson(string path)
    {
        using var fs = File.OpenRead(path);
        using var sr = new StreamReader(fs, System.Text.Encoding.UTF8, true);
        int ch;
        while ((ch = sr.Read()) != -1)
        {
            if (char.IsWhiteSpace((char)ch)) continue;
            return ch == '{' || ch == '[';
        }
        return false;
    }

    private static void DecompressInPlace(string path)
    {
        string tempPath = path + ".tmp_decompress";

        using (var input = File.OpenRead(path))
        using (var gzip = new GZipStream(input, CompressionMode.Decompress))
        using (var output = File.Create(tempPath))
        {
            gzip.CopyTo(output);
        }

        File.Delete(path);
        File.Move(tempPath, path);

        Toast.Show("DUF Extractor", $"Decompressed:\n{Path.GetFileName(path)}", ToastKind.Success);
    }
}
