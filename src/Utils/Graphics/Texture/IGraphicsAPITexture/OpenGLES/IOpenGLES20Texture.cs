namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.OpenGLES
{
    /// <summary>
    /// glTexImage2D(GLenum target, GLint level, GLint internalFormat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, const GLvoid *pixels);
    /// glTexSubImage2D(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GLenum format, GLenum type, const GLvoid *pixels);
    /// </summary>
    public interface IOpenGLES20Texture
    {
        static abstract int OpenGLES20InternalFormat { get; }

        static abstract int OpenGLES20Format { get; }

        static abstract int OpenGLES20Type { get; }
    }
}
