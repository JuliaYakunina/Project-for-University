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
    public partial class PrihKontr : Form
    {
        public PrihKontr()
        {
            InitializeComponent();
        }

        private void PrihKontr_Load(object sender, EventArgs e)
        {
            barEditItem25.EditValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            barEditItem18.EditValue = DateTime.Now;
			barEditItem25.EditValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            barEditItem18.EditValue = DateTime.Now;
        }
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataClasses1DataContext db = new DataClasses1DataContext();

            var datBegin = barEditItem25.EditValue;
            var datEnd = barEditItem18.EditValue;

            var kontrs = 
                db.T_KATSOPR.Where(
                    x => 
                        x.F_DOPR >= ConvertDate.intdate((DateTime)datBegin) & 
                        x.F_DOPR <= ConvertDate.intdate((DateTime)datEnd) && x.F_VIDSOPR == 101
                        ).Select(o => new KontrPrih()
                            {
                                nrec = o.F_CORG,
                                name = db.T_KATORG.First(t => t.F_NREC == o.F_CORG).F_NAME,
                                unn = db.T_KATORG.First(t => t.F_NREC == o.F_CORG).F_UNN,
                                code = db.T_KATORG.First(t => t.F_NREC == o.F_CORG).F_CODE
                            }).ToList<KontrPrih>();
            gridControl1.DataSource = kontrs.OrderBy(x => x.name)
                                      .GroupBy(x => new { x.nrec, x.name, x.unn, x.code },
                                      (key, group) => new KontrPrih()
                                      {
                                          name = key.name,
                                          nrec = key.nrec,
                                          unn = key.unn,
                                          code = key.code,
                                      }).Select(o => o).ToList<KontrPrih>();
        }

        public class KontrPrih
        {
            public string name { get; set; }
            public System.Data.Linq.Binary nrec { get; set; }
            public string unn { get; set; }
            public string code { get; set; }
        }

        private void gridControl1_CellDoubleClick(object sender, EventArgs e)
        {
            OVP_3.ClassAct.ConvertDate.StartTimer();

            var db = new DataClasses1DataContext();

            CurrencyManager cmgr = (CurrencyManager)this.gridControl1.BindingContext[this.gridControl1.DataSource];
            var kontr = (KontrPrih)cmgr.Current;

            var beginDate = barEditItem25.EditValue;
            var endDate = barEditItem18.EditValue;

            var prihs =
                db.T_KATSOPR.Where(
                    x =>
                        x.F_DOPR >= ConvertDate.intdate((DateTime)beginDate) &
                        x.F_DOPR <= ConvertDate.intdate((DateTime)endDate) && x.F_VIDSOPR == 101 &&
                        x.F_CORG == kontr.nrec).Select(o => new Prih()
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
                    summa =  String.Format("{0:0.00}", (decimal)((double)o.F_KOL * (double)o.F_PRICE)),
                    groupSnab = (db.T_HASHANs.First(x => x.F_NREC == db.T_KATMC.First(y => y.F_NREC == o.F_CMCUSL).F_CHASHAN)) != null
                    ? (Convert.ToInt32(db.T_HASHANs.First(x => x.F_NREC == db.T_KATMC.First(y => y.F_NREC == o.F_CMCUSL).F_CHASHAN).F_CANALIT_5_.ToArray().GetValue(7))) : (int?)null,
                    vidPriemki = (db.T_HASHANs.First(x => x.F_NREC == db.T_KATMC.First(y => y.F_NREC == o.F_CMCUSL).F_CHASHAN)) != null
                    ? (Convert.ToInt32(db.T_HASHANs.First(x => x.F_NREC == db.T_KATMC.First(y => y.F_NREC == o.F_CMCUSL).F_CHASHAN).F_CANALIT_2_.ToArray().GetValue(7))) : (int?)null,
                    naklNum = db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_NSOPR,
                    dateNakl = ConvertDate.dateint(db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_DSOPR),
                    orderNum = db.T_SKLORDER.First(y => y.F_CSOPR == db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_NREC).F_NORDER,
                    datePrih = ConvertDate.dateint(db.T_KATSOPR.First(x => x.F_NREC == o.F_CSOPR).F_DOPR),
                    sklad = (db.T_KATPODR.First(x => x.F_NREC == db.T_KATSOPR.First(y => y.F_NREC == o.F_CSOPR).F_CPODRTO).F_NAME),                   
                    ndsSumma = (double)o.F_KOL * (double)o.F_PRICE + (double)o.F_SUMNDS
                }).ToList<Prih>();
                itog = itog.Concat(sp).ToList<Prih>();
            }
            int i = 0;
            int a = 0;
            double allSumNds = 0;
            foreach (var pr in itog)
            {
                if (itog.Count == 1)
                    {
                        allSumNds = itog[i].ndsSumma;
                        pr.summa = pr.summa + " Итого c НДС: " + String.Format("{0:0.00}", (decimal) (allSumNds));
                    }
                else {
                    if (i == (itog.Count - 1)) 
                    {
                        for (int j = a; j <= (itog.Count - 1); j++)
                        {
                            allSumNds += itog[j].ndsSumma;
                        }
                        pr.summa = pr.summa + " Итого c НДС: " + String.Format("{0:0.00}", (decimal)(allSumNds));
                        a = i + 1;
                        allSumNds = 0;
                    }
                    else if (itog[i].orderNum != itog[i + 1].orderNum)
                    {
                        for (int j = a; j <= i; j++)
                        {
                            allSumNds += itog[j].ndsSumma;
                        }
                        pr.summa = pr.summa + " Итого c НДС: " + String.Format("{0:0.00}", (decimal)(allSumNds));
                        a = i + 1;
                        allSumNds = 0;
                    }                   
                }
                i++;
            }
            OVP_3.ClassAct.ConvertDate.EndTimer();
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
            public string summa { get; set; }
            public int? groupSnab { get; set; }
            public int? vidPriemki { get; set; }
            public string naklNum { get; set; }
            public DateTime dateNakl { get; set; }
            public string orderNum { get; set; }
            public DateTime datePrih { get; set; }
            public string sklad { get; set; }
            public double ndsSumma { get; set; }

        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                gridControl2.ExportToXls(saveFileDialog1.FileName);
            }
        }

    }
}
