using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using AbroadConcepts.IO;
using NuGet.Frameworks;

namespace TestAbroadConcept.IO;
[ExcludeFromCodeCoverage]
public class TestZipArchiver
{
    [Fact]
    public void TestCanConstructZipArchiver()
    {
        using var stream = new MemoryStream();
        using var zip = new ZipArchiver(stream);
    }
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
    public void TestExtractFilesToZipArchiver()
    {
        using var stream = new MemoryStream();
        using var zip = new ZipArchiver(stream);
        zip.Add("*host.dll");
        var items = zip.GetEntries();
        Assert.Single(items);
        zip.Extract("Test", true, "*host.dll");
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