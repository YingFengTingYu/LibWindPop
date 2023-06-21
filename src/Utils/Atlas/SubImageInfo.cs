namespace LibWindPop.Utils.Atlas
{
    internal struct SubImageInfo
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public string? ID;

        public SubImageInfo(int Width, int Height, string ID)
        {
            this.X = 0;
            this.Y = 0;
            this.Width = Width;
            this.Height = Height;
            this.ID = ID;
        }

        public SubImageInfo(int X, int Y, int Width, int Height, string ID)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
            this.ID = ID;
        }

        public void SetPos(SubImageInfo info)
        {
            this.X = info.X;
            this.Y = info.Y;
        }
    }
}
