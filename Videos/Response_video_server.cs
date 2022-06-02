namespace Bot
{
    struct Response_video_server
    {
        public Response_video_server(Response_video Response)
        {
            this.response = Response;
        }

        public Response_video Response { get { return this.response; } set { this.response = value; } }


        #region Поля

        public Response_video response;

        #endregion
    }
}