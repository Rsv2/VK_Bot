namespace Bot
{
    struct Video_images
    {
        public Video_images(uint Height, string Url, uint Width, uint With_padding)
        {
            this.height = Height;
            this.url = Url;
            this.width = Width;
            this.with_padding = With_padding;
        }

        public uint Height { get { return this.height; } set { this.height = value; } }
        public string Url { get { return this.url; } set { this.url = value; } }
        public uint Width { get { return this.width; } set { this.width = value; } }
        public uint With_padding { get { return this.with_padding; } set { this.with_padding = value; } }


        #region Поля

        public uint height;
        public string url;
        public uint width;
        public uint with_padding;

        #endregion
    }
}