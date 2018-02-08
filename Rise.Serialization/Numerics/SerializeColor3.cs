using System;
namespace Rise.Serialization
{
    public class SerializeColor3 : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var v = (Color3)obj;
            writer.Write(v.R);
            writer.Write(v.G);
            writer.Write(v.B);
        }

        public override object ReadBytes(ByteReader reader)
        {
            Color3 v;
            v.R = reader.ReadByte();
            v.G = reader.ReadByte();
            v.B = reader.ReadByte();
            return v;
        }
    }
}
