using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVRenter_Data.Migrations
{
    /// <inheritdoc />
    public partial class InitAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ❌ KHÔNG tạo bảng, KHÔNG chạm dữ liệu.
            // ✅ Migration này chỉ đánh dấu rằng database hiện tại đã đồng bộ.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Giữ trống luôn để tránh rollback xóa nhầm dữ liệu.
        }
    }
}
