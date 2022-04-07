using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Doan.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;
namespace Doan.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        dbQLDTDataContext data = new dbQLDTDataContext();
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
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
                Admin ad = data.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        public ActionResult Sanpham(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.SANPHAMs.ToList().OrderBy(n => n.MASP).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Themmoisp()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            ViewBag.MALOAISP = new SelectList(data.LOAISPs.ToList().OrderBy(n => n.TENLOAISP), "MALOAISP", "TENLOAISP");

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoisp(SANPHAM sp, HttpPostedFileBase fileupload)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            ViewBag.MALOAISP = new SelectList(data.LOAISPs.ToList().OrderBy(n => n.TENLOAISP), "MALOAISP", "TENLOAISP");
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {

                    var fileName = Path.GetFileName(fileupload.FileName);
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    sp.ANHBIA = fileName;
                    data.SANPHAMs.InsertOnSubmit(sp);
                    data.SubmitChanges();
                }
                return RedirectToAction("SANPHAM");
            }
        }
        public ActionResult Chitietsp(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            SANPHAM sp = data.SANPHAMs.SingleOrDefault(n => n.MASP == id);
            ViewBag.MASP = sp.MASP;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        public ActionResult Xoasp(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            SANPHAM sp = data.SANPHAMs.SingleOrDefault(n => n.MASP == id);
            ViewBag.MASP = sp.MASP;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpPost, ActionName("Xoasp")]
        public ActionResult Xacnhanxoa(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            SANPHAM sp = data.SANPHAMs.SingleOrDefault(n => n.MASP == id);
            ViewBag.MASP = sp.MASP;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.SANPHAMs.DeleteOnSubmit(sp);
            data.SubmitChanges();
            return RedirectToAction("SANPHAM");
        }
        [HttpGet]
        public ActionResult Suasp(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            SANPHAM sp = data.SANPHAMs.SingleOrDefault(n => n.MASP == id);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MALOAISP = new SelectList(data.LOAISPs.ToList().OrderBy(n => n.TENLOAISP), "MALOAISP", "TENLOAISP", sp.MALOAISP);
            return View(sp);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suasp(int id, HttpPostedFileBase fileUpload, FormCollection collection)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            ViewBag.MALOAISP = new SelectList(data.LOAISPs.ToList().OrderBy(n => n.TENLOAISP), "MALOAISP", "TENLOAISP");
            var sp = data.SANPHAMs.First(m => m.MASP == id);
            ViewBag.MASP = sp.MASP;
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var fileName = Path.GetFileName(fileUpload.FileName);
                        var path = Path.Combine(Server.MapPath("~/images"), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                        }
                        else
                        {
                            fileUpload.SaveAs(path);
                        }
                        sp.TENSP = collection["TENSP"];
                        sp.MALOAISP = int.Parse(collection["MALOAISP"]);
                        sp.GIABAN = decimal.Parse(collection["GIABAN"]);
                        sp.ANHBIA = fileName;
                        sp.MOTA = collection["MOTA"];
                        sp.NGAYCAPNHAT = DateTime.Parse(collection["NGAYCAPNHAT"]);
                        sp.SOLUONGTON = int.Parse(collection["SOLUONGTON"]);
                        UpdateModel(sp);
                        data.SubmitChanges();

                    }
                    catch
                    {
                        sp.TENSP = collection["TENSP"];
                        sp.MALOAISP = int.Parse(collection["MALOAISP"]);
                        sp.GIABAN = decimal.Parse(collection["GIABAN"]);
                        sp.ANHBIA = sp.ANHBIA;
                        sp.MOTA = collection["MOTA"];
                        sp.NGAYCAPNHAT = DateTime.Parse(collection["NGAYCAPNHAT"]);
                        sp.SOLUONGTON = int.Parse(collection["SOLUONGTON"]);
                        UpdateModel(sp);
                        data.SubmitChanges();


                    }

                }
                //return RedirectToAction("SANPHAM");
                return RedirectToAction("Sanpham");
            }


        }
        public ActionResult Loaisanpham()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            return View(data.LOAISPs.ToList());
        }
        [HttpGet]
        public ActionResult Themmoilsp()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoilsp(LOAISP lsp)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            data.LOAISPs.InsertOnSubmit(lsp);
            data.SubmitChanges();

            return RedirectToAction("LOAISANPHAM");
        }
        public ActionResult Chitietlsp(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            LOAISP lsp = data.LOAISPs.SingleOrDefault(n => n.MALOAISP == id);
            ViewBag.MALOAISP = lsp.MALOAISP;
            if (lsp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(lsp);
        }
        public ActionResult Xoalsp(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            LOAISP lsp = data.LOAISPs.SingleOrDefault(n => n.MALOAISP == id);
            ViewBag.MALOAISP = lsp.MALOAISP;
            if (lsp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(lsp);
        }
        [HttpPost, ActionName("Xoalsp")]
        public ActionResult Xacnhanxoalsp(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            LOAISP lsp = data.LOAISPs.SingleOrDefault(n => n.MALOAISP == id);
            ViewBag.MALOAISP = lsp.MALOAISP;
            if (lsp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.LOAISPs.DeleteOnSubmit(lsp);
            data.SubmitChanges();
            return RedirectToAction("LOAISANPHAM");
        }
        public ActionResult Sualsp(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            LOAISP lsp = data.LOAISPs.SingleOrDefault(n => n.MALOAISP == id);
            ViewBag.MALOAISP = lsp.MALOAISP;
            if (lsp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(lsp);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Sualsp(int id, FormCollection collection)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }

            var lsp = data.LOAISPs.First(m => m.MALOAISP == id);
            lsp.TENLOAISP = collection["TENLSP"];
            UpdateModel(lsp);
            data.SubmitChanges();
            return RedirectToAction("LOAISANPHAM");


        }


        public ActionResult QLDonhang(int? page)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(data.DONDATHANGs.ToList().OrderBy(n => n.MADONDATHANG).ToPagedList(pageNumber, pageSize));
        }
        //public ActionResult QLDondathang(int? page)
        //{
        //    if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
        //    {
        //        return RedirectToAction("Login", "Admin");
        //    }
        //    int pageNumber = (page ?? 1);
        //    int pageSize = 10;
        //    return View(data.CTDONDATHANGs.ToList().OrderBy(n => n.MADONDATHANG).ToPagedList(pageNumber, pageSize));
        //}
        public ActionResult EditQLDonhang(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            DONDATHANG ddh = data.DONDATHANGs.SingleOrDefault(n => n.MADONDATHANG == id);
            ViewBag.MADONDATHANG = ddh.MADONDATHANG;
            if (ddh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MAKH = new SelectList(data.KHACHHANGs.ToList().OrderBy(n => n.HOTEN), "MAKH", "HOTEN", ddh.MAKH);
            return View(ddh);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditQLDonhang(int id, FormCollection collection)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            ViewBag.MAKH = new SelectList(data.KHACHHANGs.ToList().OrderBy(n => n.HOTEN), "MAKH", "HOTEN");
            var ddh = data.DONDATHANGs.First(n => n.MADONDATHANG == id);
            ddh.MAKH = int.Parse(collection["MAKH"]);
            ddh.NGAYDAT = DateTime.Parse(collection["NGAYDAT"]);
            ddh.NGAYGIAO = DateTime.Parse(collection["NGAYGIAO"]);
            ddh.TINHTRANGGIAOHANG = bool.Parse(collection["TINHTRANGGIAOHANG"]);
            ddh.DATHANHTOAN = bool.Parse(collection["DATHANHTOAN"]);
            UpdateModel(ddh);
            data.SubmitChanges();
            return RedirectToAction("QLDonhang");
        }
        public ActionResult ListLienhe()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            return View(data.LIENHEs.ToList());
        }
        public ActionResult TTKhachhang()
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            return View(data.KHACHHANGs.ToList());
        }
        public ActionResult DangXuat()
        {
            Session["Taikhoanadmin"] = null;
            return RedirectToAction("Login", "Admin");
        }
        public ActionResult DeleteLienHe(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            LIENHE lh = data.LIENHEs.SingleOrDefault(n => n.ID == id);
            ViewBag.MALOAISP = lh.ID;
            if (lh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(lh);
        }
        [HttpPost, ActionName("DeleteLienHe")]
        public ActionResult XacNhanDeleteLH(int id)
        {
            if (Session["Taikhoanadmin"] == null || Session["Taikhoanadmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            LIENHE lh = data.LIENHEs.SingleOrDefault(n => n.ID == id);
            ViewBag.MALOAISP = lh.ID;
            if (lh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.LIENHEs.DeleteOnSubmit(lh);
            data.SubmitChanges();
            return RedirectToAction("ListLienhe");
        }

    }
}