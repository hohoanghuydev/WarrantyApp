using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBaoHanh
{
    public class Employee
    {
        [BsonId]
        public ObjectId Id { get; set; }  // Mã định danh ObjectId tự động sinh.

        [BsonElement("name")]
        public string Name { get; set; }  // Tên của nhân viên.

        [BsonElement("position")]
        public string Position { get; set; }  // Vị trí công việc.

        [BsonElement("phone_number")]
        public string PhoneNumber { get; set; }  // Số điện thoại nhân viên.

        [BsonElement("email")]
        public string Email { get; set; }  // Email của nhân viên.

        public Employee(string name, string position, string phoneNumber, string email)
        {
            this.Name = name;
            this.Position = position;
            this.PhoneNumber = phoneNumber;
            this.Email = email;
        }
    }
}
