using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbroadConcepts.IO;

namespace TestAbroadConcept.IO;
[ExcludeFromCodeCoverage]
public class FileFinderTests
{
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
        var fileFinder = new FileFinder();
        var files = fileFinder.GetFiles(file);

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
        var fileFinder = new FileFinder();
        var iFile = fileFinder.GetFilesAsync(file, cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }

        var actualFile = (from f in filenames where f.Contains("Windows") select f).First();
        int count = fileFinder.GetEntryOffset(actualFile);


        Assert.Contains(expected, actualFile);
        Assert.Equal(1, count);
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
        var fileFinder = new FileFinder(true);
        var files = fileFinder.GetFiles(directory);
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
        var fileFinder = new FileFinder(true);
        var iFile = fileFinder.GetFilesAsync(directory, cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }
        Assert.Equal(expected, filenames.Count());
    }

}
