using System;
namespace Rise.Serialization
{
    public class SerializeVector2 : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var v = (Vector2)obj;
            writer.WriteFloat(v.X);
            writer.WriteFloat(v.Y);
        }

        public override object ReadBytes(ByteReader reader)
        {
            Vector2 v;
            v.X = reader.ReadFloat();
            v.Y = reader.ReadFloat();
            return v;
        }
    }
}
