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

    [Fact]
    public void Test_GetFileKnownDirectoryWithColon()
    {
        var file = "C:Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        var files = fileFinder.GetFiles(file);

        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
    }

    [Fact]
    public async Task Test_GetFileKnownDirectoryWithColonAsync()
    {
        var filenames = new List<string>();

        var file = "C:Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();

        var iFile = fileFinder.GetFilesAsync(file, cs.Token);
        await foreach(var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }
        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in filenames where f.Contains("Windows") select f));
    }

    [Fact]
    public void Test_GetFileKnownDirectoryWithColonSlash()
    {
        var file = "C:\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        var files = fileFinder.GetFiles(file);

        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
    }

    [Fact]
    public async Task Test_GetFileKnownDirectoryWithColonSlashAsync()
    {
        var filenames = new List<string>();
        var file = "C:\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();
        var iFile = fileFinder.GetFilesAsync(file, cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }

        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in filenames where f.Contains("Windows") select f));
    }

     [Fact]
    public void Test_GetFilesMultiplePatternInFrontWithMultipleResult()
    {
        var file = "d?r*\\s*dir";
        var fileFinder = new FileFinder(true);
        var files = fileFinder.GetFiles(file);

        Assert.Equal(5, files.Count());
    }

    [Fact]
    public async Task Test_GetFilesMultiplePatternInFrontWithMultipleResultAsync()
    {
        var filenames = new List<string>();

        var file = "d?r*\\s*dir";
        var fileFinder = new FileFinder(true);
        using var cs = new CancellationTokenSource();
        var iFile = fileFinder.GetFilesAsync(file, cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }

        Assert.Equal(5, filenames.Count);
    }

    [Fact]
    public void Test_GetFilesMultiplePatternInFrontWithSpecificFileSelection()
    {
        var file = "d?r*\\s*dir\\*st*";
        var fileFinder = new FileFinder();
        var files = fileFinder.GetFiles(file);

        Assert.Equal(3, files.Count());
    }

    [Fact]
    public async Task Test_GetFilesMultiplePatternInFrontWithSpecificFileSelectionAsync()
    {
        var filenames = new List<string>();
        var file = "d?r*\\s*dir\\*st*";
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();
       
        var iFile = fileFinder.GetFilesAsync(file, cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }


        Assert.Equal(3, filenames.Count);
    }

    [Fact]
    public void Test_GetFileQuestionDirectory()
    {
        var file = "?\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        var files = fileFinder.GetFiles(file);

        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
    }
 
    [Fact]
    public async Task Test_GetFileQuestionDirectoryAsync()
    {
        var filenames = new List<string>();

        var file = "?\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();
        var iFile = fileFinder.GetFilesAsync(file, cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }

        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in filenames where f.Contains("Windows") select f));
    }

    [Fact]
    public void Test_GetFileQuestionDirectoryWithColon()
    {
        var file = "?:\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        var files = fileFinder.GetFiles(file);

        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
    }

    [Fact]
    public async Task Test_GetFileQuestionDirectoryWithColonAsync()
    {
        var filenames = new List<string>();

        var file = "?:\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();
        var iFile = fileFinder.GetFilesAsync(file, cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }

        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in filenames where f.Contains("Windows") select f));
    }

    [Fact]
    public void Test_GetFilesSelectingDirectory1()
    {
        var file = "d?r1\\s*dir";
        var fileFinder = new FileFinder(true);
        var files = fileFinder.GetFiles(file);

        Assert.Equal(2, files.Count());
    }

    [Fact]
    public async Task Test_GetFilesSelectingDirectory1Async()
    {
        var filenames = new List<string>();

        var file = "d?r1\\s*dir";
        var fileFinder = new FileFinder(true);
        using var cs = new CancellationTokenSource();
        var iFile = fileFinder.GetFilesAsync(file, cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }


        Assert.Equal(2, filenames.Count);
    }

    [Fact]
    public void Test_GetFilesSelectingDirectory1DoesNotIncludeDirectory()
    {
        var file = "d?r1\\s*dir";
        var fileFinder = new FileFinder();
        var files = fileFinder.GetFiles(file);

        Assert.Single(files);
    }

    [Fact]
    public async Task Test_GetFilesSelectingDirectory1DoesNotIncludeDirectoryAsync()
    {
        var filenames = new List<string>();
        var file = "d?r1\\s*dir";
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();
        var iFile = fileFinder.GetFilesAsync(file, cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }

        Assert.Single(filenames);
    }


    [Fact]
    public void Test_GetFilesSelectingDirectory3()
    {
        var file = "d?r3\\s*dir";
        var fileFinder = new FileFinder(true);
        var files = fileFinder.GetFiles(file);
        Assert.Equal(3, files.Count());
    }

    [Fact]
    public async Task Test_GetFilesSelectingDirectory3Async()
    {
        var filenames = new List<string>();
        var file = "d?r3\\s*dir";
        var fileFinder = new FileFinder(true);
        using var cs = new CancellationTokenSource();
        var iFile = fileFinder.GetFilesAsync(file, cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }

        Assert.Equal(3, filenames.Count);
    }

    [Fact]
    public void Test_GetFilesSingleWildPatternInFrontWith1Result()
    {
        var file = "di?2";
        var fileFinder = new FileFinder(true);
        var files = fileFinder.GetFiles(file);

        Assert.Single(files);
    }

    [Fact]
    public async Task Test_GetFilesSingleWildPatternInFrontWith1ResultAsync()
    {
        var filenames = new List<string>();
        var file = "di?2";
        var fileFinder = new FileFinder(true);
        using var cs = new CancellationTokenSource();
        var iFile = fileFinder.GetFilesAsync(file, cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }


        Assert.Single(filenames);
    }

    [Fact]
    public void Test_GetFilesSingleWildPatternInFrontWithMultipleResult()
    {
        var file = "di?3";
        var fileFinder = new FileFinder(true);
        var files = fileFinder.GetFiles(file);

        Assert.Equal(4, files.Count());
    }

    [Fact]
    public async Task Test_GetFilesSingleWildPatternInFrontWithMultipleResultAsync()
    {
        var filenames = new List<string>();
        var file = "di?3";
        var fileFinder = new FileFinder(true);
        using var cs = new CancellationTokenSource();
        var iFile = fileFinder.GetFilesAsync(file, cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }


        Assert.Equal(4, filenames.Count);
    }

    [Fact]
    public void Test_GetFilesWildCardDirectory()
    {
        var file = "*\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        var files = fileFinder.GetFiles(file);

        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
    }

    [Fact]
    public async Task Test_GetFilesWildCardDirectoryAsync()
    {
        var filenames = new List<string>();
        var file = "*\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();
        var iFile = fileFinder.GetFilesAsync(file, cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }

        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in filenames where f.Contains("Windows") select f));
    }

    [Fact]
    public void Test_GetFileWildCardDirectoryWithColon()
    {
        var file = "*:\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        var files = fileFinder.GetFiles(file);

        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
    }
    [Fact]
    public async Task Test_GetFileWildCardDirectoryWithColonAsync()
    {
        var filenames = new List<string>();
        var file = "*:\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();
        var iFile = fileFinder.GetFilesAsync(file, cs.Token);
        await foreach (var iFilename in iFile)
        {
            filenames.Add(iFilename);
        }

        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in filenames where f.Contains("Windows") select f));
    }
}
