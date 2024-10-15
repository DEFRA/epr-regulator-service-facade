﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace EPR.RegulatorService.Facade.UnitTests.TestHelpers
{
    [ExcludeFromCodeCoverage]
    public static class TestRunDataHelper
    {
        private static string testFolderRoot = string.Empty;

        public static string LoadDataFile(string relativeTestDataFilePath, TestContext testContext)
        {
            string baseDirectory = AppContext.BaseDirectory;
            string currentDirectory = Directory.GetCurrentDirectory();
            string testrunFolder = testContext.TestRunDirectory;
            string testExecuteFolder = testContext.DeploymentDirectory;

            if (!string.IsNullOrEmpty(testFolderRoot) && File.Exists(Path.Combine(testFolderRoot, relativeTestDataFilePath)))
            {
                var jsonData = File.ReadAllText(Path.Combine(testFolderRoot, relativeTestDataFilePath));
                return jsonData;
            }
            else
            {
                DetermineTestFolderRoot(relativeTestDataFilePath, testContext);
                if (!string.IsNullOrEmpty(testFolderRoot) && File.Exists(Path.Combine(testFolderRoot, relativeTestDataFilePath)))
                {
                    var jsonData = File.ReadAllText(relativeTestDataFilePath);
                    return jsonData;
                }
            }
            throw new FileNotFoundException($"Cannot locate {relativeTestDataFilePath} from any of '{baseDirectory}', '{currentDirectory}', '{testrunFolder}' or '{testExecuteFolder}'");
        }

        public static T LoadDataFile<T>(string relativeTestDataFilePath, TestContext testContext)
        {
            string baseDirectory = AppContext.BaseDirectory;
            string currentDirectory = Directory.GetCurrentDirectory();
            string testrunFolder = testContext.TestRunDirectory;
            string testExecuteFolder = testContext.DeploymentDirectory;

            if (!string.IsNullOrEmpty(testFolderRoot) && File.Exists(Path.Combine(testFolderRoot, relativeTestDataFilePath)))
            {
                var jsonData = File.ReadAllText(Path.Combine(testFolderRoot, relativeTestDataFilePath));
                return JsonSerializer.Deserialize<T>(jsonData);
            }
            else
            {
                DetermineTestFolderRoot(relativeTestDataFilePath, testContext);

                if (!string.IsNullOrEmpty(testFolderRoot) && File.Exists(Path.Combine(testFolderRoot, relativeTestDataFilePath)))
                {
                    var jsonData = File.ReadAllText(relativeTestDataFilePath);
                    return JsonSerializer.Deserialize<T>(jsonData);
                }
            }

            throw new FileNotFoundException($"Cannot locate {relativeTestDataFilePath} from any of '{baseDirectory}', '{currentDirectory}', '{testrunFolder}' or '{testExecuteFolder}'");
        }

        private static void DetermineTestFolderRoot(string relativeFilePath, TestContext testContext)
        {
            string baseDirectory = AppContext.BaseDirectory;
            string currentDirectory = Directory.GetCurrentDirectory();
            string testrunFolder = testContext.TestRunDirectory;
            string testExecuteFolder = testContext.DeploymentDirectory;
            var reportingMessage = $"  AppContext.BaseDirectory = {baseDirectory}.\r\n  GetCurrentDirectory = {currentDirectory}. \r\n  TestRunDirectory = {testrunFolder}\r\n  DeploymentDirectory = {testExecuteFolder}\r\n";

            testContext.WriteLine($"=======================\nExecuting Folder Information:\r\n{reportingMessage}\r\n=======================");

            baseDirectory = baseDirectory.Replace("$(buildConfiguration)", "Debug");
            string testPath = Path.Combine(baseDirectory, relativeFilePath);
            if (!File.Exists(testPath)) testContext.WriteLine($"File not found from AppContext: '{testPath}'");
            if (File.Exists(testPath))
            {
                testFolderRoot = baseDirectory;
                testContext.WriteLine($"Found file in AppContext: {testPath}");
                return;
            }

            baseDirectory = baseDirectory.Replace("Debug", "Release");
            testPath = Path.Combine(baseDirectory, relativeFilePath);
            if (!File.Exists(testPath)) testContext.WriteLine($"File not found from AppContext: '{testPath}'");
            if (File.Exists(testPath))
            {
                testFolderRoot = baseDirectory;
                testContext.WriteLine($"Found file in AppContext: {testPath}");
                return;
            }

            currentDirectory = currentDirectory.Replace("$(buildConfiguration)", "Debug");
            testPath = Path.Combine(currentDirectory, relativeFilePath);
            if (!File.Exists(testPath)) testContext.WriteLine($"File not found from GetCurrentDirectory '{testPath}'");
            if (File.Exists(testPath))
            {
                testFolderRoot = currentDirectory;
                testContext.WriteLine($"Found file in GetCurrentDirectory {testPath}");
                return;
            }

            currentDirectory = currentDirectory.Replace("Debug", "Release");
            if (!File.Exists(testPath)) testContext.WriteLine($"File not found from GetCurrentDirectory '{testPath}'");
            if (File.Exists(testPath))
            {
                testFolderRoot = currentDirectory;
                testContext.WriteLine($"Found file in GetCurrentDirectory {testPath}");
            }
        }
    }
}
