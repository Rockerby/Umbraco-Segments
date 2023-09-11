using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace SegmentHelper.Events
{
    public class CreateSegmentsEvent : INotificationHandler<SendingContentNotification>
    {
        private readonly IContentTypeService contentTypeService;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly IPublishedValueFallback _publishedValueFallback;

        public CreateSegmentsEvent(IContentTypeService contentTypeService, IVariationContextAccessor variationContextAccessor, IPublishedValueFallback publishedValueFallback)
        {
            this.contentTypeService = contentTypeService;
            this._variationContextAccessor = variationContextAccessor;
            _publishedValueFallback = publishedValueFallback;
        }

        public void Handle(SendingContentNotification notification)
        {
            if (notification.Content.Id > 0)
                return;
            //Get Data folder and get segments
            var segmentDefinitions = notification.UmbracoContext.Content.GetAtRoot().FirstOrDefault(node => node.ContentType.Alias == "segments" && node.Name.Equals("Segments")).Children(_variationContextAccessor);
            var relevantSegmentDefinitions = segmentDefinitions.Where(def => def.Value<IEnumerable<string>>(_publishedValueFallback, "documentTypes").Contains(notification.Content.ContentTypeAlias));

            //get all segements with content type
            var segments = relevantSegmentDefinitions.Select(def => def.Name);
            //if no segments quit
            if (!segments.Any())
                return;

            //get all properties of contentType which allow vary by segment
            var contentType = contentTypeService.Get(notification.Content.ContentTypeId.Value);
            if (contentType == null)
                return;
            var props = contentType.PropertyTypes.Where(p => p.VariesBySegment()).Select(p => p.Alias);
            props = props.Concat(contentType.CompositionPropertyGroups.SelectMany(p => p.PropertyTypes).Where(p => p.VariesBySegment()).Select(p => p.Alias));
            // if no props, quit
            if (!props.Any())
                return;

            //segments are coupled to variants so make the segment for all variations
            foreach (var variant in notification.Content.Variants)
            {
                //Get all properties
                //check if property allows vary by segment
                var properties = variant.Tabs.SelectMany(f => f.Properties).Select(p => p.Alias).Where(p => props.Contains(p));
                //make segment for property

                foreach (var segment in segments)
                {
                    ContentVariantDisplay newVariant = new ContentVariantDisplay();
                    newVariant.AllowedActions = variant.AllowedActions;
                    newVariant.Tabs = variant.Tabs;
                    newVariant.Language = variant.Language;
                    newVariant.State = variant.State;

                    newVariant.Name = segment;
                    newVariant.Segment = segment;
                    //newVariant.DisplayName = segment;
                    foreach (var property in properties)
                    {
                        SetDefaultValue(newVariant, property, null);
                    }
                    notification.Content.Variants = notification.Content.Variants.Append(newVariant);
                }
            }
        }

        private void SetDefaultValue(ContentVariantDisplay variant, string propertyAlias, object defaultValue)
        {

            var property = variant.Tabs.SelectMany(f => f.Properties)
                       .FirstOrDefault(f => f.Alias.InvariantEquals(propertyAlias));
            if (property != null && (property.Value == null || String.IsNullOrWhiteSpace(property.Value.ToString())))
            {
                property.Value = defaultValue;
                property.Segment = variant.Segment;
            }

        }
    }

}