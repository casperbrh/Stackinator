using System;
using System.Collections.Generic;

namespace Model
{
    public class Response
    {
        public String Status { get; set; }
        public String Body { get; set; }

        public Response()
        {

        }

        public Response(string status, string body = null)
        {
            this.Status = status;
            this.Body = body;
        }

        public Response(Status status, string body = null)
        {
            this.Body = body;
            this.Status = $"{status.GetStatusCode()} {status.getStatusType()}";
            if (status.StatusMessage != null)
            {
                this.Status += $": {status.StatusMessage}";
            }
        }

        public Response(List<Status> statuses, string body = null)
        {
            this.Body = body;

            for (int i = 0; i < statuses.Count; i++)
            {
                this.Status += $"{statuses[i].GetStatusCode()} {statuses[i].getStatusType()}";
                if (statuses[i].StatusMessage != null)
                {
                    this.Status += $": {statuses[i].StatusMessage}";
                }

                if (i < statuses.Count + -1)
                {
                    this.Status += "\n";
                }
            }
        }

    }
}
