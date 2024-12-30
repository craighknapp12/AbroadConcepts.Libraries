using System.Diagnostics.CodeAnalysis;
using AbroadConcepts.IO;

namespace TestAbroadConcept.IO;

[ExcludeFromCodeCoverage]
public class FileExtensionTests : IClassFixture<TestsFixture>
{
    [Fact]
    public void Test_GetFileExtension()
    {
        var file = "di?3\\s*dir*\\test".EnsureExtension(".txt").GetFiles().FirstOrDefault();
        Assert.Contains("\\dir3\\subdir\\test.txt", file);
    }
    [Fact]
    public void Test_GetFileExtensionMissing()
    {
        var file = "di?3\\s*dir*\\missing".EnsureExtension(".txt").GetFiles().FirstOrDefault();
        Assert.Null(file);
    }
    [Fact]
    public void Test_GetFiles_SingleWildPatternInFrontWith1Result()
    {
        var files = "di?2".GetFiles(true);
        Assert.Single(files);
    }
    [Fact]
    public void Test_GetFiles_SingleWildPatternInFrontWithMultipleResult()
    {
        var files = "di?3".GetFiles(true);
        Assert.Equal(4, files.Count);
    }
    [Fact]
    public void Test_GetFiles_MultiplePatternInFrontWithMultipleResult()
    {
        var files = "d?r*\\s*dir".GetFiles(true);
        Assert.Equal(5, files.Count);
    }
    [Fact]
    public void Test_GetFiles_MultiplePatternInFrontWithSpecificFileSelection()
    {
        var files = "d?r*\\s*dir\\*st*".GetFiles();
        Assert.Equal(3, files.Count);
    }
    [Fact]
    public void Test_GetFiles_SelectingDirectory1DonotIncludeDirectory()
    {
        var files = "d?r1\\s*dir".GetFiles();
        Assert.Single(files);
    }
    [Fact]
    public void Test_GetFiles_SelectingDirectory1()
    {
        var files = "d?r1\\s*dir".GetFiles(true);
        Assert.Equal(2, files.Count);
    }
    [Fact]
    public void Test_GetFiles_SelectingDirectory3()
    {
        var files = "d?r3\\s*dir".GetFiles(true);
        Assert.Equal(3, files.Count);
    }
    [Fact]
    public void Test_IsDirectory()
    {
        var isDirectory = "C:\\Windows".IsDirectory();
        Assert.True(isDirectory);
    }

}