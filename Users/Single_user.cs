namespace Bot
{
    struct Single_user
    {
        public Single_user(string User_id, string User_name, string User_surname)
        {
            this.user_id = User_id;
            this.user_name = User_name;
            this.user_surname = User_surname;
        }

        public string User_id { get { return this.user_id; } set { this.user_id = value; } }
        public string User_name { get { return this.user_name; } set { this.user_name = value; } }
        public string User_surname { get { return this.user_surname; } set { this.user_surname = value; } }

        #region Поля

        public string user_id;
        public string user_name;
        public string user_surname;

        #endregion
    }
}