using System.Collections.Generic;
using UnityEngine;

sealed class ProcGen
{
    /// <summary>
    /// Generate a new dungeon map.
    /// </summary>
    public void GenerateDungeon(int mapWidth, int mapHeight, int roomMaxSize, int roomMinSize, int maxRooms, int maxMonstersPerRoom, List<RectangularRoom> rooms)
    {
        // Generate the rooms.
        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            int roomWidth = Random.Range(roomMinSize, roomMaxSize);
            int roomHeight = Random.Range(roomMinSize, roomMaxSize);

            int roomX = Random.Range(0, mapWidth - roomWidth - 1);
            int roomY = Random.Range(0, mapHeight - roomHeight - 1);

            RectangularRoom newRoom = new RectangularRoom(roomX, roomY, roomWidth, roomHeight);

            //Check if this room intersects with any other rooms
            if (newRoom.Overlaps(rooms))
            {
                continue;
            }
            //If there are no intersections then the room is valid.

            //Dig out this rooms inner area and builds the walls.
            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX || x == roomX + roomWidth - 1 || y == roomY || y == roomY + roomHeight - 1)
                    {
                        if (SetWallTileIfEmpty(new Vector3Int(x, y)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        SetFloorTile(new Vector3Int(x, y));
                    }
                }
            }

            if (rooms.Count != 0)
            {
                //Dig out a tunnel between this room and the previous one.
                TunnelBetween(rooms[rooms.Count - 1], newRoom);
            }

            PlaceActors(newRoom, maxMonstersPerRoom);
            PlaceObjects(newRoom);

            rooms.Add(newRoom);
        }
        //The first room, where the player starts.
        MapManager.instance.CreateEntity("Player", rooms[0].Center());
    }

    /// <summary>
    /// Return an L-shaped tunnel between these two points using Bresenham lines.
    /// </summary>
    private void TunnelBetween(RectangularRoom oldRoom, RectangularRoom newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();
        Vector2Int tunnelCorner;

        if (Random.value < 0.5f)
        {
            //Move horizontally, then vertically.
            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
        }
        else
        {
            //Move vertically, then horizontally.
            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);
        }

        //Generate the coordinates for this tunnel.
        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);

        //Set the tiles for this tunnel.
        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            SetFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y));

            //Set the wall tiles around this tile to be walls.
            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (SetWallTileIfEmpty(new Vector3Int(x, y)))
                    {
                        continue;
                    }
                }
            }
        }
    }

    private bool SetWallTileIfEmpty(Vector3Int pos)
    {
        if (MapManager.instance.FloorMap.GetTile(pos))
        {
            return true;
        }
        else
        {
            MapManager.instance.FloorMap.SetTile(pos, MapManager.instance.FloorTile);
            return false;
        }
    }

    private void SetFloorTile(Vector3Int pos)
    {
        MapManager.instance.FloorMap.SetTile(pos, MapManager.instance.FloorTile);
    }

    private void PlaceActors(RectangularRoom newRoom, int maximumMonsters)
    {
        int numberOfMonsters = Random.Range(0, maximumMonsters + 1);

        for (int monster = 0; monster < numberOfMonsters;)
        {
            int x = Random.Range(newRoom.X, newRoom.X + newRoom.Width);
            int y = Random.Range(newRoom.Y, newRoom.Y + newRoom.Height);

            if (x == newRoom.X || x == newRoom.X + newRoom.Width - 1 || y == newRoom.Y || y == newRoom.Y + newRoom.Height - 1)
            {
                continue;
            }

            for (int actor = 0; actor < GameManager.instance.Actors.Count; actor++)
            {
                Vector3Int pos = MapManager.instance.FloorMap.WorldToCell(GameManager.instance.Actors[actor].transform.position);

                if (pos.x == x && pos.y == y)
                {
                    return;
                }
            }

            if (Random.value < 0.6f)
            {
                MapManager.instance.CreateEntity("Goblin", new Vector2(x, y));
            }
            else
            {
                MapManager.instance.CreateEntity("Imp", new Vector2(x, y));
            }
            monster++;
        }
    }

    private void PlaceObjects(RectangularRoom newRoom)
    {
        // Generate pools of grass
        // -X to take into account walls
        RectInt rect = new RectInt(newRoom.X, newRoom.Y, newRoom.Width-3, newRoom.Height-3);
        HashSet<Vector2Int> grassTiles = GeneratePools(rect, 3.0f, 0.3f, Random.Range(0, 100), Random.Range(0, 100));

        foreach (Vector2Int tile in grassTiles)
        {
            MapManager.instance.CreateEntity("Grass", tile);
        }
    }

    public HashSet<Vector2Int> GeneratePools(RectInt rect, float scale, float cutoff, float xOrg, float yOrg)
    {
        HashSet<Vector2Int> poolTiles = new HashSet<Vector2Int>();
        // For each pixel in the texture...
        for(int y = 0; y < 128; y++) {
            for (int x = 0; x < 128; x++) {
                float xCoord = xOrg + x / 128f * scale;
                float yCoord = yOrg + y / 128f * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                if (sample < cutoff)
                {
                    Vector2Int tilePos = new Vector2Int(
                        Mathf.RoundToInt(rect.x + (x / 128f) * rect.width),
                        Mathf.RoundToInt(rect.y + (y / 128f) * rect.height)
                    );
                    poolTiles.Add(tilePos);
                }
            }
        }

        // Copy the pixel data to the texture and load it into the GPU.
        return poolTiles;
    }
}