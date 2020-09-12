using System.Collections.Generic;

namespace IndigoVergeTask
{
    public interface IAppConfig
    {
          
        string IpAddress { get; set; }


        int Port { get; set; }


        byte SlaveAddress { get; set; }


        int OutputIntervalMs { get; set; }


        int DbSaveIntervalMs { get; set; }


        List<Sensor> Sensors { get; set; }
    }
}