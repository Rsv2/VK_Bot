namespace Bot
{
    struct Attachment
    {
        public Attachment(List<Photo_att> Photo)
        {
            this.photo = Photo;
        }

        public List<Photo_att> Photo { get { return this.photo; } set { this.photo = value; } }


        #region Поля

        public list<photo_att> photo;

        #endregion
    }
}