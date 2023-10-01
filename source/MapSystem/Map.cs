using System.Collections.Generic;
using System.Linq;
using Steel;
using SteelCustom.Buildings;
using SteelCustom.PlayerSystem.Resources;
using SteelCustom.Units;
using Random = Steel.Random;

namespace SteelCustom.MapSystem
{
    public class Map : ScriptComponent
    {
        public int Size => SIZE;
        
        private Tile[,] _tiles;

        //private const int SIZE = 256;
        private const int SIZE = 80;
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
            int x = (int)position.X;
            int y = (int)position.Y;
            return (x, y);
        }

        public Vector2 CoordsToPosition(int x, int y)
        {
            return new Vector2(x + 0.5f, y + 0.5f);
        }

        public IEnumerable<Tile> GetPassableTilesAround(MapObject mapObject, bool withCorners = true)
        {
            int cornerFactor = withCorners ? 1 : 0;
            
            // Line below
            for (int i = -cornerFactor; i < mapObject.Size.X + cornerFactor; i++)
            {
                var tile = GetTile(mapObject.OnBottomLeftTile.X + i, mapObject.OnBottomLeftTile.Y - 1);
                if (tile != null && tile.Passable)
                    yield return tile;
            }
            // Line above
            for (int i = -cornerFactor; i < mapObject.Size.X + cornerFactor; i++)
            {
                var tile = GetTile(mapObject.OnBottomLeftTile.X + i, mapObject.OnBottomLeftTile.Y + mapObject.Size.Y);
                if (tile != null && tile.Passable)
                    yield return tile;
            }
            // Line to the left
            for (int i = -cornerFactor; i < mapObject.Size.Y + cornerFactor; i++)
            {
                var tile = GetTile(mapObject.OnBottomLeftTile.X - 1, mapObject.OnBottomLeftTile.Y + i);
                if (tile != null && tile.Passable)
                    yield return tile;
            }
            // Line to the right
            for (int i = -cornerFactor; i < mapObject.Size.Y + cornerFactor; i++)
            {
                var tile = GetTile(mapObject.OnBottomLeftTile.X + mapObject.Size.X, mapObject.OnBottomLeftTile.Y + i);
                if (tile != null && tile.Passable)
                    yield return tile;
            }
        }

        public Tile GetClosestPassableTile(Tile pivot, MapObject mapObject, bool withCorners)
        {
            if (!mapObject.IsBlocking)
            {
                // Random passable tile inside
                var tiles = mapObject.OnTiles().Where(t => t != null && t.Passable).ToList();
                return tiles.Any() ? tiles[Random.NextInt(0, tiles.Count - 1)] : null;
            }
            
            Tile closestTile = null;
            float minDistance = float.MaxValue;
            foreach (Tile tile in GetPassableTilesAround(mapObject, withCorners))
            {
                float distance = Vector2.Distance(new Vector2(tile.X, tile.Y), new Vector2(pivot.X, pivot.Y));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTile = tile;
                }
            }

