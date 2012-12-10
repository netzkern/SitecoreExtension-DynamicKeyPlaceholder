

using Sitecore.Data.Items;
using Sitecore.Web.UI.PageModes;

namespace SitecoreExtension.DynamicKeyPlaceholder.Pipelines.GetChromeData
{
    using System;
    using System.Text.RegularExpressions;
    using Sitecore.Diagnostics;
    using Sitecore.Pipelines.GetChromeData;
    using SitecoreExtension.DynamicKeyPlaceholder.Pipelines.GetPlaceholderRenderings;

    /// <summary>
    /// Reset the Placeholder reference as if it was the original placeholder id 
    /// </summary>
    public class GetDynamicPlaceholderChromeData : GetPlaceholderChromeData
    {
        public override void Process(GetChromeDataArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.ChromeData, "Chrome Data");
            if (!"placeholder".Equals(args.ChromeType, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            string placeholderKey = args.CustomData["placeHolderKey"] as string;
            Regex regex = new Regex(GetDynamicKeyAllowedRenderings.DYNAMIC_KEY_REGEX);
            Match match = regex.Match(placeholderKey);
            if (!match.Success || match.Groups.Count <= 0)
            {
                return;
            }

            string newPlaceholderKey = match.Groups[1].Value;

            // Handles replacing the displayname and description of the placeholder area to the master reference without changeing other references
            Item item = null;
            if (args.Item != null)
            {
                string layout = ChromeContext.GetLayout(args.Item);
                item = Sitecore.Client.Page.GetPlaceholderItem(newPlaceholderKey, args.Item.Database, layout);
                if (item != null)
                {
                    args.ChromeData.DisplayName = item.DisplayName;
                }
                if ((item != null) && !string.IsNullOrEmpty(item.Appearance.ShortDescription))
                {
                    args.ChromeData.ExpandedDisplayName = item.Appearance.ShortDescription;
                }
            }
        }
    }
}
