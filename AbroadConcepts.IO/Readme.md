# ==AbroadConcepts.IO==

The ==AbroadConcepts.IO== package provides some functionality for working with files.   In particular, the ==FileExtension== class has an extension method called ==GetFiles== which will resolve names with wild card characters in the file pattern provided. 

## Classes
{: style="color: Cyan;  opacity: 0.8;" }

```
public static class FileExtension    
{
    public static List<string> GetFiles(this string filePattern, bool includeDirectories = false, bool createFile = false);  // Get filenames 
    public static string EnsureExtension(this string file, string extension);   // adds the extension to the filename passed in. 
    public static bool IsDirectory(this string path)
}

public class ZipArchiver : IDisposable
{
    ZipArchiver(Stream stream);
    public void Dispose();
    public void Add(string filePattern, bool overwrite = false, CompressionLevel compression = CompressionLevel.NoCompression, Action<string,string> callback = null!);
    public void Extract(string dir, bool overwrite = true, string ? pattern = default, Action<string,string> callback = null!);
    public List<ZipArchiveEntry> GetEntries(string? pattern = default);
    public void Remove(string? pattern = default, Action<string,string> callback = null!);
    public void Save(Stream stream);
}
```

# Usage
{: style="color: Lime; opacity: 0.80;" }
```

using AbroadConcepts.IO;

```
## Example Code C#

Adding a directory to zip file:

{: style="color: Lime; opacity: 0.80;" }
```
    public void AddDirectoryToZipArchiver()
    {
        using var stream = new MemoryStream();
        using var zip = new ZipArchiver(stream);
        zip.Add("dir1");
    }

```



