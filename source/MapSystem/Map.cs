using System.Collections.Generic;
using Steel;

namespace SteelCustom.MapSystem
{
    public class Map : ScriptComponent
    {
        public int Size => SIZE;
        
        private Tile[,] _tiles;

        //private const int SIZE = 256;
        private const int SIZE = 64;
        private const int NO_TREES_SIZE = 14;

        public void Init()
        {
            GenerateMap();
        }

        public Tile GetTile(int x, int y)
        {
            if (!IsValid(x, y))
                return null;

            return _tiles[x, y];
        }

        public Tile GetTileAt(Vector2 position)
        {
            var coords = PositionToCoords(position);
            return GetTile(coords.X, coords.Y);
        }

        public Tile GetBottomLeftTile(Vector2 position, int sizeX, int sizeY)
        {
            if (!IsValid(position, sizeX, sizeY))
                return null;
            
            return GetTileAt(position - new Vector2(sizeX * 0.5f, sizeY * 0.5f));
        }

        public IEnumerable<Tile> GetTiles(Vector2 position, int sizeX, int sizeY)
        {
            if (!IsValid(position, sizeX, sizeY))
                yield break;
            
            Tile bottomLeftTile = GetTileAt(position - new Vector2(sizeX * 0.5f, sizeY * 0.5f));
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    yield return GetTile(bottomLeftTile.X + i, bottomLeftTile.Y + j);
                }
            }
        }

        public (int X, int Y) PositionToCoords(Vector2 position)
        {
            int x = (int)Math.Round(position.X + 0.5f);
            int y = (int)Math.Round(position.Y + 0.5f);
            return (x, y);
        }

        public Vector2 CoordsToPosition(int x, int y)
        {
            return new Vector2(x + 0.5f, y + 0.5f);
        }

        public IEnumerable<Tile> GetPassableTilesAround(MapObject mapObject)
        {
            // Line below
            for (int i = -1; i < mapObject.Size.X + 1; i++)
            {
                var tile = GetTile(mapObject.OnBottomLeftTile.X + i, mapObject.OnBottomLeftTile.Y - 1);
                if (tile.Passable)
                    yield return tile;
            }
            // Line above
            for (int i = -1; i < mapObject.Size.X + 1; i++)
            {
                var tile = GetTile(mapObject.OnBottomLeftTile.X + i, mapObject.OnBottomLeftTile.Y + mapObject.Size.Y);
                if (tile.Passable)
                    yield return tile;
            }
            // Line to the left
            for (int i = -1; i < mapObject.Size.Y + 1; i++)
            {
                var tile = GetTile(mapObject.OnBottomLeftTile.X - 1, mapObject.OnBottomLeftTile.Y + i);
                if (tile.Passable)
                    yield return tile;
            }
            // Line to the right
            for (int i = -1; i < mapObject.Size.Y + 1; i++)
            {
                var tile = GetTile(mapObject.OnBottomLeftTile.X + mapObject.Size.X, mapObject.OnBottomLeftTile.Y + i);
                if (tile.Passable)
                    yield return tile;
            }
        }

        public bool IsValid(int x, int y) => x >= 0 && y >= 0 && x < SIZE && y < SIZE;

        public bool IsValid(Vector2 position, int sizeX, int sizeY)
        {
            (int X, int Y) bottomLeftCoords = PositionToCoords(position - new Vector2(sizeX * 0.5f, sizeY * 0.5f));
            (int X, int Y) topRightCoords = (bottomLeftCoords.X + sizeX - 1, bottomLeftCoords.Y + sizeY - 1);
            return IsValid(bottomLeftCoords.X, bottomLeftCoords.Y) && IsValid(topRightCoords.X, topRightCoords.Y);
        }

        public Vector2 GetCenterPosition(Vector2 position, int sizeX, int sizeY)
        {
            (int X, int Y) bottomLeftCoords = PositionToCoords(position - new Vector2(sizeX * 0.5f, sizeY * 0.5f));
            return new Vector2(bottomLeftCoords.X, bottomLeftCoords.Y) + new Vector2(sizeX * 0.5f, sizeY * 0.5f);
        }

        private void GenerateMap()
        {
            _tiles = new Tile[SIZE, SIZE];
            
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    var tile = new Tile(i, j);
                    _tiles[i, j] = tile;
                }
            }
            
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    if (i % 2 == 0 && j % 2 == 0 && !IsNoTree(i, j))
                        SpawnTree(i, j);
                }
            }
        }

        private bool IsNoTree(int x, int y)
        {
            return SIZE / 2 - NO_TREES_SIZE / 2 <= x && x < SIZE / 2 + NO_TREES_SIZE / 2
                && SIZE / 2 - NO_TREES_SIZE / 2 <= y && y < SIZE / 2 + NO_TREES_SIZE / 2;
        }

        private void SpawnTree(int x, int y)
        {
            var entity = new Entity("Tree", Entity);
            var tree = entity.AddComponent<Tree>();
            tree.Init();
            tree.Place(GetBottomLeftTile(CoordsToPosition(x, y), 2, 2));
            tree.Transformation.Position = (Vector3)GetCenterPosition(CoordsToPosition(x, y), 2, 2) + new Vector3(0, 0, 0.5f);
            
            if (x == 0 && y == 0)
                Log.LogInfo($"Tree position {tree.Transformation.Position}");
            
            foreach (var tile in GetTiles(CoordsToPosition(x, y), 2, 2))
            {
                tile.SetOnObject(tree);
            }
        }
    }
}