using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBaoHanh
{
    public class Warranty
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("customer_id")]
        public ObjectId customer_id { get; set; }
        [BsonElement("product_id")]
        public ObjectId product_id { get; set; }
        [BsonElement("activation_date")]
        public DateTime activation_date { get; set; }
        [BsonElement("expiry_date")]
        public DateTime expiry_date { get; set; }
        [BsonElement("status")]
        public int status { get; set; }

        public Warranty(ObjectId customer_id, ObjectId product_id, DateTime activation_date, DateTime expiry_date, int status)
        {
            this.customer_id = customer_id;
            this.product_id = product_id;
            this.activation_date = activation_date;
            this.expiry_date = expiry_date;
            this.status = status;
        }
    }
}
