using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Amor_Amor_Impressão_de_Tag
{
    public static class Settings
    {
        private static string _filePath = "settings.txt";

        public static string ApiToken
        {
            get
            {
                if (File.Exists(_filePath))
                {
                    return File.ReadAllText(_filePath);
                }
                return string.Empty;
            }
            set
            {
                File.WriteAllText(_filePath, value);
            }
        }
    }


}
