using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBaoHanh
{
    public class RepairDetail
    {
        public RepairDetail()
        {
            
        }

        public RepairDetail(string repairPart, int price)
        {
            RepairPart = repairPart;
            Price = price;
        }

        [BsonElement("repair_part")]
        public string RepairPart { get; set; }

        [BsonElement("price")]
        public int Price { get; set; }
    }
    public class WarrantyHistory
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("warranty_id")]
        public ObjectId WarrantyId { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("reception_date")]
        public DateTime ReceptionDate { get; set; }

        [BsonElement("repair_details")]
        public List<RepairDetail> RepairDetails { get; set; }

        [BsonElement("repair_status")]
        public int RepairStatus { get; set; }

        [BsonElement("return_date")]
        public DateTime? ReturnDate { get; set; }

        [BsonElement("repair_staff")]
        public ObjectId RepairStaff { get; set; }

        [BsonElement("warranty_type")]
        public int WarrantyType { get; set; }

        [BsonElement("total")]
        public int Total { get; set; }

        public WarrantyHistory(ObjectId warrantyId, string description, DateTime receptionDate, List<RepairDetail> repairDetails, int repairStatus, DateTime returnDate, ObjectId repairStaff, int warrantyType, int total)
        {
            WarrantyId = warrantyId;
            Description = description;
            ReceptionDate = receptionDate;
            RepairDetails = repairDetails;
            RepairStatus = repairStatus;
            ReturnDate = returnDate;
            RepairStaff = repairStaff;
            WarrantyType = warrantyType;
            Total = total;
        }

        public WarrantyHistory(ObjectId warrantyId, string description, DateTime receptionDate, List<RepairDetail> repairDetails, int repairStatus, DateTime? returnDate, ObjectId repairStaff, int warrantyType, int total)
        {
            WarrantyId = warrantyId;
            Description = description;
            ReceptionDate = receptionDate;
            RepairDetails = repairDetails;
            RepairStatus = repairStatus;
            ReturnDate = returnDate;
            RepairStaff = repairStaff;
            WarrantyType = warrantyType;
            Total = total;
        }
    }

}
