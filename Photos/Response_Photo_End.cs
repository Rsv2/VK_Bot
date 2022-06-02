using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot
{
    struct Response_Photo_End
    {
        public Response_Photo_End(List<Response_photo_save> Response)
        {
            this.response = Response;
        }

        public List<Response_photo_save> Response { get { return this.response; } set { this.response = value; } }


        #region Поля

        public List<Response_photo_save> response;

        #endregion
    }
}
