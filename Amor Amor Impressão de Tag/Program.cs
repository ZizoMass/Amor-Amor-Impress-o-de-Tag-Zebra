using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

public class Program
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

    #region InvokeDeclarations
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
    #endregion

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

    public static bool SendFileToPrinter(string szPrinterName, string szFileName)
    {
        // Open the file.
        FileStream fs = new FileStream(szFileName, FileMode.Open);
        // Create a BinaryReader on the file.
        BinaryReader br = new BinaryReader(fs);
        // Dim an array of bytes big enough to hold the file's contents.
        Byte[] bytes = new Byte[fs.Length];
        bool bSuccess = false;
        // Your unmanaged pointer.
        IntPtr pUnmanagedBytes = new IntPtr(0);
        int nLength;

        nLength = Convert.ToInt32(fs.Length);
        // Read the contents of the file into the array.
        bytes = br.ReadBytes(nLength);
        // Allocate some unmanaged memory for those bytes.
        pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
        // Copy the managed byte array into the unmanaged array.
        Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
        // Send the unmanaged bytes to the printer.
        bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
        // Free the unmanaged memory that you allocated earlier.
        Marshal.FreeCoTaskMem(pUnmanagedBytes);
        return bSuccess;
    }

    public static async Task Main(string[] args)
    {
        string apiToken = "17c44d323317a17a530e59bf3d67cf8ba282e746";
        string formato = "json";

        Console.Write("Insira o SKU do produto: ");
        string pesquisa = Console.ReadLine();

        var client = new HttpClient();

        // First API request
        string url = "https://api.tiny.com.br/api2/produtos.pesquisa.php";
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("token", apiToken),
            new KeyValuePair<string, string>("formato", formato),
            new KeyValuePair<string, string>("pesquisa", pesquisa)
        });
        var response = await client.PostAsync(url, content);
        var jsonString = await response.Content.ReadAsStringAsync();
        var jsonObject = JObject.Parse(jsonString);

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
        jsonString = await response.Content.ReadAsStringAsync();
        //Console.WriteLine("DEBUG:" + jsonString);  // add this line
        jsonObject = JObject.Parse(jsonString);

        var nome = jsonObject["retorno"]["produto"]["nome"].ToString();
        var ncm = jsonObject["retorno"]["produto"]["ncm"].ToString();
        var preco = jsonObject["retorno"]["produto"]["preco"].ToString();
        var codigo_fornecedor = jsonObject["retorno"]["produto"]["codigo_pelo_fornecedor"].ToString();
        var marca = jsonObject["retorno"]["produto"]["marca"].ToString();
        var gtin = jsonObject["retorno"]["produto"]["gtin"].ToString();
        var codigo = jsonObject["retorno"]["produto"]["codigo"].ToString();
        var idPai = jsonObject["retorno"]["produto"]["idProdutoPai"].ToString();
        var tipoVariacao = jsonObject["retorno"]["produto"]["grade"].ToString();

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

        var codigoPai = jsonObject["retorno"]["produto"]["codigo"].ToString();

        Console.WriteLine("DEBUG:" + " Nome: " + nome + " NCM: " + ncm + " Preço: " + preco + " Cod Fornecedor: " + codigo_fornecedor + " Variação: " + tipoVariacao + " Marca: " + marca + " GTIN: " + gtin + " SKU: " + codigo + " SKU Pai: " + codigoPai);  // add this line


        // Your original ZPL XML goes here
        var zpl = @"
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
        zpl = string.Format(zpl, ncm, "R$"+preco, codigo_fornecedor, codigoPai, tipoVariacao, marca, codigo, gtin, nome);

        // Print the final ZPL string to the console for debugging
        Console.WriteLine(zpl);

        // Convert ZPL string to bytes
        var zplBytes = System.Text.Encoding.UTF8.GetBytes(zpl);

        // Allocate some unmanaged memory for those bytes.
        IntPtr pUnmanagedBytes = Marshal.AllocCoTaskMem(zplBytes.Length);
        // Copy the managed byte array into the unmanaged array.
        Marshal.Copy(zplBytes, 0, pUnmanagedBytes, zplBytes.Length);

        PrintDialog printDialog = new PrintDialog();
        if (printDialog.ShowDialog() == DialogResult.OK)
        {
            string printerName = printDialog.PrinterSettings.PrinterName;

            // Send the unmanaged bytes to the printer.
            SendBytesToPrinter(printerName, pUnmanagedBytes, zplBytes.Length);

            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
        }
    }
}
