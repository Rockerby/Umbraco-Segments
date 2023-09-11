using Microsoft.Extensions.DependencyInjection;
using SegmentHelper.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace SegmentHelper.Composers
{
    public class MainSegmentComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ContentTypeSavingNotification, AllowSegmentationEvent>();
            builder.AddNotificationHandler<SendingContentNotification, CreateSegmentsEvent>();

        }
    }
}
