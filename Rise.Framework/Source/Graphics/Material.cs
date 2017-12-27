using System;
using Rise.OpenGL;
namespace Rise
{
    abstract class UniformValue
    {
        public UniformDef Def;
        public bool Changed;
        public abstract void Upload();
    }
    class UniformBool : UniformValue
    {
        public bool Val;
        public override void Upload()
        {
            GL.Uniform1I(Def.Loc, Val ? 1 : 0);
        }
    }
    class UniformInt : UniformValue
    {
        public int Val;
        public override void Upload()
        {
            GL.Uniform1I(Def.Loc, Val);
        }
    }
    class UniformFloat : UniformValue
    {
        public float Val;
        public override void Upload()
        {
            GL.Uniform1F(Def.Loc, Val);
        }
    }
    class UniformVec2 : UniformValue
    {
        public Vector2 Val;
        public override void Upload()
        {
            GL.Uniform2F(Def.Loc, Val.X, Val.Y);
        }
    }
    class UniformVec3 : UniformValue
    {
        public Vector3 Val;
        public override void Upload()
        {
            GL.Uniform3F(Def.Loc, Val.X, Val.Y, Val.Z);
        }
    }
    class UniformVec4 : UniformValue
    {
        public Vector4 Val;
        public override void Upload()
        {
            GL.Uniform4F(Def.Loc, Val.X, Val.Y, Val.Z, Val.W);
        }
    }
    class UniformMat3x2 : UniformValue
    {
        public Matrix3x2 Val;
        public unsafe override void Upload()
        {
            fixed (float* m = &Val.M0)
            {
                GL.UniformMatrix3x2FV(Def.Loc, 1, false, m);
            }
        }
    }
    class UniformMat4 : UniformValue
    {
        public Matrix4x4 Val;
        public unsafe override void Upload()
        {
            fixed (float* m = &Val.M11)
            {
                GL.UniformMatrix4FV(Def.Loc, 1, false, m);
            }
        }
    }
    class UniformSampler2D : UniformValue
    {
        public Texture Val;
        public override void Upload()
        {
            int slot = Val.Bind();
            GL.Uniform1I(Def.Loc, slot);
        }
    }

    public class Material
    {
        public Shader Shader { get; private set; }
        UniformValue[] uniforms;

        public Material(Shader shader)
        {
            Shader = shader;
            uniforms = new UniformValue[shader.uniforms.Count];
        }

        internal void Upload(bool uploadAll)
        {
            //Mark all currently binded textures to be unbinded
            Texture.MarkAllForUnbinding();

            //Upload all the uniform values
            for (int i = 0; i < uniforms.Length; ++i)
            {
                if (uploadAll || uniforms[i].Changed || uniforms[i].Def.Uploader != this)
                {
                    uniforms[i].Upload();
                    uniforms[i].Changed = false;
                    uniforms[i].Def.Uploader = this;
                }
            }

            //All textures not used by this material will be unbinded
            //But all textures that were already binded will remain so
            Texture.UnbindMarked();
        }

        T GetUniform<T>(ref string name, UniformType type) where T : UniformValue, new()
        {
            UniformDef def;

            if (!Shader.uniformLookup.TryGetValue(name, out def))
                throw new Exception($"Shader does not have uniform {name} ({type})");

            if (def.Type != type)
                throw new Exception($"Cannot assign {name} ({def.Type}) to value of type ({type})");
            
            if (uniforms[def.Index] == null)
            {
                uniforms[def.Index] = new T();
                uniforms[def.Index].Def = def;
            }
            uniforms[def.Index].Changed = true;
            return (T)uniforms[def.Index];
        }

        public void SetBool(ref string name, bool val)
        {
            var uniform = GetUniform<UniformBool>(ref name, UniformType.Bool);
            uniform.Val = val;
        }
        public void SetBool(string name, bool val)
        {
            SetBool(ref name, val);
        }

        public void SetInt(ref string name, int val)
        {
            var uniform = GetUniform<UniformInt>(ref name, UniformType.Int);
            uniform.Val = val;
        }
        public void SetInt(string name, int val)
        {
            SetInt(ref name, val);
        }

        public void SetFloat(ref string name, float val)
        {
            var uniform = GetUniform<UniformFloat>(ref name, UniformType.Float);
            uniform.Val = val;
        }
        public void SetFloat(string name, float val)
        {
            SetFloat(ref name, val);
        }

        public void SetVector2(ref string name, Vector2 val)
        {
            var uniform = GetUniform<UniformVec2>(ref name, UniformType.Vec2);
            uniform.Val = val;
        }
        public void SetVector2(string name, Vector2 val)
        {
            SetVector2(ref name, val);
        }

        public void SetVector3(ref string name, Vector3 val)
        {
            var uniform = GetUniform<UniformVec3>(ref name, UniformType.Vec3);
            uniform.Val = val;
        }
        public void SetVector3(string name, Vector3 val)
        {
            SetVector3(ref name, val);
        }

        public void SetVector4(ref string name, Vector4 val)
        {
            var uniform = GetUniform<UniformVec4>(ref name, UniformType.Vec4);
            uniform.Val = val;
        }
        public void SetVector4(string name, Vector4 val)
        {
            SetVector4(ref name, val);
        }

        public void SetColor(ref string name, Color val)
        {
            var uniform = GetUniform<UniformVec4>(ref name, UniformType.Vec4);
            uniform.Val = new Vector4(val.R / 255f, val.G / 255f, val.B / 255f, val.A / 255f);
        }
        public void SetColor(string name, Color val)
        {
            SetColor(ref name, val);
        }

        public void SetMatrix3x2(ref string name, ref Matrix3x2 val)
        {
            var uniform = GetUniform<UniformMat3x2>(ref name, UniformType.Mat3x2);
            val.CopyTo(out uniform.Val);
        }
        public void SetMatrix3x2(ref string name, Matrix3x2 val)
        {
            SetMatrix3x2(ref name, ref val);
        }
        public void SetMatrix3x2(string name, ref Matrix3x2 val)
        {
            SetMatrix3x2(ref name, ref val);
        }
        public void SetMatrix3x2(string name, Matrix3x2 val)
        {
            SetMatrix3x2(ref name, ref val);
        }

        public void SetMatrix4x4(ref string name, ref Matrix4x4 val)
        {
            var uniform = GetUniform<UniformMat4>(ref name, UniformType.Mat4);
            val.CopyTo(out uniform.Val);
        }
        public void SetMatrix4x4(ref string name, Matrix4x4 val)
        {
            SetMatrix4x4(ref name, ref val);
        }
        public void SetMatrix4x4(string name, ref Matrix4x4 val)
        {
            SetMatrix4x4(ref name, ref val);
        }
        public void SetMatrix4x4(string name, Matrix4x4 val)
        {
            SetMatrix4x4(ref name, ref val);
        }

        public void SetTexture(ref string name, Texture val)
        {
            var uniform = GetUniform<UniformSampler2D>(ref name, UniformType.Sampler2D);
            uniform.Val = val;
        }
        public void SetTexture(string name, Texture val)
        {
            SetTexture(ref name, val);
        }
    }
}
