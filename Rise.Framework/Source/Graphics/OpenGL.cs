using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using GLSizei = System.Int32;
using GLEnum = System.UInt32;

namespace Rise.OpenGL
{
    public static class GL
    {
        static int majorVersion = 0x821B;
        static int minorVersion = 0x821C;
        static int maxColorAttachments = 0x8CDF;
        static int maxCubeMapTextureSize = 0x851C;
        static int maxDrawBuffers = 0x8824;
        static int maxElementIndices = 0x80E9;
        static int maxElementVertices = 0x80E8;
        static int maxRenderbufferSize = 0x84E8;
        static int maxSamples = 0x8D57;
        static int maxTextureImageUnits = 0x8872;
        static int maxTextureSize = 0x0D33;

        public static int MajorVersion { get { return majorVersion; } }
        public static int MinorVersion { get { return minorVersion; } }
        public static int MaxColorAttachments { get { return maxColorAttachments; } }
        public static int MaxCubeMapTextureSize { get { return maxCubeMapTextureSize; } }
        public static int MaxDrawBuffers { get { return maxDrawBuffers; } }
        public static int MaxElementIndices { get { return maxElementIndices; } }
        public static int MaxElementVertices { get { return maxElementVertices; } }
        public static int MaxRenderbufferSize { get { return maxRenderbufferSize; } }
        public static int MaxSamples { get { return maxSamples; } }
        public static int MaxTextureImageUnits { get { return maxTextureImageUnits; } }
        public static int MaxTextureSize { get { return maxTextureSize; } }

