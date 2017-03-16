using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OVP_3.ClassAct;

namespace galaktika.Forms
{
    public partial class PrihSklad : Form
    {
        public PrihSklad()
        {
            InitializeComponent();
        }

        private void PrihSklad_Load(object sender, EventArgs e)
        {
            barEditItem2.EditValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            barEditItem3.EditValue = DateTime.Now;
        }
        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataClasses1DataContext db = new DataClasses1DataContext();

            var datBegin = barEditItem2.EditValue;
            var datEnd = barEditItem3.EditValue;

            var sklads = db.T_KATSOPR.Where(x => x.F_DOPR >= ConvertDate.intdate((DateTime)datBegin)
                & x.F_DOPR <= ConvertDate.intdate((DateTime)datEnd) && x.F_VIDSOPR == 101
                ).Select(o => new SkladPrih()
                {
                    nrec = db.T_KATPODR.First(x => x.F_NREC == o.F_CPODRTO).F_NREC,                                     
                    sklad = db.T_KATPODR.First(x => x.F_NREC == o.F_CPODRTO).F_NAME,             
                }).ToList<SkladPrih>();
            gridControl1.DataSource = sklads.OrderBy(x => x.sklad)
                                      .GroupBy(x => new { x.nrec, x.sklad },
                                      (key, group) => new SkladPrih()
                                      {
                                          sklad = key.sklad,
                                          nrec = key.nrec
                                      }).Select(o => o).ToList<SkladPrih>();
        }

        public class SkladPrih
        {
            public System.Data.Linq.Binary nrec { get; set; }
            public string sklad { get; set; }
        }

        private void gridControl1_CellDoubleClick(object sender, EventArgs e)
        {
            //       OVP_3.ClassAct.ConvertDate.StartTimer();

            var db = new DataClasses1DataContext();

            CurrencyManager cmgr = (CurrencyManager)this.gridControl1.BindingContext[this.gridControl1.DataSource];
            var sklad = (SkladPrih)cmgr.Current;

            var beginDate = barEditItem2.EditValue;
            var endDate = barEditItem3.EditValue;

            var prihs =
                db.T_KATSOPR.Where(
                    x =>
                        x.F_DOPR >= ConvertDate.intdate((DateTime)beginDate) &
                        x.F_DOPR <= ConvertDate.intdate((DateTime)endDate) && x.F_VIDSOPR == 101 && 
                        x.F_CPODRTO == sklad.nrec).Select(o => new Prih()
                        {
                            nrec = o.F_NREC,
                        }).ToList<Prih>();
            var itog = new List<Prih>();
            foreach (var pr in prihs)
            {
                var sp = db.T_SPSOPR.Where(x => x.F_CSOPR == pr.nrec).Select(o => new Prih()
                {
                    nomNum = db.T_KATMC.First(x => x.F_NREC == o.F_CMCUSL).F_BARKOD,
                    obozn = db.T_KATMC.First(x => x.F_NREC == o.F_CMCUSL) != null
                    ? db.T_KATMC.First(x => x.F_NREC == o.F_CMCUSL).F_OBOZN : " ",
                    naim = db.T_KATMC.First(x => x.F_NREC == o.F_CMCUSL) != null
                    ? db.T_KATMC.First(x => x.F_NREC == o.F_CMCUSL).F_NAME : " ",
                    kol = (double)o.F_KOL,
                    edizm = db.T_KATOTPED.First(x => x.F_NREC == o.F_COTPED).F_NAME,
                    price = (double)o.F_PRICE,
                    summa = (double)o.F_KOL * (double)o.F_PRICE,
                    groupSnab = (db.T_HASHANs.First(x => x.F_NREC == db.T_KATMC.First(y => y.F_NREC == o.F_CMCUSL).F_CHASHAN)) != null
                    ? (Convert.ToInt32(db.T_HASHANs.First(x => x.F_NREC == db.T_KATMC.First(y => y.F_NREC == o.F_CMCUSL).F_CHASHAN).F_CANALIT_5_.ToArray().GetValue(7))) : (int?)null,
                    vidPriemki = (db.T_HASHANs.First(x => x.F_NREC == db.T_KATMC.First(y => y.F_NREC == o.F_CMCUSL).F_CHASHAN)) != null
                    ? (Convert.ToInt32(db.T_HASHANs.First(x => x.F_NREC == db.T_KATMC.First(y => y.F_NREC == o.F_CMCUSL).F_CHASHAN).F_CANALIT_2_.ToArray().GetValue(7))) : (int?)null,
                    contragent = db.T_KATORG.First(t => t.F_NREC == db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_CORG).F_NAME,
                    unn = db.T_KATORG.First(t => t.F_NREC == db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_CORG).F_UNN,
                    code = db.T_KATORG.First(t => t.F_NREC == db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_CORG).F_CODE,
                    naklNum = db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_NSOPR,
                    dateNakl = ConvertDate.dateint(db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_DSOPR),
                    orderNum = db.T_SKLORDER.First(y => y.F_CSOPR == db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_NREC).F_NORDER,
                    datePrih = ConvertDate.dateint(db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_DOPR),                    
                }).ToList<Prih>();
                itog = itog.Concat(sp).ToList<Prih>();
            }
            //      OVP_3.ClassAct.ConvertDate.EndTimer();
            gridControl2.DataSource = itog;
        }

        public class Prih
        {
            public System.Data.Linq.Binary nrec { get; set; }
            public string nomNum { get; set; }
            public string obozn { get; set; }
            public string naim { get; set; }
            public double kol { get; set; }
            public string edizm { get; set; }
            public double price { get; set; }
            public double summa { get; set; }
            public int? groupSnab { get; set; }
            public int? vidPriemki { get; set; }
            public string contragent { get; set; }
            public string unn { get; set; }
            public string code { get; set; }
            public string naklNum { get; set; }
            public DateTime dateNakl { get; set; }
            public string orderNum { get; set; }
            public DateTime datePrih { get; set; }

        }

        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                gridControl2.ExportToXls(saveFileDialog1.FileName);
            }
        }
    }
}
