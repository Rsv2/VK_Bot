namespace Bot
{
    struct AccountData
    {
        public AccountData(string AccountID, string GroupID, string GroupToken, string SecretKey, string ApplicationID, string IndToken, string LastDate, string Exp)
        {
            this.accountid = AccountID;
            this.groupid = GroupID;
            this.grouptoken = GroupToken;
            this.secretkey = SecretKey;
            this.applicationid = ApplicationID;
            this.indtoken = IndToken;
            this.lastdate = LastDate;
            this.exp = Exp;
        }

        public string AccountID { get { return this.accountid; } set { this.accountid = value; } }
        public string GroupID { get { return this.groupid; } set { this.groupid = value; } }
        public string GroupToken { get { return this.grouptoken; } set { this.grouptoken = value; } }
        public string SecretKey { get { return this.secretkey; } set { this.secretkey = value; } }
        public string ApplicationID { get { return this.applicationid; } set { this.applicationid = value; } }
        public string IndToken { get { return this.indtoken; } set { this.indtoken = value; } }
        public string LastDate { get { return this.lastdate; } set { this.lastdate = value; } }
        public string Exp { get { return this.exp; } set { this.exp = value; } }

        #region Поля

        public string accountid;
        public string groupid;
        public string grouptoken;
        public string secretkey;
        public string applicationid;
        public string indtoken;
        public string exp;
        public string lastdate;

        #endregion
    }
}