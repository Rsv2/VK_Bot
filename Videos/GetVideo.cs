namespace Bot
{
    struct GetVideo
    {
        public GetVideo(int Owner_id, uint Size, string Video_hash, uint Video_id)
        {
            this.owner_id = Owner_id;
            this.size = Size;
            this.video_hash = Video_hash;
            this.video_id = Video_id;
        }

        public int Owner_id { get { return this.owner_id; } set { this.owner_id = value; } }
        public uint Size { get { return this.size; } set { this.size = value; } }
        public string Video_hash { get { return this.video_hash; } set { this.video_hash = value; } }
        public uint Video_id { get { return this.video_id; } set { this.video_id = value; } }


        #region Поля

        public int owner_id;
        public uint size;
        public string video_hash;
        public uint video_id;

        #endregion
    }
}