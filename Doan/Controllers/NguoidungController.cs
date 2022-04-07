using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Doan.Models;

namespace Doan.Controllers
{
    public class NguoidungController : Controller
    {
        // GET: Nguoidung
        dbQLDTDataContext data = new dbQLDTDataContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangky(FormCollection collection, KHACHHANG kh)
        {
            var hoten = collection["HOTEN"];
            var tendn = collection["TAIKHOAN"];
            var matkhau = collection["MATKHAU"];
            var mknhaplai = collection["Nhaplaimatkhau"];
            var diachi = collection["DIACHI"];
            var email = collection["EMAIL"];
            var dienthoai = collection["SDT"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["NGAYSINH"]);
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Ho tên khách hàng không được để trống";
            }
            else if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Phải nhập mật khẩu";
            }
            else if (String.IsNullOrEmpty(mknhaplai))
            {
                ViewData["Loi4"] = "Phải nhập lại mật khẩu";
            }
            else if (String.IsNullOrEmpty(diachi))
            {
                ViewData["Loi5"] = "Phai nhập địa chỉ";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi6"] = "Phải nhập Email";
            }
            else if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi7"] = "Phải nhập điện thoại";
            }
            else if (string.Format(matkhau) != string.Format(mknhaplai))
            {
                ViewData["Loi8"] = "Mật khẩu nhập lại không đúng";
            }
            else
            {
                kh.HOTEN = hoten;
                kh.TAIKHOAN = tendn;
                kh.MATKHAU = matkhau;
                kh.MAIL = email;
                kh.DIACHI = diachi;
                kh.SDT = dienthoai;
                kh.NGAYSINH = DateTime.Parse(ngaysinh);
                data.KHACHHANGs.InsertOnSubmit(kh);
                data.SubmitChanges();
                return RedirectToAction("Dangnhap");

            }
            return this.Dangky();
        }

        public ActionResult Dangnhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            var tendn = collection["TAIKHOAN"];
            var matkhau = collection["MATKHAU"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.TAIKHOAN == tendn && n.MATKHAU == matkhau);
                if (kh == null)
                {
                    ViewBag.Thongbao = "Ten đăng nhập hoặc mật khẩu không đúng";
                }
                else
                {
                    //ViewBag.Thongbao = "Chúc mừng bạn đăng nhập thành công";

                    Session["Taikhoan"] = kh;
                    return RedirectToAction("Index", "DT");
                }


            }
            return View();
        }
    }
}