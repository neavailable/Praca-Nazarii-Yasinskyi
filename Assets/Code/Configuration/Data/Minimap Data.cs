namespace Configuration.Data
{
    public class MinimapData
    {
        public readonly float OrthographicSize, Height;
        public readonly int TextureSize;


        public MinimapData(float orthographicSize, float height, int textureSize)
        {
            OrthographicSize = orthographicSize;
            Height           = height;
            TextureSize      = textureSize;
        }
    }
}
