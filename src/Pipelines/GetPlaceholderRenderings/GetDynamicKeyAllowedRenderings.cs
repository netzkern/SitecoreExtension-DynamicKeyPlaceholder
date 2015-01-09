namespace SitecoreExtension.DynamicKeyPlaceholder.Pipelines.GetPlaceholderRenderings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Sitecore.Pipelines.GetPlaceholderRenderings;
    using Sitecore.Data.Items;
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Diagnostics;

    /// <summary>
    /// Handles changing context to the references dynamic "master" renderings settings for inserting the allowed controls for the placeholder and making it editable
    /// </summary>
    public class GetDynamicKeyAllowedRenderings : GetAllowedRenderings
    {
        //text that ends in a GUID or {{uniqueId}}
        public const string DYNAMIC_KEY_REGEX = @"(.+)(?:{[\d\w]{8}\-([\d\w]{4}\-){3}[\d\w]{12}}|{{.+}})";

        public new void Process(GetPlaceholderRenderingsArgs args)
        {
            Assert.IsNotNull(args, "args");

            string placeholderKey = args.PlaceholderKey;
            Regex regex = new Regex(DYNAMIC_KEY_REGEX);
            Match match = regex.Match(placeholderKey);
            if (match.Success && match.Groups.Count > 0)
            {
                placeholderKey = match.Groups[1].Value;
            }
            else
            {
                return;
            }

            // Same code as Sitecore.Pipelines.GetPlaceholderRenderings.GetAllowedRenderings but with fake placeholderKey
            Item placeholderItem = null;
            if (ID.IsNullOrEmpty(args.DeviceId))
            {
                placeholderItem = Client.Page.GetPlaceholderItem(placeholderKey, args.ContentDatabase, args.LayoutDefinition);
            }
            else
            {
                using (new DeviceSwitcher(args.DeviceId, args.ContentDatabase))
                {
                    placeholderItem = Client.Page.GetPlaceholderItem(placeholderKey, args.ContentDatabase, args.LayoutDefinition);
                }
            }

            List<Item> renderings = null;
            if (placeholderItem != null)
            {
                bool allowedControlsSpecified;
                args.HasPlaceholderSettings = true;
                renderings = this.GetRenderings(placeholderItem, out allowedControlsSpecified);
                if (allowedControlsSpecified)
                {
                    args.CustomData["allowedControlsSpecified"] = true;
                    args.Options.ShowTree = false;
                }
            }
            if (renderings != null)
            {
                if (args.PlaceholderRenderings == null)
                {
                    args.PlaceholderRenderings = new List<Item>();
                }
                args.PlaceholderRenderings.AddRange(renderings);
            }
        }
    }
}
