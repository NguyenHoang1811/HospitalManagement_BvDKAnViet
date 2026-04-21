using HospitalManagement_BvDKAnViet.Core.DTOs.AccountDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServies;
using HospitalManagement_BvDKAnViet.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HospitalManagement_BvDKAnViet.Data.Sevices
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _db;

        public AccountRepository(ApplicationDbContext db) => _db = db;
        public Task<User> Account_Login(LoginRequestDto request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    throw new ArgumentException("Dữ liệu đầu vào không hợp lệ.");
                }

                var passwordHash = HashPassword(request.Password);

                return _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == passwordHash);
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi đăng nhập: " + ex.Message);
            }

        }

        //Ham ma hoa
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