        public static void Init()
        {
            foreach (var field in typeof(GL).GetFields(BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (field.Name.StartsWith("gl", StringComparison.Ordinal))
                {
                    var addr = App.platform.GetProcAddress(field.Name);
                    if (addr == IntPtr.Zero)
                        throw new Exception("OpenGL function not available: " + field.Name);

                    var del = Marshal.GetDelegateForFunctionPointer(addr, field.FieldType);
                    field.SetValue(null, del);
                }
            }

            GetIntegerV((GLEnum)majorVersion, out majorVersion);
            GetIntegerV((GLEnum)minorVersion, out minorVersion);
            GetIntegerV((GLEnum)maxColorAttachments, out maxColorAttachments);
            GetIntegerV((GLEnum)maxCubeMapTextureSize, out maxCubeMapTextureSize);
            GetIntegerV((GLEnum)maxDrawBuffers, out maxDrawBuffers);
            GetIntegerV((GLEnum)maxElementIndices, out maxElementIndices);
            GetIntegerV((GLEnum)maxElementVertices, out maxElementVertices);
            GetIntegerV((GLEnum)maxRenderbufferSize, out maxRenderbufferSize);
            GetIntegerV((GLEnum)maxSamples, out maxSamples);
            GetIntegerV((GLEnum)maxTextureImageUnits, out maxTextureImageUnits);
            GetIntegerV((GLEnum)maxTextureSize, out maxTextureSize);
        }

        #pragma warning disable 0649

        [Conditional("DEBUG")]
        static void CheckError()
        {
            var err = glGetError();
            Debug.Assert(err == ErrorCode.NoError, string.Format("OpenGL error: {0} ({1:X})", err.ToString(), (int)err));
        }

        delegate ErrorCode _glGetError();
        static _glGetError glGetError;

        delegate void _glEnable(EnableCap mode);
        static _glEnable glEnable;
        public static void Enable(EnableCap mode)
        {
            glEnable(mode);
            CheckError();
        }

        delegate void _glDisable(EnableCap mode);
        static _glDisable glDisable;
        public static void Disable(EnableCap mode)
        {
            glDisable(mode);
            CheckError();
        }

        delegate void _glClear(BufferBit mask);
        static _glClear glClear;
        public static void Clear(BufferBit mask)
        {
            glClear(mask);
            CheckError();
        }

        delegate void _glClearColor(float red, float green, float blue, float alpha);
        static _glClearColor glClearColor;
        public static void ClearColor(float red, float green, float blue, float alpha)
        {
            glClearColor(red, green, blue, alpha);
            CheckError();
        }

        delegate void _glClearDepth(double value);
        static _glClearDepth glClearDepth;
        public static void ClearDepth(float value)
        {
            glClearDepth(value);
            CheckError();
        }

        delegate void _glDepthRange(double min, double max);
        static _glDepthRange glDepthRange;
        public static void DepthRange(float min, float max)
        {
            glDepthRange(min, max);
            CheckError();
        }

        delegate void _glDepthMask(bool flag);
        static _glDepthMask glDepthMask;
        public static void DepthMask(bool flag)
        {
            glDepthMask(flag);
            CheckError();
        }

        delegate void _glViewport(int x, int y, GLSizei width, GLSizei height);
        static _glViewport glViewport;
        public static void Viewport(int x, int y, GLSizei width, GLSizei height)
        {
            glViewport(x, y, width, height);
            CheckError();
        }

        delegate void _glCullFace(CullFace face);
        static _glCullFace glCullFace;
        public static void CullFace(CullFace face)
        {
            glCullFace(face);
            CheckError();
        }

        unsafe delegate void _glGetIntegerv(GLEnum name, int* data);
        static _glGetIntegerv glGetIntegerv;
        static unsafe void GetIntegerV(GLEnum name, out int val)
        {
            fixed (int* p = &val)
            {
                glGetIntegerv(name, p);
                val = *p;
            }
            CheckError();
        }

        delegate void _glBlendEquation(BlendEquation eq);
        static _glBlendEquation glBlendEquation;
        public static void BlendEquation(BlendEquation eq)
        {
            glBlendEquation(eq);
            CheckError();
        }

        delegate void _glBlendFunc(BlendFactor sfactor, BlendFactor dfactor);
        static _glBlendFunc glBlendFunc;
        public static void BlendFunc(BlendFactor sFactor, BlendFactor dFactor)
        {
            glBlendFunc(sFactor, dFactor);
            CheckError();
        }

        unsafe delegate void _glGenTextures(GLSizei n, uint* textures);
        static _glGenTextures glGenTextures;
        unsafe public static void GenTextures(GLSizei n, uint[] textures)
        {
            fixed (uint* ptr = textures) { glGenTextures(n, ptr); }
            CheckError();
        }
        unsafe public static uint GenTexture()
        {
            uint texture = 0;
            glGenTextures(1, &texture);
            CheckError();
            return texture;
        }

        unsafe delegate void _glDeleteTextures(GLSizei n, uint* textures);
        static _glDeleteTextures glDeleteTextures;
        unsafe public static void DeleteTextures(GLSizei n, uint[] textures)
        {
            fixed (uint* ptr = textures) { glDeleteTextures(n, ptr); }
            CheckError();
        }
        unsafe public static void DeleteTexture(uint texture)
        {
            glDeleteTextures(1, &texture);
            CheckError();
        }

        public const int MaxTextureUnits = 32;
        static GLEnum texture0 = 0x84C0;

        delegate void _glActiveTexture(uint unit);
        static _glActiveTexture glActiveTexture;
        public static void ActiveTexture(uint unit)
        {
            if (unit >= MaxTextureUnits)
                throw new Exception("Active texture must be 0-" + (MaxTextureUnits - 1));

            glActiveTexture(texture0 + unit);
            CheckError();
        }

        delegate void _glBindTexture(TextureTarget target, uint texture);
        static _glBindTexture glBindTexture;
        public static void BindTexture(TextureTarget target, uint texture)
        {
            glBindTexture(target, texture);
            CheckError();
        }

        delegate void _glTexParameteri(TextureTarget target, TextureParam name, int param);
        static _glTexParameteri glTexParameteri;
        public static void TexParameterI(TextureTarget target, TextureParam name, int param)
        {
            glTexParameteri(target, name, param);
            CheckError();
        }

        delegate void _glGetTexParameteriv(TextureTarget target, TextureParam name, out int result);
        static _glGetTexParameteriv glGetTexParameteriv;
        public static void GetTexParameterI(TextureTarget target, TextureParam name, out int result)
        {
            glGetTexParameteriv(target, name, out result);
            CheckError();
        }

        delegate void _glTexImage2D(TextureTarget target, int level, int internalFormat, GLSizei width, GLSizei height, int border, PixelFormat format, PixelType type, IntPtr data);
        static _glTexImage2D glTexImage2D;
        public static void TexImage2D(TextureTarget target, int level, TextureFormat internalFormat, GLSizei width, GLSizei height, int border, PixelFormat format, PixelType type, IntPtr data)
        {
            glTexImage2D(target, level, (int)internalFormat, width, height, border, format, type, data);
            CheckError();
        }

        delegate void _glGetTexImage(TextureTarget target, int level, PixelFormat format, PixelType type, IntPtr data);
        static _glGetTexImage glGetTexImage;
        public static void GetTexImage(TextureTarget target, int level, PixelFormat format, PixelType type, IntPtr data)
        {
            glGetTexImage(target, level, format, type, data);
            CheckError();
        }

        delegate uint _glCreateShader(ShaderType type);
        static _glCreateShader glCreateShader;
        public static uint CreateShader(ShaderType type)
        {
            var shader = glCreateShader(type);
            CheckError();
            return shader;
        }

        delegate void _glDeleteShader(uint shader);
        static _glDeleteShader glDeleteShader;
        public static void DeleteShader(uint shader)
        {
            glDeleteShader(shader);
            CheckError();
        }

        delegate void _glAttachShader(uint program, uint shader);
        static _glAttachShader glAttachShader;
        public static void AttachShader(uint program, uint shader)
        {
            glAttachShader(program, shader);
            CheckError();
        }

        delegate void _glDetachShader(uint program, uint shader);
        static _glDetachShader glDetachShader;
        public static void DetachShader(uint program, uint shader)
        {
            glDetachShader(program, shader);
            CheckError();
        }

        delegate void _glShaderSource(uint shader, GLSizei count, string[] source, int[] length);
        static _glShaderSource glShaderSource;
        public static void ShaderSource(uint shader, string source)
        {
            var sourceArr = new string[] { source };
            var lengthArr = new int[] { source.Length };
            glShaderSource(shader, 1, sourceArr, lengthArr);
            CheckError();
        }

        delegate void _glCompileShader(uint shader);
        static _glCompileShader glCompileShader;
        public static void CompileShader(uint shader)
        {
            glCompileShader(shader);
            CheckError();
        }

        delegate void _glGetShaderiv(uint shader, ShaderParam pname, out int result);
        static _glGetShaderiv glGetShaderiv;
        public static void GetShader(uint shader, ShaderParam pname, out int result)
        {
            glGetShaderiv(shader, pname, out result);
            CheckError();
        }

        delegate void _glGetShaderInfoLog(uint shader, GLSizei maxLength, out GLSizei length, byte[] infoLog);
        static _glGetShaderInfoLog glGetShaderInfoLog;
        public static string GetShaderInfoLog(uint shader)
        {
            int len;
            GetShader(shader, ShaderParam.InfoLogLength, out len);
            var bytes = new byte[len];
            glGetShaderInfoLog(shader, len, out len, bytes);
            CheckError();
            return Encoding.UTF8.GetString(bytes);
        }

        delegate uint _glCreateProgram();
        static _glCreateProgram glCreateProgram;
        public static uint CreateProgram()
        {
            var program = glCreateProgram();
            CheckError();
            return program;
        }

        delegate void _glDeleteProgram(uint program);
        static _glDeleteProgram glDeleteProgram;
        public static void DeleteProgram(uint program)
        {
            glDeleteProgram(program);
            CheckError();
        }

        delegate void _glLinkProgram(uint program);
        static _glLinkProgram glLinkProgram;
        public static void LinkProgram(uint program)
        {
            glLinkProgram(program);
            CheckError();
        }

        delegate void _glGetProgramiv(uint program, ProgramParam pname, out int result);
        static _glGetProgramiv glGetProgramiv;
        public static void GetProgram(uint program, ProgramParam pname, out int result)
        {
            glGetProgramiv(program, pname, out result);
            CheckError();
        }

        delegate void _glGetProgramInfoLog(uint program, GLSizei maxLength, out GLSizei length, byte[] infoLog);
        static _glGetProgramInfoLog glGetProgramInfoLog;
        public static string GetProgramInfoLog(uint program)
        {
            int len;
            GetProgram(program, ProgramParam.InfoLogLength, out len);
            var bytes = new byte[len];
            glGetProgramInfoLog(program, len, out len, bytes);
            CheckError();
            return Encoding.UTF8.GetString(bytes);
        }

        static byte[] uniformName = new byte[32];
        static UniformType[] validUniformTypes;

        unsafe delegate void _glGetActiveUniform(uint program, uint index, GLSizei bufSize, out GLSizei length, out int size, out UniformType type, byte* name);
        static _glGetActiveUniform glGetActiveUniform;
        public static unsafe void GetActiveUniform(uint program, uint index, out int size, out UniformType type, out string name)
        {
            if (validUniformTypes == null)
                validUniformTypes = (UniformType[])Enum.GetValues(typeof(UniformType));
            GLSizei length;
            fixed (byte* ptr = uniformName)
            {
                glGetActiveUniform(program, index, uniformName.Length, out length, out size, out type, ptr);
                name = length > 0 ? Encoding.UTF8.GetString(ptr, length) : null;
                if (!validUniformTypes.Contains(type))
                    size = 0;
            }
            CheckError();
        }

        delegate void _glUseProgram(uint program);
        static _glUseProgram glUseProgram;
        public static void UseProgram(uint program)
        {
            glUseProgram(program);
            CheckError();
        }

        delegate void _glBindAttribLocation(uint program, uint index, string name);
        static _glBindAttribLocation glBindAttribLocation;
        public static void BindAttribLocation(uint program, uint index, string name)
        {
            glBindAttribLocation(program, index, name);
            CheckError();
        }

        delegate int _glGetUniformLocation(uint program, string name);
        static _glGetUniformLocation glGetUniformLocation;
        public static int GetUniformLocation(uint program, string name)
        {
            var loc = glGetUniformLocation(program, name);
            CheckError();
            return loc;
        }

        delegate void _glVertexAttribPointer(uint index, int size, VertexType type, bool normalized, GLSizei stride, IntPtr pointer);
        static _glVertexAttribPointer glVertexAttribPointer;
        public static void VertexAttribPointer(uint index, int size, VertexType type, bool normalized, GLSizei stride, IntPtr pointer)
        {
            glVertexAttribPointer(index, size, type, normalized, stride, pointer);
            CheckError();
        }

        delegate void _glEnableVertexAttribArray(uint index);
        static _glEnableVertexAttribArray glEnableVertexAttribArray;
        public static void EnableVertexAttribArray(uint index)
        {
            glEnableVertexAttribArray(index);
            CheckError();
        }

        delegate void _glDisableVertexAttribArray(uint index);
        static _glDisableVertexAttribArray glDisableVertexAttribArray;
        public static void DisableVertexAttribArray(uint index)
        {
            glDisableVertexAttribArray(index);
            CheckError();
        }

        unsafe delegate void _glGenBuffers(GLSizei n, uint* buffers);
        static _glGenBuffers glGenBuffers;
        unsafe public static void GenBuffers(GLSizei n, uint[] buffers)
        {
            fixed (uint* ptr = buffers) { glGenBuffers(n, ptr); }
            CheckError();
        }
        public static void GenBuffers(uint[] buffers)
        {
            GenBuffers(buffers.Length, buffers);
        }
        unsafe public static uint GenBuffer()
        {
            uint buffer = 0;
            glGenBuffers(1, &buffer);
            CheckError();
            return buffer;
        }

        unsafe delegate void _glDeleteBuffers(GLSizei n, uint* buffers);
        static _glDeleteBuffers glDeleteBuffers;
        unsafe public static void DeleteBuffers(GLSizei n, uint[] buffers)
        {
            fixed (uint* ptr = buffers) { glDeleteBuffers(n, ptr); }
            CheckError();
        }
        public static void DeleteBuffers(uint[] buffers)
        {
            DeleteBuffers((GLSizei)buffers.Length, buffers);
        }
        unsafe public static void DeleteBuffer(uint buffer)
        {
            glDeleteBuffers(1, &buffer);
            CheckError();
        }

        delegate void _glBindBuffer(BufferTarget target, uint buffer);
        static _glBindBuffer glBindBuffer;
        public static void BindBuffer(BufferTarget target, uint buffer)
        {
            glBindBuffer(target, buffer);
            CheckError();
        }

        unsafe delegate void _glGenVertexArrays(GLSizei n, uint* arrays);
        static _glGenVertexArrays glGenVertexArrays;
        unsafe public static void GenVertexArrays(GLSizei n, uint[] arrays)
        {
            fixed (uint* ptr = arrays) { glGenVertexArrays(n, ptr); }
            CheckError();
        }
        unsafe public static uint GenVertexArray()
        {
            uint arr = 0;
            glGenVertexArrays(1, &arr);
            CheckError();
            return arr;
        }

        unsafe delegate void _glDeleteVertexArrays(GLSizei n, uint* arrays);
        static _glDeleteVertexArrays glDeleteVertexArrays;
        unsafe public static void DeleteVertexArrays(GLSizei n, uint[] arrays)
        {
            fixed (uint* ptr = arrays) { glDeleteVertexArrays(n, ptr); }
            CheckError();
        }
        unsafe public static void DeleteVertexArray(uint array)
        {
            glDeleteVertexArrays(1, &array);
            CheckError();
        }

        delegate void _glBindVertexArray(uint array);
        static _glBindVertexArray glBindVertexArray;
        public static void BindVertexArray(uint array)
        {
            glBindVertexArray(array);
            CheckError();
        }

        delegate void _glBufferData(BufferTarget target, IntPtr size, IntPtr data, BufferUsage usage);
        static _glBufferData glBufferData;
        public static void BufferData(BufferTarget target, int size, IntPtr data, BufferUsage usage)
        {
            glBufferData(target, new IntPtr(size), data, usage);
            CheckError();
        }
        public static void BufferData<T>(BufferTarget target, int size, T[] data, BufferUsage usage) where T : struct
        {
            var dataPtr = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
            BufferData(target, size, dataPtr, usage);
        }

        unsafe delegate void _glGenFramebuffers(GLSizei n, uint* framebuffers);
        static _glGenFramebuffers glGenFramebuffers;
        unsafe public static void GenFramebuffers(GLSizei n, uint[] framebuffers)
        {
            fixed (uint* ptr = framebuffers) { glGenFramebuffers(n, ptr); }
            CheckError();
        }
        unsafe public static uint GenFramebuffer()
        {
            uint fbo = 0;
            glGenFramebuffers(1, &fbo);
            CheckError();
            return fbo;
        }

        unsafe delegate void _glDeleteFramebuffers(GLSizei n, uint* framebuffers);
        static _glDeleteFramebuffers glDeleteFramebuffers;
        unsafe public static void DeleteFramebuffers(GLSizei n, uint[] framebuffers)
        {
            fixed (uint* ptr = framebuffers) { glDeleteFramebuffers(n, ptr); }
            CheckError();
        }
        unsafe public static void DeleteFramebuffer(uint framebuffer)
        {
            glDeleteFramebuffers(1, &framebuffer);
            CheckError();
        }

        delegate void _glBindFramebuffer(FramebufferTarget target, uint framebuffer);
        static _glBindFramebuffer glBindFramebuffer;
        public static void BindFramebuffer(FramebufferTarget target, uint framebuffer)
        {
            glBindFramebuffer(target, framebuffer);
            CheckError();
        }

        delegate void _glFramebufferTexture2D(FramebufferTarget target, TextureAttachment attachment, TextureTarget textarget, uint texture, int level);
        static _glFramebufferTexture2D glFramebufferTexture2D;
        public static void FramebufferTexture2D(FramebufferTarget target, TextureAttachment attachment, TextureTarget textarget, uint texture, int level)
        {
            if (attachment != TextureAttachment.Depth)
            {
                uint texn = (uint)attachment - (uint)TextureAttachment.Color0;
                if (texn >= maxColorAttachments)
                    throw new Exception("Exceeding max color attachments: " + maxColorAttachments);
            }
            glFramebufferTexture2D(target, attachment, textarget, texture, level);
            CheckError();
        }

        unsafe delegate void _glDrawBuffers(GLSizei n, DrawBuffer* bufs);
        static _glDrawBuffers glDrawBuffers;
        unsafe public static void DrawBuffers(GLSizei n, DrawBuffer[] bufs)
        {
            if (n > bufs.Length)
                throw new Exception("Not enough buffers in array.");
            if (n > maxDrawBuffers)
                throw new Exception("Exceeded maximum number of draw buffers: " + maxDrawBuffers);
            
            fixed (DrawBuffer* ptr = bufs) { glDrawBuffers(n, ptr); }
            CheckError();
        }

        delegate void _glReadBuffer(ReadBuffer buffer);
        static _glReadBuffer glReadBuffer;
        public static void ReadBuffer(ReadBuffer buffer)
        {
            glReadBuffer(buffer);
            CheckError();
        }

        unsafe delegate void _glReadPixels(int x, int y, GLSizei w, GLSizei h, PixelFormat format, PixelType type, byte* data);
        static _glReadPixels glReadPixels;
        unsafe public static void ReadPixels(RectangleI rect, byte[] data)
        {
            if (data.Length < rect.Area)
                throw new Exception("Data array is not large enough.");
            fixed (byte* ptr = data) { glReadPixels(rect.X, rect.Y, rect.W, rect.H, PixelFormat.RGBA, PixelType.UnsignedByte, ptr); }
            CheckError();
        }

        unsafe delegate void _glGenRenderbuffers(GLSizei n, uint* renderbuffers);
        static _glGenRenderbuffers glGenRenderbuffers;
        unsafe public static void GenRenderbuffers(GLSizei n, uint[] renderbuffers)
        {
            fixed (uint* ptr = renderbuffers) { glGenFramebuffers(n, ptr); }
            CheckError();
        }
        unsafe public static uint GenRenderbuffer()
        {
            uint rbo = 0;
            glGenRenderbuffers(1, &rbo);
            CheckError();
            return rbo;
        }

        const uint GL_RENDERBUFFER = 0x8D41;

        unsafe delegate void _glBindRenderbuffer(GLEnum target, uint buffer);
        static _glBindRenderbuffer glBindRenderbuffer;
        unsafe public static void BindRenderbuffer(uint buffer)
        {
            glBindRenderbuffer(GL_RENDERBUFFER, buffer);
            CheckError();
        }

        unsafe delegate void _glRenderbufferStorage(GLEnum target, TextureFormat format, int width, int height);
        static _glRenderbufferStorage glRenderbufferStorage;
        unsafe public static void RenderbufferStorage(TextureFormat format, int width, int height)
        {
            glRenderbufferStorage(GL_RENDERBUFFER, format, width, height);
            CheckError();
        }

        unsafe delegate void _glFramebufferRenderbuffer(FramebufferTarget target, TextureAttachment attachment, GLEnum renderbufferTarget, uint renderbuffer);
        static _glFramebufferRenderbuffer glFramebufferRenderbuffer;
        unsafe public static void FramebufferRenderbuffer(FramebufferTarget target, TextureAttachment attachment, uint renderbuffer)
        {
            glFramebufferRenderbuffer(target, attachment, GL_RENDERBUFFER, renderbuffer);
            CheckError();
        }

        delegate void _glDrawElements(DrawMode mode, GLSizei count, IndexType type, IntPtr indices);
        static _glDrawElements glDrawElements;
        public static void DrawElements(DrawMode mode, GLSizei count, IndexType type, IntPtr indices)
        {
            glDrawElements(mode, count, type, indices);
            CheckError();
        }

        delegate void _glBlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, BufferBit mask, BlitFilter filter);
        static _glBlitFramebuffer glBlitFramebuffer;
        public static void BlitFramebuffer(RectangleI src, RectangleI dst, BufferBit mask, BlitFilter filter)
        {
            glBlitFramebuffer(src.X, src.Y, src.MaxX, src.MaxY, dst.X, dst.Y, dst.MaxX, dst.MaxY, mask, filter);
            CheckError();
        }

