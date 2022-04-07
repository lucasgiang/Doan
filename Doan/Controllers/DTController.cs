using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Doan.Models;
using PagedList;
using PagedList.Mvc;


namespace Doan.Controllers
{
    public class DTController : Controller
    {
        //
        // GET: /Guitar
        dbQLDTDataContext data = new dbQLDTDataContext();
        private List<SANPHAM> Laysanphammoi(int count)
        {
            return data.SANPHAMs.OrderByDescending(a => a.NGAYCAPNHAT).Take(count).ToList();
        }
        public ActionResult Gioithieu()
        {
            return View();
        }
        public ActionResult Nhanlienhe()
        {
            return View();
        }
        public ActionResult Lienhe()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Lienhe(LIENHE lh, FormCollection collection)
        {
            var hoten = collection["HOTEN"];
            var dienthoai = collection["DIENTHOAI"];
            var email = collection["EMAIL"];
            var noidung = collection["NOIDUNG"];
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên trống";
            }
            else if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi3"] = "Số điện thoại trống";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi4"] = "Email trống";
            }
            else if (String.IsNullOrEmpty(noidung))
            {
                ViewData["Loi5"] = "Nội dung trống";
            }
            else
            {
                data.LIENHEs.InsertOnSubmit(lh);
                data.SubmitChanges();
                return RedirectToAction("Nhanlienhe");
            }
            return this.Lienhe();

        }
        public ActionResult Index(int? page)
        {
            int pagesize = 5;
            int pageNUm = (page ?? 1);

            var SanPhamMoi = Laysanphammoi(15);
            return View(SanPhamMoi.ToPagedList(pageNUm, pagesize));
        }
        public ActionResult Loaisp()
        {
            var loaisp = from ld in data.LOAISPs select ld;
            return PartialView(loaisp);
        }
        public ActionResult Details(int id)
        {
            var dan = from d in data.SANPHAMs where d.MASP == id select d;
            return View(dan.Single());
        }
        public ActionResult DanhsachSP(int id, int? page)
        {
            int pagesize = 5;
            int pageNUm = (page ?? 1);

            var dt = from g in data.SANPHAMs where g.MALOAISP == id select g;
            return View(dt.ToPagedList(pageNUm, pagesize));
        }
        public ActionResult DangXuat()
        {
            Session["TAIKHOAN"] = null;
            return RedirectToAction("Index", "DT");
        }
    }
}