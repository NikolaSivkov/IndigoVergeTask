using IndigoVergeTask.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IndigoVergeTask
{
    class DbSensorDataProcessor : ISensorDataProcessor
    {
        private readonly SensorDataDB db;
        private readonly IAppConfig appConfig;
        private IEnumerable<ISensorDataReader> SensorReaders { get; }

        public int IntervalMs => appConfig.DbSaveIntervalMs;

        public DbSensorDataProcessor(SensorDataDB sensorDataDB, IEnumerable<ISensorDataReader> sensorReaders, IAppConfig appConfig)
        {
            db = sensorDataDB;
            SensorReaders = sensorReaders;
            this.appConfig = appConfig;
        }

        public async Task<bool> ProcessSensorRecord()
        {
            try
            {
                foreach (var sensorReader in SensorReaders)
                {
                    var sensorData = await sensorReader.ReadSensorRecord();
                    db.SensorsData.Add(sensorData);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // log error and/or throw it for further processing/recovery
                return false;
            }
            
            return true;
        }
    }
}
