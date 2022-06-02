namespace Bot
{
    struct GetLongPollServer
    {
        public GetLongPollServer(string Key, string Server, uint Ts)
        {
            this.key = Key;
            this.server = Server;
            this.ts = Ts;
        }

        public string Key { get { return this.key; } set { this.key = value; } }
        public string Server { get { return this.server; } set { this.server = value; } }
        public uint Ts { get { return this.ts; } set { this.ts = value; } }


        #region Поля

        public string key;
        public string server;
        public uint ts;

        #endregion
    }
}