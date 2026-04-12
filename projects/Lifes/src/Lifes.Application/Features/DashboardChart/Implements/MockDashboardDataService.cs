using Lifes.Application.Features.DashboardChart.Interfaces;
using Lifes.Core.Models;
using Lifes.Domain.Features.DashboardChart.Entities;

namespace Lifes.Application.Features.DashboardChart.Implements;

public class MockDashboardDataService : IDashboardDataService
{
    public Task<Result<IEnumerable<DashboardBlock>>> GetBlocksAsync()
    {
        var blocks = new List<DashboardBlock>
        {
            CreateAstrologyBlock(0, "Quý Tỵ", "Mệnh Cung", "2-11", new[] { "Thiên Phủ", "Hữu Bật", "Hỏa Tinh" }),
            CreateAstrologyBlock(1, "Giáp Ngọ", "Phụ Mẫu", "112-121", new[] { "Thiên Đồng", "Thái Âm", "Văn Khúc", "Thiên Khôi" }),
            CreateAstrologyBlock(2, "Ất Mùi", "Phúc Đức", "102-111", new[] { "Vũ Khúc", "Tham Lang" }),
            CreateAstrologyBlock(3, "Bính Thân", "Điền Trạch", "92-101", new[] { "Thái Dương", "Cự Môn", "Văn Xương", "Đà La" }),
            CreateAstrologyBlock(4, "Đinh Dậu", "Quan Lộc", "82-91", new[] { "Thiên Tướng", "Tả Phù", "Lộc Tồn", "Địa Không" }),
            CreateAstrologyBlock(5, "Mậu Tuất", "Nô Bộc", "72-81", new[] { "Thiên Cơ", "Thiên Lương", "Kình Dương" }),
            CreateAstrologyBlock(6, "Kỷ Hợi", "Thiên Di", "62-71", new[] { "Tử Vi", "Thất Sát", "Thiên Mã" }),
            CreateAstrologyBlock(7, "Canh Tý", "Tật Ách", "52-61", new[] { "Linh Tinh" }),
            CreateAstrologyBlock(8, "Tân Sửu", "Tài Bạch", "42-51", new[] { "Địa Kiếp" }),
            CreateAstrologyBlock(9, "Nhâm Dần", "Tử Tức", "32-41", new[] { "Thiên Việt" }),
            CreateAstrologyBlock(10, "Quý Mão", "Phu Thê", "22-31", new[] { "Liêm Trinh", "Phá Quân" }),
            new DashboardBlock { Index = 11, BlockType = "UnknownDemo", Name = "Demo Default View", Data = null } // Demo Default View
        };

        return Task.FromResult(Result<IEnumerable<DashboardBlock>>.Success(blocks));
    }

    private DashboardBlock CreateAstrologyBlock(int index, string headerText, string title, string minorText, string[] elements)
    {
        return new DashboardBlock
        {
            Index = index,
            BlockType = "AstrologyCell",
            Name = title,
            Data = new AstrologyBlockData
            {
                HeaderText = headerText,
                MinorText = minorText,
                Elements = elements
            }
        };
    }

    public Task<Result<DashboardCenterInfo>> GetCenterInfoAsync()
    {
        var info = new DashboardCenterInfo
        {
            Title = "Lá Số Tử Vi",
            SolarDate = "Dương lịch: 2/8/1961",
            LunarDate = "Âm lịch: 21/6/1961",
            Bazi = "Can chi: Tân Sửu, Ất Mùi, Đinh Mão, Nhâm Dần",
            BirthTime = "Giờ sinh: Dần (03:00~05:00)",
            Gender = "Giới tính: Nam",
            Element = "Mệnh: Bích Thượng Thổ",
            ElementBureau = "Cục: Thủy Nhị Cục",
            DestinyMaster = "Mệnh Chủ: Cự Môn",
            BodyMaster = "Thân Chủ: Thiên Tướng",
            ZodiacAnimal = "Năm con: Trâu (Sửu)",
            ZodiacSign = "Cung hoàng đạo: Sư Tử"
        };
        
        return Task.FromResult(Result<DashboardCenterInfo>.Success(info));
    }
}
