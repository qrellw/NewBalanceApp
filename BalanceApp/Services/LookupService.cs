using System.Collections.Generic;
using System.Linq;

namespace BalanceApp.Services;

public class LookupService
{
    public static List<string> Ethnicities { get; } = new()
    {
        "Kinh", "Tày", "Thái", "Mường", "Khmer", "Hoa", "Nùng", "H'Mông", "Dao", "Gia Rai", "Ê Đê", "Ba Na", "Xơ Đăng", "Sán Chay", "Cơ Ho", "Chăm", "Sán Dìu", "Hrê", "Ra Glai", "Mnông", "M'Nông", "Xtiêng", "Bru-Vân Kiều", "Thổ", "Giáy", "Cơ Tu", "Giẻ Triêng", "Mạ", "Khơ Mú", "Co", "Ta Ôi", "Chơ Ro", "Hà Nhì", "Chu Ru", "Lao", "La Chi", "La Ha", "Phù Lá", "La Hủ", "Lự", "Lô Lô", "Chứt", "Mảng", "Pà Thẻn", "Cơ Lao", "Cống", "Bố Y", "Si La", "Pu Péo", "Brâu", "Ơ Đu", "Rơ Măm", "Hutech" // :D
    };

    // Dictionary: Province -> List of Districts
    // Note: This is a simplified list for demonstration. In a real app, this would be a database table or JSON file.
    public static Dictionary<string, List<string>> Provinces { get; } = new()
    {
        { "Hà Nội", new List<string> { "Ba Đình", "Hoàn Kiếm", "Tây Hồ", "Long Biên", "Cầu Giấy", "Đống Đa", "Hai Bà Trưng", "Hoàng Mai", "Thanh Xuân", "Sóc Sơn", "Đông Anh", "Gia Lâm", "Nam Từ Liêm", "Bắc Từ Liêm", "Thanh Trì" } },
        { "Hồ Chí Minh", new List<string> { "Quận 1", "Quận 3", "Quận 4", "Quận 5", "Quận 6", "Quận 7", "Quận 8", "Quận 10", "Quận 11", "Quận 12", "Bình Thạnh", "Gò Vấp", "Phú Nhuận", "Tân Bình", "Tân Phú", "Bình Tân", "Thủ Đức" } },
        { "Đà Nẵng", new List<string> { "Hải Châu", "Thanh Khê", "Sơn Trà", "Ngũ Hành Sơn", "Liên Chiểu", "Cẩm Lệ", "Hòa Vang" } },
        { "Hải Phòng", new List<string> { "Hồng Bàng", "Ngô Quyền", "Lê Chân", "Hải An", "Kiến An", "Đồ Sơn", "Dương Kinh" } },
        { "Cần Thơ", new List<string> { "Ninh Kiều", "Bình Thủy", "Cái Răng", "Ô Môn", "Thốt Nốt" } },
        // Add generic provincial logic if needed or expanding later
        { "Khác", new List<string> { "Khác" } }
    };

    // Mock Ward data (Generic)
    public static List<string> GetWards(string district)
    {
        // In real app, this would query based on District ID
        return new List<string> { "Phường 1", "Phường 2", "Phường 3", "Phường 4", "Phường 5", "Xã A", "Xã B", "Thị Trấn C" };
    }
}
