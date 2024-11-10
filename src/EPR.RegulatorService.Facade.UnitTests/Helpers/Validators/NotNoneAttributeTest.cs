using System.ComponentModel.DataAnnotations;
using EPR.RegulatorService.Facade.Core.Helpers.Validators;
using JetBrains.Annotations;

namespace EPR.RegulatorService.Facade.UnitTests.Helpers.Validators;

[TestClass]
[TestSubject(typeof(NotNoneAttribute))]
public class NotNoneAttributeTest
{
    private readonly NotNoneAttribute _attribute = new NotNoneAttribute();

    public enum TestEnum
    {
        None = 0,
        Value1 = 1,
        Value2 = 2
    }

    [TestMethod]
    public void IsValid_NullValue_ReturnsValidationError()
    {
        var result = _attribute.IsValid(null);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValid_NonEnumValue_ThrowsInvalidOperationException()
    {
        var ex = Assert.ThrowsException<InvalidOperationException>(() => _attribute.IsValid("not-an-enum"));
        Assert.AreEqual("The NotDefaultEnumAttribute can only be used on enum types.", ex.Message);
    }

    [TestMethod]
    public void IsValid_EnumDefaultValue_ReturnsValidationError()
    {
        var result = _attribute.IsValid(TestEnum.None);
        Assert.IsNotNull(result);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValid_EnumNonDefaultValue_ReturnsSuccess()
    {
        var result = _attribute.IsValid(TestEnum.Value1);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void IsValid_EnumOtherNonDefaultValue_ReturnsSuccess()
    {
        var result = _attribute.IsValid(TestEnum.Value2);
        Assert.AreEqual(true, result);
    }
}