using System;
namespace Rise.Serialization
{
    public class SerializePoint3 : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var v = (Point3)obj;
            writer.Write(v.X);
            writer.Write(v.Y);
            writer.Write(v.Z);
        }

        public override object ReadBytes(ByteReader reader)
        {
            Point3 v;
            v.X = reader.ReadInt();
            v.Y = reader.ReadInt();
            v.Z = reader.ReadInt();
            return v;
        }
    }
}
