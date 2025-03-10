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

    [Fact]
    public void Test_GetFileKnownDirectoryWithColon()
    {
        var files = "C:Windows\\winhlp32".EnsureExtension(".exe").GetFiles();
        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
    }

    [Fact]
    public void Test_GetFileKnownDirectoryWithColonSlash()
    {
        var files = "C:\\Windows\\winhlp32".EnsureExtension(".exe").GetFiles();
        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
    }

    [Fact]
    public void Test_GetFilesMultiplePatternInFrontWithMultipleResult()
    {
        var files = "d?r*\\s*dir".GetFiles(true);
        Assert.Equal(5, files.Count());
    }

    [Fact]
    public void Test_GetFilesMultiplePatternInFrontWithSpecificFileSelection()
    {
        var files = "d?r*\\s*dir\\*st*".GetFiles();
        Assert.Equal(3, files.Count());
    }

    [Fact]
    public void Test_GetFileQuestionDirectory()
    {
        var files = "?\\Windows\\winhlp32".EnsureExtension(".exe").GetFiles();
        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
    }

    [Fact]
    public void Test_GetFileQuestionDirectoryWithColon()
    {
        var files = "?:\\Windows\\winhlp32".EnsureExtension(".exe").GetFiles();
        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
    }

    [Fact]
    public void Test_GetFilesSelectingDirectory1()
    {
        var files = "d?r1\\s*dir".GetFiles(true);
        Assert.Equal(2, files.Count());
    }

    [Fact]
    public void Test_GetFilesSelectingDirectory1DoesNotIncludeDirectory()
    {
        var files = "d?r1\\s*dir".GetFiles();
        Assert.Single(files);
    }


    [Fact]
    public void Test_GetFilesSelectingDirectory3()
    {
        var files = "d?r3\\s*dir".GetFiles(true);
        Assert.Equal(3, files.Count());
    }

    [Fact]
    public void Test_GetFilesSingleWildPatternInFrontWith1Result()
    {
        var files = "di?2".GetFiles(true);
        Assert.Single(files);
    }

    [Fact]
    public void Test_GetFilesSingleWildPatternInFrontWithMultipleResult()
    {
        var files = "di?3".GetFiles(true);
        Assert.Equal(4, files.Count());
    }
    
    [Fact]
    public void Test_GetFilesWildCardDirectory()
    {
        var files = "*\\Windows\\winhlp32".EnsureExtension(".exe").GetFiles();
        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
    }

    [Fact]
    public void Test_GetFileWildCardDirectoryWithColon()
    {
        var files = "*:\\Windows\\winhlp32".EnsureExtension(".exe").GetFiles();
        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
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