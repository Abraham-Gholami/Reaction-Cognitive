using System.Collections.Generic;

namespace gamingCloud.templates
{

    public class ApiResponse
    {
        public Dictionary<string, object> response= new Dictionary<string, object>();
        public RestfulMessages status = RestfulMessages.successful;
        public bool isSuccessful = true;

        public ApiResponse(bool isSuccessful, int status, Dictionary<string,object> resp = null)
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
