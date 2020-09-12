using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IndigoVergeTask
{
  
    public partial class AppConfig : IAppConfig
    {
        public const string SectionName = "SensorOptions";

        [JsonProperty("IpAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("Port")]
        public int Port { get; set; }

        [JsonProperty("SlaveAddress")]
        public byte SlaveAddress { get; set; }

        [JsonProperty("OutputIntervalMs")]
        public int OutputIntervalMs { get; set; }

        [JsonProperty("DbSaveIntervalMs")]
        public int DbSaveIntervalMs { get; set; }

        [JsonProperty("Sensors")]
        public List<Sensor> Sensors { get; set; }
    }

    public partial class Sensor :ISensor
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("StartAddress")]
        public ushort StartAddress { get; set; }

        [JsonProperty("NumberOfPoints")]
        public ushort NumberOfPoints { get; set; }
    }
}
