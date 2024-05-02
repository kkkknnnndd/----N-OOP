using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLCVN3.CS
{
    // Encapsulation: đóng gói thông tin vào các thuộc tính và tránh truy cập trực tiếp đến các trường dữ liệu
    public class Employee
    {
        private string id;
        private string name;
        private string email;
        private bool gender;
        private int age;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public bool Gender
        {
            get { return gender; }
            set { gender = value; }
        }

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        // Constructor
        public Employee(string id, string name, string email, bool gender, int age)
        {
            Id = id;
            Name = name;
            Email = email;
            Gender = gender;
            Age = age;
        }
    }
}