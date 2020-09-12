using IndigoVergeTask.DB;
using System.Threading.Tasks;

namespace IndigoVergeTask
{
    interface ISensorDataProcessor
    {
        Task<bool> ProcessSensorRecord();
        int IntervalMs { get; }
    }
}
