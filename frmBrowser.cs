using CsQuery;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CaptchaBypassTest
{
    public partial class frmBrowser : Form
    {
        string firstUrl = CommonMethods.ReadSetting("FirstUrl");
        string baseUrl = CommonMethods.ReadSetting("BaseUrl");

        byte loadCount = 0;
        string _searchValue = string.Empty;
        public List<SchoolData> schoolDataList = null;
        PictureBox pbMain;

        public frmBrowser(string searchValue)
        {
            InitializeComponent();
            _searchValue = searchValue;
        }


        private void frmWebBrowser_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            tmScroll.Enabled = false;
            webBrowser.AutoScrollOffset = new Point(0, 0);
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.AllowNavigation = true;
            webBrowser.Navigate(firstUrl);
            webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
        }

        private void CloseAll()
        {
            this.Close();
        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                loadCount++;

                if (loadCount == 1)
                {
                    tmScroll.Interval = 500;
                    tmScroll.Enabled = true;
                }
                else if (loadCount == 2)
                {
                    var cap = webBrowser.Document.GetElementById(CommonMethods.ReadSetting("CaptchaId"));
                    var xCap = CommonMethods.getXoffset(cap);
                    var ycap = CommonMethods.getYoffset(cap);
                    webBrowser.Document.Window.ScrollTo(xCap, ycap);
                    tmScroll.Interval = 1000;
                    tmScroll.Enabled = true;
                }
                else
                {
                    loadCount = 0;

                    string html = ((WebBrowser)sender).DocumentText;
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);

                    //Convertir a lista de listas y a modelo.
                    HtmlNode tablebody = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'rf-dt table')]/tbody");
                    schoolDataList = new List<SchoolData>();

                    if (tablebody != null)
                    {
                        foreach (HtmlNode tr in tablebody.SelectNodes("./tr"))
                        {
                            string[] data = new string[9];
                            foreach (HtmlNode td in tr.SelectNodes("./td"))
                            {
                                if (td.GetAttributeValue("class", "null") == "rf-dt-c")
                                {
                                    data[tr.SelectNodes("./td").IndexOf(td)] = td.InnerText.Replace("\n", "").Trim();
                                }
                                else
                                {
                                    data[tr.SelectNodes("./td").IndexOf(td)] = string.Empty;
                                }
                            }

                            SchoolData schoolData = new SchoolData(data);
                            schoolDataList.Add(schoolData);
                        }
                    }

                    CloseAll();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void tmScroll_Tick(object sender, EventArgs e)
        {
            try
            {
                tmScroll.Enabled = false;

                if (loadCount == 1)
                {
                    webBrowser.Navigate(firstUrl);
                }
                else
                {
                    Bitmap bitmap = new Bitmap(webBrowser.Width, webBrowser.Height);
                    webBrowser.DrawToBitmap(bitmap, new Rectangle(0, 0, webBrowser.Width, webBrowser.Height));
                    pbMain = new PictureBox();
                    pbMain.Image = bitmap;

                    tmReadCap.Interval = 1000;
                    tmReadCap.Enabled = true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void tmReadCap_Tick(object sender, EventArgs e)
        {
            try
            {
                tmReadCap.Enabled = false;

                string html = webBrowser.DocumentText;
                string captchaText = string.Empty;
                captchaText = CommonMethods.GetText(new Bitmap(pbMain.Image)).Replace("\n", "").Replace("'", "").Replace("‘", "").Replace(" ", "").Trim();

                if (!string.IsNullOrEmpty(captchaText))
                {
                    CQ dom = CQ.Create(html);
                    string viewstate = dom["input#javax\\.faces\\.ViewState"].Val();
                    webBrowser.Navigate(string.Format(baseUrl, captchaText, viewstate, CommonMethods.SeparateLastName(_searchValue)));
                }
                else
                {
                    CloseAll();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
