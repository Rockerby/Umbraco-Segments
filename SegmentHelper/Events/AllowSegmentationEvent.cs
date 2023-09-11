using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

namespace SegmentHelper.Events
{
    /// <summary>
    /// This will enable segmentation on a document type when it's saving
    /// We use the saving event instead of saved to not have trigger our controller again 
    /// </summary>
    public class AllowSegmentationEvent : INotificationHandler<ContentTypeSavingNotification>
    {
        public void Handle(ContentTypeSavingNotification notification)
        {
            var contentTypes = notification.SavedEntities;
            foreach (var contentType in contentTypes)
            {
                //Doesn't allow segementation when content type is element or varies by segment is already set
                if (contentType.IsElement || contentType.VariesBySegment())
                    continue;

                contentType.SetVariesBy(ContentVariation.CultureAndSegment, true);
            }
        }
    }
}
