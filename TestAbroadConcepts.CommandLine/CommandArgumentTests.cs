using System.Diagnostics.CodeAnalysis;
using AbroadConcepts.CommandLine;

namespace TestAbroadConcepts.CommandLine;

[ExcludeFromCodeCoverage]
public class CommandArgumentTests
{
    [Fact]
    public void TestLoadKnownParametersWithOneOptional()
    {
        string[] args = ["name", "5", "6.2", "Blue", "true", "-a", "name"];
        TestSettings testSettings = new TestSettings();
        var cmdArgs = new CommandArguments(args);
        var possibleParameters = new ICommandArgument[] { new SettingA() };
        var result = cmdArgs.Parse(testSettings, possibleParameters);

        Assert.True(result);
        OptionA optionA = cmdArgs.GetNextArgument<OptionA>();

        Assert.Equal("name", testSettings.StringValue);
        Assert.Equal(5, testSettings.IntValue);
        Assert.Equal(6.2m, testSettings.DecimalValue);
        Assert.Equal(Color.Blue, testSettings.Color);
        Assert.True(testSettings.BoolValue);
        Assert.Equal("name", optionA.Name);
    }
    [Fact]
    public void TestParseLoadKnownParameters()
    {
        string[] args = ["name", "5", "6.2", "Blue", "true" ];
        TestSettings testSettings = new TestSettings();
        var cmdArgs = new CommandArguments(args);
        var result = cmdArgs.Parse(testSettings);

        Assert.True(result);
        Assert.Equal("name", testSettings.StringValue);
        Assert.Equal(5, testSettings.IntValue);
        Assert.Equal(6.2m, testSettings.DecimalValue);
        Assert.Equal(Color.Blue, testSettings.Color);
        Assert.True(testSettings.BoolValue);
    }
    [Fact]
    public void TestParseLoadKnownParametersWithOneOptionWithTooManyOptions()
    {
        string[] args = ["name", "5", "6.2", "Blue", "true", "0"];
        TestSettings testSettings = new TestSettings();
        var cmdArgs = new CommandArguments(args);
        var result = cmdArgs.Parse(testSettings);

        Assert.False(result);
        Assert.Equal("Unknown argument", cmdArgs.Message);
    }
    [Fact]
    public void TestLoadKnownParametersWithOneOptionWithTooManyOptions()
    {
        string[] args = ["name", "5", "6.2", "Blue", "true", "0", "-a", "name"];
        TestSettings testSettings = new TestSettings();
        var cmdArgs = new CommandArguments(args);
        var possibleParameters = new ICommandArgument[] { new SettingA() };
        var result = cmdArgs.Parse(testSettings, possibleParameters);

        Assert.False(result);
        Assert.Equal("Unknown argument", cmdArgs.Message);
    }
    [Fact]
    public void TestCopyInvalidArgument()
    {
        string[] args = ["name", "name"];
        TestSettings testSettings = new TestSettings();
        var cmdArgs = new CommandArguments(args);
        var possibleParameters = new ICommandArgument[] { new SettingA() };
        var result = cmdArgs.Parse(testSettings, possibleParameters);

        Assert.False(result);
        Assert.Equal("The input string 'name' was not in a correct format.", cmdArgs.Message);
    }

    [Fact]
    public void TestHelp()
    {
        string[] args = ["name", "-h"];
        TestSettings testSettings = new TestSettings();
        var cmdArgs = new CommandArguments(args);
        var possibleParameters = new ICommandArgument[] { new SettingA() };
        var result = cmdArgs.Parse(testSettings, possibleParameters);

        Assert.False(result);
    }

     [Fact]
    public void TestHelpWithInvalidData()
    {
        string[] args = ["name", "Invalid", "-h"];
        TestSettings testSettings = new TestSettings();
        var cmdArgs = new CommandArguments(args);
        var possibleParameters = new ICommandArgument[] { new SettingA() };
        var result = cmdArgs.Parse(testSettings, possibleParameters);

        Assert.False(result);
        Assert.Equal("The input string 'Invalid' was not in a correct format.", cmdArgs.Message);
    }
}