        delegate FramebufferStatus _glCheckFramebufferStatus(FramebufferTarget target);
        static _glCheckFramebufferStatus glCheckFramebufferStatus;
        public static FramebufferStatus CheckFramebufferStatus(FramebufferTarget target)
        {
            var status = glCheckFramebufferStatus(target);
            CheckError();
            return status;
        }

        delegate void _glUniform1f(int location, float v0);
        delegate void _glUniform2f(int location, float v0, float v1);
        delegate void _glUniform3f(int location, float v0, float v1, float v2);
        delegate void _glUniform4f(int location, float v0, float v1, float v2, float v3);
        delegate void _glUniform1fv(int location, GLSizei count, float[] value);
        delegate void _glUniform2fv(int location, GLSizei count, float[] value);
        delegate void _glUniform3fv(int location, GLSizei count, float[] value);
        delegate void _glUniform4fv(int location, GLSizei count, float[] value);
        static _glUniform1f glUniform1f;
        static _glUniform2f glUniform2f;
        static _glUniform3f glUniform3f;
        static _glUniform4f glUniform4f;
        static _glUniform1fv glUniform1fv;
        static _glUniform2fv glUniform2fv;
        static _glUniform3fv glUniform3fv;
        static _glUniform4fv glUniform4fv;
        public static void Uniform1F(int location, float v0)
        {
            glUniform1f(location, v0);
            CheckError();
        }
        public static void Uniform2F(int location, float v0, float v1)
        {
            glUniform2f(location, v0, v1);
            CheckError();
        }
        public static void Uniform3F(int location, float v0, float v1, float v2)
        {
            glUniform3f(location, v0, v1, v2);
            CheckError();
        }
        public static void Uniform4F(int location, float v0, float v1, float v2, float v3)
        {
            glUniform4f(location, v0, v1, v2, v3);
            CheckError();
        }
        public static void Uniform1FV(int location, GLSizei count, float[] value)
        {
            glUniform1fv(location, count, value);
            CheckError();
        }
        public static void Uniform2FV(int location, GLSizei count, float[] value)
        {
            glUniform2fv(location, count, value);
            CheckError();
        }
        public static void Uniform3FV(int location, GLSizei count, float[] value)
        {
            glUniform3fv(location, count, value);
            CheckError();
        }
        public static void Uniform4FV(int location, GLSizei count, float[] value)
        {
            glUniform4fv(location, count, value);
            CheckError();
        }

