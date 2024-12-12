using Microsoft.VisualStudio.TestTools.UnitTesting;
using EPR.RegulatorService.Facade.Core.Helpers;
namespace EPR.RegulatorService.Facade.Core.Helpers.Tests;

[TestClass()]
public class FileHelpersTests
{
    [TestMethod()]
    public void GetTruncatedFileName_Will_Return_FullName_When_Less_Than_RequiredLength()
    {
        var expected = "thisisafilename.txt";
        var result = FileHelpers.GetTruncatedFileName(expected, expected.Length + 1);
        Assert.AreEqual(expected, result);
    }

    [TestMethod()]
    public void GetTruncatedFileName_Will_Return_ReducedName_When_SameLength_As_RequiredLength()
    {
        var expected = "thisisafilenam.txt";
        var result = FileHelpers.GetTruncatedFileName("thisisafilename.txt", expected.Length);
        Assert.AreEqual(expected, result);
    }

    [TestMethod()]
    public void GetTruncatedFileName_Will_Return_ReducedName_When_LengthGreater_Than_RequiredLength()
    {
        var sourceFilename = "thisisalongfilename.txt";
        var expected = "this.txt";
        var result = FileHelpers.GetTruncatedFileName(sourceFilename, 8);
        Assert.AreEqual(expected, result);
    }
}