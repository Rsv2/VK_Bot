namespace Bot
{
    struct Get_Doc
    {
        public Get_Doc(Response_Docs Doc, string Type)
        {
            this.doc = Doc;
            this.type = Type;
        }

        public Response_Docs Doc { get { return this.doc; } set { this.doc = value; } }
        public string Type { get { return this.type; } set { this.type = value; } }


        #region Поля

        public Response_Docs doc;
        public string type;

        #endregion
    }
}