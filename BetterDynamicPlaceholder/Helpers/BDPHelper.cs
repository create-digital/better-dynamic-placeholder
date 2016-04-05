using BetterDynamicPlaceholder.Pipeline;
using Sitecore.Configuration;
using Sitecore.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BetterDynamicPlaceholder.Helpers
{
    class BDPHelper
    {
        private static string placeholderPattern = "_BDP";
        private static List<string> DynamicPlaceholderList
        {
            get
            {
                return (List<string>)HttpContext.Current.Items["DynamicPlaceholders"];
            }
            set
            {
                HttpContext.Current.Items["DynamicPlaceholders"] = value;
            }
        }
        public static HtmlString DynamicPlaceholder(this SitecoreHelper myScHelper, string placeholderName)
        {
            if (DynamicPlaceholderList == null)
            {
                DynamicPlaceholderList = new List<string>();
            }
            string dynamicPlaceholderKey = GetDynamicPlaceholderKey(myScHelper, placeholderName);
            if (!string.IsNullOrWhiteSpace(dynamicPlaceholderKey))
            {
                placeholderName = dynamicPlaceholderKey;
            }
            if (!placeholderName.Contains(placeholderPattern + "_"))
                DynamicPlaceholderList.Add(placeholderName);
            return myScHelper.Placeholder(placeholderName);
        }
        private static string GetDynamicPlaceholderKey(SitecoreHelper myScHelper, string placeholderName)
        {
            string DPKey = "";
            if (myScHelper.CurrentRendering != null && myScHelper.CurrentRendering.Parameters != null && myScHelper.CurrentRendering.Parameters.Contains(DPInsertRendering.PlaceholderKeyParam))
            {
                DPKey = myScHelper.CurrentRendering.Parameters[DPInsertRendering.PlaceholderKeyParam];
            }
            if (!string.IsNullOrEmpty(DPKey))
            {
                return placeholderName + placeholderPattern + "_" + DPKey;
            }
            bool flag = false;
            int num = 0;
            foreach (string current in DynamicPlaceholderList)
            {
                if (placeholderName == current || current.StartsWith(placeholderName + placeholderPattern))
                {
                    flag = true;
                    num++;
                }
            }
            if (flag)
            {
                placeholderName = placeholderName + placeholderPattern + num.ToString(CultureInfo.InvariantCulture);
            }
            return placeholderName;
        }
    }
}
