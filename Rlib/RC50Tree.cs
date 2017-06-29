using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Rlib
{
    /// <summary>
    /// C50树类
    /// </summary>
    public class RC50Tree
    {
        /// <summary>
        /// 树字符串
        /// </summary>
        private string _dstree;
        /// <summary>
        /// 树深度
        /// </summary>
        private int _treesize;

        /// <summary>
        /// 因素贡献度
        /// </summary>
        private DataTable _dtFactors;

        /// <summary>
        /// 决策树表
        /// </summary>
        private DataTable _dtDstree;

        public RC50Tree()
        {
            setDtDstreeStruct();
            setDtFactorsStruct();
        }

        public RC50Tree(string strtree, string ModGUID, string strfactors, int isize, DataTable dtcnz, DataTable dtpy, string strFormula)
        {
            Treesize = isize;
            setDtFactors(ModGUID, strfactors, dtcnz);
            setDtDstree(strtree, ModGUID, dtcnz, dtpy, strFormula);
        }

        /// <summary>
        /// 设置决策支持树dt结构
        /// </summary>
        public void setDtDstreeStruct()
        {
            DtDstree = new DataTable();
            DtDstree.Columns.Add("DSTreeGUID", typeof(System.Guid));
            DtDstree.Columns.Add("ModGUID", typeof(System.Guid));
            DtDstree.Columns.Add("ID");
            DtDstree.Columns.Add("PID");
            DtDstree.Columns.Add("FactorName");
            DtDstree.Columns.Add("FactorNameCn");
            DtDstree.Columns.Add("Operator");
            DtDstree.Columns.Add("OperatorCn");
            DtDstree.Columns.Add("FactorValue");
            DtDstree.Columns.Add("FactorValueCn");
            DtDstree.Columns.Add("Describe");
            DtDstree.Columns.Add("DescribeCn");
            DtDstree.Columns.Add("Result");
            DtDstree.Columns.Add("ResultCn");
            DtDstree.Columns.Add("CoverCount");
            DtDstree.Columns.Add("ErrorCount");

        }

        /// <summary>
        /// 根据决策树字符串生成决策树表
        /// </summary>
        public void setDtDstree(string strtree, string ModGUID, DataTable dtcnz, DataTable dtpy, string strFormula)
        {
            setDtDstreeStruct();
            strtree = strtree.Replace(":...", "    ").Replace(":   ", "    ");
            int[] icj = new int[Treesize];
            for (int i = 0; i < Treesize; i++)
            {
                icj[i] = 0;
            }
            List<string> arrtree = new List<string>();
            bool bflag = false;
            int iflag = -1;
            string[] arrtreetemp = strtree.Split(new string[] { "\n" }, StringSplitOptions.None);
            int ir = arrtreetemp.Length;
            if (ir == 0 || ir == 1)
            {
                return;
            }
            for (int i = 0; i < ir; i++)
            {
                if (bflag)
                {
                    arrtree[iflag] += arrtreetemp[i].Trim();
                }
                else
                {
                    arrtree.Add(arrtreetemp[i]);
                    iflag++;
                }

                if (arrtreetemp[i].IndexOf("in {") > 0)
                {
                    bflag = true;
                }
                if (arrtreetemp[i].IndexOf("}:") > 0 && bflag)
                {
                    bflag = false;
                }
            }
            ir = arrtree.Count;
            for (int i = 0; i < ir; i++)
            {
                int iSpaceCount = 0;
                int icjcount = 0;
                string strid = "";
                string strpid = "";
                string strFactorName = "";
                string strFactorNameCn = "";
                string strOperatorCn = "";
                string strOperator = "";
                string strFactorValue = "";
                string strFactorValueCn = "";
                string strDescribe = "";
                string strDescribeCn = "";
                string strResult = "";
                string strResultCn = "";
                int iCoverCount = 0;
                int iErroCount = 0;
                ///通过计算有多少个4个空格来获取层级
                iSpaceCount = getSpaceCount(arrtree[i]);
                icjcount = iSpaceCount / 4;
                ///层级对应id自增长
                icj[icjcount]++;
                for (int j = 0; j < icjcount + 1; j++)
                {
                    strid += "." + icj[j];
                    if (j < icjcount)
                    {
                        strpid += "." + icj[j];
                    }
                }
                strid = strid.Substring(1);
                if (strpid != "")
                {
                    strpid = strpid.Substring(1);
                }

                arrtree[i] = arrtree[i].Substring(iSpaceCount);
                ///分割结果和规则
                string[] arrstr1 = arrtree[i].Split(new string[] { ":" }, StringSplitOptions.None);
                arrstr1[0] = arrstr1[0].Trim();

                strDescribe = arrstr1[0];
                ///如果为末级则设置结果和覆盖率和错误率
                if (arrstr1.Length == 2)
                {
                    if (!string.IsNullOrEmpty(arrstr1[1]))
                    {
                        arrstr1[1] = arrstr1[1].Trim();
                        string[] arrstr2 = arrstr1[1].Split(' ');
                        strResult = arrstr2[0];
                        strResultCn = dtpy.Select("cn='" + strFormula + "' AND cvpy='" + arrstr2[0] + "'")[0][1].ToString();
                        arrstr2[1] = arrstr2[1].Replace("(", "").Replace(")", "");
                        string[] arrstr3 = arrstr2[1].Split('/');
                        iCoverCount = int.Parse(arrstr3[0]);
                        if (arrstr3.Length == 2)
                        {
                            iErroCount = int.Parse(arrstr3[1]);
                        }
                        else
                        {
                            iErroCount = 0;
                        }
                    }
                }
                string[] arrstr4 = arrstr1[0].Split(' ');
                ///设置因素
                strFactorName = arrstr4[0];
                strFactorNameCn = dtcnz.Select("cn='" + arrstr4[0] + "'")[0][1].ToString();
                //设置操作符
                strOperator = arrstr4[1];
                strOperatorCn = arrstr4[1];
                if (arrstr4[1] == "in")
                {
                    strOperatorCn = "属于";
                }
                strFactorValue = arrstr4[2];
                if (isNumberic(arrstr4[2]))
                {
                    strFactorValueCn = arrstr4[2];
                }
                else
                {
                    ///处理因素集合的中文显示
                    if (arrstr4[2].IndexOf('{') > -1)
                    {
                        string strfc = arrstr4[2].Replace("{", "").Replace("}", "");
                        string strt = "";
                        string[] arrfc = strfc.Split(',');
                        int ifcr = arrfc.Length;
                        strFactorValueCn = "(";
                        for (int j = 0; j < ifcr; j++)
                        {
                            strt += "," + dtpy.Select("cn='" + strFactorName + "' AND cvpy='" + arrfc[j] + "'")[0][1].ToString();
                        }
                        strFactorValueCn += strt.Substring(1) + ")";
                    }
                    else
                    {
                        strFactorValueCn += dtpy.Select("cn='" + strFactorName + "' AND cvpy='" + arrstr4[2] + "'")[0][1].ToString();
                    }
                }
                strDescribeCn = strFactorNameCn + " " + strOperatorCn + " " + strFactorValueCn;
                if(strResultCn !="" && iCoverCount==0 )
                {
                    continue;
                }
                DtDstree.Rows.Add(Guid.NewGuid().ToString(), ModGUID, strid, strpid, strFactorName, strFactorNameCn, strOperator, strOperatorCn, strFactorValue, strFactorValueCn, strDescribe, strDescribeCn, strResult, strResultCn, iCoverCount, iErroCount);
            }
        }

        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected bool isNumberic(string message)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(message, @"^[+-]?\d*[.]?\d*$");
        }



        /// <summary>
        /// 获得字符串前面的空格数量
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int getSpaceCount(string str)
        {
            int iSpace, iLength;
            iLength = str.Length;
            iSpace = 0;
            for (int i = 0; i < iLength; i++)
            {
                if (str.Substring(i, 1) == " ")
                {
                    iSpace++;
                }
                else
                {
                    break;
                }
            }

            return iSpace;
        }

        /// <summary>
        /// 设置因素dt结构
        /// </summary>
        public void setDtFactorsStruct()
        {
            DtFactors = new DataTable();
            ///因素GUID
            DtFactors.Columns.Add("FactorGUID", typeof(System.Guid));
            ///模型GUID
            DtFactors.Columns.Add("ModGUID", typeof(System.Guid));
            ///因素名称
            DtFactors.Columns.Add("Factorname");
            ///因素中文名称
            DtFactors.Columns.Add("FactornameCn");
            ///贡献度
            DtFactors.Columns.Add("Useage");
        }
        /// <summary>
        /// 根据因素字符串生成因素表
        /// </summary>
        public void setDtFactors(string ModGUID, string strfactors, DataTable dtcnz)
        {
            setDtFactorsStruct();
            string[] arrfactors = strfactors.Split(new string[] { "\n\t" }, StringSplitOptions.None);
            int ir = arrfactors.Length;
            if (ir > 0)
            {
                for (int i = 0; i < ir; i++)
                {
                    string[] strtemp = arrfactors[i].Split(new string[] { "\t" }, StringSplitOptions.None);
                    DtFactors.Rows.Add(Guid.NewGuid().ToString(), ModGUID, strtemp[1], dtcnz.Select("cn='" + strtemp[1] + "'")[0][1], strtemp[0].Replace(@"%", ""));
                }
            }
        }

        public string Dstree
        {
            get
            {
                return _dstree;
            }

            set
            {
                _dstree = value;
            }
        }

        public int Treesize
        {
            get
            {
                return _treesize;
            }

            set
            {
                _treesize = value;
            }
        }

        public DataTable DtFactors
        {
            get
            {
                return _dtFactors;
            }

            set
            {
                _dtFactors = value;
            }
        }

        public DataTable DtDstree
        {
            get
            {
                return _dtDstree;
            }

            set
            {
                _dtDstree = value;
            }
        }
    }
}
