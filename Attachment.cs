namespace Bot
{
    struct Attachment
    {
        public Attachment(Photo_att Photo, Response_audio_end Audio, Response_Docs Doc, Video_element Video, string Type)
        {
            this.photo = Photo;
            this.audio = Audio;
            this.doc = Doc;
            this.video = Video;
            this.type = Type;
        }

        public Photo_att Photo { get { return this.photo; } set { this.photo = value; } }
        public Response_audio_end Audio { get { return this.audio; } set { this.audio = value; } }
        public Response_Docs Doc { get { return this.doc; } set { this.doc = value; } }
        public Video_element Video { get { return this.video; } set { this.video = value; } }
        public string Type { get { return this.type; } set { this.type = value; } }

        #region Поля

        public Photo_att photo;
        public Response_audio_end audio;
        public Response_Docs doc;
        public Video_element video;
        public string type;

        #endregion
    }
}