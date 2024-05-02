
using QLCVN3.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QLCVN3.CS
{
    // Inheritance: kế thừa các thuộc tính và phương thức từ lớp cơ sở
    public class Member : Employee
    {
        public Member(string id, string name, string email, bool gender, int age, Project project)
            : base(id, name, email, gender, age)
        {
        }
        public override string ToString()
        {
            return $"ID: {Id}\n" +
                   $"Tên: {Name}\n" +
                   $"Email: {Email}\n" +
                   $"Giới tính: {(Gender ? "Nam" : "Nữ")}\n" +
                   $"Tuổi: {Age}\n";
        }
    }
}
