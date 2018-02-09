using System;
namespace Rise.Serialization
{
    public class SerializeVector3 : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var v = (Vector3)obj;
            writer.WriteFloat(v.X);
            writer.WriteFloat(v.Y);
            writer.WriteFloat(v.Z);
        }

        public override object ReadBytes(ByteReader reader)
        {
            Vector3 v;
            v.X = reader.ReadFloat();
            v.Y = reader.ReadFloat();
            v.Z = reader.ReadFloat();
            return v;
        }
    }
}
