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

            // Nếu chưa đăng nhập mà truy cập chức năng yêu cầu quyền -> Chuyển về trang Đăng nhập
            if (string.IsNullOrEmpty(currentRole))
            {
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", new { returnUrl = context.HttpContext.Request.Path });
                return;
            }

            if (!_vaiTroChoPhep.Contains(currentRole) && !_vaiTroChoPhep.Contains("ALL"))
            {
                // Bị từ chối quyền truy cập (ví dụ khách hàng cố vào trang quản trị)
                context.Result = new RedirectToActionResult("KhongCoQuyen", "Admin", new { yeuCau = string.Join(", ", _vaiTroChoPhep) });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
