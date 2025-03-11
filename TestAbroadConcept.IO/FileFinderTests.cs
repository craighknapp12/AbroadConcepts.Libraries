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
    public void Test_GetFileKnownDirectoryWithColonAsync()
    {
        var file = "C:Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();
        var files = fileFinder.GetFilesAsync(file, cs.Token).ToBlockingEnumerable();

        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
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
    public void Test_GetFileKnownDirectoryWithColonSlashAsync()
    {
        var file = "C:\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();
        var files = fileFinder.GetFilesAsync(file, cs.Token).ToBlockingEnumerable();

        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
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
    public void Test_GetFilesMultiplePatternInFrontWithMultipleResultAsync()
    {
        var file = "d?r*\\s*dir";
        var fileFinder = new FileFinder(true);
        using var cs = new CancellationTokenSource();
        var files = fileFinder.GetFilesAsync(file, cs.Token).ToBlockingEnumerable();

        Assert.Equal(5, files.Count());
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
    public void Test_GetFilesMultiplePatternInFrontWithSpecificFileSelectionAsync()
    {
        var file = "d?r*\\s*dir\\*st*";
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();
        var files = fileFinder.GetFilesAsync(file, cs.Token).ToBlockingEnumerable();


        Assert.Equal(3, files.Count());
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
    public void Test_GetFileQuestionDirectoryAsync()
    {
        var file = "?\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();
        var files = fileFinder.GetFilesAsync(file, cs.Token).ToBlockingEnumerable();


        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
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
    public void Test_GetFileQuestionDirectoryWithColonAsync()
    {
        var file = "?:\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();
        var files = fileFinder.GetFilesAsync(file, cs.Token).ToBlockingEnumerable();


        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
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
    public void Test_GetFilesSelectingDirectory1Async()
    {
        var file = "d?r1\\s*dir";
        var fileFinder = new FileFinder(true);
        using var cs = new CancellationTokenSource();
        var files = fileFinder.GetFilesAsync(file, cs.Token).ToBlockingEnumerable();


        Assert.Equal(2, files.Count());
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
    public void Test_GetFilesSelectingDirectory1DoesNotIncludeDirectoryAsync()
    {
        var file = "d?r1\\s*dir";
        var fileFinder = new FileFinder();
        var files = fileFinder.GetFiles(file);

        Assert.Single(files);
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
    public void Test_GetFilesSelectingDirectory3Async()
    {
        var file = "d?r3\\s*dir";
        var fileFinder = new FileFinder(true);
        using var cs = new CancellationTokenSource();
        var files = fileFinder.GetFilesAsync(file, cs.Token).ToBlockingEnumerable();

        Assert.Equal(3, files.Count());
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
    public void Test_GetFilesSingleWildPatternInFrontWith1ResultAsync()
    {
        var file = "di?2";
        var fileFinder = new FileFinder(true);
        using var cs = new CancellationTokenSource();
        var files = fileFinder.GetFilesAsync(file, cs.Token).ToBlockingEnumerable();


        Assert.Single(files);
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
    public void Test_GetFilesSingleWildPatternInFrontWithMultipleResultAsync()
    {
        var file = "di?3";
        var fileFinder = new FileFinder(true);
        using var cs = new CancellationTokenSource();
        var files = fileFinder.GetFilesAsync(file, cs.Token).ToBlockingEnumerable();


        Assert.Equal(4, files.Count());
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
    public void Test_GetFilesWildCardDirectoryAsync()
    {
        var file = "*\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();
        var files = fileFinder.GetFilesAsync(file, cs.Token).ToBlockingEnumerable();


        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
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
    public void Test_GetFileWildCardDirectoryWithColonAsync()
    {
        var file = "*:\\Windows\\winhlp32".EnsureExtension(".exe");
        var fileFinder = new FileFinder();
        using var cs = new CancellationTokenSource();
        var files = fileFinder.GetFilesAsync(file, cs.Token).ToBlockingEnumerable();


        Assert.Contains("C:\\Windows\\winhlp32.exe", (from f in files where f.Contains("Windows") select f));
    }
}
