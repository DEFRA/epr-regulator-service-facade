namespace EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData
{
    public interface IDummyDataLoader<out T>
    {
        T LoadData(string filePath);
    }
}
