namespace Bot
{
    struct Response_Docs
    {
        public Response_Docs(uint Date, string Ext, uint Id, int Owner_id, uint Size, string Title, uint Type, string Url)
        {
            this.date = Date;
            this.ext = Ext;
            this.id = Id;
            this.owner_id = Owner_id;
            this.size = Size;
            this.title = Title;
            this.type = Type;
            this.url = Url;
        }

        public uint Date { get { return this.date; } set { this.date = value; } }
        public string Ext { get { return this.ext; } set { this.ext = value; } }
        public uint Id { get { return this.id; } set { this.id = value; } }
        public int Owner_id { get { return this.owner_id; } set { this.owner_id = value; } }
        public uint Size { get { return this.size; } set { this.size = value; } }
        public string Title { get { return this.title; } set { this.title = value; } }
        public uint Type { get { return this.type; } set { this.type = value; } }
        public string Url { get { return this.url; } set { this.url = value; } }


        #region Поля

        public uint date;
        public string ext;
        public uint id;
        public int owner_id;
        public uint size;
        public string title;
        public uint type;
        public string url;

        #endregion
    }
}