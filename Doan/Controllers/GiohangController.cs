using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Doan.Models;

namespace Doan.Controllers
{
    public class GiohangController : Controller
    {
        // GET: Giohang
        dbQLDTDataContext data = new dbQLDTDataContext();
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang == null)
            {
                lstGiohang = new List<Giohang>();
                Session["GioHang"] = lstGiohang;
            }
            return lstGiohang;

        }
        //Them gio hang
        public ActionResult ThemGiohang(int iMasp, string strURL)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.Find(n => n.iMasp == iMasp);
            if (sanpham == null)
            {
                sanpham = new Giohang(iMasp);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        //TOng so luong
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }
        public ActionResult GioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "GDT");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        public ActionResult XoaGiohang(int iMaspp)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasp == iMaspp);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMasp == iMaspp);
                return RedirectToAction("GioHang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "DT");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapnhatGiohang(int iMaspp, FormCollection f)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasp == iMaspp);
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoatatcaGiohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "DT");
        }
        [HttpGet]
        public ActionResult Dathang()
        {
            if (Session["TAIKHOAN"] == null || Session["TAIKHOAN"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "DT");
            }
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        public ActionResult Dathang(FormCollection collection)
        {
            var ngay = collection["NGAYGIAO"];
            if (String.IsNullOrEmpty(ngay))
            {
                ViewData["Loi"] = "Quý khách chưa chọn ngày giao";
            }
            else
            {
                DONDATHANG ddh = new DONDATHANG();
                KHACHHANG kh = (KHACHHANG)Session["TAIKHOAN"];
                List<Giohang> gh = Laygiohang();
                ddh.MAKH = kh.MAKH;
                ddh.NGAYDAT = DateTime.Now;
                var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["NGAYGIAO"]);
                ddh.NGAYGIAO = DateTime.Parse(ngaygiao);
                ddh.TINHTRANGGIAOHANG = false;
                ddh.DATHANHTOAN = false;
                data.DONDATHANGs.InsertOnSubmit(ddh);
                data.SubmitChanges();
                foreach (var item in gh)
                {
                    CTDONDATHANG ctddh = new CTDONDATHANG();
                    ctddh.MADONDATHANG = ddh.MADONDATHANG;
                    ctddh.MASP = item.iMasp;
                    ctddh.SOLUONG = item.iSoluong;
                    ctddh.DONGIA = (decimal)item.dDongia;
                    data.CTDONDATHANGs.InsertOnSubmit(ctddh);
                }
                data.SubmitChanges();
                Session["GioHang"] = null;
                return RedirectToAction("Xacnhandonhang", "GioHang");
            }
            return this.Dathang();

        }
        public ActionResult Xacnhandonhang()
        {
            return View();
        }
        public ActionResult Onepay()
        {

            double t = TongTien();
            string amount = (t * 100).ToString();
            string url = RedirectOnepay("thanh toan", amount, "192.168.1.16");
            return Redirect(url);
        }
        public string RedirectOnepay(string codeInvoice, string amount, string ip)
        {
            // Khoi tao lop thu vien
            VPCRequest conn = new VPCRequest(OnePayProperty.Url_ONEPAY_TEST);
            conn.SetSecureSecret(OnePayProperty.HASH_CODE);

            // Gan cac thong so de truyen sang cong thanh toan onepay
            conn.AddDigitalOrderField("AgainLink", OnePayProperty.AGAIN_LINK);
            conn.AddDigitalOrderField("Title", "Tich hop onepay vao web asp.net mvc3,4");
            conn.AddDigitalOrderField("vpc_Locale", OnePayProperty.PAYGATE_LANGUAGE);
            conn.AddDigitalOrderField("vpc_Version", OnePayProperty.VERSION);
            conn.AddDigitalOrderField("vpc_Command", OnePayProperty.COMMAND);
            conn.AddDigitalOrderField("vpc_Merchant", OnePayProperty.MERCHANT_ID);
            conn.AddDigitalOrderField("vpc_AccessCode", OnePayProperty.ACCESS_CODE);
            conn.AddDigitalOrderField("vpc_MerchTxnRef", "thanh toan");
            conn.AddDigitalOrderField("vpc_OrderInfo", codeInvoice);
            conn.AddDigitalOrderField("vpc_Amount", amount);
            conn.AddDigitalOrderField("vpc_ReturnURL", Url.Action("OnepayResponse", "GioHang", null, Request.Url.Scheme, null));

            // Thong tin them ve khach hang. De trong neu khong co thong tin
            conn.AddDigitalOrderField("vpc_SHIP_Street01", "");
            conn.AddDigitalOrderField("vpc_SHIP_Provice", "");
            conn.AddDigitalOrderField("vpc_SHIP_City", "");
            conn.AddDigitalOrderField("vpc_SHIP_Country", "");
            conn.AddDigitalOrderField("vpc_Customer_Phone", "");
            conn.AddDigitalOrderField("vpc_Customer_Email", "");
            conn.AddDigitalOrderField("vpc_Customer_Id", "");
            conn.AddDigitalOrderField("vpc_TicketNo", ip);

            string url = conn.Create3PartyQueryString();
            return url;
        }
        public ActionResult OnepayResponse()
        {
            string hashvalidateResult = "";

            // Khoi tao lop thu vien
            VPCRequest conn = new VPCRequest(OnePayProperty.Url_ONEPAY_TEST);
            conn.SetSecureSecret(OnePayProperty.HASH_CODE);

            // Xu ly tham so tra ve va du lieu ma hoa
            hashvalidateResult = conn.Process3PartyResponse(Request.QueryString);

            // Lay tham so tra ve tu cong thanh toan
            string vpc_TxnResponseCode = conn.GetResultField("vpc_TxnResponseCode");
            string amount = conn.GetResultField("vpc_Amount");
            string localed = conn.GetResultField("vpc_Locale");
            string command = conn.GetResultField("vpc_Command");
            string version = conn.GetResultField("vpc_Version");
            string cardType = conn.GetResultField("vpc_Card");
            string orderInfo = conn.GetResultField("vpc_OrderInfo");
            string merchantID = conn.GetResultField("vpc_Merchant");
            string authorizeID = conn.GetResultField("vpc_AuthorizeId");
            string merchTxnRef = conn.GetResultField("vpc_MerchTxnRef");
            string transactionNo = conn.GetResultField("vpc_TransactionNo");
            string acqResponseCode = conn.GetResultField("vpc_AcqResponseCode");
            string txnResponseCode = vpc_TxnResponseCode;
            string message = conn.GetResultField("vpc_Message");

            // Kiem tra 2 tham so tra ve quan trong nhat
            if (hashvalidateResult == "CORRECTED" && txnResponseCode.Trim() == "0")
            {
                return View("PaySuccess");
            }
            else if (hashvalidateResult == "INVALIDATED" && txnResponseCode.Trim() == "0")
            {
                return View("PayPending");
            }
            else
            {
                return View("PayUnSuccess");
            }
        }

    }
}