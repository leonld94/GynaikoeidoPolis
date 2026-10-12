using System;
using System.Collections.Generic;
using static MapData;

public class Map
{
    public const int ChunkSize = 128;

    public List<Chunk> UnlockedChunks;  // 렌더할 것만 정리해놓는 리스트
    private MapData _data;

    public Map()
    {
        UnlockedChunks = new List<Chunk>();
    }

    public Map(MapData data)
    {
        UnlockedChunks = new List<Chunk>();

        Validate(data);
        _data = data;

        for(int chunkY = 0; chunkY < _data.height; chunkY++)
        {
            for(int chunkX = 0; chunkX < _data.width; chunkX++)
            {
                ChunkData chunkData = _data.chunks[chunkY * _data.width + chunkX];
                if (chunkData.isUnlocked)
                {
                    Chunk newChunk = new Chunk(chunkX, chunkY, chunkData);
                    UnlockedChunks.Add(newChunk);
                }
            }
        }
    }

    public ChunkData GetChunkData(int chunkX, int chunkY)
    {
        int index = chunkY * _data.width + chunkX;
        return _data.chunks[index];
    }


    //// 얘는 필요한 경우가 있나?
    //public MapData.ChunkData.TileData GetTile(
    //    int chunkX,
    //    int chunkY,
    //    int tileX,
    //    int tileY)
    //{
    //    MapData.ChunkData chunk = GetChunk(chunkX, chunkY);

    //    int index = tileY * ChunkSize + tileX;
    //    return chunk.tiles[index];
    //}

    public bool UnlockChunk()
    {
        // Todo: 이거 만들기.
        // 1. MapData 접근해서 Unlock 풀고 
        // 2. UnlockedChunks에 넣고
        // 3. 성공적인지 반환하기.
        return false;
    }

    private static void Validate(MapData data)
    {
        if (data.chunks.Length != data.width * data.height)
        {
            throw new System.ArgumentException(
                "청크 배열 길이가 width * height와 일치하지 않습니다.");
        }

        foreach (ChunkData chunkData in data.chunks)
        {
            if (chunkData.tiles.Length != ChunkSize * ChunkSize)
            {
                throw new System.ArgumentException(
                    $"모든 청크는 {ChunkSize * ChunkSize}개의 타일을 가져야 합니다.");
            }
        }
    }

    // Todo: 절차적 생성 class 만들기
    //private ProceduralGenerator proceduralGenerator;

}