        delegate void _glUniform1i(int location, int v0);
        delegate void _glUniform2i(int location, int v0, int v1);
        delegate void _glUniform3i(int location, int v0, int v1, int v2);
        delegate void _glUniform4i(int location, int v0, int v1, int v2, int v3);
        delegate void _glUniform1iv(int location, GLSizei count, int[] value);
        delegate void _glUniform2iv(int location, GLSizei count, int[] value);
        delegate void _glUniform3iv(int location, GLSizei count, int[] value);
        delegate void _glUniform4iv(int location, GLSizei count, int[] value);
        static _glUniform1i glUniform1i;
        static _glUniform2i glUniform2i;
        static _glUniform3i glUniform3i;
        static _glUniform4i glUniform4i;
        static _glUniform1iv glUniform1iv;
        static _glUniform2iv glUniform2iv;
        static _glUniform3iv glUniform3iv;
        static _glUniform4iv glUniform4iv;
        public static void Uniform1I(int location, int v0)
        {
            glUniform1i(location, v0);
            CheckError();
        }
        public static void Uniform2I(int location, int v0, int v1)
        {
            glUniform2i(location, v0, v1);
            CheckError();
        }
        public static void Uniform3I(int location, int v0, int v1, int v2)
        {
            glUniform3i(location, v0, v1, v2);
            CheckError();
        }
        public static void Uniform4I(int location, int v0, int v1, int v2, int v3)
        {
            glUniform4i(location, v0, v1, v2, v3);
            CheckError();
        }
        public static void Uniform1IV(int location, GLSizei count, int[] value)
        {
            glUniform1iv(location, count, value);
            CheckError();
        }
        public static void Uniform2IV(int location, GLSizei count, int[] value)
        {
            glUniform2iv(location, count, value);
            CheckError();
        }
        public static void Uniform3IV(int location, GLSizei count, int[] value)
        {
            glUniform3iv(location, count, value);
            CheckError();
        }
        public static void Uniform4IV(int location, GLSizei count, int[] value)
        {
            glUniform4iv(location, count, value);
            CheckError();
        }

