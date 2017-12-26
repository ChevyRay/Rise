using System;
namespace Rise
{
	public struct Quaternion : IEquatable<Quaternion>
	{
		public static readonly Quaternion Identity = new Quaternion(0f, 0f, 0f, 1f);

		public float X;
		public float Y;
		public float Z;
		public float W;

		public Quaternion(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}
		public Quaternion(Vector3 axis, float w)
		{
			X = axis.X;
			Y = axis.Y;
			Z = axis.Z;
			W = w;
		}

		public float Length
		{
			get { return (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W); }
		}

		public float LengthSquared
		{
			get { return X * X + Y * Y + Z * Z + W * W; }
		}

		public Quaternion Inverse
		{
			get
			{
				float mult = 1f / (X * X + Y * Y + Z * Z + W * W);
				return new Quaternion(-X * mult, -Y * mult, -Z * mult, W * mult);
			}
		}

		public Quaternion Normalized
		{
			get
			{
				float mult = 1f / ((float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W));
				return new Quaternion(X * mult, Y * mult, Z * mult, W * mult);
			}
		}

		public override string ToString()
		{
			return string.Format("{0},{1},{2},{3}", X, Y, Z, W);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		public bool Equals(ref Quaternion other)
		{
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}
		public bool Equals(Quaternion other)
		{
			return Equals(ref other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + X.GetHashCode();
				hash = hash * 23 + Y.GetHashCode();
				hash = hash * 23 + Z.GetHashCode();
				hash = hash * 23 + W.GetHashCode();
				return hash;
			}
		}

		public Vector3 TransformPoint(Vector3 point)
		{
			float x = (Y * point.Z - Z * point.Y) * 2f;
			float y = (Z * point.X - X * point.Z) * 2f;
			float z = (X * point.Y - Y * point.X) * 2f;
			return new Vector3(
				point.X + x * W + (Y * z - Z * y),
				point.Y + y * W + (Z * x - X * z),
				point.Z + z * W + (X * y - Y * x)
			);
		}

		public static Quaternion Concat(ref Quaternion a, ref Quaternion b)
		{
			return new Quaternion(
				((b.X * a.W) + (a.X * b.W)) + ((b.Y * a.Z) - (b.Z * a.Y)),
				((b.Y * a.W) + (a.Y * b.W)) + ((b.Z * a.X) - (b.X * a.Z)),
				((b.Z * a.W) + (a.Z * b.W)) + ((b.X * a.Y) - (b.Y * a.X)),
				(b.W * a.W) - (((b.X * a.X) + (b.Y * a.Y)) + (b.Z * a.Z))
			);
		}

		public static Quaternion AxisAngle(Vector3 axis, float angle)
		{
			float sin = (float)Math.Sin(angle * 0.5f);
			float cos = (float)Math.Cos(angle * 0.5f);
			return new Quaternion(axis.X * sin, axis.Y * sin, axis.Z * sin, cos);
		}

		public static Quaternion Euler(Vector3 axes)
		{
			return Euler(axes.X, axes.Y, axes.Z);
		}
		public static Quaternion Euler(ref Vector3 axes)
		{
			return Euler(axes.X, axes.Y, axes.Z);
		}
		public static void Euler(float x, float y, float z, out Quaternion result)
		{
			float sinX = (float)Math.Sin(x * Calc.Rad * 0.5f);
			float cosX = (float)Math.Cos(x * Calc.Rad * 0.5f);
			float sinY = (float)Math.Sin(y * Calc.Rad * 0.5f);
			float cosY = (float)Math.Cos(y * Calc.Rad * 0.5f);
			float sinZ = (float)Math.Sin(z * Calc.Rad * 0.5f);
			float cosZ = (float)Math.Cos(z * Calc.Rad * 0.5f);
			result.X = cosY * sinX * cosZ + sinY * cosX * sinZ;
			result.Y = sinY * cosX * cosZ - cosY * sinX * sinZ;
			result.Z = cosY * cosX * sinZ - sinY * sinX * cosZ;
			result.W = cosY * cosX * cosZ + sinY * sinX * sinZ;
		}
		public static Quaternion Euler(float x, float y, float z)
		{
			Quaternion result;
			Euler(x, y, z, out result);
			return result;
		}

		public static Quaternion FromTo(ref Vector3 a, ref Vector3 b)
		{
			const float epsilon = 1e-6f;
			var norms = (float)Math.Sqrt(a.LengthSquared * b.LengthSquared);
			var real = norms + Vector3.Dot(ref a, ref b);
			Quaternion result;
			if (real < epsilon * norms)
				result = Math.Abs(a.X) > Math.Abs(a.Z) ? new Quaternion(-a.Y, a.X, 0f, 0f) : new Quaternion(0f, -a.Z, a.Y, 0f);
			else
				result = new Quaternion(Vector3.Cross(a, b), real);
			return result.Normalized;
		}
		public static Quaternion FromTo(Vector3 a, Vector3 b)
		{
			return FromTo(ref a, ref b);
		}

		public static Quaternion Lerp(ref Quaternion a, ref Quaternion b, float t)
		{
			float t2 = 1f - t;
			Quaternion result;
			if (a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W >= 0f)
			{
				result.X = (t2 * a.X) + (t * b.X);
				result.Y = (t2 * a.Y) + (t * b.Y);
				result.Z = (t2 * a.Z) + (t * b.Z);
				result.W = (t2 * a.W) + (t * b.W);
			}
			else
			{
				result.X = (t2 * a.X) - (t * b.X);
				result.Y = (t2 * a.Y) - (t * b.Y);
				result.Z = (t2 * a.Z) - (t * b.Z);
				result.W = (t2 * a.W) - (t * b.W);
			}
			float num3 = 1f / (float)Math.Sqrt(result.X * result.X + result.Y * result.Y + result.Z * result.Z + result.W * result.W);
			result.X *= num3;
			result.Y *= num3;
			result.Z *= num3;
			result.W *= num3;
			return result;
		}
		public static Quaternion Lerp(Quaternion a, Quaternion b, float t)
		{
			return Lerp(ref a, ref b, t);
		}

		public static Quaternion Slerp(ref Quaternion a, ref Quaternion b, float t)
		{
			Quaternion result;
			float t2, t3;
			float t4 = a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
			bool flag = false;
			if (t4 < 0f)
			{
				flag = true;
				t4 = -t4;
			}
			if (t4 > 0.999999f)
			{
				t3 = 1f - t;
				t2 = flag ? -t : t;
			}
			else
			{
				float t5 = (float)Math.Acos((double)t4);
				float t6 = (float)(1.0 / Math.Sin((double)t5));
				t3 = ((float)Math.Sin((double)((1f - t) * t5))) * t6;
				t2 = flag ? (((float)-Math.Sin((double)(t * t5))) * t6) : (((float)Math.Sin((double)(t * t5))) * t6);
			}
			result.X = (t3 * a.X) + (t2 * b.X);
			result.Y = (t3 * a.Y) + (t2 * b.Y);
			result.Z = (t3 * a.Z) + (t2 * b.Z);
			result.W = (t3 * a.W) + (t2 * b.W);
			return result;
		}
		public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
		{
			return Slerp(ref a, ref b, t);
		}

		public static void Multiply(ref Quaternion a, ref Quaternion b, out Quaternion result)
		{
			result.X = a.X * b.W + b.X * a.W + (a.Y * b.Z - a.Z * b.Y);
			result.Y = a.Y * b.W + b.Y * a.W + (a.Z * b.X - a.X * b.Z);
			result.Z = a.Z * b.W + b.Z * a.W + (a.X * b.Y - a.Y * b.X);
			result.W = a.W * b.W - (a.X * b.X + a.Y * b.Y + a.Z * b.Z);
		}
		public static Quaternion Multiply(Quaternion a, Quaternion b)
		{
			Quaternion result;
			Multiply(ref a, ref b, out result);
			return result;
		}

		public static bool operator ==(Quaternion a, Quaternion b)
		{
			return a.Equals(ref b);
		}
		public static bool operator !=(Quaternion a, Quaternion b)
		{
			return !a.Equals(ref b);
		}
		public static Quaternion operator *(Quaternion a, Quaternion b)
		{
			Quaternion result;
			Multiply(ref a, ref b, out result);
			return result;
		}
		public static Quaternion operator *(Quaternion a, float b)
		{
			return new Quaternion(a.X * b, a.Y * b, a.Z * b, a.W * b);
		}
		public static Quaternion operator /(Quaternion a, float b)
		{
			return new Quaternion(a.X / b, a.Y / b, a.Z / b, a.W / b);
		}
	}
}
