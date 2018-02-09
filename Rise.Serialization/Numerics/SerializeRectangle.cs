using System;
namespace Rise.Serialization
{
    public class SerializeRectangle : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var v = (Rectangle)obj;
            writer.WriteFloat(v.X);
            writer.WriteFloat(v.Y);
            writer.WriteFloat(v.W);
            writer.WriteFloat(v.H);
        }

        public override object ReadBytes(ByteReader reader)
        {
            Rectangle v;
            v.X = reader.ReadFloat();
            v.Y = reader.ReadFloat();
            v.W = reader.ReadFloat();
            v.H = reader.ReadFloat();
            return v;
        }
    }
}
