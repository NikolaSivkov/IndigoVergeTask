namespace IndigoVergeTask
{
    public interface ISensor
    {
         public string Name { get; set; }

         public ushort StartAddress { get; set; }

         public ushort NumberOfPoints { get; set; }
    }
}