using QLCVN3.CS;
using System;
using System.Collections.Generic;

namespace QLCVN3.CS
{
    public interface ITaskManagement
    {
        void AssignMembers(List<Member> projectMembers);
        void UpdateTaskProgress();
    }

    public class Task : ITaskManagement
    {
        private string _name;
        private DateTime _startDate;
        private DateTime _endDate;
        private int _process;
        private Member _incharge;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }

        public int Process
        {
            get { return _process; }
            set { _process = value; }
        }

        public Member Incharge
        {
            get { return _incharge; }
            set { _incharge = value; }
        }

        public Task(string name, DateTime startDate, DateTime endDate, int process, Member incharge)
        {
            _name = name;
            _startDate = startDate;
            _endDate = endDate;
            _process = process;
            _incharge = incharge;
        }

        public void AssignMembers(List<Member> projectMembers)
        {
            Console.WriteLine($"Danh sách các thành viên tham gia dự án:");
            for (int i = 0; i < projectMembers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {projectMembers[i].Name}");
            }

            Console.Write("Chọn số thứ tự của thành viên cần phân công: ");
            int index;
            if (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > projectMembers.Count)
            {
                Console.WriteLine("Lựa chọn không hợp lệ.");
                return;
            }

            // Lấy thông tin thành viên từ danh sách
            Member selectedMember = projectMembers[index - 1];

            // Thêm thành viên vào danh sách Incharge của Task
            _incharge = selectedMember;
            Console.WriteLine($"Phân công thành viên {selectedMember.Name} vào nhiệm vụ thành công.");
        }

        public void UpdateTaskProgress()
        {
            int newProgress;
            while (true)
            {
                Console.WriteLine($"Nhập tiến độ mới cho task {_name} (0-100%):");
                // Đọc đầu vào của người dùng
                string userInput = Console.ReadLine();

                // Kiểm tra tính hợp lệ của đầu vào
                if (int.TryParse(userInput, out newProgress) && newProgress >= 0 && newProgress <= 100)
                {
                    // Nếu đầu vào hợp lệ, cập nhật tiến độ nhiệm vụ
                    _process = newProgress;
                    Console.WriteLine($"Cập nhật tiến độ nhiệm vụ {_name} thành công.");
                    Program.WaitForEscKey();
                    break;
                }
                else
                {
                    Console.WriteLine("Tiến độ không hợp lệ. Vui lòng nhập lại giá trị từ 0 đến 100.");
                }
            }
        }
    }
}