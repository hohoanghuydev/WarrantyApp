using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBaoHanh
{
    public class Products
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("color")]
        public string Color { get; set; }

        [BsonElement("import_date")]
        public DateTime ImportDate { get; set; }

        [BsonElement("export_date")]
        public DateTime ExportDate { get; set; }

        [BsonElement("warranty_period")]
        public int WarrantyPeriod { get; set; }

        [BsonElement("supplier")]
        public string Supplier { get; set; }

        // Constructor
        public Products(string name, decimal price, string color, DateTime importDate, DateTime exportDate, int warrantyPeriod, string supplier)
        {
            this.Id = ObjectId.GenerateNewId();
            this.Name = name;
            this.Price = price;
            this.Color = color;
            this.ImportDate = importDate;
            this.ExportDate = exportDate;
            this.WarrantyPeriod = warrantyPeriod;
            this.Supplier = supplier;
        }

        public Products(string name)
        {
            this.Name = name;
        }
    }
}
