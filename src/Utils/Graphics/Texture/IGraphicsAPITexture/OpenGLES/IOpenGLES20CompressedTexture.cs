namespace LibWindPop.Utils.Graphics.Texture.IGraphicsAPITexture.OpenGLES
{
    /// <summary>
    /// glCompressedTexImage2D(GLenum target, GLint level, GLenum internalformat, GLsizei width, GLsizei height, GLint border, GLsizei imageSize, const void *data);
    /// glCompressedTexSubImage2D(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GLenum format, GLsizei imageSize, const void *data);
    /// </summary>
    public interface IOpenGLES20CompressedTexture
    {
        static abstract int OpenGLES20InternalFormat { get; }
    }
}
