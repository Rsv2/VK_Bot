namespace Bot
{
    struct Audio_saved
    {
        public Audio_saved(string Audio, string Hash, string Redirect, uint Server)
        {
            this.audio = Audio;
            this.hash = Hash;
            this.redirect = Redirect;
            this.server = Server;
        }

        public string Audio { get { return this.audio; } set { this.audio = value; } }
        public string Hash { get { return this.hash; } set { this.hash = value; } }
        public string Redirect { get { return this.redirect; } set { this.redirect = value; } }
        public uint Server { get { return this.server; } set { this.server = value; } }


        #region Поля

        public string audio;
        public string hash;
        public string redirect;
        public uint server;

        #endregion
    }
}