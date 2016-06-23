using BetterDynamicPlaceholder.Helpers;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetPlaceholderRenderings;

namespace BetterDynamicPlaceholder.Pipeline
{
    class GetAllowedRenderings
    {
        private GetAllowedRenderings getRenderings = new GetAllowedRenderings();

        public void Process(GetPlaceholderRenderingsArgs args)
        {
            Assert.IsNotNull(args, "args");
            string setting = BDPHelper.placeholderPattern.ToLower();
            
            if (!string.IsNullOrEmpty(setting) && args.PlaceholderKey.ToLower().Contains(setting))
            {
                string text = args.PlaceholderKey.ToLower().Substring(0, args.PlaceholderKey.ToLower().LastIndexOf(setting));
                GetPlaceholderRenderingsArgs getPlaceholderRenderingsArgs;
                if (!args.DeviceId.IsNull)
                {
                    getPlaceholderRenderingsArgs = new GetPlaceholderRenderingsArgs(text, args.LayoutDefinition, args.ContentDatabase, args.DeviceId);
                }
                else
                {
                    getPlaceholderRenderingsArgs = new GetPlaceholderRenderingsArgs(text, args.LayoutDefinition, args.ContentDatabase);
                }
                this.getRenderings.Process(getPlaceholderRenderingsArgs);
                this.AssimilateArgumentResults(ref args, getPlaceholderRenderingsArgs);
                return;
            }
            this.getRenderings.Process(args);
        }

        private void AssimilateArgumentResults(ref GetPlaceholderRenderingsArgs args, GetPlaceholderRenderingsArgs basePlaceholderArgs)
        {

            args.Options.ShowTree = basePlaceholderArgs.Options.ShowTree;
            args.Options.ShowRoot = basePlaceholderArgs.Options.ShowRoot;
            args.Options.SetRootAsSearchRoot = basePlaceholderArgs.Options.SetRootAsSearchRoot;
            args.HasPlaceholderSettings = basePlaceholderArgs.HasPlaceholderSettings;
            args.OmitNonEditableRenderings= basePlaceholderArgs.OmitNonEditableRenderings;
            args.PlaceholderRenderings = basePlaceholderArgs.PlaceholderRenderings;
        }
    }
}
