namespace Bot
{
    struct Elements_ph
    {
        public Elements_ph(uint Height, string Type, string Url, uint Width)
        {
            this.height = Height;
            this.type = Type;
            this.url = Url;
            this.width = Width;
        }

        public uint Height { get { return this.height; } set { this.height = value; } }
        public string Type { get { return this.type; } set { this.type = value; } }
        public string Url { get { return this.url; } set { this.url = value; } }
        public uint Width { get { return this.width; } set { this.width = value; } }


        #region Поля

        public uint height;
        public string type;
        public string url;
        public uint width;

        #endregion
    }
}