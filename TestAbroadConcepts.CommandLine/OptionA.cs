﻿using System.Diagnostics.CodeAnalysis;
using AbroadConcepts.CommandLine;

namespace TestAbroadConcepts.CommandLine;

[ExcludeFromCodeCoverage]
public class OptionA : IArgument
{
    public string Name { get; set; } = string.Empty;
}
