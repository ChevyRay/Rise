using System;
namespace Rise.Serialization
{
    public class SerializeVector4 : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var v = (Vector4)obj;
            writer.WriteFloat(v.X);
            writer.WriteFloat(v.Y);
            writer.WriteFloat(v.Z);
            writer.WriteFloat(v.W);
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
