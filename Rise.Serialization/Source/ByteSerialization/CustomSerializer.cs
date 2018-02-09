using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Rise.Serialization
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class CustomSerializerAttribute : Attribute
    {
        public Type Type { get; private set; }

        public CustomSerializerAttribute(Type type)
        {
            Type = type;
            if (!typeof(CustomSerializer).IsAssignableFrom(type))
                throw new Exception("Type is not a CustomSerializer: " + type);
        }
    }

    public abstract class CustomSerializer
    {
        static Dictionary<Type, CustomSerializer> lookup = new Dictionary<Type, CustomSerializer>();

        static CustomSerializer()
        {
            Add<bool, BoolSerializer>();
            Add<byte, ByteSerializer>();
            Add<short, ShortSerializer>();
            Add<ushort, UShortSerializer>();
            Add<int, IntSerializer>();
            Add<uint, UIntSerializer>();
            Add<long, LongSerializer>();
            Add<ulong, ULongSerializer>();
            Add<float, FloatSerializer>();
            Add<double, DoubleSerializer>();
            Add<string, StringSerializer>();
            Add<Color3, Color3Serializer>();
            Add<Color4, Color4Serializer>();
            Add<Vector2, Vector2Serializer>();
            Add<Vector3, Vector3Serializer>();
            Add<Vector4, Vector4Serializer>();
            Add<Point2, Point2Serializer>();
            Add<Point3, Point3Serializer>();
            Add<Rectangle, RectangleSerializer>();
            Add<RectangleI, RectangleISerializer>();
            Add<Circle, CircleSerializer>();
            Add<Quaternion, QuaternionSerializer>();
        }

        public static void Add(Type type, CustomSerializer serializer)
        {
            if (lookup.ContainsKey(type))
                throw new Exception("CustomSerializer for type already exists: " + type);
            lookup.Add(type, serializer);
        }
        public static void Add<T>(Type type) where T : CustomSerializer, new()
        {
            Add(type, new T());
        }
        public static void Add<T, U>() where U : CustomSerializer, new()
        {
            Add(typeof(T), new U());
        }

        public static CustomSerializer Get(Type type)
        {
            CustomSerializer result;
            if (!lookup.TryGetValue(type, out result))
            {
                var attrs = (CustomSerializerAttribute[])type.GetCustomAttributes(typeof(CustomSerializerAttribute), false);
                if (attrs.Length == 0)
                    throw new Exception("No CustomSerializer for type: " + type);
                result = (CustomSerializer)Activator.CreateInstance(attrs[0].Type);
                lookup[type] = result;
            }
            return result;
        }
        public static CustomSerializer Get<T>()
        {
            return Get(typeof(T));
        }

        static IntPtr buffer;
        static int bufferSize;

        public abstract void WriteBytes(object obj, ByteWriter writer);
        public abstract object ReadBytes(ByteReader reader);
    }

    public class BoolSerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            writer.Write((bool)obj);
        }
        public override object ReadBytes(ByteReader reader)
        {
            return reader.ReadBool();
        }
    }

    public class ByteSerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            writer.Write((byte)obj);
        }
        public override object ReadBytes(ByteReader reader)
        {
            return reader.ReadByte();
        }
    }

    public class ShortSerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            writer.Write((short)obj);
        }
        public override object ReadBytes(ByteReader reader)
        {
            return reader.ReadShort();
        }
    }

    public class UShortSerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            writer.Write((ushort)obj);
        }
        public override object ReadBytes(ByteReader reader)
        {
            return reader.ReadUShort();
        }
    }

    public class IntSerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            writer.Write((int)obj);
        }
        public override object ReadBytes(ByteReader reader)
        {
            return reader.ReadInt();
        }
    }

    public class UIntSerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            writer.Write((uint)obj);
        }
        public override object ReadBytes(ByteReader reader)
        {
            return reader.ReadUInt();
        }
    }

    public class LongSerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            writer.Write((long)obj);
        }
        public override object ReadBytes(ByteReader reader)
        {
            return reader.ReadLong();
        }
    }

    public class ULongSerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            writer.Write((ulong)obj);
        }
        public override object ReadBytes(ByteReader reader)
        {
            return reader.ReadULong();
        }
    }

    public class FloatSerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            writer.Write((float)obj);
        }
        public override object ReadBytes(ByteReader reader)
        {
            return reader.ReadFloat();
        }
    }

    public class DoubleSerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            writer.Write((double)obj);
        }
        public override object ReadBytes(ByteReader reader)
        {
            return reader.ReadDouble();
        }
    }

    public class StringSerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            writer.Write((string)obj);
        }
        public override object ReadBytes(ByteReader reader)
        {
            return reader.ReadString();
        }
    }

    public class Color3Serializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var val = (Color3)obj;
            writer.Write(val.R);
            writer.Write(val.G);
            writer.Write(val.B);
        }
        public override object ReadBytes(ByteReader reader)
        {
            Color3 val;
            val.R = reader.ReadByte();
            val.G = reader.ReadByte();
            val.B = reader.ReadByte();
            return val;
        }
    }

    public class Color4Serializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var val = (Color4)obj;
            writer.Write(val.R);
            writer.Write(val.G);
            writer.Write(val.B);
            writer.Write(val.A);
        }
        public override object ReadBytes(ByteReader reader)
        {
            Color4 val;
            val.R = reader.ReadByte();
            val.G = reader.ReadByte();
            val.B = reader.ReadByte();
            val.A = reader.ReadByte();
            return val;
        }
    }

    public class Vector2Serializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var val = (Vector2)obj;
            writer.Write(val.X);
            writer.Write(val.Y);
        }
        public override object ReadBytes(ByteReader reader)
        {
            Vector2 val;
            val.X = reader.ReadFloat();
            val.Y = reader.ReadFloat();
            return val;
        }
    }

    public class Vector3Serializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var val = (Vector3)obj;
            writer.Write(val.X);
            writer.Write(val.Y);
            writer.Write(val.Z);
        }
        public override object ReadBytes(ByteReader reader)
        {
            Vector3 val;
            val.X = reader.ReadFloat();
            val.Y = reader.ReadFloat();
            val.Z = reader.ReadFloat();
            return val;
        }
    }

    public class Vector4Serializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var val = (Vector4)obj;
            writer.Write(val.X);
            writer.Write(val.Y);
            writer.Write(val.Z);
            writer.Write(val.W);
        }
        public override object ReadBytes(ByteReader reader)
        {
            Vector4 val;
            val.X = reader.ReadFloat();
            val.Y = reader.ReadFloat();
            val.Z = reader.ReadFloat();
            val.W = reader.ReadFloat();
            return val;
        }
    }

    public class Point2Serializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var val = (Point2)obj;
            writer.Write(val.X);
            writer.Write(val.Y);
        }
        public override object ReadBytes(ByteReader reader)
        {
            Point2 val;
            val.X = reader.ReadInt();
            val.Y = reader.ReadInt();
            return val;
        }
    }

    public class Point3Serializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var val = (Point3)obj;
            writer.Write(val.X);
            writer.Write(val.Y);
            writer.Write(val.Z);
        }
        public override object ReadBytes(ByteReader reader)
        {
            Point3 val;
            val.X = reader.ReadInt();
            val.Y = reader.ReadInt();
            val.Z = reader.ReadInt();
            return val;
        }
    }

    public class RectangleSerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var val = (Rectangle)obj;
            writer.Write(val.X);
            writer.Write(val.Y);
            writer.Write(val.W);
            writer.Write(val.H);
        }
        public override object ReadBytes(ByteReader reader)
        {
            Rectangle val;
            val.X = reader.ReadFloat();
            val.Y = reader.ReadFloat();
            val.W = reader.ReadFloat();
            val.H = reader.ReadFloat();
            return val;
        }
    }

    public class RectangleISerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var val = (RectangleI)obj;
            writer.Write(val.X);
            writer.Write(val.Y);
            writer.Write(val.W);
            writer.Write(val.H);
        }
        public override object ReadBytes(ByteReader reader)
        {
            RectangleI val;
            val.X = reader.ReadInt();
            val.Y = reader.ReadInt();
            val.W = reader.ReadInt();
            val.H = reader.ReadInt();
            return val;
        }
    }

    public class CircleSerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var val = (Circle)obj;
            writer.Write(val.Center.X);
            writer.Write(val.Center.Y);
            writer.Write(val.Radius);
        }
        public override object ReadBytes(ByteReader reader)
        {
            Circle val;
            val.Center.X = reader.ReadFloat();
            val.Center.Y = reader.ReadFloat();
            val.Radius = reader.ReadFloat();
            return val;
        }
    }

    public class QuaternionSerializer : CustomSerializer
    {
        public override void WriteBytes(object obj, ByteWriter writer)
        {
            var val = (Quaternion)obj;
            writer.Write(val.X);
            writer.Write(val.Y);
            writer.Write(val.Z);
            writer.Write(val.W);
        }
        public override object ReadBytes(ByteReader reader)
        {
            Quaternion val;
            val.X = reader.ReadFloat();
            val.Y = reader.ReadFloat();
            val.Z = reader.ReadFloat();
            val.W = reader.ReadFloat();
            return val;
        }
    }
}
