namespace Bot
{
    struct Response_photo
    {
        public Response_photo(int Album_id, uint Group_id, string Upload_url, uint User_id)
        {
            this.album_id = Album_id;
            this.group_id = Group_id;
            this.upload_url = Upload_url;
            this.user_id = User_id;
        }

        public int Album_id { get { return this.album_id; } set { this.album_id = value; } }
        public uint Group_id { get { return this.group_id; } set { this.group_id = value; } }
        public string Upload_url { get { return this.upload_url; } set { this.upload_url = value; } }
        public uint User_id { get { return this.user_id; } set { this.user_id = value; } }


        #region Поля

        public int album_id;
        public uint group_id;
        public string upload_url;
        public uint user_id;

        #endregion
    }
}