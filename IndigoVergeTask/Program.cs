using NModbus;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;
using System.IO;
using IndigoVergeTask.DB;

namespace IndigoVergeTask
{
    class Program
    {
        public static Config _config { get; set; }
        static async Task Main(string[] args)
        {
            var configFile = await File.ReadAllTextAsync("./appsettings.json");
            _config = JsonConvert.DeserializeObject<Config>(configFile);
            var db = new SensorDataDB();
            await db.Database.EnsureCreatedAsync();
            RepeatingTask(OutputSensoryData, _config.OutputIntervalMs, CancellationToken.None);
            RepeatingTask(RecordSensoryData, _config.DbSaveIntervalMs, CancellationToken.None);

            Console.ReadLine();
        }

        static void RepeatingTask(Func<Task> action, int miliseconds, CancellationToken token)
        {
            if (action == null)
                return;
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await action();
                    await Task.Delay(TimeSpan.FromMilliseconds(miliseconds), token);
                }
            }, token);
        }

        public static Task OutputSensoryData()
        {
            using (TcpClient client = new TcpClient(_config.IpAddress, _config.Port))
            {
                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateMaster(client);
                byte slaveAddress = _config.SlaveAddress;

                foreach (var sensor in _config.Sensors)
                {
                    ushort[] inputs = master.ReadHoldingRegisters(slaveAddress, sensor.StartAddress, sensor.NumberOfPoints);
                    Console.WriteLine($"{sensor.Name}: {ToFloat(inputs)}");
                }
            }
            return Task.FromResult(true);
        }

        public static async Task RecordSensoryData()
        {
            using TcpClient client = new TcpClient(_config.IpAddress, _config.Port);

            var factory = new ModbusFactory();
            IModbusMaster master = factory.CreateMaster(client);

            byte slaveAddress = _config.SlaveAddress;

            using var db = new SensorDataDB();

            foreach (var sensor in _config.Sensors)
            {

                ushort[] inputs = await master.ReadHoldingRegistersAsync(slaveAddress, sensor.StartAddress, sensor.NumberOfPoints);
                var sensorValue = ToFloat(inputs);

                var data = new SensorRecord()
                {
                    RecordedOn = DateTimeOffset.Now,
                    SensorName = sensor.Name,
                    SensorValue = sensorValue
                };

                db.SensorsData.Add(data);
                await db.SaveChangesAsync();

            }
        }

        public static float ToFloat(ushort[] buffer)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(buffer[0] & 0xFF);
            bytes[1] = (byte)(buffer[0] >> 8);
            bytes[2] = (byte)(buffer[1] & 0xFF);
            bytes[3] = (byte)(buffer[1] >> 8);

            float value = BitConverter.ToSingle(bytes, 0);
            return value;
        }
    }
}
