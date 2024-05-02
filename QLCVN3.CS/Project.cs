using QLCVN3.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace QLCVN3.CS
{
    public interface IProject
    {
        void AddMemberToProject();
        void AddTasksToProject();
        void AssignMembers();
        void EditTaskInProject();
        List<Task> ShowTasksForAccount(Account account);
        void RemoveTaskFromProject();
        void DisplayAllMembers();
        void DisplayAndRemoveMember();
        void CreateMeeting();
        void RemoveMeetingFromProject();
        void DisplayMeetingsAfter();
        void CreateMessage(Account currentAccount);
        // Add other project-related functionalities here
    }

    public class Project : IProject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Target { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public List<Task> Tasks { get; set; }
        public List<Meeting> Meetings { get; set; }

        public List<Member> Members { get; set; }

        public List<Message> Messages { get; set; }

        // Constructor của lớp Project

        public Project() { }
        public Project(string id, string name, int target, DateTime startDate, DateTime endDate, string description, string status)
        {
            Id = id;
            Name = name;
            Target = target;
            StartDate = startDate;
            EndDate = endDate;
            Description = description;
            Status = status;
            Tasks = new List<Task>();
            Meetings = new List<Meeting>();
            Members = new List<Member>();
            Messages = new List<Message>();

        }
        public static Project operator +(Project project1, Project project2)
        {
            // Yêu cầu người dùng nhập mô tả cho dự án kết hợp
            Console.WriteLine("Vui lòng nhập mô tả cho dự án kết hợp:");
            string userDescription = Console.ReadLine(); // Đọc mô tả từ đầu vào của người dùng và lưu vào biến

            // Tạo danh sách thành viên kết hợp và thêm thành viên từ cả hai dự án
            List<Member> combinedMembers = new List<Member>(project1.Members);
            foreach (Member member in project2.Members)
            {
                if (!combinedMembers.Contains(member))
                {
                    combinedMembers.Add(member);
                }
            }

            // Kết hợp danh sách công việc từ hai dự án
            List<Task> combinedTasks = new List<Task>(project1.Tasks);
            combinedTasks.AddRange(project2.Tasks);

            // Đặt tên ID là tên của hai dự án kết hợp bằng dấu gạch ngang
            string combinedProjectId = $"{project1.Id}-{project2.Id}";

            // Tạo dự án mới để kết hợp hai dự án ban đầu
            Project combinedProject = new Project
            {
                Id = combinedProjectId, // Sử dụng tên ID kết hợp
                Name = $"{project1.Name} & {project2.Name}",
                Target = project1.Target + project2.Target,
                StartDate = project1.StartDate < project2.StartDate ? project1.StartDate : project2.StartDate,
                EndDate = project1.EndDate > project2.EndDate ? project1.EndDate : project2.EndDate,
                Description = userDescription, // Lưu mô tả người dùng đã nhập
                Status = "Đang triễn khai",
                Tasks = combinedTasks, // Sử dụng danh sách công việc kết hợp
                Meetings = new List<Meeting>(), // Khởi tạo danh sách Meetings trống
                Messages = new List<Message>(),
                Members = combinedMembers // Sử dụng danh sách thành viên kết hợp
            };

            // Trả về dự án kết hợp
            return combinedProject;
        }
        public void AddMemberToProject()
        {
            Console.Clear();
            Console.WriteLine("Danh sách nhân viên có sẵn:");
            // Hiển thị danh sách nhân viên
            for (int i = 0; i < Program.employees.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Program.employees[i].Name}");
            }

            Console.Write("Chọn số thứ tự của nhân viên cần thêm vào dự án: ");
            int index;
            if (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > Program.employees.Length)
            {
                Console.WriteLine("Lựa chọn không hợp lệ.");
                return;
            }

            // Lấy thông tin nhân viên từ danh sách nhân viên
            Employee selectedEmployee = Program.employees[index - 1];

            // Kiểm tra xem nhân viên đã tồn tại trong dự án chưa
            bool isAlreadyMember = false;
            if (Members != null)
            {
                foreach (Member member in Members)
                {
                    if (member.Id == selectedEmployee.Id)
                    {
                        isAlreadyMember = true;
                        break;
                    }
                }
            }
            if (isAlreadyMember)
            {
                Console.WriteLine("Nhân viên này đã tồn tại trong dự án.");
                return;
            }

            // Chuyển đổi Employee thành Member
            Member newMember = new Member(selectedEmployee.Id, selectedEmployee.Name, selectedEmployee.Email, selectedEmployee.Gender, selectedEmployee.Age, this);

            // Thêm nhân viên mới vào danh sách thành viên của dự án
            if (Members == null)
            {
                Members = new List<Member>();
            }
            Members.Add(newMember);

            Console.WriteLine($"Thêm nhân viên {newMember.Name} vào dự án thành công.");

            // Tạo tài khoản đăng nhập cho nhân viên
            string fullname = selectedEmployee.Name;
            string username = selectedEmployee.Id; // Tạo tên đăng nhập theo thứ tự NV01, NV02, ...
            string password = "123"; // Mật khẩu mặc định là 123
            string id = selectedEmployee.Id; // Mật khẩu mặc định là 123
            Program.AccManager.CreateEmployeeAccount(fullname, username, password, id, this);
            Program.WaitForEscKey();
        }

        public void AddTask(Task task)
        {

            // Kiểm tra nếu danh sách Tasks chưa được khởi tạo, thì khởi tạo nó
            if (Tasks == null)
            {
                Tasks = new List<Task>();
            }

            // Thêm task vào danh sách
            Tasks.Add(task);
        }
        public void AddTasksToProject()
        {
            string name;
            // string nameE;
            DateTime startDate;
            DateTime endDate;
            int process = 0;
            //List<Member> inchagre;
            ;
            Console.Clear();
            Console.Write("Nhập số lượng Task muốn thêm: ");
            int numberOfTasks;
            while (!int.TryParse(Console.ReadLine(), out numberOfTasks) || numberOfTasks <= 0)
            {
                Console.WriteLine("Số lượng Task phải là một số nguyên dương. Vui lòng nhập lại.");
                Console.Write("Nhập số lượng Task muốn thêm: ");
            }

            for (int i = 0; i < numberOfTasks; i++)
            {
                Console.WriteLine($"\nNhập thông tin cho Task {i + 1}:");

                while (true)
                {
                    Console.Write("Name of Task: ");
                    name = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        Console.WriteLine("Tên không được để trống. Vui lòng nhập lại.");
                        continue;
                    }
                    break;
                }


                while (true)
                {
                    Console.Write("Start date (dd/mm/yyyy): ");
                    string inputDate = Console.ReadLine();

                    // Định dạng chuỗi ngày bạn mong muốn là "dd/MM/yyyy"
                    string dateFormat = "dd/MM/yyyy";

                    if (DateTime.TryParseExact(inputDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out startDate))
                    {
                        // Phân tích cú pháp ngày thành công
                        break;
                    }
                    else
                    {
                        // Ngày không hợp lệ
                        Console.WriteLine("Ngày không hợp lệ. Vui lòng nhập lại.");
                    }
                }


                while (true)
                {
                    Console.Write("End date (dd/mm/yyyy): ");
                    string inputDate = Console.ReadLine();

                    // Định dạng chuỗi ngày mong muốn là "dd/MM/yyyy"
                    string dateFormat = "dd/MM/yyyy";

                    // Sử dụng DateTime.TryParseExact để phân tích cú pháp chuỗi ngày theo định dạng "dd/MM/yyyy"
                    if (DateTime.TryParseExact(inputDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out endDate))
                    {
                        // Kiểm tra nếu ngày kết thúc lớn hơn ngày bắt đầu
                        if (endDate > startDate)
                        {
                            break;  // Ngày kết thúc hợp lệ và sau ngày bắt đầu
                        }
                        else
                        {
                            Console.WriteLine("Ngày kết thúc phải sau ngày bắt đầu. Vui lòng nhập lại.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ngày không hợp lệ. Vui lòng nhập lại.");
                    }
                }



                // Tạo một đối tượng Task từ thông tin đã nhập
                Task newTask = new Task(name, startDate, endDate, process, null);

                // Thêm Task mới vào danh sách các Task của Project
                AddTask(newTask);

                Console.WriteLine($"Task {i + 1} đã được thêm vào danh sách của Project.");
                Program.WaitForEscKey();
            }
        }

        public void AssignMembers()
        {
            Console.Clear();
            // Kiểm tra nếu danh sách task hoặc danh sách thành viên là null hoặc rỗng
            if (Tasks == null || Tasks.Count == 0 || Members == null || Members.Count == 0)
            {
                Console.WriteLine("Dự án chưa có task hoặc không có thành viên tham gia. Không thể phân công.");
                Program.WaitForEscKey();
                return;
            }

            bool continueAssigning = true; // Biến để kiểm soát việc tiếp tục phân công

            while (continueAssigning)
            {
                // Hiển thị danh sách task
                Console.WriteLine($"Danh sách các task trong dự án {Name}:");

                for (int i = 0; i < Tasks.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {Tasks[i].Name}");
                }

                // Chọn task
                Console.Write("Chọn số thứ tự của task cần phân công: ");
                int taskIndex;
                if (!int.TryParse(Console.ReadLine(), out taskIndex) || taskIndex < 1 || taskIndex > Tasks.Count)
                {
                    Console.WriteLine("Lựa chọn không hợp lệ.");
                    continue; // Quay lại vòng lặp để chọn lại task
                }

                // Hiển thị danh sách thành viên
                Console.WriteLine($"Danh sách các thành viên tham gia dự án:");
                for (int i = 0; i < Members.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {Members[i].Name}");
                }

                // Chọn thành viên
                Console.Write("Chọn số thứ tự của thành viên cần phân công: ");
                int memberIndex;
                if (!int.TryParse(Console.ReadLine(), out memberIndex) || memberIndex < 1 || memberIndex > Members.Count)
                {
                    Console.WriteLine("Lựa chọn không hợp lệ.");
                    continue; // Quay lại vòng lặp để chọn lại thành viên
                }

                // Lấy thông tin thành viên từ danh sách
                Member selectedMember = Members[memberIndex - 1];

                // Kiểm tra xem thành viên đã được phân công cho task này chưa
                if (Tasks[taskIndex - 1].Incharge != null && Tasks[taskIndex - 1].Incharge == selectedMember)
                {
                    Console.WriteLine($"Thành viên {selectedMember.Name} đã được phân công cho task '{Tasks[taskIndex - 1].Name}'.");
                }
                else
                {
                    // Thêm thành viên vào danh sách Incharge của Task
                    if (Tasks[taskIndex - 1].Incharge == null)
                    {
                        Tasks[taskIndex - 1].Incharge = selectedMember;
                    }
                    Tasks[taskIndex - 1].Incharge = selectedMember;
                    Console.WriteLine($"Phân công thành viên {selectedMember.Name} vào nhiệm vụ '{Tasks[taskIndex - 1].Name}' thành công.");
                }

                // Hỏi người dùng có muốn phân công tiếp không
                Console.Write("Bạn có muốn phân công tiếp không? (yes/no): ");
                string continueInput = Console.ReadLine();

                continueAssigning = (continueInput.ToLower() == "yes");
            }
        }
        public void EditTaskInProject()
        {
            Console.Clear();
            if (Tasks.Count == 0)
            {
                Console.WriteLine("Không có task nào trong dự án.");
                return;
            }

            Console.WriteLine("Danh sách các task trong dự án:");
            for (int i = 0; i < Tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Tasks[i].Name}");
            }

            Console.Write("Chọn số thứ tự của task cần chỉnh sửa: ");
            int index;
            if (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > Tasks.Count)
            {
                Console.WriteLine("Lựa chọn không hợp lệ.");
                return;
            }

            Task taskToEdit = Tasks[index - 1];
            if (taskToEdit.Incharge != null)
            {
                Console.WriteLine($"Task '{taskToEdit.Name}' đã có người phụ trách ({taskToEdit.Incharge.Name}). Không thể chỉnh sửa.");

            }
            else
            {
                Console.WriteLine($"Thông tin cũ của task '{taskToEdit.Name}':");
                Console.WriteLine($"1. Tên: {taskToEdit.Name}");
                // Console.WriteLine($"2. Nhân viên: {taskToEdit.Employee}");
                Console.WriteLine($"2. Ngày bắt đầu: {taskToEdit.StartDate.ToString("dd/MM/yyyy")}");
                Console.WriteLine($"3. Ngày kết thúc: {taskToEdit.EndDate.ToString("dd/MM/yyyy")}");

                // Console.WriteLine($"5. Tiến độ: {taskToEdit.Process}");

                Console.Write("Chọn số thứ tự của thông tin cần chỉnh sửa: ");
                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 4)
                {
                    Console.WriteLine("Lựa chọn không hợp lệ.");
                    return;
                }

                switch (choice)
                {
                    case 1:
                        Console.Write("Nhập tên mới: ");
                        string newName = Console.ReadLine();
                        taskToEdit.Name = newName;
                        break;
                    case 2:
                        Console.Write("Nhập ngày bắt đầu mới (yyyy-MM-dd): ");
                        DateTime newStartDate;
                        if (DateTime.TryParse(Console.ReadLine(), out newStartDate))
                        {
                            taskToEdit.StartDate = newStartDate;
                        }
                        else
                        {
                            Console.WriteLine("Ngày không hợp lệ.");
                        }
                        break;
                    case 3:
                        Console.Write("Nhập ngày kết thúc mới (dd/MM/yyyy): ");
                        DateTime newEndDate;
                        if (DateTime.TryParse(Console.ReadLine(), out newEndDate))
                        {
                            taskToEdit.EndDate = newEndDate;
                        }
                        else
                        {
                            Console.WriteLine("Ngày không hợp lệ.");
                        }
                        break;

                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        break;
                }

                Console.WriteLine("Thông tin của task đã được cập nhật.");
                Program.WaitForEscKey();
            }

        }

        public List<Task> ShowTasksForAccount(Account account)
        {

            // Tạo một danh sách để lưu trữ các nhiệm vụ được phụ trách bởi tài khoản
            List<Task> tasksForAccount = new List<Task>();

            // Kiểm tra xem danh sách nhiệm vụ của dự án có rỗng hoặc null không
            if (Tasks == null || Tasks.Count == 0)
            {
                // Thông báo rằng không có nhiệm vụ nào để hiển thị trong dự án
                Console.WriteLine($"Dự án {Name} không có nhiệm vụ nào.");
                return tasksForAccount; // Trả về danh sách trống
            }

            // Duyệt qua danh sách nhiệm vụ của dự án
            foreach (Task task in Tasks)
            {
                // Kiểm tra xem thành viên phụ trách nhiệm vụ có trùng với tài khoản cung cấp không
                if (task.Incharge != null && task.Incharge.Id == account.Id)
                {
                    // Thêm nhiệm vụ vào danh sách
                    tasksForAccount.Add(task);
                }
            }

            // Trả về danh sách các nhiệm vụ được phụ trách bởi tài khoản
            return tasksForAccount;
        }
        public void RemoveTaskFromProject()
        {
            Console.Clear();
            if (Tasks.Count == 0)
            {
                Console.WriteLine("Không có task nào trong dự án.");
                return;
            }

            Console.WriteLine("Danh sách các task trong dự án:");
            for (int i = 0; i < Tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Tasks[i].Name}");
            }

            Console.Write("Chọn số thứ tự của task cần xóa: ");
            int index;
            if (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > Tasks.Count)
            {
                Console.WriteLine("Lựa chọn không hợp lệ.");
                return;
            }

            // Xóa task khỏi danh sách
            Task taskToRemove = Tasks[index - 1];
            if (taskToRemove.Incharge != null)
            {
                Console.WriteLine($"Task '{taskToRemove.Name}' đã được phân công cho thành viên '{taskToRemove.Incharge.Name}'.");


                // Thông báo task đã được xóa khỏi danh sách task của thành viên
                Console.WriteLine($"Task '{taskToRemove.Name}' đã được xóa khỏi danh sách task của thành viên '{taskToRemove.Incharge.Name}'.");

                // Xóa task khỏi danh sách task của thành viên
                taskToRemove.Incharge = null;

            }
            Tasks.Remove(taskToRemove);
            Console.WriteLine($"Task '{taskToRemove.Name}' đã được xóa khỏi dự án.");
            Program.WaitForEscKey();
        }
        public void DisplayAllMembers()
        {
            Console.Clear();
            Console.WriteLine($"Danh sách thành viên của dự án {Name}:");
            if (Members.Count == 0)
            {
                Console.WriteLine("Chưa có thành viên");
            }
            for (int i = 0; i < Members.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Members[i]}");
            }

        }
        public void DisplayAndRemoveMember()
        {
            Console.Clear();
            bool continueDeleting = true;
            while (continueDeleting)

            {
                DisplayAllMembers();

                Console.Write("Chọn số thứ tự của thành viên cần xóa: ");
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice >= 1 && choice <= Members.Count)
                    {

                        Members.RemoveAt(choice - 1);
                        // Account accountToUpdate = null;
                        foreach (Account account in Program.AccManager.accounts)
                        {
                            if (account.Type != AccountType.Admin && account.Active == true)
                            {


                                for (int j = 0; j < account.ProjectsID.Count; j++)
                                {
                                    // Kiểm tra sự trùng khớp của dự án
                                    if (account.ProjectsID[j] == Id)
                                    {
                                        account.ProjectsID.RemoveAt(j);
                                        if (account.ProjectsID.Count == 0)
                                        {
                                            // Đặt trạng thái của tài khoản thành false
                                            account.Active = false;
                                        }
                                    }

                                }
                            }



                        }

                        Console.WriteLine("Thành viên đã được xóa khỏi dự án.");

                    }
                    else
                    {
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                    }
                }
                else
                {
                    Console.WriteLine("Lựa chọn không hợp lệ.");
                }

                Console.Write("Bạn có muốn tiếp tục xóa thành viên khác không? (y/n): ");
                string userInput = Console.ReadLine().ToLower();
                continueDeleting = (userInput == "y");
            }
        }

        public void CreateMeeting()
        {
            Console.Clear();
            Console.WriteLine("Nhập thông tin cho cuộc họp:");

            // Sử dụng thuộc tính Name của dự án để lấy tên dự án
            string project = Name;

            // Nhập thông tin khác từ người dùng
            Console.Write("Date (dd/MM/yyyy): ");
            DateTime date;
            while (!DateTime.TryParse(Console.ReadLine(), out date))
            {
                Console.WriteLine("Ngày không hợp lệ. Vui lòng nhập lại.");
            }

            Console.Write("Topic: ");
            string topic = Console.ReadLine();

            Console.Write("Content: ");
            string content = Console.ReadLine();

            // Tạo đối tượng cuộc họp và thêm vào danh sách hoặc làm gì đó với nó
            Meeting meeting = new Meeting(project, date, topic, content);
            if (Meetings == null)
            {
                Meetings = new List<Meeting>();
            }

            Meetings.Add(meeting);
            // In ra thông tin của cuộc họp vừa tạo
            Console.WriteLine("Thông tin của cuộc họp:");
            meeting.DisplayMeetingInfo();
            Program.WaitForEscKey();
            // Sau đó, bạn có thể làm gì đó khác với cuộc họp này, ví dụ: thêm vào danh sách cuộc họp của dự án
        }
        public void RemoveMeetingFromProject()
        {
            Console.Clear();
            if (Meetings == null || Meetings.Count == 0)
            {
                Console.WriteLine("Không có cuộc họp nào trong dự án.");
                return;
            }

            Console.WriteLine("Danh sách các cuộc họp trong dự án:");
            for (int i = 0; i < Meetings.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Ngày: {Meetings[i].Date.ToString("dd/MM/yyyy")}, Chủ đề: {Meetings[i].Topic}");
            }

            Console.Write("Chọn số thứ tự của cuộc họp cần xóa: ");
            int index;
            if (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > Meetings.Count)
            {
                Console.WriteLine("Lựa chọn không hợp lệ.");
                return;
            }

            // Xóa cuộc họp khỏi danh sách
            Meeting meetingToRemove = Meetings[index - 1];
            Meetings.Remove(meetingToRemove);
            Console.WriteLine($"Cuộc họp '{meetingToRemove.Topic}' vào ngày {meetingToRemove.Date.ToString("dd/MM/yyyy")} đã được xóa khỏi dự án.");
            Program.WaitForEscKey();
        }
        public void DisplayMeetingsAfter()
        {
            Console.Clear();
            DateTime currentDate = DateTime.Now.Date;
            Console.WriteLine($"Các cuộc họp của dự án {Name} sau ngày {currentDate.ToShortDateString()}:");
            bool hasMeetingsAfter = false;

            foreach (Meeting meeting in Meetings)
            {
                if (meeting.Date >= currentDate)
                {
                    Console.WriteLine($"Ngày: {meeting.Date}");
                    Console.WriteLine($"Chủ đề: {meeting.Topic}");
                    Console.WriteLine($"Nội dung: {meeting.Content}");
                    Console.WriteLine();
                    hasMeetingsAfter = true;
                }
            }

            if (!hasMeetingsAfter)
            {
                Console.WriteLine("Không có cuộc họp nào sau ngày hiện tại.");
            }
            Program.WaitForEscKey();
        }
        public void CreateMessage(Account currentAccount)
        {
            Console.Clear();
            string Fn = currentAccount.Fullname;
            while (true)
            {
                Console.Clear();
                // Hiển thị tất cả tin nhắn hiện có trước khi yêu cầu người dùng nhập tin nhắn mới
                Console.WriteLine("Danh sách tin nhắn:");
                if (Messages != null)
                {
                    foreach (Message message in Messages)
                    {
                        Console.WriteLine();
                        if (message.Fullname == currentAccount.Fullname)
                        {

                            ;                            // Đặt màu xanh lá cây và in hoa tên người dùng
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write($"{message.Fullname.ToUpper()}");
                            // Đặt lại màu mặc định và in nội dung tin nhắn và thời gian

                            Console.WriteLine($": {message.Content} ({message.DateTimeOfContent})");
                            Console.ResetColor();
                        }
                        else
                        {
                            // Đặt màu xanh lá cây cho tên người dùng thông thường

                            Console.Write($"{message.Fullname}");
                            // Đặt lại màu mặc định và in nội dung tin nhắn và thời gian

                            Console.WriteLine($": {message.Content} ({message.DateTimeOfContent})");
                        }
                    }
                }

                // Yêu cầu người dùng nhập nội dung của tin nhắn
                Console.Write("\nNhập nội dung tin nhắn (Nhấn ESC để thoát): ");

                // Đọc phím nhấn của người dùng
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                // Kiểm tra xem người dùng có nhấn phím ESC để thoát không
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    break;
                }

                // Nếu không phải phím ESC, tiếp tục đọc chuỗi nội dung
                string content = Console.ReadLine();

                // Tạo một tin nhắn mới
                Message newMessage = new Message(content, DateTime.Now, Id, Fn);

                // Kiểm tra nếu danh sách tin nhắn chưa được khởi tạo, thì khởi tạo nó
                if (Messages == null)
                {
                    Messages = new List<Message>();
                }

                // Thêm tin nhắn vào danh sách
                Messages.Add(newMessage);

                Console.WriteLine("Tin nhắn đã được tạo và lưu thành công.\n");
            }
        }


        public void ProjectDetailMenu()
        {

            bool menu2 = true;
            // while (menu2)
            // {
            Console.WriteLine($"\nChi tiết dự án '{Name}':");
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"Tên: {Name}");
            Console.WriteLine($"Mục tiêu: {Target}");
            Console.WriteLine($"Ngày bắt đầu: {StartDate.Date.ToString("dd/MM/yyyy")}");
            Console.WriteLine($"Ngày kết thúc: {EndDate.Date.ToString("dd/MM/yyyy")}");
            Console.WriteLine($"Mô tả: {Description}");
            Console.WriteLine($"Trạng thái: {Status}");
            while (menu2)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("╔══════════════════════════════════════════╗");
                Console.WriteLine("║               Menu thiết lập              ║");
                Console.WriteLine("╚══════════════════════════════════════════╝");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("1. Xem list task hiện tại");
                Console.WriteLine("2. Thêm task");
                Console.WriteLine("3. Xóa task");
                Console.WriteLine("4. Sửa task");
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("5. Thêm thành viên vào project");
                Console.WriteLine("6. Xem thành viên tham gia project");
                Console.WriteLine("7. Xoá thành viên tham gia project");
                Console.WriteLine("8. Phân công task cho thành viên của project");
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("9. Tạo meeting");
                Console.WriteLine("10. Xóa meeting");
                Console.WriteLine("11. Xem meeting");
                Console.WriteLine("12. Group chat");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("13. Quay lại");
                Console.ResetColor();
                Console.WriteLine();
                Console.Write("Chọn một tùy chọn: ");
                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Vui lòng chọn một tùy chọn hợp lệ.");
                    continue;
                }

                switch (choice)
                {
                    case 10:
                        RemoveMeetingFromProject();
                        Program.WaitForEscKey();
                        break;

                    case 11:
                        DisplayMeetingsAfter();

                        break;

                    case 8:
                        AssignMembers();
                        break;
                    case 5:
                        AddMemberToProject();

                        break;
                    case 6:
                        DisplayAllMembers();
                        Program.WaitForEscKey();
                        break;
                    case 7:
                        DisplayAndRemoveMember();

                        break;
                    case 2:
                        AddTasksToProject();

                        break;
                    case 3:
                        RemoveTaskFromProject();

                        break;
                    case 4:
                        EditTaskInProject();

                        break;
                    case 9:
                        CreateMeeting();

                        break;
                    case 12:
                        CreateMessage(Program.currentAccount);

                        break;
                    case 13:
                        menu2 = false;
                        break;
                    case 1:

                        if (Tasks == null)
                        {
                            Console.WriteLine("Không có task nào trong dự án.");
                            break;
                        }
                        Console.Clear();
                        Console.WriteLine($"Danh sách các task trong dự án : {Name}");
                        for (int i = 0; i < Tasks.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {Tasks[i].Name}");

                            if (Tasks[i].Incharge == null)
                            {
                                Console.WriteLine("Nhân viên phụ trách: Chưa có phân công");
                            }
                            else
                            {
                                Console.WriteLine($"Nhân viên phụ trách: {Tasks[i].Incharge.Name}");
                            }
                            Console.WriteLine($"Ngày bắt đầu: {Tasks[i].StartDate.Date}");
                            Console.WriteLine($"Ngày kết thúc: {Tasks[i].EndDate.Date}");
                            Console.WriteLine($"Tiến trình: {Tasks[i].Process}");
                            TimeSpan duration = Tasks[i].EndDate.Date - Tasks[i].StartDate.Date;
                            Console.WriteLine($"Thời gian còn lại: {duration.Days} ngày");
                            Console.WriteLine();
                        }
                        Program.WaitForEscKey();
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Tùy chọn không hợp lệ. Vui lòng chọn lại.");
                        break;
                }
            }
        }
    }
}