using System;
using System.IO;
using System.Collections.Generic;
using Rise.OpenGL;
namespace Rise
{
    class UniformDef
    {
        public int Index;
        public string Name;
        public UniformType Type;
        public int Loc;
        public Material Uploader;

        public UniformDef(int index, string name, UniformType type, int loc)
        {
            Index = index;
            Name = name;
            Type = type;
            Loc = loc;
        }
    }

    public class Shader : ResourceHandle
    {
        internal uint id;
        internal List<UniformDef> uniforms = new List<UniformDef>();
        internal Dictionary<string, UniformDef> uniformLookup = new Dictionary<string, UniformDef>(StringComparer.Ordinal);

        public Shader(ref string source)
        {
            Compile(ref source);
        }
        public Shader(string source)
        {
            Compile(ref source);
        }
        public Shader(ref string vertSource, ref string fragSource)
        {
            Compile(ref vertSource, ref fragSource);
        }
        public Shader(string vertSource, string fragSource)
        {
            Compile(ref vertSource, ref fragSource);
        }

        protected override void Dispose()
        {
            GL.DeleteShader(id);
        }

        public int UniformCount
        {
            get { return uniforms.Count; }
        }

        public void GetUniformInfo(int index, out string name, out UniformType type)
        {
            if (index >= uniforms.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            name = uniforms[index].Name;
            type = uniforms[index].Type;
        }

        public static Shader FromFile(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException("Shader file not found.", file);
            string source = File.ReadAllText(file);
            return new Shader(ref source);
        }

        void CompileShader(uint shaderID, string source)
        {
            //Populate the shader and compile it
            GL.ShaderSource(shaderID, source);
            GL.CompileShader(shaderID);

            //Check for shader compile errors
            int status;
            GL.GetShader(shaderID, ShaderParam.CompileStatus, out status);
            if (status == 0)
                throw new Exception("Shader compile error: " + GL.GetShaderInfoLog(shaderID));
        }

        void Compile(ref string source)
        {
            int i = source.IndexOf("...", StringComparison.Ordinal);
            if (i < 0)
                throw new Exception("Shader source text must separate vertex and fragment shaders with \"...\"");

            string vertSource = source.Substring(0, i);
            string fragSource = source.Substring(i + 3);

            Compile(ref vertSource, ref fragSource);
        }
        void Compile(ref string vertSource, ref string fragSource)
        {
            //Create the shaders and compile them
            uint vertID = GL.CreateShader(ShaderType.Vertex);
            uint fragID = GL.CreateShader(ShaderType.Fragment);
            CompileShader(vertID, vertSource);
            CompileShader(fragID, fragSource);

            //Create the program and attach the shaders to it
            id = GL.CreateProgram();
            GL.AttachShader(id, vertID);
            GL.AttachShader(id, fragID);

            //Link the program and check for errors
            GL.LinkProgram(id);
            int status;
            GL.GetProgram(id, ProgramParam.LinkStatus, out status);
            if (status == 0)
                throw new Exception("Program link error: " + GL.GetProgramInfoLog(id));

            //Once linked, we can detach and delete the shaders
            GL.DetachShader(id, vertID);
            GL.DetachShader(id, fragID);
            GL.DeleteShader(vertID);
            GL.DeleteShader(fragID);

            //Get all the uniforms the shader has and store their information
            int numUniforms;
            GL.GetProgram(id, ProgramParam.ActiveUniforms, out numUniforms);
            for (int i = 0; i < numUniforms; ++i)
            {
                int count;
                UniformType type;
                string name;
                GL.GetActiveUniform(id, (uint)i, out count, out type, out name);
                if (count > 0 && name != null)
                {
                    if (count > 1)
                    {
                        name = name.Substring(0, name.LastIndexOf('['));
                        string arrName;
                        for (int n = 0; n < count; ++n)
                        {
                            arrName = $"{name}[{n}]";
                            int loc = GL.GetUniformLocation(id, arrName);
                            //Console.WriteLine("index:{0} name:{1} type:{2} loc:{3} count:{4}", i, arrName, type, loc, count);
                            var uniform = new UniformDef(i, arrName, type, loc);
                            uniforms.Add(uniform);
                            uniformLookup.Add(arrName, uniform);
                        }
                    }
                    else
                    {
                        int loc = GL.GetUniformLocation(id, name);
                        //Console.WriteLine("index:{0} name:{1} type:{2} loc:{3} count:{4}", i, name, type, loc, count);
                        var uniform = new UniformDef(i, name, type, loc);
                        uniforms.Add(uniform);
                        uniformLookup.Add(name, uniform);
                    }
                }
            }
        }

        public static readonly string Basic2D = @"#version 330
uniform mat4 Matrix;
layout(location = 0) in vec2 vertPos;
layout(location = 1) in vec2 vertUV;
layout(location = 2) in vec4 vertMult;
layout(location = 3) in vec4 vertAdd;
out vec2 fragUV;
out vec4 fragMult;
out vec4 fragAdd;
void main(void)
{
    gl_Position = Matrix * vec4(vertPos, 0.0, 1.0);
    fragUV = vertUV;
    fragMult = vertMult;
    fragAdd = vertAdd;
}
...
#version 330
uniform sampler2D Texture;
in vec2 fragUV;
in vec4 fragMult;
in vec4 fragAdd;
layout(location = 0) out vec4 outColor;
void main(void)
{
    vec4 color = texture(Texture, fragUV);
    outColor = color * fragMult + fragAdd;// * color.a;
}";


    }
}
