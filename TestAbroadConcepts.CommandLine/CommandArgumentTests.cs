using System.Diagnostics.CodeAnalysis;
using AbroadConcepts.CommandLine;

namespace TestAbroadConcepts.CommandLine;

[ExcludeFromCodeCoverage]
public class CommandArgumentTests
{
    [Fact]
    public void TestLoadKnownParametersWithOneOptional()
    {
        TestSettings testSettings = new TestSettings();
        OptionA optionA = new OptionA();
        var cmdArgs = new CommandArguments();
        cmdArgs.Register("-a", optionA);
        string[] args = ["name", "5", "6.2", "Blue", "true", "-a", "name"];
        var result = cmdArgs.Parse(testSettings, args);

        Assert.True(result);
        Assert.Equal("name", testSettings.StringValue);
        Assert.Equal(5, testSettings.IntValue);
        Assert.Equal(6.2m, testSettings.DecimalValue);
        Assert.Equal(Color.Blue, testSettings.Color);
        Assert.True(testSettings.BoolValue);
        Assert.Equal("name", optionA.Name);
    }
    [Fact]
    public void TestCopyInvalidArgument()
    {
        TestSettings testSettings = new TestSettings();
        OptionA optionA = new OptionA();
        var cmdArgs = new CommandArguments();
        cmdArgs.Register("-a", optionA);
        string[] args = ["name", "name"];
        var result = cmdArgs.Parse(testSettings, args);

        Assert.False(result);
        Assert.Equal("The input string 'name' was not in a correct format.", cmdArgs.Message);
    }

    [Fact]
    public void TestHelp()
    {
        TestSettings testSettings = new TestSettings();
        OptionA optionA = new OptionA();
        var cmdArgs = new CommandArguments();
        cmdArgs.Register("-a", optionA);
        string[] args = ["name", "-h"];
        var result = cmdArgs.Parse(testSettings, args);

        Assert.False(result);
    }

    [Fact]
    public void TestHelpWithMoreData()
    {
        TestSettings testSettings = new TestSettings();
        OptionA optionA = new OptionA();
        var cmdArgs = new CommandArguments();
        cmdArgs.Register("-a", optionA);
        string[] args = ["name", "5", "-h"];
        var result = cmdArgs.Parse(testSettings, args);

        Assert.False(result);
        Assert.Equal("User passed help option", cmdArgs.Message);
    }
    [Fact]
    public void TestHelpWithInvalidData()
    {
        TestSettings testSettings = new TestSettings();
        OptionA optionA = new OptionA();
        var cmdArgs = new CommandArguments();
        cmdArgs.Register("-a", optionA);
        string[] args = ["name", "Invalid", "-h"];
        var result = cmdArgs.Parse(testSettings, args);

        Assert.False(result);
        Assert.Equal("The input string 'Invalid' was not in a correct format.", cmdArgs.Message);
    }
}