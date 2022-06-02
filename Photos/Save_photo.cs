namespace Bot
{
    struct Save_photo
    {
        public Save_photo(string Hash, string Photo, uint Server)
        {
            this.hash = Hash;
            this.photo = Photo;
            this.server = Server;
        }

        public string Hash { get { return this.hash; } set { this.hash = value; } }
        public string Photo { get { return this.photo; } set { this.photo = value; } }
        public uint Server { get { return this.server; } set { this.server = value; } }

        #region Поля

        public string hash;
        public string photo;
        public uint server;

        #endregion
    }
}