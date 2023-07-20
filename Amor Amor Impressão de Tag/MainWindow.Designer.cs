using Amor_Amor_Impressão_de_Tag.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Resources;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Drawing;

namespace Amor_Amor_Impressão_de_Tag
{
    public partial class MainWindow : Form
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        // P/Invoke declarations
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter(string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }

            // If you did not succeed, GetLastError may give more information about the failure.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        private TextBox productSearchTextBox;
        private Button searchButton;
        private Button printButton;
        private Button optionsButton;
        private Label productInfoLabel;
        public PictureBox logoAmorAmor;
        private Label label1;
        private Label label2;
        private PictureBox zplPreviewPictureBox;
        public PictureBox tagLogo;

        private string apiToken;  // Move this to the class level

        // Declare zpl as a private instance variable so it can be accessed by multiple methods
        private string zpl;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.productSearchTextBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.printButton = new System.Windows.Forms.Button();
            this.optionsButton = new System.Windows.Forms.Button();
            this.productInfoLabel = new System.Windows.Forms.Label();
            this.logoAmorAmor = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.zplPreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.tagLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.logoAmorAmor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zplPreviewPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tagLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // productSearchTextBox
            // 
            this.productSearchTextBox.AllowDrop = true;
            this.productSearchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.productSearchTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.productSearchTextBox.Location = new System.Drawing.Point(12, 136);
            this.productSearchTextBox.Name = "productSearchTextBox";
            this.productSearchTextBox.Size = new System.Drawing.Size(447, 38);
            this.productSearchTextBox.TabIndex = 0;
            this.productSearchTextBox.TextChanged += new System.EventHandler(this.productSearchTextBox_TextChanged);
            this.productSearchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ProductSearchTextBox_KeyDown);
            // 
            // searchButton
            // 
            this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchButton.Enabled = false;
            this.searchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchButton.Location = new System.Drawing.Point(465, 136);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(105, 38);
            this.searchButton.TabIndex = 1;
            this.searchButton.Text = "Buscar";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // printButton
            // 
            this.printButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.printButton.BackColor = System.Drawing.Color.DarkGreen;
            this.printButton.Enabled = false;
            this.printButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printButton.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.printButton.Location = new System.Drawing.Point(369, 562);
            this.printButton.Name = "printButton";
            this.printButton.Size = new System.Drawing.Size(201, 45);
            this.printButton.TabIndex = 2;
            this.printButton.Text = "Imprimir";
            this.printButton.UseVisualStyleBackColor = false;
            this.printButton.Click += new System.EventHandler(this.PrintButton_Click);
            // 
            // optionsButton
            // 
            this.optionsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.optionsButton.Location = new System.Drawing.Point(12, 584);
            this.optionsButton.Name = "optionsButton";
            this.optionsButton.Size = new System.Drawing.Size(75, 23);
            this.optionsButton.TabIndex = 3;
            this.optionsButton.Text = "Opções";
            this.optionsButton.UseVisualStyleBackColor = true;
            this.optionsButton.Click += new System.EventHandler(this.OptionsButton_Click);
            // 
            // productInfoLabel
            // 
            this.productInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.productInfoLabel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.productInfoLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.productInfoLabel.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold);
            this.productInfoLabel.Location = new System.Drawing.Point(12, 195);
            this.productInfoLabel.Name = "productInfoLabel";
            this.productInfoLabel.Size = new System.Drawing.Size(352, 350);
            this.productInfoLabel.TabIndex = 4;
            // 
            // logoAmorAmor
            // 
            this.logoAmorAmor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logoAmorAmor.Image = ((System.Drawing.Image)(resources.GetObject("logoAmorAmor.Image")));
            this.logoAmorAmor.Location = new System.Drawing.Point(15, 12);
            this.logoAmorAmor.Name = "logoAmorAmor";
            this.logoAmorAmor.Size = new System.Drawing.Size(474, 92);
            this.logoAmorAmor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoAmorAmor.TabIndex = 5;
            this.logoAmorAmor.TabStop = false;
            this.logoAmorAmor.Click += new System.EventHandler(this.logoAmorAmor_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(377, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Insira o nome ou código do produto que deseja consultar:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 178);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(163, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Informações do produto:";
            // 
            // zplPreviewPictureBox
            // 
            this.zplPreviewPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.zplPreviewPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.zplPreviewPictureBox.Location = new System.Drawing.Point(370, 195);
            this.zplPreviewPictureBox.Name = "zplPreviewPictureBox";
            this.zplPreviewPictureBox.Size = new System.Drawing.Size(200, 350);
            this.zplPreviewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.zplPreviewPictureBox.TabIndex = 7;
            this.zplPreviewPictureBox.TabStop = false;
            // 
            // tagLogo
            // 
            this.tagLogo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.tagLogo.Image = ((System.Drawing.Image)(resources.GetObject("tagLogo.Image")));
            this.tagLogo.Location = new System.Drawing.Point(377, 202);
            this.tagLogo.Name = "tagLogo";
            this.tagLogo.Size = new System.Drawing.Size(186, 45);
            this.tagLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.tagLogo.TabIndex = 8;
            this.tagLogo.TabStop = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 619);
            this.Controls.Add(this.tagLogo);
            this.Controls.Add(this.zplPreviewPictureBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.logoAmorAmor);
            this.Controls.Add(this.productInfoLabel);
            this.Controls.Add(this.optionsButton);
            this.Controls.Add(this.printButton);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.productSearchTextBox);
            this.Icon = global::Amor_Amor_Impressão_de_Tag.Properties.Resources.transparente_4x;
            this.MinimumSize = new System.Drawing.Size(600, 658);
            this.Name = "MainWindow";
            this.Text = "Impressão de tags Amor Amor";
            ((System.ComponentModel.ISupportInitialize)(this.logoAmorAmor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zplPreviewPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tagLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void productSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            searchButton.Enabled = !string.IsNullOrWhiteSpace(productSearchTextBox.Text);
            MainWindow_Restart();
        }



        private async void SearchButton_Click(object sender, EventArgs e)
        {
            MainWindow_Restart();

            string productSearchText = productSearchTextBox.Text;

            apiToken = Settings.ApiToken;

            string formato = "json";

            var client = new HttpClient();

            try
            {
                // First API request
                string url = "https://api.tiny.com.br/api2/produtos.pesquisa.php";
                var content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("token", apiToken),
            new KeyValuePair<string, string>("formato", formato),
            new KeyValuePair<string, string>("pesquisa", productSearchText)
        });

                var response = await client.PostAsync(url, content);

                // Check for server errors
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(jsonString);

                // Check if a valid product was returned
                if (jsonObject["retorno"]["produtos"]?.Any() != true)
                {
                    printButton.Enabled = false;

                    JToken errorsToken = jsonObject["retorno"]["erros"];
                    if (errorsToken != null && errorsToken.Any())
                    {
                        string errorCode = errorsToken[0]?.ToString();
                        if (errorCode != null)
                        {
                            productInfoLabel.Text = "Erro:" + errorCode;
                        }
                        else
                        {
                            productInfoLabel.Text = "Erro desconhecido";
                        }
                    }
                    else
                    {
                        productInfoLabel.Text = "Erro desconhecido";
                    }
                    return;
                }

                string productId = jsonObject["retorno"]["produtos"][0]["produto"]["id"].ToString();

                // Second API request
                url = "https://api.tiny.com.br/api2/produto.obter.php";
                content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("token", apiToken),
            new KeyValuePair<string, string>("id", productId),
            new KeyValuePair<string, string>("formato", formato)
        });

                response = await client.PostAsync(url, content);

                // Check for server errors
                response.EnsureSuccessStatusCode();

                jsonString = await response.Content.ReadAsStringAsync();
                jsonObject = JObject.Parse(jsonString);

                var produto = jsonObject["retorno"]["produto"];

                var nome = produto["nome"].ToString();
                var ncm = produto["ncm"].ToString();
                var codigo_fornecedor = produto["codigo_pelo_fornecedor"].ToString();
                var marca = produto["marca"].ToString();
                var gtin = produto["gtin"].ToString();
                var codigo = produto["codigo"].ToString();
                string idPai = produto["idProdutoPai"].ToString();
                var preco = Decimal.Parse(produto["preco"].ToString()).ToString("F2");

                var tipoVariacao = "";

                // Check if the product has variations
                if (produto["grade"] != null)
                {
                    tipoVariacao = produto["grade"].ToString();
                    // Parse the JSON object
                    JObject variacaoObject = JObject.Parse(tipoVariacao);

                    // Format the object into the desired string format
                    tipoVariacao = "";
                    foreach (var pair in variacaoObject)
                    {
                        tipoVariacao += $"{pair.Key}: {pair.Value} || ";
                    }
                    // Remove the last " || " from the string
                    tipoVariacao = tipoVariacao.TrimEnd(' ', '|');
                }

                var codigoPai = "";

                if (produto["idProdutoPai"] != null) // Check if idProdutoPai exists and is not null or empty
                {
                    // Third API request
                    url = "https://api.tiny.com.br/api2/produto.obter.php";
                    content = new FormUrlEncodedContent(new[]
                    {
        new KeyValuePair<string, string>("token", apiToken),
        new KeyValuePair<string, string>("id", idPai),
        new KeyValuePair<string, string>("formato", formato)
    });
                    response = await client.PostAsync(url, content);
                    jsonString = await response.Content.ReadAsStringAsync();
                    jsonObject = JObject.Parse(jsonString);
                    if (jsonObject["retorno"]["produto"] != null)
                    {
                        codigoPai = jsonObject["retorno"]["produto"]["codigo"].ToString();
                    }
                }

                // Format the product information
                string productInfo = $"Nome: {nome}\n";

                if (!string.IsNullOrEmpty(tipoVariacao.ToString()))
                {
                    productInfo += $"Variação: {tipoVariacao}\n";
                }

                productInfo += $"Marca: {marca}\n\n";

                if (!string.IsNullOrEmpty(gtin))
                {
                    productInfo += $"GTIN: {gtin}\n";
                }

                if (!string.IsNullOrEmpty(codigo))
                {
                    productInfo += $"SKU: {codigo}\n\n";
                }

                if (!string.IsNullOrEmpty(codigoPai))
                {
                    productInfo += $"SKU do produto pai: {codigoPai}\n";
                }

                if (!string.IsNullOrEmpty(codigo_fornecedor))
                {
                    productInfo += $"Codigo do fornecedor: {codigo_fornecedor}\n";
                }

                if (!string.IsNullOrEmpty(ncm))
                {
                    productInfo += $"NCM: {ncm}\n\n";
                }

                if (!string.IsNullOrEmpty(preco))
                {
                    productInfo += $"Preço: {preco}";
                }

                // Display the product information
                productInfoLabel.Text = productInfo;

                // Your original ZPL XML goes here
                zpl = @"
