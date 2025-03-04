using System.IO;

namespace KCD2HidingGroupsEditor.Skin
{
    public class SkinFile
    {
        public static uint Magic = 0x68437243;
        public static int Version = 0x746;
        public Dictionary<int, Chunk> Chunks = new();
        private Chunk? MeshChunk = null;
        public SkinFile()
        {

        }

        public SkinFile(string fileName)
        {
            Read(fileName);
        }

        public SkinFile(Stream s)
        {
            Read(s);
        }

        public SkinFile(BinaryReader br)
        {
            Read(br);
        }

        public void Read(string fileName)
        {
            using (FileStream fs = new(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Read(fs);
            }
        }

        public void Read(Stream s)
        {
            using (BinaryReader br = new(s))
            {
                Read(br);
            }
        }

        public void Read(BinaryReader br)
        {
            uint _magic = br.ReadUInt32();

            if (_magic != Magic)
            {
                throw new Exception("Incorrect file format.");
            }

            int _version = br.ReadInt32();

            if (_version != Version)
            {
                throw new Exception("Incorrect file version.");
            }

            int chunkCount = br.ReadInt32();
            int chunkTableOffset = br.ReadInt32();

            br.BaseStream.Position = chunkTableOffset;

            for (int i = 0; i < chunkCount; i++)
            {
                Chunk chunk = new(br);

                Chunks.Add(chunk.ID, chunk);
            }

            foreach (var chunk in Chunks)
            {
                chunk.Value.LoadData(br);

                if (chunk.Value.ChunkType == 0x1000)
                {
                    MeshChunk = chunk.Value;
                }
            }

            if (MeshChunk == null)
            {
                throw new Exception("Could not find Mesh chunk.");
            }
        }

        public void Write(string fileName)
        {
            using (MemoryStream ms = new())
            {
                Write(ms);

                File.WriteAllBytes(fileName, ms.ToArray());
            }
        }

        public void Write(Stream s)
        {
            using (BinaryWriter bw = new(s))
            {
                Write(bw);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Magic);
            bw.Write(Version);
            bw.Write(Chunks.Count);
            bw.Write(16);

            foreach (var chunk in Chunks)
            {
                chunk.Value.Write(bw); //Sizes and offsets shouldn't change, so we don't need to update anything
            }

            foreach (var chunk in Chunks) //Could be combined into 1 loop, but meh
            {
                chunk.Value.WriteData(bw);
            }
        }

        public void PatchColors(uint newAlpha)
        {
            int colorsChunkIndex = 0;

            using (MemoryStream ms = new(MeshChunk!.Data!))
            {
                using (BinaryReader chunkReader = new(ms))
                {
                    chunkReader.BaseStream.Position = 40; //Don't care
                    colorsChunkIndex = chunkReader.ReadInt32();
                }
            }

            if (colorsChunkIndex == 0)
            {
                throw new Exception("Skin does not contain vertex colors.");
            }

            Chunk? colorsChunk = Chunks[colorsChunkIndex];
            List<uint> colors = new();

            using (MemoryStream ms = new(colorsChunk.Data!))
            {
                using (BinaryReader chunkReader = new(ms))
                {
                    chunkReader.BaseStream.Position = 24; //Still don't care

                    int count = (colorsChunk.Size - 24) / 4;

                    for (int i = 0; i < count; i++)
                    {
                        colors.Add(chunkReader.ReadUInt32());
                    }
                }
            }

            using (MemoryStream ms = new(colorsChunk.Data!))
            {
                using (BinaryWriter chunkWriter = new(ms))
                {
                    chunkWriter.BaseStream.Position = 24; //Still don't care

                    foreach (var color in colors)
                    {
                        uint rgb = color & 0x00FFFFFF; //Isolate RGB values, yeet the alpha
                        uint rgba = rgb | (newAlpha << 24); //Add the new alpha

                        chunkWriter.Write(rgba);
                    }

                    if (chunkWriter.BaseStream.Position != chunkWriter.BaseStream.Length)
                    {
                        throw new Exception("Error overwriting vertex colors.");
                    }
                }
            }
        }
    }
}
