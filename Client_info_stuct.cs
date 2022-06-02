using System.Collections.Generic;

namespace Bot
{
    struct Client_info_stuct
    {
        public Client_info_stuct(List<string> Button_actions, bool Inline_keyboard, bool Keyboard, uint Lang_id)
        {
            this.button_actions = Button_actions;
            this.inline_keyboard = Inline_keyboard;
            this.keyboard = Keyboard;
            this.lang_id = Lang_id;
        }

        public List<string> Button_actions { get { return this.button_actions; } set { this.button_actions = value; } }
        public bool Inline_keyboard { get { return this.inline_keyboard; } set { this.inline_keyboard = value; } }
        public bool Keyboard { get { return this.keyboard; } set { this.keyboard = value; } }
        public uint Lang_id { get { return this.lang_id; } set { this.lang_id = value; } }


        #region Поля

        public List<string> button_actions;
        public bool inline_keyboard;
        public bool keyboard;
        public uint lang_id;

        #endregion
    }
}