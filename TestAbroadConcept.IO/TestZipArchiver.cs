using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using AbroadConcepts.IO;
using NuGet.Frameworks;
using Xunit.Abstractions;

namespace TestAbroadConcept.IO;
[ExcludeFromCodeCoverage]
public class TestZipArchiver
{
    [Fact]
    public void TestAddDirectoryToZipArchiver()
    {
        using var stream = new MemoryStream();
        using var zip = new ZipArchiver(stream);
        zip.Add("dir1");
        var items = zip.GetEntries();
        Assert.Equal(4, items.Count);

    }
    [Fact]
    public void TestAddFilesToZipArchiver()
    {
        using var stream = new MemoryStream();
        using var zip = new ZipArchiver(stream);
        zip.Add("*host.dll");
        var items = zip.GetEntries();
        Assert.Single(items);
    }
    [Fact]
    public void TestAddFilesWithOverwriteToZipArchiver()
    {
        using var stream = new MemoryStream();
        using var zip = new ZipArchiver(stream);
        zip.Add("*host.dll");
        zip.Add("*host.dll", overwrite: true);
        var items = zip.GetEntries();
        Assert.Single(items);
    }
    [Fact]
    public void TestAddFilesWithOverwriteToZipArchiverCallBack()
    {
        int addCount = 0;
        using var stream = new MemoryStream();
        using var zip = new ZipArchiver(stream);
        zip.Add("*host.dll");
        zip.Add("*host.dll", overwrite: true, callback: (file, status) => {
            addCount++;
            Console.WriteLine($"Added {file} {status}");
        });
        var items = zip.GetEntries();
        Assert.Equal(1, addCount);
        Assert.Single(items);
    }
    [Fact]
    public void TestExtractFilesToZipArchiver()
    {
        using var stream = new MemoryStream();
        using var zip = new ZipArchiver(stream);
        zip.Add("*host.dll");
        var items = zip.GetEntries();
        Assert.Single(items);
        zip.Extract("Test", true, "*host.dll");
        Assert.Single(items);
    }
    [Fact]
    public void TestExtractFilesToZipArchiverWithCallBack()
    {
        int extractCount = 0;
        using var stream = new MemoryStream();
        using var zip = new ZipArchiver(stream);
        zip.Add("*host.dll");
        var items = zip.GetEntries();
        Assert.Single(items);
        zip.Extract("Test", true, "*host.dll", (File, status) =>
        {
            extractCount++;
            Console.WriteLine($"Extracted {File} {status}");
        });
        Assert.Equal(1, extractCount);
        Assert.Single(items);
    }
    [Fact]
    public void TestRemoveFilesToZipArchiver()
    {
        using var stream = new MemoryStream();
        using var zip = new ZipArchiver(stream);
        zip.Add("*host.dll");
        var items = zip.GetEntries();
        Assert.Single(items);
        zip.Remove("*host.dll");
        items = zip.GetEntries();
        Assert.Empty(items);
    }
    [Fact]
    public void TestRemoveFilesToZipArchiverWithCallback()
    {
        int removeCount = 0;
        using var stream = new MemoryStream();
        using var zip = new ZipArchiver(stream);
        zip.Add("*host.dll");
        var items = zip.GetEntries();
        Assert.Single(items);
        zip.Remove("*host.dll", (file,status) =>
        {
            removeCount++;
            Console.WriteLine($"Removed {file} {status}");
        });
        items = zip.GetEntries();
        Assert.Equal(1, removeCount);
        Assert.Empty(items);
    }
    [Fact]
    public void TestSaveDirectoryToZipArchiver()
    {
        using var stream = new MemoryStream();
        using var zip = new ZipArchiver(stream);
        zip.Add("dir1");
        var items = zip.GetEntries();
        Assert.Equal(4, items.Count);
        zip.Save(stream);
        using var zip2 = new ZipArchiver(stream);
        var items2 = zip.GetEntries();
        Assert.Equal(4, items2.Count);
    }

}