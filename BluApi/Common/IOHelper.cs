using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluApi.Common
{
    public static class IOHelper
    {
        public static void EmptyFolder(this string folderName)
        {
            DirectoryInfo dir = new DirectoryInfo(folderName);

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
            else
            {
                foreach (FileInfo fi in dir.GetFiles())
                {
                    fi.Delete();
                }

                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    EmptyFolder(di.FullName);
                    di.Delete();
                }
            }
        }

    }
}
