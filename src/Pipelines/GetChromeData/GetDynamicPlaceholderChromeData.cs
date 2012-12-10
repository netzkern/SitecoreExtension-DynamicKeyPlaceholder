

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
            if ("placeholder".Equals(args.ChromeType, StringComparison.OrdinalIgnoreCase))
            {
                string placeholderKey = args.CustomData["placeHolderKey"] as string;
                Regex regex = new Regex(GetDynamicKeyAllowedRenderings.DYNAMIC_KEY_REGEX);
                Match match = regex.Match(placeholderKey);
                if (match.Success && match.Groups.Count > 0)
                {
                    string newPlaceholderKey = match.Groups[1].Value;
                    args.CustomData["placeHolderKey"] = newPlaceholderKey;
                    base.Process(args);
                    args.CustomData["placeHolderKey"] = placeholderKey;
                }
                else
                {
                    base.Process(args);
                }

            }
        }
    }
}
