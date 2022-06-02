namespace Bot
{
    struct GetFile
    {
        public GetFile(string File)
        {
            this.file = File;
        }

        public string File { get { return this.file; } set { this.file = value; } }


        #region Поля

        public string file;

        #endregion
    }
}