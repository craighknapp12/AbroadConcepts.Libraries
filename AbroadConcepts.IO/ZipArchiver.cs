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
    private readonly Stream _stream;
    private Zip? _zipArchive;
    private bool _isDisposed;

    public ZipArchiver(Stream stream)
    {
        _stream = stream;
        _zipArchive = new Zip(_stream, ZipArchiveMode.Update, true);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            _isDisposed = true;
            if (_zipArchive != null)
            {
                _zipArchive.Dispose();
                _zipArchive = null;
            }
        }
    }

    public void Add(string filePattern, int entryLevel = 0, bool overwrite = false, CompressionLevel compression = CompressionLevel.NoCompression, Action<string, string> callback = null!)
    {
        var fileFinder = new FileFinder(true, false);
        var files = fileFinder.GetFiles(filePattern);
        foreach (var filename in files)
        {
            try
            {
                var offset = fileFinder.GetEntryOffset(filename);
                string entryName = GetEntryName(filename, entryLevel + offset);
                if (overwrite)
                {
                    Remove(entryName);
                }

                AddInternal(filename, entryName, compression);
                callback?.Invoke(filename, "OK");
            }
            catch (Exception e)
            {
                callback?.Invoke(filename, e.Message);
            }
        }
    }

    public void Extract(string dir, bool overwrite = true, string ? pattern = default, Action<string, string> callback = null!)
    {
        var zipEntries = GetEntries(pattern);

        if (string.IsNullOrEmpty(dir))
        {
            dir = Directory.GetCurrentDirectory();
        }
        foreach (var zipEntry in zipEntries)
        {
            var filename = dir + Path.DirectorySeparatorChar + zipEntry.FullName;
            try
            {
                var path = Path.GetDirectoryName(filename);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path!);
                }
                zipEntry.ExtractToFile(filename, overwrite);
                callback?.Invoke(filename, "OK");
            }
            catch (Exception ex)
            {
                callback?.Invoke(filename, ex.Message);
            }
        }

    }

    public List<ZipArchiveEntry> GetEntries(string? pattern = default)
    {
        var regPattern = pattern?.Replace("*", ".*");
        Regex reg = new Regex(regPattern ?? "");

        return (from e in _zipArchive?.Entries where reg.IsMatch(e.FullName) orderby e.FullName descending select e).ToList();
    }

    public void Remove(string? pattern = default, Action<string, string> callback = null!)
    {
        var zipEntries = GetEntries(pattern);
        foreach (var zipEntry in zipEntries)
        {
            try
            {
                zipEntry.Delete();
                callback?.Invoke(zipEntry.FullName, "OK");
            }
            catch (Exception e)
            {
                callback?.Invoke(zipEntry.FullName, e.Message);
            }
        }
    }

    public void Save(Stream stream)
    {
        _stream!.Position = 0;
        _stream!.CopyTo(stream);
    }

    private void AddInternal(string filename, string entryName, CompressionLevel compression = CompressionLevel.NoCompression)
    {
        if (filename.IsDirectory())
        {
            var items = GetEntries(entryName);
            if (!(from e in items where e.FullName.StartsWith(entryName) select e.FullName).Any() )
            {
                _zipArchive?.CreateEntry(entryName +"/", compression);
            }
        }
        else
        {
            _zipArchive?.CreateEntryFromFile(filename, entryName, compression);
        }
    }

    private static string GetEntryName(string filename, int entryLevel = 0)
    {
        var filenameSplit = filename.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        var entryParts = new List<string>();
        
        var start = filenameSplit.Length - entryLevel;
        if (start < 0)
        {
            start = 0;
        }

        for (var i = start; i < filenameSplit.Length; i++)
        {
            entryParts.Add(filenameSplit[i]);
        }

        return string.Join("/", entryParts);
    }

}
