using System;
namespace Rise.Serialization
{
    public class SerializePoint2 : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var v = (Point2)obj;
            writer.WriteInt(v.X);
            writer.WriteInt(v.Y);
        }

        public override object ReadBytes(ByteReader reader)
        {
            Point2 v;
            v.X = reader.ReadInt();
            v.Y = reader.ReadInt();
            return v;
        }
    }
}
