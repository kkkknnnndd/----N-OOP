using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using QLCVN3.CS;

namespace QLCVN3.CS
{
    // Lớp quản lý tài khoản
    public class AccountManager
    {
        public List<Account> accounts; // Danh sách các tài khoản

        // Constructor mặc định
        public AccountManager()
        {
            accounts = new List<Account>();
        }

        // Phương thức để thêm một tài khoản mới vào danh sách

        public void ExportAccountsToJson(string directory, string fileName)
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
                string json = JsonConvert.SerializeObject(accounts, settings);
                File.WriteAllText(fullPath, json);

                //  Console.WriteLine("Xuất JSON của danh sách tài khoản thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi xuất JSON của danh sách tài khoản: " + ex.Message);
            }
        }
        public List<Account> LoadAccountsFromJson(string filePath)
        {
            try
            {
                // Đọc nội dung từ tệp tin JSON
                string json = File.ReadAllText(filePath);

                // Deserialize JSON thành danh sách Account và trả về
                List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(json);

                // Thông báo rằng dữ liệu đã được tải thành công (nếu bạn muốn)
                // Console.WriteLine("Dữ liệu từ tệp tin JSON đã được tải thành công.");

                return accounts;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và thông báo chi tiết
                Console.WriteLine("Lỗi khi tải dữ liệu từ tệp tin JSON: " + ex.Message);

                // Trả về null hoặc danh sách rỗng trong trường hợp lỗi xảy ra
                return null;
            }
        }

        public void AddAccount(Account account)
        {
            // Kiểm tra xem tài khoản đã tồn tại trong danh sách chưa
            if (accounts.Exists(acc => acc.Username == account.Username))
            {
                Console.WriteLine("Tên đăng nhập đã tồn tại.");
                return;
            }

            // Thêm tài khoản vào danh sách
            accounts.Add(account);
            Console.WriteLine($"Tài khoản cho {account.Username} đã được tạo thành công.");
        }

        // Phương thức để tạo một tài khoản nhân viên
        public virtual void CreateEmployeeAccount(string fullname, string username, string password, string id, Project project)
        {
            bool accountExists = false;

            // Kiểm tra xem ID đã tồn tại trong danh sách tài khoản chưa
            foreach (Account acc in accounts)
            {
                if (acc.Id == id)
                {
                    accountExists = true;
                    // Thêm dự án vào danh sách dự án của tài khoản
                    if (acc.ProjectsID == null)
                        acc.ProjectsID = new List<string>(); // Tạo danh sách dự án nếu chưa tồn tại
                    acc.ProjectsID.Add(project.Id); // Thêm dự án vào danh sách dự án của tài khoản
                    // Đặt thuộc tính Active thành true
                    acc.Active = true;
                    Console.WriteLine($"Thêm dự án cho tài khoản {username} thành công.");
                    break;
                }
            }

            // Nếu tài khoản không tồn tại, tạo một tài khoản mới
            if (!accountExists)
            {
                // Tạo danh sách dự án mới và thêm dự án vào
                List<string> projectsID = new List<string>();
                projectsID.Add(project.Id);

                // Tạo tài khoản cho nhân viên với mật khẩu mặc định và loại tài khoản là "employee"
                Account employeeAccount = new Account(fullname, username, password, AccountType.Member, id, project.Id, true);

                // Thêm tài khoản vào danh sách tài khoản
                accounts.Add(employeeAccount);

                Console.WriteLine($"Tài khoản cho {username} đã được tạo thành công.");
            }
        }

        // Phương thức để kiểm tra đăng nhập
        public virtual Account CheckLogin(string username, string password)
        {
            // Duyệt qua danh sách tài khoản để kiểm tra username và password
            foreach (Account account in accounts)
            {
                if (account.Username == username && account.Password == password)
                {
                    // Kiểm tra trạng thái của tài khoản
                    if (!account.Active)
                    {
                        // Tài khoản không hoạt động, thông báo với người dùng
                        Console.WriteLine("Tài khoản của bạn đang bị khóa. Vui lòng liên hệ quản lý.");
                        return null;
                    }
                    else
                    {
                        // Đăng nhập thành công, trả về tài khoản đã đăng nhập
                        return account;
                    }
                }
            }

            // Tên đăng nhập hoặc mật khẩu không đúng, thông báo với người dùng
            Console.WriteLine("Tên đăng nhập hoặc mật khẩu sai.");
            return null;
        }
    }
}