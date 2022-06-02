namespace Bot
{
    struct Response_audio_end
    {
        public Response_audio_end(string Artist, uint Date, uint Duration, uint Genre_id, uint Id, uint Owner_id, string Title, string Track_code, string Url)
        {
            this.artist = Artist;
            this.date = Date;
            this.duration = Duration;
            this.genre_id = Genre_id;
            this.id = Id;
            this.owner_id = Owner_id;
            this.title = Title;
            this.track_code = Track_code;
            this.url = Url;
        }

        public string Artist { get { return this.artist; } set { this.artist = value; } }
        public uint Date { get { return this.date; } set { this.date = value; } }
        public uint Duration { get { return this.duration; } set { this.duration = value; } }
        public uint Genre_id { get { return this.genre_id; } set { this.genre_id = value; } }
        public uint Id { get { return this.id; } set { this.id = value; } }
        public uint Owner_id { get { return this.owner_id; } set { this.owner_id = value; } }
        public string Title { get { return this.title; } set { this.title = value; } }
        public string Track_code { get { return this.track_code; } set { this.track_code = value; } }
        public string Url { get { return this.url; } set { this.url = value; } }


        #region Поля

        public string artist;
        public uint date;
        public uint duration;
        public uint genre_id;
        public uint id;
        public uint owner_id;
        public string title;
        public string track_code;
        public string url;

        #endregion
    }
}