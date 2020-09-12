using IndigoVergeTask.DB;
using NModbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndigoVergeTask
{
    public class HumiditySensorReader : ISensorDataReader
    {
        public string SensorName { get; } = "Humidity";
        private readonly IModbusMaster modbusMaster;
        private readonly IAppConfig AppConfig;
        private readonly Sensor Sensor;

        public HumiditySensorReader(IModbusMaster modbusMaster, IAppConfig appConfig)
        {
            this.modbusMaster = modbusMaster;
            AppConfig = appConfig;
            this.Sensor = appConfig.Sensors.First(x => x.Name == SensorName);
        }
 
        public async Task<SensorRecord> ReadSensorRecord()
        {
            ushort[] inputs = await modbusMaster.ReadHoldingRegistersAsync(AppConfig.SlaveAddress, Sensor.StartAddress, Sensor.NumberOfPoints);
            return new SensorRecord()
            {
                RecordedOn = DateTimeOffset.Now,
                SensorName = Sensor.Name,
                SensorValue = inputs.ToFloat()
            };
        }
    }
}
