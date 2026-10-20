using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TravelWeb.Filters
{
    public class PhanQuyenAttribute : ActionFilterAttribute
    {
        private readonly string[] _vaiTroChoPhep;

        public PhanQuyenAttribute(params string[] vaiTroChoPhep)
        {
            _vaiTroChoPhep = vaiTroChoPhep;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            string? currentRole = session.GetString("CURRENT_ROLE");

            // Mặc định nếu chưa đăng nhập hoặc chưa chọn role, ta có thể đặt là NHAN_VIEN hoặc ADMIN để tiện demo ban đầu,
            // nhưng nếu đã set KHACH_HANG thì sẽ kiểm tra nghiêm ngặt
            if (string.IsNullOrEmpty(currentRole))
            {
                // Mặc định cấp quyền NHAN_VIEN cho session mới để tiện thao tác kiểm thử ngay
                currentRole = "NHAN_VIEN";
                session.SetString("CURRENT_ROLE", currentRole);
                session.SetString("CURRENT_USER", "Bùi Trung Kiên (Nhân viên)");
            }

            if (!_vaiTroChoPhep.Contains(currentRole) && !_vaiTroChoPhep.Contains("ALL"))
            {
                // Bị từ chối quyền truy cập
                context.Result = new RedirectToActionResult("KhongCoQuyen", "Admin", new { yeuCau = string.Join(", ", _vaiTroChoPhep) });
            }

            base.OnActionExecuting(context);
        }
    }
}
