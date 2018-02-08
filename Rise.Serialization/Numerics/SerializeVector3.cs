using System;
namespace Rise.Serialization
{
    public class SerializeVector3 : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var v = (Vector3)obj;
            writer.Write(v.X);
            writer.Write(v.Y);
            writer.Write(v.Z);
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
