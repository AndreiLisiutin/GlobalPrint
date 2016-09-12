using BitMiracle.LibTiff.Classic;
using System.IO;

namespace GlobalPrint.Infrastructure.FileUtility
{
    /// <summary> Class working with Tiff picture files.
    /// </summary>
    public class TiffFileReader : IFileReader
    {
        /// <summary> Get number of pages in file.
        /// </summary>
        /// <param name="file">File.</param>
        /// <returns>Number of pages in the file.</returns>
        public int GetPagesCount(byte[] file)
        {
            var tmpFile = Path.GetTempFileName();
            using (var tmpFileStream = File.OpenWrite(tmpFile))
            {
                try
                {
                    tmpFileStream.Write(file, 0, file.Length);
                    tmpFileStream.Close();

                    using (Tiff image = Tiff.Open(tmpFile, "r"))
                    {
                        return CalculatePageNumber(image);
                    }
                }
                finally
                {
                    new FileInfo(tmpFile).Delete();
                }
            }
        }

        private static int CalculatePageNumber(Tiff image)
        {
            int pageCount = 0;
            do
            {
                ++pageCount;
            }
            while (image.ReadDirectory());

            return pageCount;
        }
    }
}
