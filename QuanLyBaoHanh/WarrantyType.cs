using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBaoHanh
{
    public class WarrantyType
    {
        public int Type { get; set; }
        public string NameType { get; set; }

        public WarrantyType()
        {
            
        }

        public WarrantyType(int type, string nameType)
        {
            this.Type = type;
            this.NameType = nameType;
        }

        public override string ToString()
        {
            return NameType;
        }
    }
}
