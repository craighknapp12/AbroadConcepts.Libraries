using System.Diagnostics.CodeAnalysis;
using AbroadConcepts.IO;

namespace TestAbroadConcept.IO;

[ExcludeFromCodeCoverage]
public class FileExtensionTests : IClassFixture<TestsFixture>
{

    [Fact]
    public void Test_EnsureExtension()
    {
        var dir = Directory.GetCurrentDirectory();
        var files = "di?3\\s*dir*\\test".EnsureExtension(".txt").GetFiles();

        Assert.Contains($"{dir}\\dir3\\subdir\\test.txt", (from f in files where f.Contains("dir3") select f));
    }

    [Fact]
    public void Test_EnsureExtensionMissing()
    {
        var file = "di?3\\s*dir*\\missing".EnsureExtension(".txt").GetFiles().FirstOrDefault();
        Assert.Null(file);
    }


    [Fact]
    public void Test_EnsureExtensionMissingButCreate()
    {
        var file = "di?3\\s*dir*\\test2".EnsureExtension(".txt").GetFiles(false, true).FirstOrDefault();
        Assert.Contains("\\dir3\\subdir\\test2.txt", file);
    }


    [Fact]
    public void Test_EnsureExtensionWithAsterisk()
    {
        var dir = Directory.GetCurrentDirectory();
        var files = "*\\s*dir*\\test".EnsureExtension(".txt").GetFiles();
        Assert.Contains($"{dir}\\dir1\\subdir\\test.txt", (from f in files where f.Contains("dir1") select f));
        Assert.Contains($"{dir}\\dir3\\subdir\\test.txt", (from f in files where f.Contains("dir3") select f));
    }

    [Theory]
    [InlineData("d?r*\\s*dir", 5)]
    [InlineData("d?r*\\s*dir\\*st*", 3)]
    [InlineData("d?r1\\s*dir", 2)]
    [InlineData("d?r3\\s*dir", 3)]
    [InlineData("di?2", 1)]
    [InlineData("di?3", 4)]
    public void CheckGetFilesCounts(string directory, int expected)
    {
        var files = directory.GetFiles(includeDirectories: true);
        Assert.Equal(expected, files.Count());
    }

    [Theory]
    [InlineData("d?r*\\s*dir", 5)]
    [InlineData("d?r*\\s*dir\\*st*", 3)]
    [InlineData("d?r1\\s*dir", 2)]
    [InlineData("d?r3\\s*dir", 3)]
    [InlineData("di?2", 1)]
    [InlineData("di?3", 4)]
    public async Task CheckGetFilesAsyncCounts(string directory, int expected)
    {
        var filenames = new List<string>();
        using var cs = new CancellationTokenSource();
        var iFile = directory.GetFilesAsync( cs.Token, true);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }
        Assert.Equal(expected, filenames.Count());
    }

    [Theory]
    [InlineData("C:Windows\\winhlp32", "C:\\Windows\\winhlp32.exe")]
    [InlineData("C:\\Windows\\winhlp32", "C:\\Windows\\winhlp32.exe")]
    [InlineData("*\\Windows\\winhlp32", "C:\\Windows\\winhlp32.exe")]
    [InlineData("?\\Windows\\winhlp32", "C:\\Windows\\winhlp32.exe")]
    [InlineData("*:\\Windows\\winhlp32", "C:\\Windows\\winhlp32.exe")]
    [InlineData("?:\\Windows\\winhlp32", "C:\\Windows\\winhlp32.exe")]
    public void CheckGetFileWithExtension(string directory, string expected)
    {
        var file = directory.EnsureExtension(".exe");
        var files = file.GetFiles();
        Assert.Contains(expected, (from f in files where f.Contains("Windows") select f));

    }

    [Theory]
    [InlineData("C:Windows\\winhlp32", "C:\\Windows\\winhlp32.exe")]
    [InlineData("C:\\Windows\\winhlp32", "C:\\Windows\\winhlp32.exe")]
    [InlineData("*\\Windows\\winhlp32", "C:\\Windows\\winhlp32.exe")]
    [InlineData("?\\Windows\\winhlp32", "C:\\Windows\\winhlp32.exe")]
    [InlineData("*:\\Windows\\winhlp32", "C:\\Windows\\winhlp32.exe")]
    [InlineData("?:\\Windows\\winhlp32", "C:\\Windows\\winhlp32.exe")]
    public async Task CheckGetFileAsyncWithExtension(string directory, string expected)
    {
        using var cs = new CancellationTokenSource();
        var filenames = new List<string>();
        var file = directory.EnsureExtension(".exe");
        var iFile = file.GetFilesAsync(cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }

        Assert.Contains(expected, (from f in filenames where f.Contains("Windows") select f));

    }

    [Fact]
    public void Test_IsDirectory()
    {
        var isDirectory = "C:\\Windows".IsDirectory();
        Assert.True(isDirectory);
    }
    
    [Fact]
    public void Test_IsFile()
    {
        var isDirectory = "C:\\Windows\\System32\\drivers\\etc\\hosts".IsDirectory();
        Assert.True(!isDirectory);
    }
    
    [Fact]
    public void Test_IsNotDirectoryOrFile()
    {

        var isDirectoryOrFile = "C:\\WinTestDir".IsDirectory();
        Assert.True(!isDirectoryOrFile);

    }

}