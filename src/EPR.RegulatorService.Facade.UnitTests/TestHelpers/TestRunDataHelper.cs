using System;
using System.Text.Json;

namespace EPR.RegulatorService.Facade.UnitTests.TestHelpers
{
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
            string testPath = Path.Combine(baseDirectory, relativeFilePath);
            if (!File.Exists(testPath)) testContext.WriteLine($"File not found from AppContext: '{testPath}'");
            if (File.Exists(testPath))
            {
                testFolderRoot = baseDirectory;
                testContext.WriteLine($"Found file in AppContext: {testPath}");
                return;
            }

            testPath = Path.Combine(currentDirectory, relativeFilePath);
            if (!File.Exists(testPath)) testContext.WriteLine($"File not found from GetCurrentDirectory '{testPath}'");
            if (File.Exists(testPath))
            {
                testFolderRoot = currentDirectory;
                testContext.WriteLine($"Found file in GetCurrentDirectory {testPath}");
                return;
            }

            testPath = Path.Combine(testrunFolder, relativeFilePath);
            if (!File.Exists(testPath)) testContext.WriteLine($"File not found from TestRunDirectory '{testPath}'");
            if (File.Exists(testPath))
            {
                testFolderRoot = testrunFolder;
                testContext.WriteLine($"Found file in TestRunDirectory {testPath}");
                return;
            }

            testPath = Path.Combine(testExecuteFolder, relativeFilePath);
            if (!File.Exists(testPath)) testContext.WriteLine($"File not found from DeploymentDirectory '{testPath}'");
            if (File.Exists(testPath))
            {
                testFolderRoot = testExecuteFolder;
                testContext.WriteLine($"Found file in DeploymentDirectory {testPath}");
            }
        }
    }
}
