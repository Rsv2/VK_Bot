using System.Collections.Generic;

namespace Bot
{
    struct Video_element
    {
        public Video_element(string Access_key, uint Can_add, uint Can_attach_link, uint Can_edit, uint Comments,
            uint Date, string Description, uint Duration, List<Video_firstframe> First_frame, uint Height, uint Id,
            List<Video_images> Image, int Owner_id, string Title, string Type, uint Views, uint Width)
        {
            this.access_key = Access_key;
            this.can_add = Can_add;
            this.can_attach_link = Can_attach_link;
            this.can_edit = Can_edit;
            this.comments = Comments;
            this.date = Date;
            this.description = Description;
            this.duration = Duration;
            this.first_frame = First_frame;
            this.height = Height;
            this.id = Id;
            this.image = Image;
            this.owner_id = Owner_id;
            this.title = Title;
            this.type = Type;
            this.views = Views;
            this.width = Width;
        }

        public string Access_key { get { return this.access_key; } set { this.access_key = value; } }
        public uint Can_add { get { return this.can_add; } set { this.can_add = value; } }
        public uint Can_attach_link { get { return this.can_attach_link; } set { this.can_attach_link = value; } }
        public uint Can_edit { get { return this.can_edit; } set { this.can_edit = value; } }
        public uint Comments { get { return this.comments; } set { this.comments = value; } }
        public uint Date { get { return this.date; } set { this.date = value; } }
        public string Description { get { return this.description; } set { this.description = value; } }
        public uint Duration { get { return this.duration; } set { this.duration = value; } }
        public List<Video_firstframe> First_frame { get { return this.first_frame; } set { this.first_frame = value; } }
        public uint Height { get { return this.height; } set { this.height = value; } }
        public uint Id { get { return this.id; } set { this.id = value; } }
        public List<Video_images> Image { get { return this.image; } set { this.image = value; } }
        public int Owner_id { get { return this.owner_id; } set { this.owner_id = value; } }
        public string Title { get { return this.title; } set { this.title = value; } }
        public string Type { get { return this.type; } set { this.type = value; } }
        public uint Views { get { return this.views; } set { this.views = value; } }
        public uint Width { get { return this.width; } set { this.width = value; } }


        #region Поля

        public string access_key;
        public uint can_add;
        public uint can_attach_link;
        public uint can_edit;
        public uint comments;
        public uint date;
        public string description;
        public uint duration;
        public List<Video_firstframe> first_frame;
        public uint height;
        public uint id;
        public List<Video_images> image;
        public int owner_id;
        public string title;
        public string type;
        public uint views;
        public uint width;

        #endregion
    }
}