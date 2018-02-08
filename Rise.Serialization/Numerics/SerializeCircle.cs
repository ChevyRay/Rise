using System;
namespace Rise.Serialization
{
    public class SerializeCircle : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var v = (Circle)obj;
            writer.Write(v.Center.X);
            writer.Write(v.Center.Y);
            writer.Write(v.Radius);
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
