using System.Collections.Generic;

namespace gamingCloud.templates
{

    public class ApiResponseArray
    {
        public List<Dictionary<string, object> >response = new List<Dictionary<string, object>>();
        public RestfulMessages status = RestfulMessages.successful;
        public bool isSuccessful = true;

        public ApiResponseArray(bool isSuccessful, int status, List<Dictionary<string, object> >resp = null)
        {
            if (isSuccessful)
                response = resp;
            else
            {
                this.status = (RestfulMessages)status;
                this.isSuccessful = false;
            }

        }
    }
}
