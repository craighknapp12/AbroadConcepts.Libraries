# AbroadConcepts.IO

This package provides extension methods and class supporting IO operations.

The current supported version is 1.0.1.

# Description

This package provides some functionality for processing files.


## Classes
```
public static class FileExtension    
{
    public static List<string> GetFiles(this string filePattern);  // Get filenames 
    public static string EnsureExtension(this string file, string extension);   // adds the extension to the filename passed in. 
    public static string GetFileWithExtension(this string pattern, string extension)
    public static bool IsDirectory(this string path)

}
public class ZipArchiver : IDisposable
{
    ZipArchiver(Stream stream);
    public void Dispose();
    public void Add(string filePattern, bool overwrite = false, CompressionLevel compression = CompressionLevel.NoCompression);
    public void Extract(string dir, bool overwrite = true, string ? pattern = default);
    public List<ZipArchiveEntry> GetEntries(string? pattern = default);
    public void Remove(string? pattern = default);
    public void Save(Stream stream);
}
```

## Example Code C#

Adding a directory to zip file:
```
    public void AddDirectoryToZipArchiver()
    {
        using var stream = new MemoryStream();
        using var zip = new ZipArchiver(stream);
        zip.Add("dir1");
        var items = zip.GetEntries();
    }

```


