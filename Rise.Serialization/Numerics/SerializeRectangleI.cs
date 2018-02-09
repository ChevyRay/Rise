using System;
namespace Rise.Serialization
{
    public class SerializeRectangleI : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var v = (RectangleI)obj;
            writer.WriteInt(v.X);
            writer.WriteInt(v.Y);
            writer.WriteInt(v.W);
            writer.WriteInt(v.H);
        }

        public override object ReadBytes(ByteReader reader)
        {
            RectangleI v;
            v.X = reader.ReadInt();
            v.Y = reader.ReadInt();
            v.W = reader.ReadInt();
            v.H = reader.ReadInt();
            return v;
        }
    }
}
