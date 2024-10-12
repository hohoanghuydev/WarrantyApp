using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBaoHanh
{
    public class Customers
    {
        [BsonId]
        public ObjectId Id { get; set; } // ID tự động sinh bởi MongoDB.

        [BsonElement("name")]
        public string Name { get; set; } // Tên của khách hàng.

        [BsonElement("phone_number")]
        public string PhoneNumber { get; set; } // Số điện thoại của khách hàng.

        [BsonElement("email")]
        public string Email { get; set; } // Email của khách hàng.

        [BsonElement("address")]
        public string Address { get; set; } // Địa chỉ của khách hàng.

        // Constructor để khởi tạo khách hàng
        public Customers(string name, string phoneNumber, string email, string address)
        {
            this.Name = name;
            this.PhoneNumber = phoneNumber;
            this.Email = email;
            this.Address = address;
        }

        public Customers(string name)
        {
            this.Name = name;
        }
    }
}
