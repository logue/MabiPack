namespace MabiPacker.Library
{
    public struct Entry
    {
        public uint Index;
        public string Name;

        public Entry(uint Index, string Name)
        {
            this.Index = Index;
            this.Name = Name;
        }
    }
}
