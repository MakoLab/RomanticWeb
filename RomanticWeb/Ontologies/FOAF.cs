using System;
using System.Diagnostics.CodeAnalysis;

namespace RomanticWeb.Vocabularies
{
    /// <summary>Friend of a Friend (FOAF) vocabulary (http://xmlns.com/foaf/0.1/).</summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK here.")]
    public static class Foaf
    {
#pragma warning disable 1591 // ReSharper disable InconsistentNaming
        public const string BaseUri = "http://xmlns.com/foaf/0.1/";

        public static readonly Uri LabelProperty = new Uri(BaseUri + "LabelProperty");

        public static readonly Uri Person = new Uri(BaseUri + "Person");

        public static readonly Uri Document = new Uri(BaseUri + "Document");

        public static readonly Uri Organization = new Uri(BaseUri + "Organization");

        public static readonly Uri Group = new Uri(BaseUri + "Group");

        public static readonly Uri Agent = new Uri(BaseUri + "Agent");

        public static readonly Uri Project = new Uri(BaseUri + "Project");

        public static readonly Uri Image = new Uri(BaseUri + "Image");

        public static readonly Uri PersonalProfileDocument = new Uri(BaseUri + "PersonalProfileDocument");

        public static readonly Uri OnlineAccount = new Uri(BaseUri + "OnlineAccount");

        public static readonly Uri OnlineGamingAccount = new Uri(BaseUri + "OnlineGamingAccount");

        public static readonly Uri OnlineEcommerceAccount = new Uri(BaseUri + "OnlineEcommerceAccount");

        public static readonly Uri OnlineChatAccount = new Uri(BaseUri + "OnlineChatAccount");

        public static readonly Uri mbox = new Uri(BaseUri + "mbox");

        public static readonly Uri mbox_sha1sum = new Uri(BaseUri + "mbox_sha1sum");

        public static readonly Uri gender = new Uri(BaseUri + "gender");

        public static readonly Uri geekcode = new Uri(BaseUri + "geekcode");

        public static readonly Uri dnaChecksum = new Uri(BaseUri + "dnaChecksum");

        public static readonly Uri sha1 = new Uri(BaseUri + "sha1");

        public static readonly Uri based_near = new Uri(BaseUri + "based_near");

        public static readonly Uri title = new Uri(BaseUri + "title");

        public static readonly Uri nick = new Uri(BaseUri + "nick");

        public static readonly Uri jabberID = new Uri(BaseUri + "jabberID");

        public static readonly Uri aimChatID = new Uri(BaseUri + "aimChatID");

        public static readonly Uri skypeID = new Uri(BaseUri + "skypeID");

        public static readonly Uri icqChatID = new Uri(BaseUri + "icqChatID");

        public static readonly Uri yahooChatID = new Uri(BaseUri + "yahooChatID");

        public static readonly Uri msnChatID = new Uri(BaseUri + "msnChatID");

        public static readonly Uri name = new Uri(BaseUri + "name");

        public static readonly Uri firstName = new Uri(BaseUri + "firstName");

        public static readonly Uri lastName = new Uri(BaseUri + "lastName");

        public static readonly Uri givenName = new Uri(BaseUri + "givenName");

        public static readonly Uri givenname = new Uri(BaseUri + "givenname");

        public static readonly Uri surname = new Uri(BaseUri + "surname");

        public static readonly Uri family_name = new Uri(BaseUri + "family_name");

        public static readonly Uri familyName = new Uri(BaseUri + "familyName");

        public static readonly Uri phone = new Uri(BaseUri + "phone");

        public static readonly Uri homepage = new Uri(BaseUri + "homepage");

        public static readonly Uri weblog = new Uri(BaseUri + "weblog");

        public static readonly Uri openid = new Uri(BaseUri + "openid");

        public static readonly Uri tipjar = new Uri(BaseUri + "tipjar");

        public static readonly Uri plan = new Uri(BaseUri + "plan");

        public static readonly Uri made = new Uri(BaseUri + "made");

        public static readonly Uri maker = new Uri(BaseUri + "maker");

        public static readonly Uri img = new Uri(BaseUri + "img");

        public static readonly Uri depiction = new Uri(BaseUri + "depiction");

        public static readonly Uri depicts = new Uri(BaseUri + "depicts");

        public static readonly Uri thumbnail = new Uri(BaseUri + "thumbnail");

        public static readonly Uri myersBriggs = new Uri(BaseUri + "myersBriggs");

        public static readonly Uri workplaceHomepage = new Uri(BaseUri + "workplaceHomepage");

        public static readonly Uri workInfoHomepage = new Uri(BaseUri + "workInfoHomepage");

        public static readonly Uri schoolHomepage = new Uri(BaseUri + "schoolHomepage");

        public static readonly Uri knows = new Uri(BaseUri + "knows");

        public static readonly Uri interest = new Uri(BaseUri + "interest");

        public static readonly Uri topic_interest = new Uri(BaseUri + "topic_interest");

        public static readonly Uri publications = new Uri(BaseUri + "publications");

        public static readonly Uri currentProject = new Uri(BaseUri + "currentProject");

        public static readonly Uri pastProject = new Uri(BaseUri + "pastProject");

        public static readonly Uri fundedBy = new Uri(BaseUri + "fundedBy");

        public static readonly Uri logo = new Uri(BaseUri + "logo");

        public static readonly Uri topic = new Uri(BaseUri + "topic");

        public static readonly Uri primaryTopic = new Uri(BaseUri + "primaryTopic");

        public static readonly Uri focus = new Uri(BaseUri + "focus");

        public static readonly Uri isPrimaryTopicOf = new Uri(BaseUri + "isPrimaryTopicOf");

        public static readonly Uri page = new Uri(BaseUri + "page");

        public static readonly Uri theme = new Uri(BaseUri + "theme");

        public static readonly Uri account = new Uri(BaseUri + "account");

        public static readonly Uri holdsAccount = new Uri(BaseUri + "holdsAccount");

        public static readonly Uri accountServiceHomepage = new Uri(BaseUri + "accountServiceHomepage");

        public static readonly Uri accountName = new Uri(BaseUri + "accountName");

        public static readonly Uri member = new Uri(BaseUri + "member");

        public static readonly Uri membershipClass = new Uri(BaseUri + "membershipClass");

        public static readonly Uri birthday = new Uri(BaseUri + "birthday");

        public static readonly Uri age = new Uri(BaseUri + "age");

        public static readonly Uri status = new Uri(BaseUri + "status");
#pragma warning restore 1591 // ReSharper restore InconsistentNaming
    }
}