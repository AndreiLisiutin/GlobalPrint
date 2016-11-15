using GlobalPrint.Infrastructure.FileUtility.FileExporters.Exportable;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GlobalPrint.Infrastructure.FileUtility.FileExporters
{

    /// <summary>
    /// Excel format register exporter. 
    /// Every object in list lays on the new row and its properties are placed in separated cells.
    /// </summary>
    public class ExcelExporter : IFileExporter
    {
        public ExcelExporter()
        {
        }

        /// <summary>
        /// Exports List of objects into Excel format and serializes it.
        /// </summary>
        /// <param name="exportable">List of objects to export.</param>
        /// <returns>Serialized Excel file of register.</returns>
        public byte[] ExportToMemory(IEnumerable<ExportableEntity> exportable)
        {
            Application exApp = new Application();
            exApp.DisplayAlerts = false;
            Workbook workbook = exApp.Workbooks.Add();
            try
            {
                ExportableEntity firstEntity = exportable.FirstOrDefault();
                if (firstEntity == null)
                {
                    return this._SerializeWorkbook(workbook);
                }

                Worksheet workSheet = (Worksheet)exApp.ActiveSheet;

                int column = 1;
                foreach (var property in firstEntity.Properties)
                {
                    workSheet.Cells[1, column] = property.PropertyName;
                    column++;
                }


                int row = 2;
                foreach (var entity in exportable)
                {
                    column = 1;
                    foreach (var property in entity.Properties)
                    {
                        workSheet.Cells[row, column] = property.Value;
                        switch (property.Format)
                        {
                            case ExportFormatting.Date:
                                workSheet.Cells[row, column].NumberFormat = "dd.MM.yyyy HH:mm:ss";
                                break;
                            //case ExportFormatting.Money:
                            //    workSheet.Cells[row, column].NumberFormat = "0.00";
                            //    break;
                            case ExportFormatting.Number:
                                workSheet.Cells[row, column].NumberFormat = "#";
                                break;
                            case ExportFormatting.Text:
                                workSheet.Cells[row, column].NumberFormat = "@";
                                break;
                        }
                        column++;
                    }
                    row++;
                }

                workSheet.Columns.AutoFit();
                return this._SerializeWorkbook(workbook);

            }
            finally
            {
                exApp.Quit();
                this._ReleaseObject(workbook);
                this._ReleaseObject(exApp);
            }
        }

        /// <summary>
        /// Serialize Excel book.
        /// </summary>
        /// <param name="workbook">Excel book.</param>
        /// <returns>Serialized Excel book.</returns>
        private byte[] _SerializeWorkbook(Workbook workbook)
        {
            string path = Path.GetTempFileName();
            object missin = Missing.Value;
            try
            {
                workbook.SaveAs(path, XlFileFormat.xlWorkbookDefault, missin, missin, false, false, XlSaveAsAccessMode.xlNoChange, missin, missin, missin, missin, missin);
                workbook.Close(true, missin, missin);
                return File.ReadAllBytes(path);
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        /// <summary>
        /// Release managed Excel resource.
        /// </summary>
        /// <param name="obj">Resource.</param>
        private void _ReleaseObject(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
        }

        /// <summary>
        /// Determines file extension for the format.
        /// </summary>
        /// <returns>String file extension.</returns>
        public string GetFileExtension()
        {
            return "xlsx";
        }
    }
}
