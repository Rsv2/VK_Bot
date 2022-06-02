namespace Bot
{
    struct Response_video
    {
        public Response_video(string Access_key, string Description, int Owner_id, string Title, string Upload_url, uint Video_id)
        {
            this.access_key = Access_key;
            this.description = Description;
            this.owner_id = Owner_id;
            this.title = Title;
            this.upload_url = Upload_url;
            this.video_id = Video_id;
        }

        public string Access_key { get { return this.access_key; } set { this.access_key = value; } }
        public string Description { get { return this.description; } set { this.description = value; } }
        public int Owner_id { get { return this.owner_id; } set { this.owner_id = value; } }
        public string Title { get { return this.title; } set { this.title = value; } }
        public string Upload_url { get { return this.upload_url; } set { this.upload_url = value; } }
        public uint Video_id { get { return this.video_id; } set { this.video_id = value; } }


        #region Поля

        public string access_key;
        public string description;
        public int owner_id;
        public string title;
        public string upload_url;
        public uint video_id;

        #endregion
    }
}