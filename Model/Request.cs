using System;
namespace Model
{
    public class Request
    {
        public Request()
        {
        }

        public String Method { get; set; }
        public String Path { get; set; }
        //UNIX format
        public long DateTime { get; set; }
        public String Body { get; set; }
    }
}
