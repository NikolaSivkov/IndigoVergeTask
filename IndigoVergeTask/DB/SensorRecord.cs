using System;

namespace IndigoVergeTask.DB
{
    public class SensorRecord
    {
        public long Id { get; set; }
        public string SensorName { get; set; }
        public float SensorValue { get; set; }
        public DateTimeOffset RecordedOn { get; set; }
    }
}