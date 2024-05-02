using System;
using System.Collections.Generic;

namespace QLCVN3.CS
{
    // Lớp cơ sở cho tài khoản
    public class BaseAccount
    {
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public AccountType Type { get; set; }
        public string Id { get; set; }
        public List<string> ProjectsID { get; set; }
        public bool Active { get; set; }

        public BaseAccount() { }

        public BaseAccount(string fullname, string username, string password, AccountType type, string id, string projectid, bool active)
        {
            Fullname = fullname;
            Username = username;
            Password = password;
            Type = type;
            ProjectsID = new List<string>();
            ProjectsID.Add(projectid);
            Id = id;
            Active = active;
        }

        // Phương thức để thêm một dự án vào danh sách dự án của tài khoản
        public virtual void AddProject(string projectid)
        {
            if (ProjectsID == null)
                ProjectsID = new List<string>();
            ProjectsID.Add(projectid);
            Console.WriteLine($"Thêm dự án cho tài khoản {Username} thành công.");
        }
    }

    // Lớp tài khoản kế thừa từ lớp cơ sở BaseAccount
    public class Account : BaseAccount
    {
        public Account() { }

        public Account(string fullname, string username, string password, AccountType type, string id, string projectid, bool active)
            : base(fullname, username, password, type, id, projectid, active)
        {
        }

        // Ghi đè phương thức AddProject để cung cấp triển khai riêng cho lớp Account
        public override void AddProject(string projectid)
        {
            // Thực hiện thêm dự án vào danh sách dự án của tài khoản
            base.AddProject(projectid);

            // Đặt thuộc tính Active thành true
            Active = true;
        }
        public bool IsActive()
        {
            return Active;
        }
        public interface IAccount
        {
            void AddProject(string projectid);
        }
    }

    // Enum định nghĩa loại tài khoản
    public enum AccountType
    {
        Admin,
        Member
    }
}