using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Loglib;
using RDotNet;

namespace Rlib
{
    /// <summary>
    /// C50控制器
    /// </summary>
    public class RC50Control
    {
        private string _subset;
        private string _bands;
        private string _winnow;
        private string _noGlobalPruning;
        /// <summary>
        /// 最小置信度
        /// </summary>
        private double _CF;
        /// <summary>
        /// 最小数量
        /// </summary>
        private int _minCases;
        private string _fuzzyThresHold;
        private int _sample;
        private int _seed;
        private string _earlyStopping;
        private string _label;
        private string _ctName;

        public double CF
        {
            get
            {
                return _CF;
            }

            set
            {
                _CF = value;
            }
        }

        public int MinCases
        {
            get
            {
                return _minCases;
            }

            set
            {
                _minCases = value;
            }
        }

        public string CtName
        {
            get
            {
                return _ctName;
            }

            set
            {
                _ctName = value;
            }
        }

        public RC50Control(double dcf = 0.25, int imc = 10, string strCtName = "tc")
        {
            CF = dcf;
            MinCases = imc;
            CtName = strCtName;
        }


        /// <summary>
        /// 获得C5.0control格式：tc<-C5.0Control(CF=0.01,minCases =1)
        /// </summary>
        /// <returns></returns>
        public string getCtString()
        {
            if (CF == 0 && MinCases == 0)
            {
                return "";
            }
            string str1 = "";
            str1 = CtName + "<-C5.0Control(";
            if (CF != 0)
            {
                str1 += "CF=" + CF + ",";
            }
            if (MinCases != 0)
            {
                str1 += "minCases=" + MinCases;
            }
            str1 += ")";
            return str1;
        }
    }

    /// <summary>
    /// C50算法类
    /// </summary>
    public class RC50 : RClass
    {
        /// <summary>
        /// 模型名称
        /// </summary>
        private string _c5Name;
        /// <summary>
        /// 迭代次数次数
        /// </summary>
        private int _trials;
        /// <summary>
        /// 是否以规则形式输出
        /// </summary>
        private string _rules;
        /// <summary>
        /// 特定权重
        /// </summary>
        private string _weights;
        /// <summary>
        /// 损失矩阵
        /// </summary>
        private string _costs;
        /// <summary>
        /// 结果字段
        /// </summary>
        private string _formula;
        /// <summary>
        /// 数据集名称
        /// </summary>
        private string _data;
        /// <summary>
        /// 缺省值动作
        /// </summary>
        private string _naaction;
        /// <summary>
        /// c50控制器
        /// </summary>
        private RC50Control _rct;

        public string Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }

        public string Formula
        {
            get
            {
                return _formula;
            }

            set
            {
                _formula = value;
            }
        }

        public int Trials
        {
            get
            {
                return _trials;
            }

            set
            {
                _trials = value;
            }
        }

        public RC50Control Rct
        {
            get
            {
                return _rct;
            }

            set
            {
                _rct = value;
            }
        }

        public string C5Name
        {
            get
            {
                return _c5Name;
            }

            set
            {
                _c5Name = value;
            }
        }

        public string Rules
        {
            get
            {
                return _rules;
            }

            set
            {
                _rules = value;
            }
        }

        public RC50(string strData, string strFormula, int imc = 10, string strC5Name = "module", string strRules = "F", int itrials = 10, double dcf = 0.25, string strCtName = "ct")
        {
            Data = strData;
            Formula = strFormula;
            Trials = itrials;
            C5Name = strC5Name;
            Rules = strRules;
            Rct = new RC50Control(dcf, imc, strCtName);
            EvaluateByR("library(C50)");
        }

        /// <summary>
        /// 获得C50字符串
        /// </summary>
        /// <returns></returns>
        public string getC50String()
        {
            string strct = Rct.getCtString();
            string strC50 = "";
            if (Data == "" && Formula == "")
            {
                return strC50;
            }
            if (strct == "")
            {
                strC50 = C5Name + "<- C5.0(formula=" + Formula + " ~ ., data = " + Data + ",rules=" + Rules + ")";
            }
            else
            {
                strC50 = C5Name + "<- C5.0(formula=" + Formula + " ~ ., data = " + Data + ",rules=" + Rules + ",control =" + Rct.CtName + ")";
            }
            return strC50;
        }

        /// <summary>
        /// 获得决策支持树
        /// </summary>
        /// <param name="ModGUID">模型GUID</param>
        /// <param name="dtcnz">字段中文对照表</param>
        /// <param name="dtpy">内容拼音对照表</param>
        /// <returns></returns>
        public RC50Tree getC50Tree(string ModGUID, DataTable dtcnz, DataTable dtpy)
        {
            try
            {
                string strResult = "";
                int iResult = -1;
                string strP = "";
                string strC50 = getC50String();
                strP = Rct.getCtString() + "\r\n";
                strP += strC50 + "\r\n";
                string strR = "";
                if (!string.IsNullOrEmpty(strP))
                {
                    if (EvaluateByR(strP))
                    {
                        strR = C5Name + "$output";
                        strResult = getStringByR(strR);
                        strR = C5Name + "$size";
                        iResult = getIntByR(strR);
                        if (!string.IsNullOrEmpty(strResult) && iResult != -1 && iResult != 1)
                        {
                            string strtree = getSpecifiedString(strResult, "tree");
                            string strfactors = getSpecifiedString(strResult, "factors");
                            RC50Tree Rctree = new RC50Tree(strtree, ModGUID, strfactors, iResult, dtcnz, dtpy, Formula);
                            return Rctree;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
            return null;
        }

        /// <summary>
        /// 截取特定字符串字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string getSpecifiedString(string str, string sType)
        {
            string strIndex1 = "";
            string strIndex2 = "";
            int istart, iLength;
            switch (sType)
            {
                ///获取树字符串
                case "tree":
                    strIndex1 = "tree:\n\n";
                    strIndex2 = "\n\n\nEvaluation";
                    break;
                ///获取因素贡献度字符串
                case "factors":
                    strIndex1 = "usage:\n\n\t";
                    strIndex2 = "\n\n\nTime:";
                    break;
            }
            istart = str.IndexOf(strIndex1) + strIndex1.Length;
            iLength = str.IndexOf(strIndex2) - istart;
            return str.Substring(istart, iLength);
        }
    }
}
