using System.IO;

namespace KCD2HidingGroupsEditor.Skin
{
    public class Chunk
    {
        public short ChunkType { get; set; }
        public short Version { get; set; }
        public int ID { get; set; }
        public int Size { get; set; }
        public int Offset { get; set; }
        public byte[]? Data { get; set; } = null;

        public Chunk()
        {

        }

        public Chunk(BinaryReader br)
        {
            Read(br);
        }

        public void Read(BinaryReader br)
        {
            ChunkType = br.ReadInt16();
            Version = br.ReadInt16();
            ID = br.ReadInt32();
            Size = br.ReadInt32();
            Offset = br.ReadInt32();
        }

        public void LoadData(BinaryReader br)
        {
            br.BaseStream.Position = Offset;
            Data = br.ReadBytes(Size);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(ChunkType);
            bw.Write(Version);
            bw.Write(ID);
            bw.Write(Size);
            bw.Write(Offset);
        }

        public void WriteData(BinaryWriter bw)
        {
            bw.BaseStream.Position = Offset;
            bw.Write(Data!);
        }
    }
}
