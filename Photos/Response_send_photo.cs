namespace Bot
{
    struct Response_send_photo
    {
        public Response_send_photo(Response_photo Response)
        {
            this.response = Response;
        }

        public Response_photo Response { get { return this.response; } set { this.response = value; } }


        #region Поля

        public Response_photo response;

        #endregion
    }
}