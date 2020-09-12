using IndigoVergeTask.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IndigoVergeTask
{
    class OutputSensorDataProcessor : ISensorDataProcessor
    {
        private readonly IAppConfig appConfig;
        private IEnumerable<ISensorDataReader> SensorReaders { get; }

        private Dictionary<string, float> previousSensorValues = new Dictionary<string, float>();
        public int IntervalMs => appConfig.OutputIntervalMs;

        public OutputSensorDataProcessor(IEnumerable<ISensorDataReader> sensorReaders,IAppConfig appConfig)
        {
            SensorReaders = sensorReaders;
            this.appConfig = appConfig;

            //add default values in the "previsous" values before starting
            foreach (var sensorReader in SensorReaders)
            {
                previousSensorValues.Add(sensorReader.SensorName,0);
            }
        }
        
        public async Task<bool> ProcessSensorRecord()
        {
            
            foreach (var sensorReader in SensorReaders)
            {
                var sensorData = await sensorReader.ReadSensorRecord();

                if (previousSensorValues[sensorReader.SensorName] != sensorData.SensorValue)
                {
                    Console.WriteLine($"[{sensorData.RecordedOn}]{sensorData.SensorName}: {sensorData.SensorValue}");
                    previousSensorValues[sensorReader.SensorName] = sensorData.SensorValue;
                }
#if DEBUG 
                Console.WriteLine($"[{sensorData.RecordedOn}]{sensorData.SensorName}: {sensorData.SensorValue}");
#endif
            }

            return true;
        }
    }
}
