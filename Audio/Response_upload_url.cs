namespace Bot
{
    struct Response_upload_url
    {
        public Response_upload_url(string Upload_url)
        {
            this.upload_url = Upload_url;
        }

        public string Upload_url { get { return this.upload_url; } set { this.upload_url = value; } }


        #region Поля

        public string upload_url;

        #endregion
    }
}