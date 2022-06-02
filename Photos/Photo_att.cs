using System.Collections.Generic;

namespace Bot
{
    struct Photo_att
    {
        public Photo_att(string Access_key, int Album_id, uint Date, bool Has_tags, uint Height, uint Id, uint Owner_id,
            string Photo_1280, string Photo_130, string Photo_2560, string Photo_604, string Photo_75,
            string Photo_807, string Text, uint Width, uint Post_id, List<Elements_ph> Sizes, uint User_id)
        {
            this.access_key = Access_key;
            this.album_id = Album_id;
            this.date = Date;
            this.has_tags = Has_tags;
            this.height = Height;
            this.id = Id;
            this.owner_id = Owner_id;
            this.photo_1280 = Photo_1280;
            this.photo_130 = Photo_130;
            this.photo_2560 = Photo_2560;
            this.photo_604 = Photo_604;
            this.photo_75 = Photo_75;
            this.photo_807 = Photo_807;
            this.text = Text;
            this.width = Width;
            this.post_id = Post_id;
            this.sizes = Sizes;
            this.user_id = User_id;
        }

        public string Access_key { get { return this.access_key; } set { this.access_key = value; } }
        public int Album_id { get { return this.album_id; } set { this.album_id = value; } }
        public uint Date { get { return this.date; } set { this.date = value; } }
        public bool Has_tags { get { return this.has_tags; } set { this.has_tags = value; } }
        public uint Height { get { return this.height; } set { this.height = value; } }
        public uint Id { get { return this.id; } set { this.id = value; } }
        public uint Owner_id { get { return this.owner_id; } set { this.owner_id = value; } }
        public string Photo_1280 { get { return this.photo_1280; } set { this.photo_1280 = value; } }
        public string Photo_130 { get { return this.photo_130; } set { this.photo_130 = value; } }
        public string Photo_2560 { get { return this.photo_2560; } set { this.photo_2560 = value; } }
        public string Photo_604 { get { return this.photo_604; } set { this.photo_604 = value; } }
        public string Photo_75 { get { return this.photo_75; } set { this.photo_75 = value; } }
        public string Photo_807 { get { return this.photo_807; } set { this.photo_807 = value; } }
        public string Text { get { return this.text; } set { this.text = value; } }
        public uint Width { get { return this.width; } set { this.width = value; } }
        public uint Post_id { get { return this.post_id; } set { this.post_id = value; } }
        public List<Elements_ph> Sizes { get { return this.sizes; } set { this.sizes = value; } }
        public uint User_id { get { return this.user_id; } set { this.user_id = value; } }


        #region Поля

        public string access_key;
        public int album_id;
        public uint date;
        public bool has_tags;
        public uint height;
        public uint id;
        public uint owner_id;
        public string photo_1280;
        public string photo_130;
        public string photo_2560;
        public string photo_604;
        public string photo_75;
        public string photo_807;
        public string text;
        public uint width;
        public uint post_id;
        public List<Elements_ph> sizes;
        public uint user_id;

        #endregion
    }
}