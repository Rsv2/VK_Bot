namespace Bot
{
    struct Response_audioserver
    {
        public Response_audioserver(Response_upload_url Response)
        {
            this.response = Response;
        }

        public Response_upload_url Response { get { return this.response; } set { this.response = value; } }


        #region Поля

        public Response_upload_url response;

        #endregion
    }
}