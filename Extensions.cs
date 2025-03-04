namespace KCD2HidingGroupsEditor
{
    static class Extensions
    {
        public static bool GetBit(this byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }

        public static byte SetBit(this byte b, int bitNumber)
        {
            b &= (byte)~(1 << bitNumber);
            return b;
        }

        public static uint ConstructByte(this bool[] Values)
        {
            if (Values.Length != 8)
            {
                throw new Exception("Incorrect bit count");
            }

            byte b = 0xFF;

            for (int i = 0; i < Values.Length; i++)
            {
                if (!Values[i])
                {
                    b = b.SetBit(i);
                }
            }

            return b;
        }
    }
}