        delegate void _glUniform1ui(int location, uint v0);
        delegate void _glUniform2ui(int location, uint v0, uint v1);
        delegate void _glUniform3ui(int location, uint v0, uint v1, uint v2);
        delegate void _glUniform4ui(int location, uint v0, uint v1, uint v2, uint v3);
        delegate void _glUniform1uiv(int location, GLSizei count, uint[] value);
        delegate void _glUniform2uiv(int location, GLSizei count, uint[] value);
        delegate void _glUniform3uiv(int location, GLSizei count, uint[] value);
        delegate void _glUniform4uiv(int location, GLSizei count, uint[] value);
        static _glUniform1ui glUniform1ui;
        static _glUniform2ui glUniform2ui;
        static _glUniform3ui glUniform3ui;
        static _glUniform4ui glUniform4ui;
        static _glUniform1uiv glUniform1uiv;
        static _glUniform2uiv glUniform2uiv;
        static _glUniform3uiv glUniform3uiv;
        static _glUniform4uiv glUniform4uiv;
        public static void Uniform1UI(int location, uint v0)
        {
            glUniform1ui(location, v0);
            CheckError();
        }
        public static void Uniform2UI(int location, uint v0, uint v1)
        {
            glUniform2ui(location, v0, v1);
            CheckError();
        }
        public static void Uniform3UI(int location, uint v0, uint v1, uint v2)
        {
            glUniform3ui(location, v0, v1, v2);
            CheckError();
        }
        public static void Uniform4UI(int location, uint v0, uint v1, uint v2, uint v3)
        {
            glUniform4ui(location, v0, v1, v2, v3);
            CheckError();
        }
        public static void Uniform1UIV(int location, GLSizei count, uint[] value)
        {
            glUniform1uiv(location, count, value);
            CheckError();
        }
        public static void Uniform2UIV(int location, GLSizei count, uint[] value)
        {
            glUniform2uiv(location, count, value);
            CheckError();
        }
        public static void Uniform3UIV(int location, GLSizei count, uint[] value)
        {
            glUniform3uiv(location, count, value);
            CheckError();
        }
        public static void Uniform4UIV(int location, GLSizei count, uint[] value)
        {
            glUniform4uiv(location, count, value);
            CheckError();
        }

        delegate void _glUniformMatrix2fv(int location, GLSizei count, bool transpose, float[] value);
        static _glUniformMatrix2fv glUniformMatrix2fv;
        public static void UniformMatrix2FV(int location, GLSizei count, bool transpose, float[] value)
        {
            glUniformMatrix2fv(location, count, transpose, value);
            CheckError();
        }

        delegate void _glUniformMatrix3fv(int location, GLSizei count, bool transpose, float[] value);
        static _glUniformMatrix3fv glUniformMatrix3fv;
        public static void UniformMatrix3FV(int location, GLSizei count, bool transpose, float[] value)
        {
            glUniformMatrix3fv(location, count, transpose, value);
            CheckError();
        }

        unsafe delegate void _glUniformMatrix4fv(int location, GLSizei count, bool transpose, float* value);
        static _glUniformMatrix4fv glUniformMatrix4fv;
        public static unsafe void UniformMatrix4FV(int location, GLSizei count, bool transpose, float* value)
        {
            glUniformMatrix4fv(location, count, transpose, value);
            CheckError();
        }
        public static unsafe void UniformMatrix4FV(int location, GLSizei count, bool transpose, float[] value)
        {
            fixed (float* ptr = value) { glUniformMatrix4fv(location, count, transpose, ptr); }
            CheckError();
        }

        delegate void _glUniformMatrix2x3fv(int location, GLSizei count, bool transpose, float[] value);
        static _glUniformMatrix2x3fv glUniformMatrix2x3fv;
        public static void UniformMatrix2x3FV(int location, GLSizei count, bool transpose, float[] value)
        {
            glUniformMatrix2x3fv(location, count, transpose, value);
            CheckError();
        }

