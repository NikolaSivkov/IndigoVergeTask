using IndigoVergeTask.DB;
using NModbus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IndigoVergeTask
{
    interface ISensorDataReader
    {
        public string SensorName { get; }
        Task<SensorRecord> ReadSensorRecord();
    }
}
