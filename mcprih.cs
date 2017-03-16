using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data.WcfLinq.Helpers;
using DevExpress.XtraPrinting.Native;
using OVP_3.ClassAct;

namespace galaktika.Forms
{
    public partial class PrihMC : Form
    {
        public PrihMC()
        {
            InitializeComponent();
        }
		
		        public PrihMC()
        {
            InitializeComponent();
        }

        private void PrihMC_Load(object sender, EventArgs e)
        {
            barEditItem42.EditValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            barEditItem43.EditValue = DateTime.Now;
        }
        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataClasses1DataContext db = new DataClasses1DataContext();

            var datBegin = barEditItem42.EditValue;
            var datEnd = barEditItem43.EditValue;

            var mcs = db.T_KATSOPR.Where(x => x.F_DOPR >= ConvertDate.intdate((DateTime)datBegin) 
                & x.F_DOPR <= ConvertDate.intdate((DateTime)datEnd) 
                && x.F_VIDSOPR == 101 
                ).Select(o => new MCPrih()
            {
                
              

                nrec = db.T_KATMC.First(x => x.F_NREC == db.T_SPSOPR.First(y => y.F_CSOPR == o.F_NREC).F_CMCUSL).F_NREC,

                nomNum1 = (db.T_KATMC.First(x => x.F_NREC == db.T_SPSOPR.First(y => y.F_CSOPR == o.F_NREC).F_CMCUSL).F_BARKOD),

                obozn1 = db.T_KATMC.First(x => x.F_NREC == db.T_SPSOPR.First(y => y.F_CSOPR == o.F_NREC).F_CMCUSL) != null
                ? db.T_KATMC.First(x => x.F_NREC == db.T_SPSOPR.First(y => y.F_CSOPR == o.F_NREC).F_CMCUSL).F_OBOZN : " ",

                naim1 = db.T_KATMC.First(x => x.F_NREC == db.T_SPSOPR.First(y => y.F_CSOPR == o.F_NREC).F_CMCUSL) != null
                ? db.T_KATMC.First(x => x.F_NREC == db.T_SPSOPR.First(y => y.F_CSOPR == o.F_NREC).F_CMCUSL).F_NAME : " ",

                groupSnab1 = (db.T_HASHANs.First(x => x.F_NREC == db.T_KATMC.First(y => y.F_NREC == db.T_SPSOPR.First(z => z.F_CSOPR == o.F_NREC).F_CMCUSL).F_CHASHAN)) != null
                ? (Convert.ToInt32(db.T_HASHANs.First(x => x.F_NREC == db.T_KATMC.First(i => i.F_NREC == db.T_SPSOPR.First(z => z.F_CSOPR == o.F_NREC).F_CMCUSL).F_CHASHAN).F_CANALIT_5_.ToArray().GetValue(7))) : (int?)null,

                vidPriemki1 = (db.T_HASHANs.First(x => x.F_NREC == db.T_KATMC.First(y => y.F_NREC == db.T_SPSOPR.First(z => z.F_CSOPR == o.F_NREC).F_CMCUSL).F_CHASHAN)) != null
                ? (Convert.ToInt32(db.T_HASHANs.First(x => x.F_NREC == db.T_KATMC.First(i => i.F_NREC == db.T_SPSOPR.First(z => z.F_CSOPR == o.F_NREC).F_CMCUSL).F_CHASHAN).F_CANALIT_2_.ToArray().GetValue(7))) : (int?)null,

            }).ToList<MCPrih>();
            //gridControl1.DataSource = mcs.OrderBy(x => x.nomNum1)
            //                          .GroupBy(x => new { x.nrec, x.nomNum1, x.obozn1, x.naim1, x.groupSnab1, x.vidPriemki1 },
            //                          (key, group) => new MCPrih()
            //                          {
            //                              nomNum1 = key.nomNum1,
            //                              nrec = key.nrec,
            //                              obozn1 = key.obozn1,
            //                              naim1 = key.naim1,
            //                              groupSnab1 = key.groupSnab1,
            //                              vidPriemki1 = key.vidPriemki1
            //                          }).Select(o => o).ToList<MCPrih>();
            gridControl1.DataSource = mcs.Select(o => o).ToList<MCPrih>();
        }

        public class MCPrih
        {
            public System.Data.Linq.Binary nrec { get; set; }
            public string nomNum1 { get; set; }
            public string obozn1 { get; set; }
            public string naim1 { get; set; }
            public int? groupSnab1 { get; set; }
            public int? vidPriemki1 { get; set; }

            public int count { get; set; }
        }

        private void gridControl1_CellDoubleClick(object sender, EventArgs e)
        {
            OVP_3.ClassAct.ConvertDate.StartTimer();

            var db = new DataClasses1DataContext();

            CurrencyManager cmgr = (CurrencyManager)this.gridControl1.BindingContext[this.gridControl1.DataSource];
            var mc = (MCPrih)cmgr.Current;

            var beginDate = barEditItem42.EditValue;
            var endDate = barEditItem43.EditValue;

            var prihs =
                db.T_KATSOPR.Where(
                    x =>
                        x.F_DOPR >= ConvertDate.intdate((DateTime)beginDate) &
                        x.F_DOPR <= ConvertDate.intdate((DateTime)endDate) && x.F_VIDSOPR == 101                         
                        ).Select(o => new Prih()
                        {
                            nrec = o.F_NREC,
                        }).ToList<Prih>();
            var itog = new List<Prih>();
            foreach (var pr in prihs)
            {
                var sp = db.T_SPSOPR.Where(x => x.F_CSOPR == pr.nrec &&
                mc.nrec == x.F_CMCUSL
                ).Select(o => new Prih()
                {
                    kol = (double)o.F_KOL,
                    edizm = db.T_KATOTPED.First(x => x.F_NREC == o.F_COTPED).F_NAME,
                    price = (double)o.F_PRICE,
                    summa = (double)o.F_KOL * (double)o.F_PRICE,     
                    contragent = db.T_KATORG.First(t => t.F_NREC == db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_CORG).F_NAME,
                    unn = db.T_KATORG.First(t => t.F_NREC == db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_CORG).F_UNN,
                    code = db.T_KATORG.First(t => t.F_NREC == db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_CORG).F_CODE,
                    naklNum = db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_NSOPR,
                    dateNakl = ConvertDate.dateint(db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_DSOPR),
                    orderNum = db.T_SKLORDER.First(y => y.F_CSOPR == db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_NREC).F_NORDER,
                    datePrih = ConvertDate.dateint(db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_DOPR),
                    sklad = (db.T_KATPODR.First(x => x.F_NREC == db.T_KATSOPR.First(y => y.F_NREC == o.F_CSOPR).F_CPODRTO).F_NAME)
                }).ToList<Prih>();
                itog = itog.Concat(sp).ToList<Prih>();
            }
            OVP_3.ClassAct.ConvertDate.EndTimer();
            gridControl2.DataSource = itog;
        }

        public class Prih
        {
            public System.Data.Linq.Binary nrec { get; set; }
            public double kol { get; set; }
            public string edizm { get; set; }
            public double price { get; set; }
            public double summa { get; set; }
            public string contragent { get; set; }
            public string unn { get; set; }
            public string code { get; set; }
            public DateTime dateNakl { get; set; }
            public string orderNum { get; set; }
            public DateTime datePrih { get; set; }
            public string sklad { get; set; }
            public string naklNum { get; set; }

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