        unsafe delegate void _glUniformMatrix3x2fv(int location, GLSizei count, bool transpose, float* value);
        static _glUniformMatrix3x2fv glUniformMatrix3x2fv;
        public static unsafe void UniformMatrix3x2FV(int location, GLSizei count, bool transpose, float* value)
        {
            glUniformMatrix3x2fv(location, count, transpose, value);
            CheckError();
        }
        public static unsafe void UniformMatrix3x2FV(int location, GLSizei count, bool transpose, float[] value)
        {
            fixed (float* ptr = value) { glUniformMatrix3x2fv(location, count, transpose, ptr); }
            CheckError();
        }

        delegate void _glUniformMatrix2x4fv(int location, GLSizei count, bool transpose, float[] value);
        static _glUniformMatrix2x4fv glUniformMatrix2x4fv;
        public static void UniformMatrix2x4FV(int location, GLSizei count, bool transpose, float[] value)
        {
            glUniformMatrix2x4fv(location, count, transpose, value);
            CheckError();
        }

        delegate void _glUniformMatrix4x2fv(int location, GLSizei count, bool transpose, float[] value);
        static _glUniformMatrix4x2fv glUniformMatrix4x2fv;
        public static void UniformMatrix4x2FV(int location, GLSizei count, bool transpose, float[] value)
        {
            glUniformMatrix4x2fv(location, count, transpose, value);
            CheckError();
        }

        delegate void _glUniformMatrix3x4fv(int location, GLSizei count, bool transpose, float[] value);
        static _glUniformMatrix3x4fv glUniformMatrix3x4fv;
        public static void UniformMatrix3x4FV(int location, GLSizei count, bool transpose, float[] value)
        {
            glUniformMatrix3x4fv(location, count, transpose, value);
            CheckError();
        }

        delegate void _glUniformMatrix4x3fv(int location, GLSizei count, bool transpose, float[] value);
        static _glUniformMatrix4x3fv glUniformMatrix4x3fv;
        public static void UniformMatrix4x3FV(int location, GLSizei count, bool transpose, float[] value)
        {
            glUniformMatrix4x3fv(location, count, transpose, value);
            CheckError();
        }

        delegate void _glScissor(int x, int y, GLSizei width, GLSizei height);
        static _glScissor glScissor;
        public static void Scissor(int x, int y, GLSizei width, GLSizei height)
        {
            glScissor(x, y, width, height);
            CheckError();
        }

        delegate void _glDepthFunc(DepthFunc func);
        static _glDepthFunc glDepthFunc;
        public static void DepthFunc(DepthFunc func)
        {
            glDepthFunc(func);
            CheckError();
        }

        #pragma warning restore 0649
    }

    public enum FramebufferStatus
    {
        Complete = 0x8CD5, 
        Undefined = 0x8219,
        IncompleteAttachment = 0x8CD6,
        IncompleteMissingAttachment = 0x8CD7,
        IncompleteDrawBuffer = 0x8CDB,
        IncompleteReadBuffer = 0x8CDC,
        Unsupported = 0x8CDD,
        IncompleteMultisample = 0x8D56,
        IncompleteLayerTargets = 0x8DA8
    }

    public enum DepthFunc
    {
        Never = 0x0200,
        Less,
        Equal,
        LessEqual,
        Greater,
        NotEqual,
        GreaterEqual,
        Always
    }

    public enum ErrorCode
    {
        NoError = 0,
        InvalidEnum = 0x0500,
        InvalidValue = 0x0501,
        InvalidOperation = 0x0502,
        InvalidFramebufferOperation = 0x0506,
        OutOfMemory = 0x0505
    }

    public enum EnableCap : GLEnum
    {
        Blend = 0x0BE2,
        CullFace = 0x0B44,
        DepthTest = 0x0B71,
        Dither = 0x0BD0,
        PolygonOffsetFill = 0x8037,
        RasterizerDiscard = 0x8C89,
        SampleAlphaToCoverage = 0x809E,
        SampleCoverage = 0x80A0,
        ScissorTest = 0x0C11,
        StencilTest = 0x0B90
    }

    [Flags]
    public enum BufferBit : GLEnum
    {
        Depth = 0x00000100,
        Color = 0x00004000
    }

    public enum TextureTarget : GLEnum
    {
        Texture2D = 0x0DE1,
        Texture3D = 0x806F,
        Texture2DArray = 0x8C1A,
        TextureCubeMap = 0x8513,
        TextureCubeMapPosX = 0x8515,
        TextureCubeMapNegX = 0x8516,
        TextureCubeMapPosY = 0x8517,
        TextureCubeMapNegY = 0x8518,
        TextureCubeMapPosZ = 0x8519,
        TextureCubeMapNegZ = 0x851A,
    }

    public enum TextureParam : GLEnum
    {
        BaseLevel = 0x813C,
        CompareFunc = 0x884D,
        CompareMode = 0x884C,
        MinFilter = 0x2801,
        MagFilter = 0x2800,
        MinLOD = 0x813A,
        MaxLOD = 0x813B,
        MaxLevel = 0x813D,
        SwizzleR = 0x8E42,
        SwizzleG = 0x8E43,
        SwizzleB = 0x8E44,
        SwizzleA = 0x8E45,
        WrapS = 0x2802,
        WrapT = 0x2803,
        WrapR = 0x8072,
        DepthTextureMode = 0x884B
    }

    public enum DepthTextureMode
    {
        Intensity = 0x8049,
        Alpha = 0x1906,
        Luminance = 0x1909
    }

    public enum ShaderType : GLEnum
    {
        Vertex = 0x8B31,
        Fragment = 0x8B30
    }

    public enum ShaderParam : GLEnum
    {
        ShaderType = 0x8B4F,
        DeleteStatus = 0x8B80,
        CompileStatus = 0x8B81,
        InfoLogLength = 0x8B84,
        ShaderSourceLength = 0x8B88
    }

    public enum ProgramParam : GLEnum
    {
        DeleteStatus = 0x8B80,
        LinkStatus = 0x8B82,
        ValidateStatus = 0x8B83,
        InfoLogLength = 0x8B84,
        AttachedShaders = 0x8B85,
        ActiveAttributes = 0x8B89,
        ActiveAttributeMaxLength = 0x8B8A,
        ActiveUniforms = 0x8B86,
        ActiveUniformMaxLength = 0x8B87
    }

