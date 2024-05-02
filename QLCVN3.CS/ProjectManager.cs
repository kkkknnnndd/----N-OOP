using Newtonsoft.Json;
using QLCVN3.CS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QLCVN3.CS
{
    public interface IProjectManagement
    {

        List<Project> DisplayProjects(Account currentAccount);
        Project CreateNewProject();
        void AddProject(Project project);
        int ChooseProjectToDelete();
        void DeleteProjectByIndex(int index);
        void EditProject();
        void SetUpProject();
        void DisplayMeetingsAfterToday(Account currentAccount);

    }
    public class ProjectManager : IProjectManagement
    {
        public List<Project> projects; // Danh sách các dự án
        int projectCounter = 3;
        public ProjectManager()
        {
            projects = new List<Project>();
        }

        public void ExportProjectsToJson(string directory, string fileName)
        {
            try
            {
                string fullPath = Path.Combine(directory, fileName);

                // Tạo cài đặt JSON
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                // Serialize danh sách tài khoản vào JSON và ghi vào tệp tin
                string json = JsonConvert.SerializeObject(projects, settings);
                File.WriteAllText(fullPath, json);

                //  Console.WriteLine("Xuất JSON của danh sách tài khoản thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi xuất JSON của danh sách tài khoản: " + ex.Message);
            }
        }

        public List<Project> LoadProjectFromJson(string filePath)
        {
            try
            {
                // Đọc nội dung từ tệp tin JSON
                string json = File.ReadAllText(filePath);

                // Deserialize JSON thành danh sách Project
                List<Project> projects = JsonConvert.DeserializeObject<List<Project>>(json);

                // Trả về danh sách Project
                return projects;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và thông báo chi tiết
                Console.WriteLine("Lỗi khi tải dữ liệu từ tệp tin JSON: " + ex.Message);

                // Trả về null hoặc danh sách rỗng trong trường hợp lỗi xảy ra
                return null; // hoặc có thể trả về một danh sách Project rỗng: new List<Project>()
            }
        }

        public List<Project> DisplayProjects(Account currentAccount)
        {
            Console.Clear();
            // Tạo một danh sách để chứa các dự án mà người dùng có thể xem
            List<Project> displayedProjects = new List<Project>();

            // Kiểm tra loại tài khoản của người dùng
            if (currentAccount.Type == AccountType.Admin)
            {
                // Nếu là admin, hiển thị và thêm tất cả các dự án
                if (projects.Count == 0)
                {
                    Console.WriteLine("Hiện tại chưa có dự án nào.");
                    return displayedProjects;
                }

                Console.WriteLine("Danh sách các dự án:");
                for (int i = 0; i < projects.Count; i++)
                {
                    Project project = projects[i];
                    displayedProjects.Add(project); // Thêm dự án vào danh sách

                    // In ra thông tin về dự án
                    Console.WriteLine($"{i + 1}. ID: {project.Id}");
                    Console.WriteLine($"   Tên: {project.Name}");
                    Console.WriteLine($"   Mục tiêu: {project.Target}");
                    Console.WriteLine($"   Ngày bắt đầu: {project.StartDate:dd/MM/yyyy}");
                    Console.WriteLine($"   Ngày kết thúc: {project.EndDate:dd/MM/yyyy}");
                    Console.WriteLine($"   Mô tả: {project.Description}");
                    Console.WriteLine($"   Trạng thái: {project.Status}");
                    Console.WriteLine($"   Số lượng thành viên: {project.Members.Count}");
                    TimeSpan duration = project.EndDate.Date - project.StartDate.Date;
                    Console.WriteLine($"   Thời gian còn lại: {duration.Days} ngày");
                    Console.WriteLine(); // Tạo khoảng trống giữa các dự án
                }
            }
            else if (currentAccount.Type == AccountType.Member)
            {
                // Nếu là member, hiển thị và thêm các dự án họ đang tham gia
                List<string> userProjectsID = currentAccount.ProjectsID;

                if (userProjectsID == null || userProjectsID.Count == 0)
                {
                    Console.WriteLine("Bạn không tham gia dự án nào.");
                    return displayedProjects;
                }

                Console.WriteLine("Danh sách các dự án bạn đang tham gia:");
                // Lọc các dự án dựa trên danh sách ID của tài khoản
                int projectNumber = 0;  // Đếm số thứ tự của dự án đang tham gia
                for (int i = 0; i < projects.Count; i++)
                {
                    Project project = projects[i];
                    // Kiểm tra xem dự án có trong danh sách ID của tài khoản không
                    for (int j = 0; j < userProjectsID.Count; j++)
                    {
                        if (project.Id == userProjectsID[j])
                        {
                            // Dự án được tìm thấy trong danh sách của tài khoản, thêm vào danh sách hiển thị
                            displayedProjects.Add(project);

                            // In thông tin về dự án
                            projectNumber++;
                            Console.WriteLine($"{projectNumber}. ID: {project.Id}");
                            Console.WriteLine($"   Tên: {project.Name}");
                            Console.WriteLine($"   Mục tiêu: {project.Target}");
                            Console.WriteLine($"   Ngày bắt đầu: {project.StartDate:dd/MM/yyyy}");
                            Console.WriteLine($"   Ngày kết thúc: {project.EndDate:dd/MM/yyyy}");
                            Console.WriteLine($"   Mô tả: {project.Description}");
                            Console.WriteLine($"   Trạng thái: {project.Status}");
                            Console.WriteLine($"   Số lượng thành viên: {project.Members.Count}");
                            Console.WriteLine(); // Tạo khoảng trống giữa các dự án
                            break; // Thoát khỏi vòng lặp `userProjectsID` sau khi tìm thấy dự án
                        }
                    }
                }
            }

            // Trả về danh sách các dự án
            return displayedProjects;
        }
        public Project CreateNewProject()
        {
            projectCounter++;

            string id = $"P{projectCounter}";
            string name;
            int target;
            DateTime startDate;
            DateTime endDate;
            string description;
            string status;
            Console.Clear();
            Console.WriteLine("Nhập thông tin cho dự án mới:");


            while (true)
            {
                Console.Write("Name: ");
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
                Console.Write("Target: ");
                string targetInput = Console.ReadLine();
                if (!int.TryParse(targetInput, out target))
                {
                    Console.WriteLine("Mục tiêu phải là một số nguyên. Vui lòng nhập lại.");
                    continue;
                }
                break;
            }

            while (true)
            {
                Console.Write("Start Date (dd/mm/yyyy): ");
                if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out startDate))
                {
                    Console.WriteLine("Ngày không hợp lệ. Vui lòng nhập lại.");
                    continue;
                }

                break;
            }

            while (true)
            {
                Console.Write("End Date (yyyy-mm-dd): ");
                if (!DateTime.TryParse(Console.ReadLine(), out endDate))
                {
                    Console.WriteLine("Ngày không hợp lệ. Vui lòng nhập lại.");
                    continue;
                }
                if (endDate <= startDate)
                {
                    Console.WriteLine("Ngày kết thúc phải sau ngày bắt đầu. Vui lòng nhập lại.");
                    continue;
                }
                break;
            }

            Console.Write("Description: ");
            description = Console.ReadLine();

            Console.Write("Status: ");
            status = Console.ReadLine();

            // Sử dụng biến target đã được lưu trữ
            Project newProject = new Project(id, name, target, startDate, endDate, description, status);

            // Hỏi người dùng có muốn thêm dự án này vào danh sách không
            Console.Write("Bạn có muốn thêm dự án này vào danh sách không? (Y/N): ");
            string addChoice = Console.ReadLine();

            if (addChoice.ToLower() == "y")
            {
                // Thêm dự án vào danh sách projects
                AddProject(newProject);
            }

            // Trả về đối tượng Project mới bất kể người dùng đã chọn "Y" hay không
            return newProject;
        }

        public void AddProject(Project project)
        {
            projects.Add(project);
            Console.WriteLine("Dự án đã được thêm vào danh sách.");
        }
        public int ChooseProjectToDelete()
        {
            Console.WriteLine("Danh sách các dự án:");
            for (int i = 0; i < projects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. ID: {projects[i].Id} - Tên: {projects[i].Name}");
            }

            Console.Write("Chọn số thứ tự của dự án cần xóa: ");
            int index;
            if (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > projects.Count)
            {
                Console.WriteLine("Lựa chọn không hợp lệ.");
                return -1;
            }

            return index - 1;
        }
        public void DeleteProjectByIndex(int index)
        {
            if (index >= 0 && index < projects.Count)
            {
                Project projectToDelete = projects[index];
                projects.RemoveAt(index);
                foreach (Account account in Program.AccManager.accounts)
                {
                    if (account.Type != AccountType.Admin)
                    {
                        for (int i = 0; i < account.ProjectsID.Count; i++)
                        {
                            // Kiểm tra nếu ID của dự án trùng khớp với ID cần xóa
                            if (account.Type != AccountType.Admin)
                            {
                                if (account.ProjectsID[i] == projectToDelete.Id)
                                {
                                    // Xóa dự án khỏi danh sách dự án của tài khoản
                                    account.ProjectsID.RemoveAt(i);
                                    if (account.ProjectsID.Count == 0)
                                    {
                                        account.Active = false;
                                    }

                                    return;
                                }

                            }


                        }
                    }
                    // Lặp qua từng dự án trong danh sách dự án của tài khoản

                }
                Console.WriteLine($"Dự án '{projectToDelete.Name}' đã được xóa khỏi danh sách.");
                Program.WaitForEscKey();
            }
            else
            {
                Console.WriteLine("Dự án lựa chọn không hợp lệ.");
            }
        }
        private int ChooseProjectToEdit()
        {
            Console.WriteLine("Danh sách các dự án:");
            for (int i = 0; i < projects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. ID: {projects[i].Id} - Tên: {projects[i].Name}");
            }

            Console.Write("Chọn số thứ tự của dự án: ");
            int index;
            if (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > projects.Count)
            {
                Console.WriteLine("Lựa chọn không hợp lệ.");
                return -1;
            }

            return index - 1;
        }
        public void EditProject()
        {
            // Chọn dự án cần sửa
            Console.Clear();
            Console.WriteLine("Chọn dự án cần sửa:");
            int index = ChooseProjectToEdit();
            if (index == -1)
            {
                Console.WriteLine("Dự án không tồn tại.");
                return;
            }

            // Hiển thị thông tin của dự án cần sửa
            Project projectToEdit = projects[index];
            Console.WriteLine($"Thông tin của dự án '{projectToEdit.Name}':");
            // Console.WriteLine($"1. ID: {projectToEdit.Id}");
            Console.WriteLine($"1. Tên: {projectToEdit.Name}");
            Console.WriteLine($"2. Mục tiêu: {projectToEdit.Target}");
            Console.WriteLine($"3. Ngày bắt đầu: {projectToEdit.StartDate}");
            Console.WriteLine($"4. Ngày kết thúc: {projectToEdit.EndDate}");
            Console.WriteLine($"5. Mô tả: {projectToEdit.Description}");
            Console.WriteLine($"6. Trạng thái: {projectToEdit.Status}");

            while (true)
            {
                // Chọn thông tin cần sửa
                Console.Write("Chọn số thứ tự của thông tin cần sửa: ");
                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 7)
                {
                    Console.WriteLine("Lựa chọn không hợp lệ.");
                    continue;
                }

                // Nhập thông tin mới
                Console.Write("Nhập thông tin mới: ");
                string newInfo = Console.ReadLine();

                // Cập nhật thông tin của dự án
                switch (choice)
                {
                    case 1:
                        if (string.IsNullOrWhiteSpace(newInfo))
                        {
                            Console.WriteLine("Tên không được để trống. Vui lòng nhập lại.");
                            continue;
                        }
                        projectToEdit.Name = newInfo;
                        break;
                    case 2:
                        if (!int.TryParse(newInfo, out int newInfo1))
                        {
                            Console.WriteLine("Mục tiêu phải là một số. Vui lòng nhập lại.");
                            continue;
                        }
                        projectToEdit.Target = newInfo1;
                        break;
                    case 3:
                        if (!DateTime.TryParse(newInfo, out DateTime startDate))
                        {
                            Console.WriteLine("Ngày không hợp lệ. Vui lòng nhập lại.");
                            continue;
                        }
                        projectToEdit.StartDate = startDate;
                        break;
                    case 4:
                        if (!DateTime.TryParse(newInfo, out DateTime endDate))
                        {
                            Console.WriteLine("Ngày không hợp lệ. Vui lòng nhập lại.");
                            continue;
                        }
                        projectToEdit.EndDate = endDate;
                        break;
                    case 5:
                        projectToEdit.Description = newInfo;
                        break;
                    case 6:
                        projectToEdit.Status = newInfo;
                        break;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        continue;
                }

                Console.WriteLine("Thông tin của dự án đã được cập nhật.");
                break;
            }
        }
        public void SetUpProject()
        {
            DisplayProjects(Program.currentAccount);
            if ((projects.Count) == 0)
            {
                Console.WriteLine("Không có dự án nào để thực hiện set up.");
                return;
            }

            Console.Write("Chọn số thứ tự của dự án cần set up: ");
            int index;
            if (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > projects.Count)
            {
                Console.WriteLine("Lựa chọn không hợp lệ.");
                return;
            }

            Project selectedProject = projects[index - 1];
            Console.Clear();
            selectedProject.ProjectDetailMenu();
        }

        public void DisplayMeetingsAfterToday(Account currentAccount)
        {
            // Lấy ngày hiện tại
            DateTime currentDate = DateTime.Today;

            // Kiểm tra xem có dự án nào không
            if (projects.Count == 0)
            {
                Console.WriteLine("Không có dự án nào.");
                Program.WaitForEscKey();
                return;
            }

            bool hasMeeting = false;

            // Kiểm tra loại tài khoản
            if (currentAccount.Type == AccountType.Admin)
            {
                // Nếu là admin, duyệt qua tất cả các dự án
                foreach (Project project in projects)
                {
                    if (project.Meetings.Count != 0)

                    {
                        Console.WriteLine($"Dự án: {project.Name}");

                        // Duyệt qua tất cả các cuộc họp trong dự án
                        foreach (Meeting meeting in project.Meetings)
                        {
                            // Kiểm tra nếu ngày của cuộc họp sau ngày hiện tại
                            if (meeting.Date >= currentDate)
                            {
                                // Hiển thị thông tin cuộc họp
                                Console.WriteLine($"- Ngày: {meeting.Date:dd/MM/yyyy}, Chủ đề: {meeting.Topic}, Nội dung: {meeting.Content}");
                                hasMeeting = true;
                            }
                        }

                    }
                    // In ra thông tin dự án

                }
            }
            else if (currentAccount.Type == AccountType.Member)
            {
                // Nếu là member, chỉ hiển thị các cuộc họp trong các dự án mà member đang tham gia
                List<string> userProjectsID = currentAccount.ProjectsID;

                // Duyệt qua tất cả các dự án
                foreach (Project project in projects)
                {
                    // Kiểm tra nếu ID của dự án hiện tại nằm trong danh sách ID của tài khoản
                    bool isProjectInAccount = false;
                    foreach (string projectId in userProjectsID)
                    {
                        if (project.Id == projectId)
                        {
                            isProjectInAccount = true;
                            break;
                        }
                    }

                    // Nếu dự án không nằm trong danh sách ID của tài khoản, bỏ qua nó
                    if (!isProjectInAccount)
                    {
                        continue;
                    }

                    // In ra thông tin dự án
                    Console.WriteLine($"Dự án: {project.Name}");

                    // Duyệt qua tất cả các cuộc họp trong dự án
                    foreach (Meeting meeting in project.Meetings)
                    {
                        // Kiểm tra nếu ngày của cuộc họp sau ngày hiện tại
                        if (meeting.Date >= currentDate)
                        {
                            // Hiển thị thông tin cuộc họp
                            Console.WriteLine($"- Ngày: {meeting.Date:dd/MM/yyyy}, Chủ đề: {meeting.Topic}, Nội dung: {meeting.Content}");
                            hasMeeting = true;
                        }
                    }
                }
            }

            // Kiểm tra xem có cuộc họp nào sau ngày hiện tại không
            if (!hasMeeting)
            {
                Console.WriteLine("Không có cuộc họp nào sau ngày hiện tại.");
            }
            Program.WaitForEscKey();
        }
        public List<Project> FindProjectsByName(List<Project> projects)
        {
            // Yêu cầu người dùng nhập tên dự án cần tìm
            Console.Write("Nhập tên dự án cần tìm: ");
            string name = Console.ReadLine();

            // Chuyển đổi tên tìm kiếm và tên dự án về dạng không dấu và chữ thường
            string normalizedSearchName = Program.RemoveDiacritics(name).ToLower();

            // Khai báo danh sách dự án tìm được
            List<Project> foundProjects = new List<Project>();

            // Lặp qua danh sách dự án để tìm dự án có tên chứa chuỗi tìm kiếm
            foreach (Project project in projects)
            {
                // Chuyển đổi tên dự án hiện tại về dạng không dấu và chữ thường
                string normalizedProjectName = Program.RemoveDiacritics(project.Name).ToLower();

                // Kiểm tra xem tên dự án có chứa chuỗi tìm kiếm không
                if (normalizedProjectName.Contains(normalizedSearchName))
                {
                    // Thêm dự án vào danh sách kết quả
                    foundProjects.Add(project);
                }
            }

            // Trả về danh sách các dự án tìm được
            return foundProjects;
        }

        public List<Project> FindProjectsByStatus(List<Project> projects)
        {
            // Yêu cầu người dùng nhập tên dự án cần tìm
            Console.Write("Nhập tình trạng dự án cần tìm: ");
            string status = Console.ReadLine();

            // Chuyển đổi tên tìm kiếm và tên dự án về dạng không dấu và chữ thường
            string normalizedSearchStatus = Program.RemoveDiacritics(status).ToLower();

            // Khai báo danh sách dự án tìm được
            List<Project> foundProjects = new List<Project>();

            // Lặp qua danh sách dự án để tìm dự án có tên chứa chuỗi tìm kiếm
            foreach (Project project in projects)
            {
                // Chuyển đổi tên dự án hiện tại về dạng không dấu và chữ thường
                string normalizedProjectStatus = Program.RemoveDiacritics(project.Status).ToLower();

                // Kiểm tra xem tên dự átus.Contains(normalizedSearchName))
                if (normalizedProjectStatus.Contains(normalizedSearchStatus))
                {
                    // Thêm dự án vào danh sách kết quả
                    foundProjects.Add(project);
                }
            }

            // Trả về danh sách các dự án tìm được
            return foundProjects;
        }
        public List<Project> FindProjectsByStartDateRange(List<Project> projects)
        {
            // Nhập thời gian bắt đầu tối thiểu từ người dùng
            Console.Write("Nhập thời gian bắt đầu tối thiểu (dd/MM/yyyy): ");
            DateTime startDateMin = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

            // Khai báo biến để lưu thời gian bắt đầu tối đa
            DateTime startDateMax;

            // Vòng lặp để đảm bảo thời gian bắt đầu tối đa phải lớn hơn hoặc bằng thời gian bắt đầu tối thiểu
            while (true)
            {
                Console.Write("Nhập thời gian bắt đầu tối đa (dd/MM/yyyy): ");
                startDateMax = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                // Kiểm tra thời gian bắt đầu tối đa lớn hơn hoặc bằng thời gian bắt đầu tối thiểu
                if (startDateMax >= startDateMin)
                {
                    // Thoát vòng lặp nếu điều kiện thỏa mãn
                    break;
                }
                else
                {
                    // Thông báo lỗi và yêu cầu nhập lại thời gian bắt đầu tối đa
                    Console.WriteLine("Thời gian bắt đầu tối đa phải lớn hơn hoặc bằng thời gian bắt đầu tối thiểu. Vui lòng nhập lại.");
                }
            }

            // Khai báo danh sách dự án tìm được
            List<Project> foundProjects = new List<Project>();

            // Lặp qua danh sách dự án để tìm các dự án có thời gian bắt đầu trong khoảng thời gian dao động
            foreach (Project project in projects)
            {
                // Kiểm tra thời gian bắt đầu của dự án có nằm trong khoảng thời gian dao động không
                if (project.StartDate >= startDateMin && project.StartDate <= startDateMax)
                {
                    // Thêm dự án vào danh sách kết quả
                    foundProjects.Add(project);
                }
            }

            // Trả về danh sách các dự án tìm được
            return foundProjects;
        }

    }
}