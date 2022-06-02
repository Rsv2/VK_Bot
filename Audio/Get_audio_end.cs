namespace Bot
{
    struct Get_audio_end
    {
        public Get_audio_end(Response_audio_end Response)
        {
            this.response = Response;
        }

        public Response_audio_end Response { get { return this.response; } set { this.response = value; } }


        #region Поля

        public Response_audio_end response;

        #endregion
    }
}