    public enum IndexType : GLEnum
    {
        UnsignedByte = 0x1401,
        UnsignedShort = 0x1403,
        UnsignedInt = 0x1405,
    }

    public enum BufferTarget : GLEnum
    {
        Array = 0x8892, 
        ElementArray = 0x8893,
        CopyRead = 0x8F36, 
        CopyWrite = 0x8F37, 
        PixelPack = 0x88EB, 
        PixelUnpack = 0x88EC,
        TransformFeedback = 0x8C8E,
        Uniform = 0x8A11
    }

    public enum BufferUsage : GLEnum
    {
        StreamDraw = 0x88E0, 
        StreamRead = 0x88E1, 
        StreamCopy = 0x88E2, 
        StaticDraw = 0x88E4, 
        StaticRead = 0x88E5, 
        StaticCopy = 0x88E6, 
        DynamicDraw = 0x88E8, 
        DynamicRead = 0x88E9, 
        DynamicCopy = 0x88EA
    }

    public enum FramebufferTarget : GLEnum
    {
        Framebuffer = 0x8D40,
        Draw = 0x8CA9,
        Read = 0x8CA8,
    }

    public enum UniformType : GLEnum
    {
        Bool = 0x8B56,
        Int = 0x1404,
        Float = 0x1406,
        Vec2 = 0x8B50,
        Vec3 = 0x8B51,
        Vec4 = 0x8B52,
        Mat3x2 = 0x8B67,
        Mat4 = 0x8B5C,
        Sampler2D = 0x8B5E,
        SamplerCube = 0x8B60,
    }

    public enum DrawBuffer : GLEnum
    {
        None = 0,
        Back = 0x0405,
        Color0 = 0x8CE0,
        Color1,
        Color2,
        Color3,
        Color4,
        Color5,
        Color6,
        Color7,
        Color8,
        Color9,
        Color10,
        Color11,
        Color12,
        Color13,
        Color14,
        Color15,
    }

    public enum ReadBuffer : GLEnum
    {
        Depth = 0x8D00,
        Color0 = 0x8CE0,
        Color1,
        Color2,
        Color3,
        Color4,
        Color5,
        Color6,
        Color7,
        Color8,
        Color9,
        Color10,
        Color11,
        Color12,
        Color13,
        Color14,
        Color15,
    }

    public enum TextureAttachment : GLEnum
    {
        //Depth = 0x8D00,
        Depth = 0x8D00,
        DepthStencil = 0x821A,
        Color0 = 0x8CE0,
        Color1,
        Color2,
        Color3,
        Color4,
        Color5,
        Color6,
        Color7,
        Color8,
        Color9,
        Color10,
        Color11,
        Color12,
        Color13,
        Color14,
        Color15,
    }
}

namespace Rise
{
    public enum DrawMode : GLEnum
    {
        Points = 0x0000,
        Lines = 0x0001,
        LineLoop = 0x0002,
        LineStrip = 0x0003,
        Triangles = 0x0004,
        TriangleStrip = 0x0005,
        TriangleFan = 0x0006
    }

    public enum TextureFilter : GLEnum
    {
        Nearest = 0x2600,
        Linear = 0x2601,
        NearestMipmapNearest = 0x2700,
        LinearMipmapNearest = 0x2701,
        NearestMipmapLinear = 0x2702,
        LinearMipmapLinear = 0x2703
    }

    public enum TextureWrap : GLEnum
    {
        Repeat = 0x2901,
        ClampToEdge = 0x812F,
        MirroredRepeat = 0x8370
    }

    public enum BlitFilter : GLEnum
    {
        Nearest = 0x2600,
        Linear = 0x2601
    }

    public enum MinFilter : GLEnum
    {
        Nearest = 0x2600,
        Linear = 0x2601,
        NearestMipmapNearest = 0x2700,
        LinearMipmapNearest = 0x2701,
        NearestMipmapLinear = 0x2702,
        LinearMipmapLinear = 0x2703
    }

    public enum MagFilter : GLEnum
    {
        Nearest = 0x2600,
        Linear = 0x2601
    }

    public enum BlendEquation : GLEnum
    {
        Add = 32774,
        Subtract = 32778,
        ReverseSubtract = 32779,
        Max = 32776,
        Min = 32775
    }

    public enum BlendFactor : GLEnum
    {
        Zero = 0,
        One = 1,
        SrcColor = 0x0300,
        OneMinusSrcColor,
        SrcAlpha,
        OneMinusSrcAlpha,
        DstAlpha,
        OneMinusDstAlpha,
        DstColor,
        OneMinusDstcolor,
        SrcAlphaSaturate,
        ConstantColor = 0x8001,
        OneMinusConstantColor,
        ConstantAlpha,
        OneMinusConstantAlpha
    }

    public enum TextureFormat
    {
        //Depth textures
        Depth = 0x1902,
        Depth16 = 0x81A5,
        Depth24 = 0x81A6,
        Depth32 = 0x81A7,
        Depth32F = 0x81A8,

        DepthStencil = 0x84F9,
        Depth24Stencil8 = 0x88F0,


        //Red textures
        R = 0x1903,
        R8 = 0x8229,
        R8SNorm = 0x8F94,
        R16F = 0x822D,
        R32F = 0x822E,
        R8I = 0x8231,
        R8UI = 0x8232,
        R16I = 0x8233,
        R16UI = 0x8234,
        R32I = 0x8235,
        R32UI = 0x8236,

        //RG textures
        RG = 0x8227,
        RG8 = 0x822B,
        RG8SNorm = 0x8F95,
        RG16F = 0x822F,
        RG32F = 0x8230,
        RG8I = 0x8237,
        RG8UI = 0x8238,
        RG16I = 0x8239,
        RG16UI = 0x823A,
        RG32I = 0x823B,
        RG32UI = 0x823C,

        //RGB textures
        RGB = 0x1907,
        RGB8 = 0x8051,
        RGB8SNorm = 0x8F96,
        RGB16F = 0x881B,
        RGB32F = 0x8815,
        RGB8I = 0x8D8F,
        RGB8UI = 0x8D7D,
        RGB16I = 0x8D89,
        RGB16UI = 0x8D77,
        RGB32I = 0x8D83,
        RGB32UI = 0x8D71,
        R3G3B2 = 0x2A10,
        R5G6B5 = 0x8D62,
        R11G11B10F = 0x8C3A,

