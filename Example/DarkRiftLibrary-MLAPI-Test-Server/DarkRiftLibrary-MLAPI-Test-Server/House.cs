using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift;
using Penca53.DarkRift.MLAPI;

namespace DarkRiftLibrary_MLAPI_Test_Server
{
    public class House : ISync
    {
        public int TypeID { get; set; } = 1;

        public float Width;
        public float Height;
        public int Inhabitants;

        public void Deserialize(DeserializeEvent e)
        {
            Width = e.Reader.ReadSingle();
            Height = e.Reader.ReadSingle();
            Inhabitants = e.Reader.ReadInt32();
        }

        public void DeserializeOptional(DarkRiftReader reader, int tag)
        {
            if (tag == 1)
            {
                Width = reader.ReadSingle();
                Height = reader.ReadSingle();
            }
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Width);
            e.Writer.Write(Height);
            e.Writer.Write(Inhabitants);
        }

        public void SerializeOptional(DarkRiftWriter writer, int tag)
        {
            if (tag == 1)
            {
                writer.Write(Width);
                writer.Write(Height);
            }
        }
    }
}
