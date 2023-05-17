namespace Common.Constants
{
    public static class MessageConstants
    {
        public const string Required = "Trường {0} là bắt buộc!";
        public const string BodyNotification = "Bạn đã được phân xử lý một hồ sơ mới!";
        public const string Invalid = "Trường {0} không hợp lệ!";
        public const string MaxLength = "Trường {0} không được vượt quá {1} ký tự!";
        public const string MinLength = "Trường {0} không được ngắn hơn {1} ký tự!";
        public const string CreateFailed = "Thêm mới {0} không thành công!{1}";
        public const string CreateSuccessful = "Thêm mới {0} thành công!";
        public const string UpdateSuccessful = "Cập nhật {0} thành công!";
        public const string UpdateFailed = "Cập nhật {0} không thành công!{1}";
        public const string DeleteFailed = "Xóa {0} không thành công!{1}";
        public const string DeleteInvalid = "Bản ghi đã phát sinh dữ liệu. Xóa không thành công!";
        public const string DeleteSuccessful = "Xóa {0} thành công!";
        public const string GetListSuccessful = "Lấy danh sách {0} thành công!";
        public const string GetListFailed = "Lấy danh sách {0} không thành công!{1}";
        public const string GetDetailSuccessful = "Lấy chi tiết {0} thành công!";
        public const string GetDetailFailed = "Lấy chi tiết {0} không thành công!{1}";
        public const string CodeIsExisted = "Mã {0} đã tồn tại!";
        public const string NotExisted = "{0} không tồn tại!";
        public static string ErrorLogMessage = "An error occurred: ";
        public static string CreateSuccessMessage = "Thêm mới thành công";
        public static string CreateErrorMessage = "Thêm mới thất bại";
        public static string UpdateSuccessMessage = "Cập nhật thành công";
        public static string UpdateErrorMessage = "Cập nhật thất bại";
        public static string DeleteSuccessMessage = "Kết quả xóa";
        public static string DeleteErrorMessage = "Xóa thất bại";
        public static string DeleteItemSuccessMessage = "Xóa thành công";
        public static string DeleteItemErrorMessage = "Xóa không thành công";
        public static string DeleteItemNotFoundMessage = "Không tìm thấy đối tượng";
        public static string GetDataSuccessMessage = "Tải dữ liệu thành công";
        public static string GetDataErrorMessage = "Tải dữ liệu thất bại";
    }

    public static class StoreConstants
    {
        #region Ngôn ngữ

        public const string GetFilterDmNgonNgu = "DM_NgonNgu_GetFilter";
        public const string GetDetailDmNgonNgu = "DM_NgonNgu_GetDetail";
        public const string CheckValidDeleteNgonNgu = "DM_NgonNgu_CheckValidDelete";

        #endregion

        #region Loại văn bản

        public const string GetFilterDmLoaiVb = "DM_LoaiVanBan_GetFilter";
        public const string GetDetailDmLoaiVb = "DM_LoaiVanBan_GetDetail";
        public const string CheckValidDeleteLoaiVb = "DM_LoaiVb_CheckValidDelete";

        #endregion

        #region Lĩnh vực

        public const string GetFilterDmLinhVuc = "DM_LinhVuc_GetFilter";
        public const string GetDetailDmLinhVuc = "DM_LinhVuc_GetDetail";
        public const string CheckValidDeleteLinhVuc = "DM_LinhVuc_CheckValidDelete";

        #endregion

        #region Nhóm cơ quan ban hành

        public const string GetFilterDmNhomCoQuanBanHanh = "DM_NhomCoQuanBanHanh_GetFilter";
        public const string GetDetailDmNhomCoQuanBanHanh = "DM_NhomCoQuanBanHanh_GetDetail";
        public const string CheckValidDeleteNhomCoQuanBanHanh = "DM_NhomCoQuanBanHanh_CheckValidDelete";

        #endregion
        #region Cơ quan ban hành

        public const string GetFilterDmCoQuanBanHanh = "DM_CoQuanBanHanh_GetFilter";
        public const string GetDetailDmCoQuanBanHanh = "DM_CoQuanBanHanh_GetDetail";
        public const string CheckValidDeleteCoQuanBanHanh = "DM_CoQuanBanHanh_CheckValidDelete";

        #endregion
        #region Common

        public const string CommonLayCanBoTheoDonViGoc = "Common_LayCanBoTheoDonViGoc";
        public const string CommonLayDonViTheoDonViGocHoacCha = "Common_LayDonViTheoDonViGocHoacCha";


        #endregion
    }

    public static class SystemConstants
    {
        public const string SysParamKey = "SystemParams";
    }
}
