using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Data.Models;

namespace App.Extensions;


public static class PostRssExtensions
{
    public static byte[] ToBytes(this SyndicationFeed feed)
    {
        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            NewLineHandling = NewLineHandling.Entitize,
            NewLineOnAttributes = true,
            Indent = true,
        };
        using var stream = new MemoryStream();
        using var xmlWriter = XmlWriter.Create(stream, settings);

        var rssFormatter = new Rss20FeedFormatter(feed, false);
        rssFormatter.WriteTo(xmlWriter);
        xmlWriter.Flush();

        return stream.ToArray();
    }

    public static SyndicationFeed FeedForPost(this Post post, Uri postUri, TimeProvider timeProvider)
    {
        var feed = new SyndicationFeed(post.Title, $"Comments on Post {post.Number}", postUri, "RSSUrl", timeProvider.GetUtcNow())
        {
            Items = post.Comments.Select(c => c.ItemForComment(postUri))
        };
        return feed;
    }

    public static SyndicationItem ItemForComment(this Comment item, Uri postUri)
    {
        var commentUri = new UriBuilder(postUri)
            {
                Fragment = $"{item.ID}"
            };
            var title = $"By {item.PostedBy.UserName}";
            var description = item.Body;
        return new SyndicationItem(title, description, commentUri.Uri, $"comment-{item.ID}", item.PostedOn);
    }
}
