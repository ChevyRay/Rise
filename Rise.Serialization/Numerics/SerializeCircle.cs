using System;
namespace Rise.Serialization
{
    public class SerializeCircle : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var v = (Circle)obj;
            writer.WriteFloat(v.Center.X);
            writer.WriteFloat(v.Center.Y);
            writer.WriteFloat(v.Radius);
        }

        public override object ReadBytes(ByteReader reader)
        {
            Circle v;
            v.Center.X = reader.ReadFloat();
            v.Center.Y = reader.ReadFloat();
            v.Radius = reader.ReadFloat();
            return v;
        }
    }
}
