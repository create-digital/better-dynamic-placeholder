using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Mvc.Presentation;
using Sitecore.Pipelines;
using Sitecore.Pipelines.ExecutePageEditorAction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterDynamicPlaceholder.Pipeline
{
    public class DPInsertRendering : InsertRendering
    {
        public const string PlaceholderKeyParam = "DPKey";
        public static char[] paramDelimieters = { '&', '=' };
        public new void Process(PipelineArgs args)
        {
            base.Process(args);

            Assert.ArgumentNotNull(args, "args");
            if (args is ExecuteInsertRenderingArgs)
            {

                ExecuteInsertRenderingArgs eirArgs = args as ExecuteInsertRenderingArgs;

                if (eirArgs != null) //skip if it's another type
                {
                    //TODO - see if rendering item contains properties we can look at rather than looking at a path
                    //put on all renderings for now
                    if (true)  //(eirArgs.RenderingItem.Paths.Path.ToLower().Contains("hearingfirst/modules") || eirArgs.RenderingItem.Paths.Path.ToLower().Contains("blog"))
                    {
                        int curIndex = 0;
                        for (int i = 0; i < eirArgs.Device.Renderings.Count; i++)
                        {
                            RenderingDefinition rd = (RenderingDefinition)eirArgs.Device.Renderings[i];
                            if (rd.Placeholder == eirArgs.PlaceholderKey)
                            {

                                //TODO - is position specific to the device?  What about language, etc.?
                                if (curIndex == eirArgs.Position)
                                {
                                    if (!string.IsNullOrEmpty(rd.Parameters))
                                        rd.Parameters += "&";
                                    rd.Parameters += PlaceholderKeyParam + "=" + FindUniquePlaceholderID(eirArgs.Device.Renderings); //TODO - generate random - look for duplicate
                                    eirArgs.Result.Settings.Parameters = rd.Parameters; //setting just in case they need to be in sync
                                    //return;
                                }
                                curIndex++;
                            }
                        }
                    }
                }
            }
        }
        public static string FindUniquePlaceholderID(ArrayList renderings)
        {
            int placeholderNum = 500;
            int highestPlaceholderNum = 0;
            int placeholderNumTemp = 0;

            bool parseNextVal = false;
            foreach (RenderingDefinition rd in renderings)
            {
                if (rd != null && rd.Parameters != null && rd.Parameters.Contains(PlaceholderKeyParam))
                {

                    String[] paramVals = rd.Parameters.Split(paramDelimieters);
                    placeholderNumTemp = 0;
                    parseNextVal = false;
                    for (int i = 0; i < paramVals.Count(); i++)
                    {
                        if (parseNextVal)
                        {
                            if (int.TryParse(paramVals[i], out placeholderNumTemp))
                            {
                                if (placeholderNumTemp > highestPlaceholderNum)
                                    highestPlaceholderNum = placeholderNumTemp;
                            }
                        }
                        else if (paramVals[i] == PlaceholderKeyParam)
                        {
                            parseNextVal = true;
                        }
                    }
                }
            }
            if (highestPlaceholderNum > 0) //increment for the next item
            {
                placeholderNum = highestPlaceholderNum + 1;
            }
            return placeholderNum.ToString(CultureInfo.InvariantCulture);
        }

    }
}
