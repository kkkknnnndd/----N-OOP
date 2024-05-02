using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLCVN3.CS
{
    public class Report
    {
        private string _title;
        private DateTime _date;

        // Encapsulation: Đảm bảo tính đóng gói của các thuộc tính
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        // Phương thức để tạo báo cáo về các task trong dự án và sắp xếp theo tiến độ
        public virtual void GenerateTaskReportForAdmin(Project project)
        {
            Console.Clear();
            Title = "Báo cáo chi tiết task trong dự án";
            Date = DateTime.Now;

            Console.WriteLine($"{Title}");
            Console.WriteLine($"Ngày báo cáo: {Date}");

            // Sắp xếp danh sách các task theo thứ tự tiến độ (process) bằng cách sử dụng thuật toán sắp xếp
            List<Task> tasks = project.Tasks;
            for (int i = 0; i < tasks.Count - 1; i++)
            {
                for (int j = i + 1; j < tasks.Count; j++)
                {
                    if (tasks[i].Process > tasks[j].Process)
                    {
                        // Hoán đổi task nếu cần
                        Task temp = tasks[i];
                        tasks[i] = tasks[j];
                        tasks[j] = temp;
                    }
                }
            }

            // In ra thông tin về từng task
            foreach (Task task in tasks)
            {
                Console.WriteLine($"Task: {task.Name}");
                Console.WriteLine($"Tiến độ: {task.Process}%");
                Console.WriteLine($"Ngày bắt đầu: {task.StartDate:dd/MM/yyyy}");
                Console.WriteLine($"Ngày kết thúc: {task.EndDate:dd/MM/yyyy}");
                TimeSpan duration = task.EndDate.Date - task.StartDate.Date;
                Console.WriteLine($"Thời gian còn lại: {duration.Days} ngày");

                // In ra thông tin về người phụ trách task
                if (task.Incharge != null)
                {
                    Console.WriteLine($"Nhân viên phụ trách: {task.Incharge.Name}");
                }
                else
                {
                    Console.WriteLine("Chưa có phân công");
                }

                Console.WriteLine(); // Dòng trống để phân biệt giữa các task
            }
        }
        public virtual void GenerateTaskReportForUser(Project project, Account currentAccount)
        {
            Console.Clear();
            Title = "Báo cáo chi tiết task trong dự án";
            Date = DateTime.Now;

            Console.WriteLine($"{Title} {project.Name} ");
            Console.WriteLine($"Ngày báo cáo: {Date}");

            // Sắp xếp danh sách các task theo thứ tự tiến độ (process) bằng cách sử dụng thuật toán sắp xếp



            List<Task> tasks = project.Tasks;

            // In ra thông tin về từng task
            foreach (Task task in tasks)
            {
                if (task.Incharge != null && task.Incharge.Id == currentAccount.Id)
                {
                    Console.WriteLine($"Task: {task.Name}");
                    Console.WriteLine($"Tiến độ: {task.Process}%");
                    Console.WriteLine($"Ngày bắt đầu: {task.StartDate:dd/MM/yyyy}");
                    Console.WriteLine($"Ngày kết thúc: {task.EndDate:dd/MM/yyyy}");
                    Console.WriteLine($"Tiến độ: {task.Process}");
                    TimeSpan duration = task.EndDate.Date - task.StartDate.Date;
                    Console.WriteLine($"Thời gian còn lại: {duration.Days} ngày");
                }





                Console.WriteLine(); // Dòng trống để phân biệt giữa các task
            }
        }
        public virtual void GenerateAdminProjectReport(List<Project> projects)
        {
            Title = "Báo cáo tổng quán các dự án";
            Date = DateTime.Now;

            // Kiểm tra xem có dự án nào không
            if (projects == null || projects.Count == 0)
            {
                Console.WriteLine("Hiện không có dự án nào.");
                return;
            }

            // In tiêu đề báo cáo
            Console.WriteLine($"Báo cáo các dự án");
            Console.WriteLine($"Ngày báo cáo: {Date}");
            Console.WriteLine();

            // Duyệt qua từng dự án trong danh sách
            foreach (Project project in projects)
            {
                // In thông tin về dự án
                Console.WriteLine($"ID dự án: {project.Id}");
                Console.WriteLine($"Tên dự án: {project.Name}");
                Console.WriteLine($"Mục tiêu: {project.Target}");
                Console.WriteLine($"Ngày bắt đầu: {project.StartDate:dd/MM/yyyy}");
                Console.WriteLine($"Ngày kết thúc: {project.EndDate:dd/MM/yyyy}");
                Console.WriteLine($"Mô tả: {project.Description}");
                Console.WriteLine($"Trạng thái: {project.Status}");
                Console.WriteLine($"Thời gian còn lại: {project.EndDate.Date - DateTime.Now.Date}");
                // Tính phần trăm hoàn thành của dự án
                double totalProgress = 0;
                int totalTasks = project.Tasks.Count;

                // Duyệt qua từng task để tính tổng tiến độ
                foreach (Task task in project.Tasks)
                {
                    totalProgress += task.Process;
                }

                // Tính phần trăm hoàn thành của dự án
                double projectCompletionPercentage = totalProgress / totalTasks;

                Console.WriteLine($"Phần trăm hoàn thành của dự án: {projectCompletionPercentage:0.00}%");


                Console.WriteLine(); // Tạo khoảng trống giữa các dự án
            }
        }
    }
}