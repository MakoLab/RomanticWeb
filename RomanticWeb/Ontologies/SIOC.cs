using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>SIOC Core Ontology Namespace (http://rdfs.org/sioc/ns#).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK here.")]
    public static class Sioc
    {
#pragma warning disable 1591 // ReSharper disable InconsistentNaming
        public const string BaseUri = "http://rdfs.org/sioc/ns#";

        public static readonly Uri Community = new Uri(BaseUri + "Community");

        public static readonly Uri Container = new Uri(BaseUri + "Container");

        public static readonly Uri Forum = new Uri(BaseUri + "Forum");

        public static readonly Uri Item = new Uri(BaseUri + "Item");

        public static readonly Uri Post = new Uri(BaseUri + "Post");

        public static readonly Uri Role = new Uri(BaseUri + "Role");

        public static readonly Uri Space = new Uri(BaseUri + "Space");

        public static readonly Uri Site = new Uri(BaseUri + "Site");

        public static readonly Uri Thread = new Uri(BaseUri + "Thread");

        public static readonly Uri UserAccount = new Uri(BaseUri + "UserAccount");

        public static readonly Uri Usergroup = new Uri(BaseUri + "Usergroup");

        public static readonly Uri content = new Uri(BaseUri + "content");

        public static readonly Uri email_sha1 = new Uri(BaseUri + "email_sha1");

        public static readonly Uri id = new Uri(BaseUri + "id");

        public static readonly Uri ip_address = new Uri(BaseUri + "ip_address");

        public static readonly Uri last_activity_date = new Uri(BaseUri + "last_activity_date");

        public static readonly Uri last_item_date = new Uri(BaseUri + "last_item_date");

        public static readonly Uri last_reply_date = new Uri(BaseUri + "last_reply_date");

        public static readonly Uri name = new Uri(BaseUri + "name");

        public static readonly Uri note = new Uri(BaseUri + "note");

        public static readonly Uri num_authors = new Uri(BaseUri + "num_authors");

        public static readonly Uri num_items = new Uri(BaseUri + "num_items");

        public static readonly Uri num_replies = new Uri(BaseUri + "num_replies");

        public static readonly Uri num_threads = new Uri(BaseUri + "num_threads");

        public static readonly Uri num_views = new Uri(BaseUri + "num_views");

        public static readonly Uri about = new Uri(BaseUri + "about");

        public static readonly Uri account_of = new Uri(BaseUri + "account_of");

        public static readonly Uri addressed_to = new Uri(BaseUri + "addressed_to");

        public static readonly Uri administrator_of = new Uri(BaseUri + "administrator_of");

        public static readonly Uri attachment = new Uri(BaseUri + "attachment");

        public static readonly Uri avatar = new Uri(BaseUri + "avatar");

        public static readonly Uri container_of = new Uri(BaseUri + "container_of");

        public static readonly Uri creator_of = new Uri(BaseUri + "creator_of");

        public static readonly Uri email = new Uri(BaseUri + "email");

        public static readonly Uri embeds_knowledge = new Uri(BaseUri + "embeds_knowledge");

        public static readonly Uri feed = new Uri(BaseUri + "feed");

        public static readonly Uri follows = new Uri(BaseUri + "follows");

        public static readonly Uri function_of = new Uri(BaseUri + "function_of");

        public static readonly Uri has_administrator = new Uri(BaseUri + "has_administrator");

        public static readonly Uri has_container = new Uri(BaseUri + "has_container");

        public static readonly Uri has_creator = new Uri(BaseUri + "has_creator");

        public static readonly Uri has_discussion = new Uri(BaseUri + "has_discussion");

        public static readonly Uri has_function = new Uri(BaseUri + "has_function");

        public static readonly Uri has_host = new Uri(BaseUri + "has_host");

        public static readonly Uri has_member = new Uri(BaseUri + "has_member");

        public static readonly Uri has_moderator = new Uri(BaseUri + "has_moderator");

        public static readonly Uri has_modifier = new Uri(BaseUri + "has_modifier");

        public static readonly Uri has_owner = new Uri(BaseUri + "has_owner");

        public static readonly Uri has_parent = new Uri(BaseUri + "has_parent");

        public static readonly Uri has_reply = new Uri(BaseUri + "has_reply");

        public static readonly Uri has_scope = new Uri(BaseUri + "has_scope");

        public static readonly Uri has_space = new Uri(BaseUri + "has_space");

        public static readonly Uri has_subscriber = new Uri(BaseUri + "has_subscriber");

        public static readonly Uri has_usergroup = new Uri(BaseUri + "has_usergroup");

        public static readonly Uri host_of = new Uri(BaseUri + "host_of");

        public static readonly Uri latest_version = new Uri(BaseUri + "latest_version");

        public static readonly Uri link = new Uri(BaseUri + "link");

        public static readonly Uri links_to = new Uri(BaseUri + "links_to");

        public static readonly Uri member_of = new Uri(BaseUri + "member_of");

        public static readonly Uri moderator_of = new Uri(BaseUri + "moderator_of");

        public static readonly Uri modifier_of = new Uri(BaseUri + "modifier_of");

        public static readonly Uri next_by_date = new Uri(BaseUri + "next_by_date");

        public static readonly Uri next_version = new Uri(BaseUri + "next_version");

        public static readonly Uri owner_of = new Uri(BaseUri + "owner_of");

        public static readonly Uri parent_of = new Uri(BaseUri + "parent_of");

        public static readonly Uri previous_by_date = new Uri(BaseUri + "previous_by_date");

        public static readonly Uri previous_version = new Uri(BaseUri + "previous_version");

        public static readonly Uri related_to = new Uri(BaseUri + "related_to");

        public static readonly Uri reply_of = new Uri(BaseUri + "reply_of");

        public static readonly Uri scope_of = new Uri(BaseUri + "scope_of");

        public static readonly Uri space_of = new Uri(BaseUri + "space_of");

        public static readonly Uri subscriber_of = new Uri(BaseUri + "subscriber_of");

        public static readonly Uri topic = new Uri(BaseUri + "topic");

        public static readonly Uri usergroup_of = new Uri(BaseUri + "usergroup_of");
#pragma warning restore 1591 // ReSharper restore InconsistentNaming
    }
}