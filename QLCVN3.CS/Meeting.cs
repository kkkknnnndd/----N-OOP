using System;

namespace QLCVN3.CS
{
    public class Meeting
    {
        private string project;
        private DateTime _date;
        private string _topic;
        private string _content;

        // Encapsulation: Đảm bảo tính đóng gói của các thuộc tính
        public string Project
        {
            get { return project; }
            set { project = value; }
        }

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public string Topic
        {
            get { return _topic; }
            set { _topic = value; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        // Constructor để khởi tạo một đối tượng Meeting
        public Meeting(string project, DateTime date, string topic, string content)
        {
            Project = project;
            Date = date;
            Topic = topic;
            Content = content;
        }
        public virtual void DisplayMeetingInfo()
        {
            Console.WriteLine($"Project: {Project}");
            Console.WriteLine($"Date: {Date.ToShortDateString()}");
            Console.WriteLine($"Topic: {Topic}");
            Console.WriteLine($"Content: {Content}");
        }
    }
}