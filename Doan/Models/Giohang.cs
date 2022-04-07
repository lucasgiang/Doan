using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doan.Models
{
    public class Giohang
    {
        dbQLDTDataContext data = new dbQLDTDataContext();
        public int iMasp { set; get; }
        public String sTensp { set; get; }
        public String sAnhbia { set; get; }
        public Double dDongia { set; get; }
        public int iSoluong { set; get; }
        public Double dThanhtien
        {
            get { return iSoluong * dDongia; }
        }
        public Giohang(int Masp)
        {
            iMasp = Masp;
            SANPHAM sanpham = data.SANPHAMs.Single(n => n.MASP == iMasp);
            sTensp = sanpham.TENSP;
            sAnhbia = sanpham.ANHBIA;
            dDongia = double.Parse(sanpham.GIABAN.ToString());
            iSoluong = 1;
        }
    }

}