using System;

namespace QLCVN3.CS
{
    // Abstract class để định nghĩa cấu trúc chung cho các loại tin nhắn
    public abstract class MessageBase
    {
        public string Content { get; set; }
        public DateTime DateTimeOfContent { get; set; }
        public string ProjectId { get; set; }
        public string Fullname { get; set; }

        public abstract void SendMessage();
    }

    // Lớp kế thừa từ lớp abstract
    public class Message : MessageBase
    {
        // Constructor
        public Message(string content, DateTime dateTimeOfContent, string projectId, string fullname)
        {
            Content = content;
            DateTimeOfContent = dateTimeOfContent;
            ProjectId = projectId;
            Fullname = fullname;
        }

        // Override phương thức abstract
        public override void SendMessage()
        {
            // Đây là nơi triển khai logic gửi tin nhắn, chẳng hạn sử dụng một service nào đó để gửi tin nhắn
            Console.WriteLine($"Sending message: {Content}");
            Console.WriteLine($"Project ID: {ProjectId}");
            Console.WriteLine($"Sent by: {Fullname}");
            Console.WriteLine($"Sent at: {DateTimeOfContent}");
        }
    }
}