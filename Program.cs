﻿using System.CommandLine;
using System.Text.RegularExpressions;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var templateFileArg = new Argument<FileInfo>("template-file", "Template file to analyze");
        var susbcriptionIdArg = new Argument<string>("subscription-id", "Susbcription ID");
        var resourceGroupArg = new Argument<string>("resource-group", "Resource group name");

        var command = new RootCommand("Azure Resource Manager cost estimator.");
        command.AddArgument(templateFileArg);
        command.AddArgument(susbcriptionIdArg);
        command.AddArgument(resourceGroupArg);
        command.SetHandler(async (file, subscription, resourceGroup) =>
            await Estimate(file, subscription, resourceGroup), templateFileArg, susbcriptionIdArg, resourceGroupArg);

        return await command.InvokeAsync(args);
    }

    private static async Task Estimate(FileInfo file, string subscriptionId, string resourceGroupName)
    {
        Console.WriteLine($"Processing file {file.FullName}.");

  
        var template = Regex.Replace(File.ReadAllText(file.FullName), @"\s+", string.Empty);  // Make JSON a single-line value
        var whatIfData = await AzureWhatIfHandler.GetResponseWithRetries(subscriptionId, resourceGroupName, template);

        if (whatIfData == null || whatIfData.properties == null || whatIfData.properties.changes == null)
        {
            Console.WriteLine("No changes detected.");
            return;
        }

        Console.WriteLine($"Detected {whatIfData.properties.changes.Length} changes.");
    }
}