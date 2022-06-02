namespace Bot
{
    struct Response_getdoc
    {
        public Response_getdoc(Get_Doc Response)
        {
            this.response = Response;
        }

        public Get_Doc Response { get { return this.response; } set { this.response = value; } }


        #region Поля

        public Get_Doc response;

        #endregion
    }
}