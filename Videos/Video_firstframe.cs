namespace Bot
{
    struct Video_firstframe
    {
        public Video_firstframe(uint Height, string Url, uint Width)
        {
            this.height = Height;
            this.url = Url;
            this.width = Width;
        }

        public uint Height { get { return this.height; } set { this.height = value; } }
        public string Url { get { return this.url; } set { this.url = value; } }
        public uint Width { get { return this.width; } set { this.width = value; } }


        #region Поля

        public uint height;
        public string url;
        public uint width;

        #endregion
    }
}