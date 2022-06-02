namespace Bot
{
    struct Element
    {
        public Element(string Event_id, uint Group_id, Message_reply_object Object, string Type)
        {
            this.event_id = Event_id;
            this.group_id = Group_id;
            this.object_a = Object;
            this.type = Type;
        }

        public string Event_id { get { return this.event_id; } set { this.event_id = value; } }
        public uint Group_id { get { return this.group_id; } set { this.group_id = value; } }
        public Message_reply_object Object { get { return this.object_a; } set { this.object_a = value; } }
        public string Type { get { return this.type; } set { this.type = value; } }


        #region Поля

        public string event_id;
        public uint group_id;
        public Message_reply_object object_a;
        public string type;

        #endregion
    }
}