using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData
{
    public interface IDummyDataLoader<out T>
    {
        T LoadData(string filePath);
    }
}