            return closestTile;
        }

        public IEnumerable<Tile> GetPassableTilesAround(Tile targetTile)
        {
            // TODO: direction as parameter

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    
                    var tile = GetTile(targetTile.X + i, targetTile.Y + j);
                    if (tile != null && tile.Passable)
                        yield return tile;
                }
            }
        }

        public IResource GetClosestResource(Worker worker, Vector2 targetPosition, ResourceType resourceType)
        {
            var targetTile = GetTileAt(targetPosition);
            if (targetTile == null)
                return null;
            
            const int maxTilesToSearch = 400;
            IResource closestResource = null;
            float minDistance = float.MaxValue;
            
            foreach (Tile tile in SpiralSearch(targetTile, maxTilesToSearch, 2))
            {
                if (tile.OnObject is IResource resource && resource.CanBeGathered(worker) && resource.ResourceType == resourceType)
                {
                    float distance = Vector2.Distance(targetPosition, new Vector2(resource.Position.X, resource.Position.Y));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestResource = resource;
                    }
                }
            }
            return closestResource;
        }

        public IEnumerable<Tile> SpiralSearch(Tile targetTile, int maxTilesToSearch = int.MaxValue, int radiusStep = 1)
        {
            int x = targetTile.X;
            int y = targetTile.Y;
            int searched = 0;
            int size = 3;

            while (searched < maxTilesToSearch)
            {
                // Line below
                for (int i = 0; i < size + 1; i++)
                {
                    Tile tile = GetTile(x - size / 2 + i, y - size / 2);
                    if (tile != null)
                    {
                        yield return tile;
                        searched++;
                    }
                }
                // Line above
                for (int i = 0; i < size + 1; i++)
                {
                    Tile tile = GetTile(x - size / 2 + i, y + size / 2);
                    if (tile != null)
                    {
                        yield return tile;
                        searched++;
                    }
                }
                // Line to the left
                for (int i = 1; i < size; i++)
                {
                    Tile tile = GetTile(x - size / 2, y - size / 2 + i);
                    if (tile != null)
                    {
                        yield return tile;
                        searched++;
                    }
                }
                // Line to the right
                for (int i = 1; i < size; i++)
                {
                    Tile tile = GetTile(x + size / 2, y - size / 2 + i);
                    if (tile != null)
                    {
                        yield return tile;
                        searched++;
                    }
                }

                size += radiusStep * 2;
            }
        }

        public bool IsNextTo(Tile onTile, MapObject mapObject)
        {
            return (onTile.X == mapObject.OnBottomLeftTile.X - 1 || onTile.X == mapObject.OnBottomLeftTile.X + mapObject.Size.X)
                && (onTile.Y >= mapObject.OnBottomLeftTile.Y && onTile.Y <= mapObject.OnBottomLeftTile.Y + mapObject.Size.Y - 1)
                || (onTile.X >= mapObject.OnBottomLeftTile.X && onTile.X <= mapObject.OnBottomLeftTile.X + mapObject.Size.X - 1)
                && (onTile.Y == mapObject.OnBottomLeftTile.Y - 1 || onTile.Y == mapObject.OnBottomLeftTile.Y + mapObject.Size.Y);
        }

        public Building GetClosestStorage(Tile targetTile, ResourceType resourceType)
        {
            const int maxTilesToSearch = 400;
            Building closestBuilding = null;
            float minDistance = float.MaxValue;

            Vector2 targetPosition = CoordsToPosition(targetTile.X, targetTile.Y);
            
            foreach (Tile tile in SpiralSearch(targetTile, maxTilesToSearch, 2))
            {
                if (tile.OnObject is Building building && building.IsBuilt && building.IsStorage(resourceType))
                {
                    float distance = Vector2.Distance(targetPosition, new Vector2(building.Transformation.Position.X, building.Transformation.Position.Y));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestBuilding = building;
                    }
                }
            }
            return closestBuilding;
        }
        
        public List<Tile> GetPath(Tile a, Tile b)
        {
            List<Tile> path = PathFinder<Tile>.FindPath(a, b, _tiles, true);
            if (path.Any() && !b.Passable)
                path.Remove(b);
            return path;
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
                    if (i % 2 == 1 && j % 2 == 1 && !IsNoTree(i, j))
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
            var bottomLeftTile = GetBottomLeftTile(CoordsToPosition(x, y), 2, 2);
            var allTiles = GetTiles(CoordsToPosition(x, y), 2, 2).ToList();
            tree.Init();
            tree.Place(bottomLeftTile, allTiles);
            tree.Transformation.Position = (Vector3)GetCenterPosition(CoordsToPosition(x, y), 2, 2) + new Vector3(0, 0, 0.5f);
            
            if (x == 1 && y == 1)
                Log.LogInfo($"First tree position {tree.Transformation.Position}");
        }
    }
}