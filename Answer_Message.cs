using System.Collections.Generic;

namespace Bot
{
    struct Answer_Message
    {
        public Answer_Message(List<Attachment> Attachments, uint Conversation_message_id, uint Date, string From_id, List<string> Fwd_messages,
            uint Id, string Important, string Is_hidden, uint Out, uint Peer_id, uint Random_id, string Text)
        {
            this.attachments = Attachments;
            this.conversation_message_id = Conversation_message_id;
            this.date = Date;
            this.from_id = From_id;
            this.fwd_messages = Fwd_messages;
            this.id = Id;
            this.important = Important;
            this.is_hidden = Is_hidden;
            this.out_a = Out;
            this.peer_id = Peer_id;
            this.random_id = Random_id;
            this.text = Text;
        }

        public List<Attachment> Attachments { get { return this.attachments; } set { this.attachments = value; } }
        public uint Conversation_message_id { get { return this.conversation_message_id; } set { this.conversation_message_id = value; } }
        public uint Date { get { return this.date; } set { this.date = value; } }
        public string From_id { get { return this.from_id; } set { this.from_id = value; } }
        public List<string> Fwd_messages { get { return this.fwd_messages; } set { this.fwd_messages = value; } }
        public uint Id { get { return this.id; } set { this.id = value; } }
        public string Important { get { return this.important; } set { this.important = value; } }
        public string Is_hidden { get { return this.is_hidden; } set { this.is_hidden = value; } }
        public uint Out { get { return this.out_a; } set { this.out_a = value; } }
        public uint Peer_id { get { return this.peer_id; } set { this.peer_id = value; } }
        public uint Random_id { get { return this.random_id; } set { this.random_id = value; } }
        public string Text { get { return this.text; } set { this.text = value; } }


        #region Поля

        public List<Attachment> attachments;
        public uint conversation_message_id;
        public uint date;
        public string from_id;
        public List<string> fwd_messages;
        public uint id;
        public string important;
        public string is_hidden;
        public uint out_a;
        public uint peer_id;
        public uint random_id;
        public string text;

        #endregion
    }
}