using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Channels;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.IO;
using static QLCVN3.CS.Account;
using QLCVN3.CS;

namespace QLCVN3.CS
{
    public class Program
    {

        public static ProjectManager projectManager = new ProjectManager();
        public static AccountManager AccManager = new AccountManager();

        public static Employee[] employees = new Employee[10];
        public static Account currentAccount;
        public static Report report;

        // Khởi tạo các nhân viên

        public static Employee[] LoadEmployee(string path)
        {
            // Đọc chuỗi JSON từ tệp tin
            string json = File.ReadAllText(path);

            // Chuyển đổi chuỗi JSON thành mảng Employee[]
            Employee[] employees = JsonConvert.DeserializeObject<Employee[]>(json);

            // Trả về mảng Employee
            return employees;
        }

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            foreach (char c in normalizedString)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }


        public static void WaitForEscKey()
        {
            Console.WriteLine("Nhấn Esc để thoát.");
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    // Lấy phím bấm
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    // Kiểm tra nếu phím bấm là Esc
                    if (key.Key == ConsoleKey.Escape)
                    {
                        // Thoát khỏi vòng lặp và case

                        break;
                    }
                }

                // Ngủ trong một thời gian ngắn để tránh lặp quá nhanh
                System.Threading.Thread.Sleep(100);
            }
        }
        static void Main(string[] args)
        {
            int dangnhap = 0;
            Console.OutputEncoding = Encoding.UTF8;

            string directory = @"C:\Users\Admin\source\repos\QLCVN3.CS\QLCVN3.CS";
            string filename = "employees.json";
            string filename1 = "projects.json";
            string filename2 = "accounts.json";


            string path = Path.Combine(directory, filename);
            string path1 = Path.Combine(directory, filename1);
            string path2 = Path.Combine(directory, filename2);


            employees = LoadEmployee(path);
            projectManager.projects = projectManager.LoadProjectFromJson(path1);
            AccManager.accounts = AccManager.LoadAccountsFromJson(path2);


            string fileName = "accounts.json";

            string fileName1 = "projects.json";




            report = new Report();
            while (true)
            {
                // Đặt currentAccount thành null trước khi bắt đầu kiểm tra đăng nhập
                currentAccount = null;

                // Vòng lặp kiểm tra đăng nhập
                int maxAttempts = 3;
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    //Console.Clear();

                    // Khởi tạo màu và font chữ cho tiêu đề
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
                    Console.WriteLine("║                      QUẢN LÝ CÔNG VIỆC NHÓM              ║");
                    Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
                    Console.ResetColor();

                    // Lấy độ dài của dòng tiêu đề
                    int titleLength = "╔══════════════════════════════════════════════════════════╗".Length;

                    // Tính toán độ dài của dòng nhập username và password
                    string inputPrompt = "Nhập username: ";
                    int inputLength = inputPrompt.Length;
                    int remainingSpaces = (titleLength - inputLength) / 2;

                    // In ra màn hình sao cho nội dung được căn giữa
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(new string(' ', remainingSpaces));
                    Console.WriteLine(inputPrompt);
                    Console.ResetColor();
                    string inputUsername = Console.ReadLine();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    inputPrompt = "Nhập password: ";
                    inputLength = inputPrompt.Length;
                    remainingSpaces = (titleLength - inputLength) / 2;
                    Console.Write(new string(' ', remainingSpaces));
                    Console.WriteLine(inputPrompt);
                    Console.ResetColor();
                    string inputPassword = Console.ReadLine();

                    // Kiểm tra đăng nhập
                    currentAccount = AccManager.CheckLogin(inputUsername, inputPassword);

                    if (currentAccount != null)
                    {
                        // Đăng nhập thành công
                        Console.Clear();

                        Console.WriteLine("Đăng nhập thành công!");
                        break;
                    }
                    else
                    {
                        // Đăng nhập thất bại
                        //Console.WriteLine("Đăng nhập thất bại! Username hoặc password không chính xác hoặc tài khoản đã bị khoá");

                        // Nếu đã thử 3 lần mà vẫn không đăng nhập được, thông báo cho người dùng và thoát
                        if (attempt == maxAttempts)
                        {
                            Console.WriteLine("Bạn đã thử đăng nhập sai 3 lần. Hãy thử lại sau.");
                            Environment.Exit(0);  // Kết thúc toàn bộ chương trình
                        }
                    }
                }

                // Sau khi đăng nhập thành công, thực hiện các hoạt động theo kiểu tài khoản (Admin hoặc Member)
                if (currentAccount != null)
                {
                    if (currentAccount.Type == AccountType.Admin)
                    {
                        // Menu Admin
                        while (true)
                        {
                            dangnhap++;
                            if (dangnhap > 1) Console.Clear();

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("╔═════════════════════════╗");
                            Console.WriteLine("║      Menu Quản Trị      ║");
                            Console.WriteLine("╚═════════════════════════╝");
                            Console.ResetColor();

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("1. Xem dự án hiện có");
                            Console.WriteLine("2. Set up dự án");
                            Console.WriteLine("3. Tạo dự án mới");
                            Console.WriteLine("4. Sửa dự án");
                            Console.WriteLine("5. Xoá dự án");
                            Console.WriteLine("6. Các meeting sắp đến");
                            Console.WriteLine("7. Sáp nhập dự án");
                            Console.WriteLine("8. Tìm kiếm project theo tên:");
                            Console.WriteLine("9. Tìm kiếm project theo tình trạng:");
                            Console.WriteLine("10. Tìm kiếm project theo ngày bằt đầu:");
                            Console.WriteLine("11. Báo cáo");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("-------------------------");
                            Console.WriteLine("12. Thoát");
                            Console.ResetColor();

                            Console.WriteLine();
                            Console.Write("Chọn một tùy chọn: ");
                            int choice2;
                            int choice1;
                            if (!int.TryParse(Console.ReadLine(), out choice2))
                            {
                                Console.WriteLine("Vui lòng chọn một tùy chọn hợp lệ.");
                                continue;
                            }

                            switch (choice2)
                            {
                                case 10:
                                    List<Project> Findedprojectstartdate = projectManager.FindProjectsByStartDateRange(projectManager.projects);
                                    if (Findedprojectstartdate != null)
                                    {
                                        Console.WriteLine("Danh sách dự án tìm thấy:");
                                        foreach (Project project in Findedprojectstartdate)
                                        {
                                            Console.WriteLine($"   ID: {project.Id}");
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
                                    else
                                    {
                                        Console.WriteLine($"   Không tìm thấy thông tin dự án");
                                    }

                                    Program.WaitForEscKey();
                                    break;

                                case 9:
                                    List<Project> Findedprojectstatus = projectManager.FindProjectsByStatus(projectManager.projects);
                                    if (Findedprojectstatus != null)
                                    {
                                        Console.WriteLine("Danh sách dự án tìm thấy:");
                                        foreach (Project project in Findedprojectstatus)
                                        {
                                            Console.WriteLine($"   ID: {project.Id}");
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
                                    else
                                    {
                                        Console.WriteLine($"   Không tìm thấy thông tin dự án");
                                    }

                                    Program.WaitForEscKey();
                                    break;


                                case 8:
                                    List<Project> Findedprojectname = projectManager.FindProjectsByName(projectManager.projects);
                                    if (Findedprojectname != null)
                                    {
                                        Console.WriteLine("Danh sách dự án tìm thấy:");
                                        foreach (Project project in Findedprojectname)
                                        {
                                            Console.WriteLine($"   ID: {project.Id}");
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
                                    else
                                    {
                                        Console.WriteLine($"   Không tìm thấy thông tin dự án");
                                    }

                                    Program.WaitForEscKey();
                                    break;


                                case 7:

                                    Project projectCombine = new Project();
                                    List<Project> selectetoCombine = projectManager.DisplayProjects(currentAccount);
                                    int index1 = -1;
                                    int index2 = -1;
                                    bool validInput = false;

                                    // Yêu cầu người dùng nhập chỉ số cho hai dự án
                                    while (!validInput)
                                    {
                                        // Nhập chỉ số của dự án thứ nhất
                                        Console.WriteLine($"Vui lòng nhập chỉ số của dự án thứ nhất (1-{selectetoCombine.Count}):");
                                        index1 = int.Parse(Console.ReadLine()) - 1;

                                        // Nhập chỉ số của dự án thứ hai
                                        Console.WriteLine($"Vui lòng nhập chỉ số của dự án thứ hai (1-{selectetoCombine.Count}):");
                                        index2 = int.Parse(Console.ReadLine()) - 1;

                                        // Kiểm tra xem chỉ số có hợp lệ và không trùng lặp không
                                        if (index1 >= 0 && index1 < selectetoCombine.Count && index2 >= 0 && index2 < selectetoCombine.Count && index1 != index2)
                                        {
                                            validInput = true;
                                        }
                                        else
                                        {
                                            // Nếu chỉ số không hợp lệ hoặc trùng nhau, yêu cầu người dùng nhập lại
                                            Console.WriteLine("Chỉ số không hợp lệ hoặc trùng nhau. Vui lòng nhập lại.");
                                        }
                                    }

                                    // Lấy dự án từ danh sách dựa trên chỉ số đã nhập
                                    Project project1 = selectetoCombine[index1];
                                    project1.Status = "Đã kết hợp";
                                    Project project2 = selectetoCombine[index2];
                                    project2.Status = "Đã kết hợp";
                                    // Kết hợp hai dự án lại với nhau bằng cách sử dụng toán tử cộng (`+`)
                                    Project combinedProject = project1 + project2;
                                    projectManager.AddProject(combinedProject);
                                    foreach (Member member in combinedProject.Members)
                                    {
                                        // Lặp qua danh sách các Account
                                        foreach (Account account in AccManager.accounts)
                                        {
                                            // Kiểm tra nếu member.EmployeeId trùng với account.EmployeeId
                                            if (member.Id == account.Id)
                                            {
                                                // Gán ProjectId của account là project.Id
                                                account.ProjectsID.Add(combinedProject.Id);
                                                account.Active = true;

                                            }
                                        }
                                    }
                                    Program.WaitForEscKey();
                                    break;
                                case 2:

                                    projectManager.SetUpProject();
                                    break;
                                case 1:
                                    Console.Clear();
                                    projectManager.DisplayProjects(currentAccount);
                                    Console.WriteLine("Nhấn Esc để thoát.");

                                    // Lắng nghe phím bấm từ người dùng
                                    while (true)
                                    {
                                        if (Console.KeyAvailable)
                                        {
                                            // Lấy phím bấm
                                            ConsoleKeyInfo key = Console.ReadKey(true);

                                            // Kiểm tra nếu phím bấm là Esc
                                            if (key.Key == ConsoleKey.Escape)
                                            {
                                                // Thoát khỏi vòng lặp và case
                                                break;
                                            }
                                        }

                                        // Ngủ trong một thời gian ngắn để tránh lặp quá nhanh
                                        System.Threading.Thread.Sleep(100);
                                    }
                                    break;
                                case 3:
                                    projectManager.CreateNewProject();
                                    break;
                                case 4:
                                    projectManager.EditProject();
                                    break;
                                case 5:
                                    int projectIndex = projectManager.ChooseProjectToDelete();
                                    projectManager.DeleteProjectByIndex(projectIndex);
                                    break;
                                case 6:
                                    projectManager.DisplayMeetingsAfterToday(currentAccount);
                                    break;
                                case 11:
                                    Console.WriteLine("Danh sách các báo cáo :");
                                    Console.WriteLine($"1. Báo cáo tiến độ project");
                                    Console.WriteLine($"2. Báo cáo tiến độ task");
                                    Console.WriteLine($"Nhâp sự lựa chọn:");

                                    while (!int.TryParse(Console.ReadLine(), out choice1) || choice1 < 1 || choice1 > 2)
                                    {
                                        Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng nhập lại.");
                                        Console.Write("\nNhập số thứ tự của báo cáo bạn muốn chọn: ");
                                    }
                                    switch (choice1)
                                    {
                                        case 1:
                                            report.GenerateAdminProjectReport(projectManager.projects);
                                            break;
                                        case 2:
                                            List<Project> displayedProjects = projectManager.DisplayProjects(currentAccount);

                                            if (displayedProjects.Count == 0)
                                            {
                                                return; // Nếu không có dự án nào để hiển thị, dừng lại
                                            }

                                            // Yêu cầu người dùng chọn số thứ tự của dự án
                                            Console.Write("Vui lòng chọn số thứ tự của dự án: ");
                                            int userInput;
                                            while (!int.TryParse(Console.ReadLine(), out userInput) || userInput < 1 || userInput > displayedProjects.Count)
                                            {
                                                Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng nhập lại.");
                                                Console.Write("\nNhập số thứ tự của dự án bạn muốn chọn: ");
                                            }

                                            // Lưu lại dự án mà người dùng đã chọn
                                            Project selectedProject = displayedProjects[userInput - 1];
                                            report.GenerateTaskReportForAdmin(selectedProject);
                                            break;

                                    }
                                    Program.WaitForEscKey();
                                    break;
                                case 12:
                                    // Thoát menu và quay lại bước đăng nhập
                                    AccManager.ExportAccountsToJson(directory, fileName);
                                    projectManager.ExportProjectsToJson(directory, fileName1);
                                    Console.Clear();
                                    break;

                                default:
                                    Console.WriteLine("Tùy chọn không hợp lệ. Vui lòng chọn lại.");
                                    break;
                            }

                            if (choice2 == 12)
                            {

                                // Thoát menu và quay lại bước đăng nhập
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Menu Member
                        while (true)
                        {
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("╭──────────────────────────────────╮");
                            Console.WriteLine("│           Menu Member            │");
                            Console.WriteLine("╰──────────────────────────────────╯");
                            Console.ResetColor();

                            Console.WriteLine("1. Xem dự án tham gia");
                            Console.WriteLine("2. Cập nhật tiến độ dự án");
                            Console.WriteLine("3. Groupchat");
                            Console.WriteLine("4. Các meeting sắp đến");
                            Console.WriteLine("5. Báo cáo tiến độ task");
                            Console.WriteLine("6. Quay lại");
                            Console.WriteLine();

                            Console.WriteLine("Chọn một tùy chọn:");

                            int choice;
                            if (!int.TryParse(Console.ReadLine(), out choice))
                            {
                                Console.WriteLine("Vui lòng chọn một tùy chọn hợp lệ.");
                                continue;
                            }

                            switch (choice)
                            {
                                case 1:
                                    projectManager.DisplayProjects(currentAccount);
                                    Program.WaitForEscKey();
                                    break;
                                case 2:
                                    List<Project> selectedproject = projectManager.DisplayProjects(currentAccount);
                                    if (selectedproject.Count == 0)
                                    {
                                        Console.WriteLine("Bạn chưa tham gia project nào");

                                        break;
                                    }

                                    int luaChon;
                                    do
                                    {
                                        Console.Write("Vui lòng chọn một dự án bằng cách nhập số thứ tự: ");
                                        bool validInput = int.TryParse(Console.ReadLine(), out luaChon);

                                        // Kiểm tra tính hợp lệ của đầu vào
                                        if (validInput && luaChon >= 1 && luaChon <= selectedproject.Count)
                                        {
                                            break; // Thoát vòng lặp nếu đầu vào hợp lệ
                                        }
                                        else
                                        {
                                            Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng thử lại.");
                                        }
                                    } while (true); // Lặp lại cho đến khi người dùng nhập đúng
                                    Project duAnDuocChon = selectedproject[luaChon - 1];
                                    List<Task> taskselected = duAnDuocChon.ShowTasksForAccount(currentAccount);
                                    if (taskselected.Count == 0)
                                    {
                                        Console.WriteLine("Bạn chưa được phân công task trong project này.");
                                        break;
                                    }
                                    Console.WriteLine("Danh sách các task bạn đang tham gia trong project:");
                                    for (int i = 0; i < taskselected.Count; i++)
                                    {
                                        Console.WriteLine($"{i + 1}. Tên task : {taskselected[i].Name}");
                                        Console.WriteLine($"   Ngày bắt đầu: {taskselected[i].StartDate:dd/MM/yyyy}"); // Định dạng ngày
                                        Console.WriteLine($"   Ngày kết thúc: {taskselected[i].EndDate:dd/MM/yyyy}"); // Định dạng ngày
                                        Console.WriteLine($"   Process: {taskselected[i].Process}");
                                        Console.WriteLine();
                                    }

                                    do
                                    {
                                        Console.Write("Vui lòng chọn một task bằng cách nhập số thứ tự: ");
                                        bool validInput = int.TryParse(Console.ReadLine(), out luaChon);

                                        // Kiểm tra tính hợp lệ của đầu vào
                                        if (validInput && luaChon >= 1 && luaChon <= taskselected.Count)
                                        {
                                            break; // Thoát vòng lặp nếu đầu vào hợp lệ
                                        }
                                        else
                                        {
                                            Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng thử lại.");
                                        }
                                    } while (true); // Lặp lại cho đến khi người dùng nhập đúng

                                    Task newtaks = taskselected[luaChon - 1];
                                    newtaks.UpdateTaskProgress();
                                    // Thực hiện cập nhật tiến độ (bạn có thể bổ sung mã ở đây)
                                    break;
                                case 3:
                                    List<Project> selectedproject1 = projectManager.DisplayProjects(currentAccount);
                                    if (selectedproject1.Count == 0)
                                    {
                                        Console.WriteLine("Bạn chưa tham gia project nào");
                                        break;
                                    }


                                    do
                                    {
                                        Console.Write("Vui lòng chọn một dự án bằng cách nhập số thứ tự: ");
                                        bool validInput = int.TryParse(Console.ReadLine(), out luaChon);

                                        // Kiểm tra tính hợp lệ của đầu vào
                                        if (validInput && luaChon >= 1 && luaChon <= selectedproject1.Count)
                                        {
                                            break; // Thoát vòng lặp nếu đầu vào hợp lệ
                                        }
                                        else
                                        {
                                            Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng thử lại.");
                                        }
                                    } while (true); // Lặp lại cho đến khi người dùng nhập đúng
                                    Project duAnDuocChon1 = selectedproject1[luaChon - 1];
                                    duAnDuocChon1.CreateMessage(currentAccount);
                                    break;
                                case 4:
                                    projectManager.DisplayMeetingsAfterToday(currentAccount);
                                    break;
                                case 5:
                                    Console.Clear();
                                    List<Project> projectsForCurrentAccount = new List<Project>();

                                    // Duyệt qua danh sách dự án
                                    foreach (Project project in projectManager.projects)
                                    {
                                        // Kiểm tra nếu ID của dự án trùng với ID trong danh sách dự án của tài khoản hiện tại
                                        if (currentAccount.ProjectsID.Contains(project.Id))
                                        {
                                            // Nếu ID của dự án trùng với ID trong danh sách dự án của tài khoản hiện tại,
                                            // thêm dự án đó vào danh sách projectsForCurrentAccount
                                            projectsForCurrentAccount.Add(project);
                                        }
                                    }

                                    // Duyệt qua từng dự án mà nhân viên tham gia
                                    foreach (Project project in projectsForCurrentAccount)
                                    {
                                        report.GenerateTaskReportForUser(project, currentAccount);
                                    }
                                    Program.WaitForEscKey();
                                    break;
                                case 6:
                                    AccManager.ExportAccountsToJson(directory, fileName);
                                    projectManager.ExportProjectsToJson(directory, fileName1);
                                    Console.Clear();

                                    break;
                                default:
                                    Console.WriteLine("Tùy chọn không hợp lệ. Vui lòng chọn lại.");
                                    break;
                            }

                            if (choice == 6)
                            {
                                // Thoát menu và quay lại bước đăng nhập
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}