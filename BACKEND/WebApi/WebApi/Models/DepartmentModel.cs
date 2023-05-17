using System.Collections.Generic;

namespace WebApi.Models
{

    public class DepartmentRequest
    {
        public string Token { get; set; }
    }
    public class DepartmentResponse
    {
        public Department Data { get; set; }
    }
    public class Department
    {
        public string id { get; set; }
        public string tenDonVi { get; set; }
        public string Label { get { return tenDonVi; } }             
        public string iD_DonVi_Cha { get; set; }
        public string tenDonViCha { get; set; }
        public string iD_DonVi_Goc { get; set; }
        public string tenDonViGoc { get; set; }
        public string maDonVi { get; set; }
        public int capDonVi { get; set; }
        public string duongDan_ID_DonVi_Cha { get; set; }
        public string duongDan_ID_DonVi { get; set; }
        public string maLienThong { get; set; }
        public bool bit_DonViBoPhan { get; set; }
        public int thuTu { get; set; }
        public bool ? bit_LanhDao { get; set; }
        public bool ? bitXoa { get; set; }
        public string soDienThoai { get; set; }
        public string email { get; set; }
        public string fax { get; set; }
        public bool ?  bit_DonViThuocBo { get; set; }
        public bool ? bit_DonViSuNghiep { get; set; }
        public bool ? bit_KieuDonVi { get; set; }
        public List<Department> children { get; set; }

    }
}
