using System.Collections.Generic;

namespace Bot
{
    struct Response_photo_save
    {
        public Response_photo_save(List<Photo_att> Response)
        {
            this.response = Response;
        }

        public List<Photo_att> Response { get { return this.response; } set { this.response = value; } }

        #region Поля

        public List<Photo_att> response;

        #endregion
    }
}