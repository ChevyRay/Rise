using System;
namespace Rise.Serialization
{
    public class SerializeVector4 : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var v = (Vector4)obj;
            writer.Write(v.X);
            writer.Write(v.Y);
            writer.Write(v.Z);
            writer.Write(v.W);
        }

        public override object ReadBytes(ByteReader reader)
        {
            Vector4 v;
            v.X = reader.ReadFloat();
            v.Y = reader.ReadFloat();
            v.Z = reader.ReadFloat();
            v.W = reader.ReadFloat();
            return v;
        }
    }
}
