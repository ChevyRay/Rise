using System;
namespace Rise.Serialization
{
    public class SerializeQuaternion : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var v = (Quaternion)obj;
            writer.WriteFloat(v.X);
            writer.WriteFloat(v.Y);
            writer.WriteFloat(v.Z);
            writer.WriteFloat(v.W);
        }

        public override object ReadBytes(ByteReader reader)
        {
            Quaternion v;
            v.X = reader.ReadFloat();
            v.Y = reader.ReadFloat();
            v.Z = reader.ReadFloat();
            v.W = reader.ReadFloat();
            return v;
        }
    }
}
