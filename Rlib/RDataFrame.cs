using System.Data;
using Pylib;

namespace Rlib
{
    /// <summary>
    /// R环境中DataFrame对象C#中适配
    /// </summary>
    public class RDataFrame
    {
        /// <summary>
        /// R环境中dataFrame名称
        /// </summary>
        private string _dfName;
        /// <summary>
        /// dt转换dataFrame的R脚本，格式：data<-data.frame(a=c('1','2','3'),b=c(1,2,3))
        /// </summary>
        private string _dfR;


        public RDataFrame(string strDfname = "dt")
        {
            DfName = strDfname;
        }

        /// <summary>
        /// 将datatable转换成rdataframe对象
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public virtual void setDataFrameInRByDt(DataTable dt)
        {
            string str1, str2, str3;
            ///dt列数
            int ic = dt.Columns.Count;
            int ir = dt.Rows.Count;
            if (ir > 0)
            {
                str1 = DfName + "<-data.frame(";
                str2 = "";
                for (int i = 0; i < ic; i++)
                {
                    string strcn = dt.Columns[i].ColumnName;
                    str2 += @"," + strcn + "=c(";
                    str3 = "";
                    for (int j = 0; j < ir; j++)
                    {
                        string strcvz = dt.Rows[j][i].ToString();
                        if (dt.Columns[i].DataType.Name == "String")
                        {
                            str3 = @",'" + strcvz + "'";
                        }
                        else
                        {
                            str3 = @"," + strcvz;
                        }
                    }
                    str2 += str3.Substring(1) + ")";
                }
                str1 = str2.Substring(1) + ")";
                DfR = str1;
            }
        }

        public string DfName
        {
            get
            {
                return _dfName;
            }

            set
            {
                _dfName = value;
            }
        }
        public string DfR
        {
            get
            {
                return _dfR;
            }

            set
            {
                _dfR = value;
            }
        }
    }

    public class RDataFramePy : RDataFrame
    {
        /// <summary>
        /// 将datatable中的中文转换成拼音
        /// </summary>
        private DataTable _dtPy;

        public RDataFramePy(string strDfname = "dt")
        {
            DfName = strDfname;
            DtPy = new DataTable();
            DtPy.Columns.Add("cn");
            DtPy.Columns.Add("cvz");
            DtPy.Columns.Add("cpy");
        }

        /// <summary>
        /// 将dt中中文转换成拼音形成dataFrame格式
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public override void setDataFrameInRByDt(DataTable dt)
        {
            int ir = dt.Rows.Count;
            int ic = dt.Columns.Count;
            string str1, str2, str3;
            if (ir > 0)
            {
                DtPy = NPy.getPyDt(dt, ic);
                str1 = DfName + "<-data.frame(";
                str2 = "";
                for (int i = 0; i < ic; i++)
                {
                    string strcn = dt.Columns[i].ColumnName;
                    str2 += @"," + strcn + "=c(";
                    str3 = "";
                    for (int j = 0; j < ir; j++)
                    {
                        string strcvz = dt.Rows[j][i].ToString();
                        if (dt.Columns[i].DataType.Name == "String")
                        {
                            string strcvpy = DtPy.Select("cn='" + strcn + "' AND cvz = '" + strcvz + "'")[0][2].ToString();
                            str3 += @",'" + strcvpy + "'";
                        }
                        else
                        {
                            str3 += @"," + strcvz;
                        }
                    }
                    str2 += str3.Substring(1) + ")";
                }
                str1 += str2.Substring(1) + ")";
                DfR = str1;
            }
        }

        public DataTable DtPy
        {
            get
            {
                return _dtPy;
            }

            set
            {
                _dtPy = value;
            }
        }
    }
}
