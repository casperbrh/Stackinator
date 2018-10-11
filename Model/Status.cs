using System;
namespace Model
{
    public class Status
    {
        public Status()
        {
        }

        private int _statusCode;
        private string _statusType;

        public string StatusMessage { get; set; }

        public Status(int statusCode = 6, string message = null)
        {
            this._statusCode = statusCode;
            this.StatusMessage = message;
            SetErrorType(this._statusCode);
        }

        public string getStatusType()
        {
            return this._statusType;
        }

        private void SetErrorType(int statuscode)
        {
            switch (statuscode)
            {
                case 1:
                    this._statusType = "Ok";
                    break;
                case 2:
                    this._statusType = "Created";
                    break;
                case 3:
                    this._statusType = "Updated";
                    break;
                case 4:
                    this._statusType = "Bad Request";
                    break;
                case 5:
                    this._statusType = "Not found";
                    break;
                case 6:
                    this._statusType = "Error";
                    break;
                default:
                    this._statusType = "Unknown status";
                    break;
            }
        }

        public int GetStatusCode()
        {
            return this._statusCode;
        }

        public void SetStatusCode(int statusCode = 6)
        {
            this._statusCode = statusCode;
            SetErrorType(statusCode);
        }
    }
}