CT~~CD,~CC^~CT~
^XA
^DFR:Etiqueta.ZPL^FS
~TA000
~JSN
^LT0
^MNW
^MTT
^PON
^PMN
^LH0,0
^JMA
^PR6,6
~SD10
^JUS
^LRN
^CI27
^PA0,1,1,0
^MMT
^PW575
^LL320
^LS0
^FPH,3^FT245,16^A0R,20,20^FH\^CI28^FD{0}^FS^CI27
^FT22,16^AFR,52,26^FH\^FD{1}^FS
^FPH^FT245,170^A0R,14,15^FB575,1,1^FH\^CI28^FD{2}^FS^CI27
^FPH,1^FT315,16^ACR,18,10^FH\^FD{3}^FS
^FT291,16^A0R,18,18^FH\^CI28^FD{4}^FS^CI27
^FT269,16^A0R,17,18^FH\^CI28^FD{5}^FS^CI27
^BY1,3,45^FT196,16^BCR,,Y,N
^FD{6}^FS
^BY2,3,78^FT102,16^BCR,,Y,N,,A
^FD{7}^FS
^FO339,16^A0R,31,30^FB288,3,8,L^FH\^CI28^FD{8}^FS^CI27
^XZ
^XA
^XFR:Etiqueta.ZPL^FS
^PQ1,0,1
^XZ";

                // Replace variables in the ZPL
                zpl = string.Format(zpl, ncm, "R$" + preco, codigo_fornecedor, codigoPai, tipoVariacao, marca, codigo, gtin, nome);

                // Print the final ZPL string to the console for debugging
                Console.WriteLine(zpl);
                printButton.Enabled = true;

                // Convert ZPL string to bytes
                var zplBytes = System.Text.Encoding.UTF8.GetBytes(zpl);

                // Allocate some unmanaged memory for those bytes.
                IntPtr pUnmanagedBytes = Marshal.AllocCoTaskMem(zplBytes.Length);

                // Copy the managed byte array into the unmanaged array.
                Marshal.Copy(zplBytes, 0, pUnmanagedBytes, zplBytes.Length);

                try
                {
                    // Get the ZPL preview
                    Image zplPreview = await GetZplPreview(zpl);

                    // Display the preview in a PictureBox control
                    zplPreviewPictureBox.Image = zplPreview;
                }
                catch (HttpRequestException ex)
                {
                    // Handle network errors
                    productInfoLabel.Text = "Erro de rede. Por favor, verifique sua conexão com a internet.";
                    Console.Error.WriteLine($"Network error: {ex.Message}");
                    this.printButton.Enabled = false;
                }

            }
            catch (HttpRequestException ex)
            {
                // Handle network errors
                productInfoLabel.Text = "Erro de rede. Por favor, verifique sua conexão com a internet.";
                Console.Error.WriteLine($"Network error: {ex.Message}");
                this.printButton.Enabled = false;
            }
            catch (JsonReaderException ex)
            {
                // Handle JSON parsing errors
                productInfoLabel.Text = "Erro ao processar os dados do produto. Por favor, tente novamente mais tarde.";
                Console.Error.WriteLine($"JSON parsing error: {ex.Message}");
                this.printButton.Enabled = false;
            }
            catch (Exception ex)
            {
                // Handle other unexpected errors
                productInfoLabel.Text = "Um erro inesperado ocorreu. Por favor, tente novamente mais tarde.";
                Console.Error.WriteLine($"Unexpected error: {ex.Message}");
                this.printButton.Enabled = false;
            }


        }

        private void ProductSearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(productSearchTextBox.Text))
            {
                SearchButton_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            // Check if product information is available and ZPL label has been updated
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                // Convert ZPL string to bytes
                var zplBytes = System.Text.Encoding.UTF8.GetBytes(zpl);

                // Allocate some unmanaged memory for those bytes.
                IntPtr pUnmanagedBytes = Marshal.AllocCoTaskMem(zplBytes.Length);
                // Copy the managed byte array into the unmanaged array.
                Marshal.Copy(zplBytes, 0, pUnmanagedBytes, zplBytes.Length);

                // No need to redeclare printDialog, just use the existing one
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    string printerName = printDialog.PrinterSettings.PrinterName;

                    // Send the unmanaged bytes to the printer.
                    SendBytesToPrinter(printerName, pUnmanagedBytes, zplBytes.Length);

                    // Free the unmanaged memory that you allocated earlier.
                    Marshal.FreeCoTaskMem(pUnmanagedBytes);
                }
            }
            else
            {
                MessageBox.Show("Por favor, realize uma busca válida e atualize a etiqueta ZPL antes de imprimir.");
            }
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            var optionForm = new OptionForm(apiToken);
            var result = optionForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                apiToken = optionForm.ApiToken;
            }
        }

        public async Task<Image> GetZplPreview(string zpl)
        {
            using (var client = new HttpClient())
            {
                string url = $"http://api.labelary.com/v1/printers/8dpmm/labels/2.756x1.575/0/{Uri.EscapeDataString(zpl)}";

                // Add the X-Rotation header
                client.DefaultRequestHeaders.Add("X-Rotation", "270");

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    return Image.FromStream(stream);
                }
            }
        }

        private void MainWindow_Restart()
        {
            productInfoLabel.Text = "";
            zplPreviewPictureBox.Image = null;
            this.printButton.Enabled = false;
        }

        private void logoAmorAmor_Click(object sender, EventArgs e)
        {

        }


    }
}

