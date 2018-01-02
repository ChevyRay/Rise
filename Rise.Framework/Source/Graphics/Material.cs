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
            int slot = Texture.Bind(Val.ID, Val.BindTarget);
            GL.Uniform1I(Def.Loc, slot);
        }
    }
    class UniformSamplerCube : UniformValue
    {
        public CubeMap Val;
        public override void Upload()
        {
            int slot = Texture.Bind(Val.id, TextureTarget.TextureCubeMap);
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
        T GetUniform<T>(int index, UniformType type) where T : UniformValue, new()
        {
            if (index < 0 || index >= uniforms.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            var def = Shader.uniforms[index];

            if (def.Type != type)
                throw new Exception($"Cannot assign {def.Name} ({def.Type}) to value of type ({type})");
            
            if (uniforms[def.Index] == null)
            {
                uniforms[def.Index] = new T();
                uniforms[def.Index].Def = def;
            }
            uniforms[def.Index].Changed = true;
            return (T)uniforms[def.Index];

        }

        public int GetIndex(ref string name)
        {
            int index;
            if (!TryGetIndex(ref name, out index))
                throw new Exception("Material does not have uniform with name: " + name);
            return index;
        }
        public int GetIndex(string name)
        {
            return GetIndex(ref name);
        }
        public bool TryGetIndex(ref string name, out int index)
        {
            UniformDef def;
            if (!Shader.uniformLookup.TryGetValue(name, out def))
            {
                index = -1;
                return false;
            }
            index = def.Index;
            return true;
        }
        public bool TryGetIndex(string name, out int index)
        {
            return TryGetIndex(ref name, out index);
        }

        public void SetBool(ref string name, bool val)
        {
            GetUniform<UniformBool>(ref name, UniformType.Bool).Val = val;
        }
        public void SetBool(string name, bool val)
        {
            SetBool(ref name, val);
        }
        public void SetBool(int index, bool val)
        {
            GetUniform<UniformBool>(index, UniformType.Bool).Val = val;
        }

        public void SetInt(ref string name, int val)
        {
            GetUniform<UniformInt>(ref name, UniformType.Int).Val = val;
        }
        public void SetInt(string name, int val)
        {
            SetInt(ref name, val);
        }
        public void SetInt(int index, int val)
        {
            GetUniform<UniformInt>(index, UniformType.Int).Val = val;
        }

        public void SetFloat(ref string name, float val)
        {
            GetUniform<UniformFloat>(ref name, UniformType.Float).Val = val;
        }
        public void SetFloat(string name, float val)
        {
            SetFloat(ref name, val);
        }
        public void SetFloat(int index, float val)
        {
            GetUniform<UniformFloat>(index, UniformType.Float).Val = val;
        }

        public void SetVector2(ref string name, Vector2 val)
        {
            GetUniform<UniformVec2>(ref name, UniformType.Vec2).Val = val;
        }
        public void SetVector2(string name, Vector2 val)
        {
            SetVector2(ref name, val);
        }
        public void SetVector2(int index, Vector2 val)
        {
            GetUniform<UniformVec2>(index, UniformType.Vec2).Val = val;
        }

        public void SetVector3(ref string name, Vector3 val)
        {
            GetUniform<UniformVec3>(ref name, UniformType.Vec3).Val = val;
        }
        public void SetVector3(string name, Vector3 val)
        {
            SetVector3(ref name, val);
        }
        public void SetVector3(int index, Vector2 val)
        {
            GetUniform<UniformVec3>(index, UniformType.Vec3).Val = val;
        }

        public void SetVector4(ref string name, Vector4 val)
        {
            var uniform = GetUniform<UniformVec4>(ref name, UniformType.Vec4).Val = val;
        }
        public void SetVector4(string name, Vector4 val)
        {
            SetVector4(ref name, val);
        }
        public void SetVector4(int index, Vector2 val)
        {
            GetUniform<UniformVec4>(index, UniformType.Vec4).Val = val;
        }

        public void SetColor4(ref string name, Color4 val)
        {
            GetUniform<UniformVec4>(ref name, UniformType.Vec4).Val = new Vector4(val.R / 255f, val.G / 255f, val.B / 255f, val.A / 255f);
        }
        public void SetColor4(string name, Color4 val)
        {
            SetColor4(ref name, val);
        }
        public void SetColor4(int index, Color4 val)
        {
            GetUniform<UniformVec4>(index, UniformType.Vec4).Val = new Vector4(val.R / 255f, val.G / 255f, val.B / 255f, val.A / 255f);
        }

        public void SetColor3(ref string name, Color3 val)
        {
            GetUniform<UniformVec3>(ref name, UniformType.Vec3).Val = new Vector3(val.R / 255f, val.G / 255f, val.B / 255f);
        }
        public void SetColor3(string name, Color3 val)
        {
            SetColor3(ref name, val);
        }
        public void SetColor3(int index, Color3 val)
        {
            GetUniform<UniformVec3>(index, UniformType.Vec3).Val = new Vector3(val.R / 255f, val.G / 255f, val.B / 255f);
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
        public void SetMatrix3x2(int index, ref Matrix3x2 val)
        {
            var uniform = GetUniform<UniformMat3x2>(index, UniformType.Mat3x2);
            val.CopyTo(out uniform.Val);
        }
        public void SetMatrix3x2(int index, Matrix3x2 val)
        {
            var uniform = GetUniform<UniformMat3x2>(index, UniformType.Mat3x2);
            val.CopyTo(out uniform.Val);
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
        public void SetMatrix4x4(int index, ref Matrix4x4 val)
        {
            var uniform = GetUniform<UniformMat4>(index, UniformType.Mat4);
            val.CopyTo(out uniform.Val);
        }
        public void SetMatrix4x4(int index, Matrix4x4 val)
        {
            var uniform = GetUniform<UniformMat4>(index, UniformType.Mat4);
            val.CopyTo(out uniform.Val);
        }

        public void SetTexture(ref string name, Texture val)
        {
            GetUniform<UniformSampler2D>(ref name, UniformType.Sampler2D).Val = val;
        }
        public void SetTexture(string name, Texture val)
        {
            SetTexture(ref name, val);
        }
        public void SetTexture(int index, Texture val)
        {
            GetUniform<UniformSampler2D>(index, UniformType.Sampler2D).Val = val;
        }

        public void SetCubeMap(ref string name, CubeMap val)
        {
            GetUniform<UniformSamplerCube>(ref name, UniformType.SamplerCube).Val = val;
        }
        public void SetCubeMap(string name, CubeMap val)
        {
            SetCubeMap(ref name, val);
        }
        public void SetCubeMap(int index, CubeMap val)
        {
            GetUniform<UniformSamplerCube>(index, UniformType.SamplerCube).Val = val;
        }
    }
}
