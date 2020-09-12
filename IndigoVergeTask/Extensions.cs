using System;
using System.Collections.Generic;
using System.Text;

namespace IndigoVergeTask
{
    public static class Extensions
    {

        public static float ToFloat(this ushort[] buffer)
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
