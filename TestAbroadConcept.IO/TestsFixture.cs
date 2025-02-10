using System.Diagnostics.CodeAnalysis;

namespace TestAbroadConcept.IO;
[ExcludeFromCodeCoverage]
public class TestsFixture : IDisposable
{
    public TestsFixture()
    {
        CreateDirectory("dir1\\subdir");
        CreateDirectory("dir1\\subdir2");
        CreateDirectory("dir2");
        CreateDirectory("dir3");
        CreateDirectory("dir3\\subdir");
        using var fileStream1 = File.CreateText("dir1\\subdir\\test.txt");
        fileStream1.Write("test");
        using var fileStream2 = File.CreateText("dir3\\subdir\\test.txt");
        fileStream2.Write("test");
        using var fileStream3 = File.CreateText("dir3\\subdir\\test2.txt");
        fileStream3.Write("test");
    }

    public void Dispose()
    {
        // Do "global" teardown here; Only called once.
    }

    private void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
