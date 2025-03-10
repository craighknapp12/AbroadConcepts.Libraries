using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zip = System.IO.Compression.ZipArchive;


namespace AbroadConcepts.IO;
public class ZipArchiver : IDisposable
{
    private Stream _stream;
    private Zip? _zipArchive;

    public ZipArchiver(Stream stream)
    {
        _stream = stream;
        _zipArchive = new Zip(_stream, ZipArchiveMode.Update, true);
    }

    public void Dispose()
    {
        if (_zipArchive != null)
        {
            _zipArchive.Dispose();
            _zipArchive = null;
        }

        GC.SuppressFinalize(this);
    }

    public void Add(string filePattern, int entryLevel = 1, bool overwrite = false, CompressionLevel compression = CompressionLevel.NoCompression)
    {
        var files = filePattern.GetFiles(true);
        foreach (var filename in files)
        {
            string entryName = GetEntryName(filename, entryLevel);
            if (overwrite)
            {
                Remove(entryName);
            }

            AddInternal(filename, entryName, compression);
        }
    }

    public void Extract(string dir, bool overwrite = true, string ? pattern = default)
    {
        var zipEntries = GetEntries(pattern);

        if (string.IsNullOrEmpty(dir))
        {
            dir = Directory.GetCurrentDirectory();
        }
        foreach (var zipEntry in zipEntries)
        {
            var filename = dir + Path.DirectorySeparatorChar + zipEntry.FullName;
            var path = Path.GetDirectoryName(filename);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path!);
            }

            zipEntry.ExtractToFile(filename, overwrite);
        }

    }

    public List<ZipArchiveEntry> GetEntries(string? pattern = default)
    { 
 
        pattern = !string.IsNullOrEmpty(pattern) ? "^" + pattern + "$" : "^*$";
        Regex reg = new Regex(pattern);
        return (from e in _zipArchive?.Entries where reg.IsMatch(e.FullName) orderby e.FullName select e).ToList();
    }

    public void Remove(string? pattern = default)
    {
        var zipEntries = GetEntries(pattern);
        foreach (var zipEntry in zipEntries)
        {
            zipEntry.Delete();
        }
    }

    public void Save(Stream stream)
    {
        _stream!.Position = 0;
        _stream!.CopyTo(stream);
    }

    private void AddInternal(string filename, string entryName, CompressionLevel compression = CompressionLevel.NoCompression)
    {
        var filePath = Path.GetFullPath(filename);

        if (filePath.IsDirectory())
        {
             _zipArchive?.CreateEntry(entryName);
        }
        else
        {
            _zipArchive?.CreateEntryFromFile(filename, entryName, compression);
        }
    }

    private static string GetEntryName(string filename, int entryLevel = 1)
    {
        var filenameSplit = filename.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        var entryName = new StringBuilder();
        
        var start = filenameSplit.Length - entryLevel;
        if (start < 0)
        {
            start = 0;
        }

        for (var i = start; i < filenameSplit.Length - 1; i++)
        {
            entryName.Append(filenameSplit[i]);
            entryName.Append("/");
        }

        entryName.Append(filenameSplit[filenameSplit.Length - 1]);

        return entryName.ToString();
    }


}
