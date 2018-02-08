using System;
namespace Rise.Serialization
{
    public class SerializeColor4 : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var v = (Color4)obj;
            writer.Write(v.R);
            writer.Write(v.G);
            writer.Write(v.B);
            writer.Write(v.A);
        }

        public override object ReadBytes(ByteReader reader)
        {
            Color4 v;
            v.R = reader.ReadByte();
            v.G = reader.ReadByte();
            v.B = reader.ReadByte();
            v.A = reader.ReadByte();
            return v;
        }
    }
}