        //RGBA textures
        RGBA = 0x1908,
        RGBA8 = 0x8058,
        RGBA8SNorm = 0x8F97,
        RGBA16F = 0x881A,
        RGBA32F = 0x8814,
        RGBA8I = 0x8D8E,
        RGBA8UI = 0x8D7C,
        RGBA16I = 0x8D88,
        RGBA16UI = 0x8D76,
        RGBA32I = 0x8D82,
        RGBA32UI = 0x8D70,
        RGBA2 = 0x8055,
        RGBA4 = 0x8056,
        RGB5A1 = 0x8057,
        RGB10A2 = 0x8059,
        RGB10A2UI = 0x906F,
    }

    public enum PixelFormat : GLEnum
    {
        R = 0x1903,
        RG = 0x8227,
        RGB = 0x1907,
        RGBA = 0x1908,
        Depth = 0x1902,
        DepthStencil = 0x84F9,
    }

    public static class TextureFormatExt
    {
        public static PixelFormat PixelFormat(this TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.Depth:
                case TextureFormat.Depth16:
                case TextureFormat.Depth24:
                case TextureFormat.Depth32:
                case TextureFormat.Depth32F:
                    return Rise.PixelFormat.Depth;
                case TextureFormat.R:
                case TextureFormat.R8:
                case TextureFormat.R8SNorm:
                case TextureFormat.R8I:
                case TextureFormat.R8UI:
                case TextureFormat.R16I:
                case TextureFormat.R16UI:
                case TextureFormat.R16F:
                case TextureFormat.R32I:
                case TextureFormat.R32UI:
                case TextureFormat.R32F:
                    return Rise.PixelFormat.R;
                case TextureFormat.RG:
                case TextureFormat.RG8:
                case TextureFormat.RG8SNorm:
                case TextureFormat.RG8I:
                case TextureFormat.RG8UI:
                case TextureFormat.RG16I:
                case TextureFormat.RG16UI:
                case TextureFormat.RG16F:
                case TextureFormat.RG32I:
                case TextureFormat.RG32UI:
                case TextureFormat.RG32F:
                    return Rise.PixelFormat.RG;
                case TextureFormat.RGB:
                case TextureFormat.RGB8:
                case TextureFormat.RGB8SNorm:
                case TextureFormat.RGB8I:
                case TextureFormat.RGB8UI:
                case TextureFormat.RGB16I:
                case TextureFormat.RGB16UI:
                case TextureFormat.RGB16F:
                case TextureFormat.RGB32I:
                case TextureFormat.RGB32UI:
                case TextureFormat.RGB32F:
                case TextureFormat.R3G3B2:
                case TextureFormat.R5G6B5:
                case TextureFormat.R11G11B10F:
                    return Rise.PixelFormat.RGB;
                case TextureFormat.RGBA:
                case TextureFormat.RGBA8:
                case TextureFormat.RGBA8SNorm:
                case TextureFormat.RGBA16F:
                case TextureFormat.RGBA32F:
                case TextureFormat.RGBA8I:
                case TextureFormat.RGBA8UI:
                case TextureFormat.RGBA16I:
                case TextureFormat.RGBA16UI:
                case TextureFormat.RGBA32I:
                case TextureFormat.RGBA32UI:
                case TextureFormat.RGBA2:
                case TextureFormat.RGBA4:
                case TextureFormat.RGB5A1:
                case TextureFormat.RGB10A2:
                case TextureFormat.RGB10A2UI:
                    return Rise.PixelFormat.RGBA;
                default:
                    throw new Exception("Unexpected pixel format.");
            }
        }

        /*public static PixelType PixelType(this TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.Depth:
                case TextureFormat.Depth16:
                case TextureFormat.Depth24:
                case TextureFormat.Depth32:
                case TextureFormat.Depth32F:
                    return Rise.PixelType.Float;
                case TextureFormat.R:
                case TextureFormat.R8:
                case TextureFormat.R8SNorm:
                case TextureFormat.R8I:
                case TextureFormat.R8UI:
                case TextureFormat.R16I:
                case TextureFormat.R16UI:
                case TextureFormat.R16F:
                case TextureFormat.R32I:
                case TextureFormat.R32UI:
                case TextureFormat.R32F:
                    return Rise.PixelFormat.R;
                case TextureFormat.RG:
                case TextureFormat.RG8:
                case TextureFormat.RG8SNorm:
                case TextureFormat.RG8I:
                case TextureFormat.RG8UI:
                case TextureFormat.RG16I:
                case TextureFormat.RG16UI:
                case TextureFormat.RG16F:
                case TextureFormat.RG32I:
                case TextureFormat.RG32UI:
                case TextureFormat.RG32F:
                    return Rise.PixelFormat.RG;
                case TextureFormat.RGB:
                case TextureFormat.RGB8:
                case TextureFormat.RGB8SNorm:
                case TextureFormat.RGB8I:
                case TextureFormat.RGB8UI:
                case TextureFormat.RGB16I:
                case TextureFormat.RGB16UI:
                case TextureFormat.RGB16F:
                case TextureFormat.RGB32I:
                case TextureFormat.RGB32UI:
                case TextureFormat.RGB32F:
                case TextureFormat.R3G3B2:
                case TextureFormat.R5G6B5:
                case TextureFormat.R11G11B10F:
                    return Rise.PixelFormat.RGB;
                case TextureFormat.RGBA:
                case TextureFormat.RGBA8:
                case TextureFormat.RGBA8SNorm:
                case TextureFormat.RGBA16F:
                case TextureFormat.RGBA32F:
                case TextureFormat.RGBA8I:
                case TextureFormat.RGBA8UI:
                case TextureFormat.RGBA16I:
                case TextureFormat.RGBA16UI:
                case TextureFormat.RGBA32I:
                case TextureFormat.RGBA32UI:
                case TextureFormat.RGBA2:
                case TextureFormat.RGBA4:
                case TextureFormat.RGB5A1:
                case TextureFormat.RGB10A2:
                case TextureFormat.RGB10A2UI:
                    return Rise.PixelFormat.RGBA;
                default:
                    throw new Exception("Unexpected pixel format.");
            }
        }*/
    }

    public enum PixelType : GLEnum
    {
        Byte = 0x1400,
        UnsignedByte = 0x1401,
        Short = 0x1402,
        UnsignedShort = 0x1403,
        Int = 0x1404,
        UnsignedInt = 0x1405,
        Float = 0x1406
    }

    public enum VertexType : GLEnum
    {
        Byte = 0x1400,
        UnsignedByte,
        Short,
        UnsignedShort,
        Int,
        UnsignedInt,
        Float
    }

    public enum CullFace : GLEnum
    {
        Front = 0x0404,
        Back = 0x0405,
        FrontAndBack = 0x0408
    }
}
