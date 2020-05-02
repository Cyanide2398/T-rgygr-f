using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ExcelDataReader; // Nuget: ExcelDataReader
using System.Diagnostics;

namespace Szoftverfejlesztes
{
    
    public partial class Form3 : Form
    {
        List<String> yellows = new List<String>();
        public Form3()
        {
            InitializeComponent();
            loadTable();
        }

        public Form3(List<String> _yellows)
        {
            InitializeComponent();
            yellows = _yellows.ToList();
            loadTable();
        }


        DataTableCollection tableCollection;

        private void loadTable()
        {
            //Debug.WriteLine("Example: "+ yellows[0]);
            using (var stream = File.Open(@"..\Debug\rszt_lista.xlsx", FileMode.Open, FileAccess.Read)) // Fajt bele kell rakni a Debugba
            {
                using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()  // Nuget: ExcelDataReader.DataSet
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true,
                            FilterRow = (rowReader) =>
                            {
                                if (yellows.Contains((string)rowReader[0]))
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                        );
                    tableCollection = result.Tables;
                    dataGridView1.DataSource = tableCollection[0];
                }
            }

        }

      
    }
}
