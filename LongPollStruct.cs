using System.Collections.Generic;

namespace Bot
{
    struct LongPollStruct
    {
        public LongPollStruct(uint Ts, List<Element> Updates)
        {
            this.ts = Ts;
            this.updates = Updates;
        }

        public uint Ts { get { return this.ts; } set { this.ts = value; } }
        public List<Element> Updates { get { return this.updates; } set { this.updates = value; } }


        #region Поля

        public uint ts;
        public List<Element> updates;

        #endregion
    }
}