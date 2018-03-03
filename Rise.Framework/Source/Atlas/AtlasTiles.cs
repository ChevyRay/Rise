using System;
namespace Rise
{
    public class AtlasTiles
    {
        public Atlas Atlas { get; private set; }
        public string Name { get; private set; }
        public int Cols { get; private set; }
        public int Rows { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public int TileCount { get; private set; }

        AtlasImage[] tiles;

        internal AtlasTiles(Atlas atlas, string name, int cols, int rows, int tw, int th)
        {
            Atlas = atlas;
            Name = name;
            Cols = cols;
            Rows = rows;
            TileWidth = tw;
            TileHeight = th;
            TileCount = cols * rows;
            tiles = new AtlasImage[cols * rows];
        }

        public void SetTile(int x, int y, AtlasImage tile)
        {
            if (tile.Width != TileWidth)
                throw new Exception("Tile width mismatch.");
            if (tile.Height != TileHeight)
                throw new Exception("Tile height mismatch.");

            tiles[y * Cols + x] = tile;
        }

        public AtlasImage GetTile(int x, int y)
        {
            return tiles[y * Cols + x];
        }
        public AtlasImage GetTile(int i)
        {
            return tiles[i];
        }
    }
}
