using System.Diagnostics.CodeAnalysis;
using AbroadConcepts.IO;

namespace TestAbroadConcept.IO;

[ExcludeFromCodeCoverage]
public class FileExtensionTests : IClassFixture<TestsFixture>
{
    [Fact]
    public void TestGetFileExtension()
    {
        var file = "di?3\\s*dir*\\test".EnsureExtension(".txt").GetFiles().FirstOrDefault();
        Assert.Contains("\\dir3\\subdir\\test.txt", file);
    }
    [Fact]
    public void TestGetFileExtensionMissingButCreate()
    {
        var file = "di?3\\s*dir*\\test2".EnsureExtension(".txt").GetFiles(false,true).FirstOrDefault();
        Assert.Contains("\\dir3\\subdir\\test2.txt", file);
    }
    [Fact]
    public void TestGetFileExtensionMissing()
    {
        var file = "di?3\\s*dir*\\missing".EnsureExtension(".txt").GetFiles().FirstOrDefault();
        Assert.Null(file);
    }
    [Fact]
    public void TestGetFiles_SingleWildPatternInFrontWith1Result()
    {
        var files = "di?2".GetFiles(true);
        Assert.Single(files);
    }
    [Fact]
    public void TestGetFiles_SingleWildPatternInFrontWithMultipleResult()
    {
        var files = "di?3".GetFiles(true);
        Assert.Equal(4, files.Count);
    }
    [Fact]
    public void TestGetFiles_MultiplePatternInFrontWithMultipleResult()
    {
        var files = "d?r*\\s*dir".GetFiles(true);
        Assert.Equal(5, files.Count);
    }
    [Fact]
    public void TestGetFiles_MultiplePatternInFrontWithSpecificFileSelection()
    {
        var files = "d?r*\\s*dir\\*st*".GetFiles();
        Assert.Equal(3, files.Count);
    }
    [Fact]
    public void TestGetFiles_SelectingDirectory1DoesNotIncludeDirectory()
    {
        var files = "d?r1\\s*dir".GetFiles();
        Assert.Single(files);
    }
    [Fact]
    public void TestGetFiles_SelectingDirectory1()
    {
        var files = "d?r1\\s*dir".GetFiles(true);
        Assert.Equal(2, files.Count);
    }
    [Fact]
    public void TestGetFiles_SelectingDirectory3()
    {
        var files = "d?r3\\s*dir".GetFiles(true);
        Assert.Equal(3, files.Count);
    }
    [Fact]
    public void TestIsDirectory()
    {
        var isDirectory = "C:\\Windows".IsDirectory();
        Assert.True(isDirectory);
    }
    [Fact]
    public void TestIsFile()
    {
        var isDirectory = "C:\\Windows\\System32\\drivers\\etc\\hosts".IsDirectory();
        Assert.True(!isDirectory);
    }
    [Fact]
    public void TestIsNotDirectoryOrFile()
    {

        try
        {
            var isDirectory = "C:\\WinTestDir".IsDirectory();

        }
        catch (Exception)
        {

            return;
        }

        Assert.False(1==0);
    }

}